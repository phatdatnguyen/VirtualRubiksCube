namespace VirtualRubiksCube.Solver
{
    // Each phase tracks only the pieces it places. Later algorithms preserve the
    // earlier layers. Complete reverse BFS tables avoid arbitrary depth limits and
    // the unbounded full-cube searches of the former last-layer solver.
    public static class LayerByLayerSolver
    {
        private static readonly int[][] BottomEdges =
        {
            new[] { 28, 25 }, new[] { 32, 16 }, new[] { 34, 52 }, new[] { 30, 43 },
        };
        private static readonly int[][] BottomCorners =
        {
            new[] { 27, 24, 44 }, new[] { 29, 26, 15 },
            new[] { 35, 17, 51 }, new[] { 33, 53, 42 },
        };
        private static readonly int[][] MiddleEdges =
        {
            new[] { 23, 12 }, new[] { 21, 41 }, new[] { 14, 48 }, new[] { 39, 50 },
        };
        private static readonly int[][] TopEdges =
        {
            new[] { 7, 19 }, new[] { 5, 10 }, new[] { 1, 46 }, new[] { 3, 37 },
        };
        private static readonly int[][] TopCorners =
        {
            new[] { 6, 18, 38 }, new[] { 8, 20, 9 },
            new[] { 2, 11, 45 }, new[] { 0, 47, 36 },
        };
        private static readonly int[][] EdgeSlots = TopEdges.Concat(MiddleEdges).Concat(BottomEdges).ToArray();
        private static readonly int[][] CornerSlots = TopCorners.Concat(BottomCorners).ToArray();
        private static readonly Phase[] Phases =
        {
            // Four distinct oriented edges: 12 * 11 * 10 * 9 * 2^4.
            new("Bottom cross", BottomEdges, FaceTurns(), 190080),
            // Four distinct oriented corners: 8 * 7 * 6 * 5 * 3^4.
            new("Bottom corners", BottomCorners, WithSideAlgorithms("R U R'"), 136080),
            // Four edges among the eight remaining slots: 8 * 7 * 6 * 5 * 2^4.
            new("Middle edges", MiddleEdges, WithSideAlgorithms("U R U' R' U' F' U F"), 26880),
            // Last-layer permutations/orientations with twist, flip and parity constraints.
            new("Last layer", TopEdges.Concat(TopCorners).ToArray(), WithInverses(new[]
            {
                "U", "U2", "F R U R' U' F'", "R U R' U R U2 R'", "U R U' L' U R' U' L",
            }), 62208),
        };

        public static List<SolverMove> Solve(FaceletCube start, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(start);
            cancellationToken.ThrowIfCancellationRequested();
            var cube = start.Clone();
            cube.Validate();
            var moves = new List<SolverMove>();
            foreach (var phase in Phases)
                phase.Solve(cube, moves, cancellationToken);

            var result = Simplify(moves);
            var verification = start.Clone();
            verification.ApplyMoves(result);
            if (!verification.IsSolved())
                throw new InvalidOperationException("The solution did not solve the input cube.");
            cancellationToken.ThrowIfCancellationRequested();
            return result;
        }

        private static string[] FaceTurns() => "URFDLB"
            .SelectMany(face => new[] { face.ToString(), face + "'", face + "2" }).ToArray();

        private static string[] WithSideAlgorithms(string algorithm)
        {
            var sequences = new List<string> { "U", "U2" };
            const string sides = "FRBL";
            for (int turn = 0; turn < 4; turn++)
                sequences.Add(new string(algorithm.Select(ch =>
                {
                    int index = sides.IndexOf(ch);
                    return index < 0 ? ch : sides[(index + turn) % 4];
                }).ToArray()));
            return WithInverses(sequences);
        }

        private static string[] WithInverses(IEnumerable<string> sequences) => sequences
            .SelectMany(sequence => new[]
            {
                sequence,
                string.Join(" ", SolverMove.ParseSequence(sequence).AsEnumerable().Reverse().Select(move => move.Inverse())),
            }).Distinct().ToArray();

        private static List<SolverMove> Simplify(IEnumerable<SolverMove> moves)
        {
            var result = new List<SolverMove>();
            foreach (var move in moves)
            {
                if (result.Count == 0 || result[^1].Face != move.Face)
                {
                    result.Add(move);
                    continue;
                }
                int quarter = (result[^1].Quarter + move.Quarter + 4) % 4;
                result.RemoveAt(result.Count - 1);
                if (quarter != 0)
                    result.Add(new SolverMove(move.Face, quarter == 3 ? -1 : quarter));
            }
            return result;
        }

        private static int ColorBit(char color) => 1 << "URFDLB".IndexOf(color);

        private sealed class Phase
        {
            private readonly string name;
            private readonly int[][] pieces;
            private readonly int[] colorMasks;
            private readonly char[] trackedColors;
            private readonly SolverMove[][] algorithms;
            private readonly int[][] inversePermutations;
            private readonly int stateCount;
            private readonly ulong solvedKey;
            private readonly object tableLock = new();
            private Dictionary<ulong, int>? table;

            public Phase(string name, int[][] pieces, string[] sequences, int stateCount)
            {
                this.name = name;
                this.pieces = pieces;
                this.stateCount = stateCount;
                var solved = FaceletCube.Solved();
                colorMasks = pieces.Select(piece => piece.Aggregate(0, (mask, index) => mask | ColorBit(solved[index]))).ToArray();
                trackedColors = pieces.Select(piece => solved[piece[0]]).ToArray();
                algorithms = sequences.Select(sequence => SolverMove.ParseSequence(sequence).ToArray()).ToArray();
                inversePermutations = algorithms.Select(InverseStickerPermutation).ToArray();
                solvedKey = Key(solved);
            }

            public void Solve(FaceletCube cube, List<SolverMove> moves, CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();
                ulong key = Key(cube);
                if (key == solvedKey) return;
                var lookup = GetTable(cancellationToken);
                while (key != solvedKey)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (!lookup.TryGetValue(key, out int algorithm))
                        throw new InvalidOperationException($"{name}: the cube does not satisfy the preceding layer constraints.");
                    cube.ApplyMoves(algorithms[algorithm]);
                    moves.AddRange(algorithms[algorithm]);
                    key = Key(cube);
                }
            }

            // On a valid cube, one sticker determines a cubie's location/orientation.
            // Six bits per sticker fit all eight last-layer cubies in a single ulong.
            private ulong Key(FaceletCube cube)
            {
                ulong key = 0;
                for (int piece = 0; piece < pieces.Length; piece++)
                {
                    int location = -1;
                    foreach (var slot in pieces[piece].Length == 2 ? EdgeSlots : CornerSlots)
                    {
                        int mask = 0;
                        foreach (int index in slot) mask |= ColorBit(cube[index]);
                        if (mask != colorMasks[piece]) continue;
                        location = slot.First(index => cube[index] == trackedColors[piece]);
                        break;
                    }
                    if (location < 0)
                        throw new InvalidOperationException($"{name}: a required piece is missing.");
                    key |= (ulong)location << (6 * piece);
                }
                return key;
            }

            private Dictionary<ulong, int> GetTable(CancellationToken cancellationToken)
            {
                lock (tableLock)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (table != null) return table;
                    var result = new Dictionary<ulong, int>(stateCount) { [solvedKey] = -1 };
                    var queue = new Queue<ulong>();
                    queue.Enqueue(solvedKey);
                    int processed = 0;
                    while (queue.TryDequeue(out ulong key))
                    {
                        if ((processed++ & 255) == 0) cancellationToken.ThrowIfCancellationRequested();
                        for (int algorithm = 0; algorithm < algorithms.Length; algorithm++)
                        {
                            ulong next = Transform(key, inversePermutations[algorithm]);
                            if (!result.TryAdd(next, algorithm)) continue;
                            if (result.Count > stateCount)
                                throw new InvalidOperationException($"{name}: an algorithm disturbed a preceding layer.");
                            queue.Enqueue(next);
                        }
                    }
                    cancellationToken.ThrowIfCancellationRequested();
                    if (result.Count != stateCount)
                        throw new InvalidOperationException($"{name}: incomplete pattern table ({result.Count}/{stateCount}).");
                    // Publish only complete tables so cancelled initialization can be retried.
                    table = result;
                    return table;
                }
            }

            private ulong Transform(ulong key, int[] permutation)
            {
                ulong result = 0;
                for (int piece = 0; piece < pieces.Length; piece++)
                    result |= (ulong)permutation[(int)((key >> (6 * piece)) & 63)] << (6 * piece);
                return result;
            }

            private static int[] InverseStickerPermutation(SolverMove[] algorithm)
            {
                var labeled = FaceletCube.Solved();
                for (int index = 0; index < FaceletCube.FaceletCount; index++)
                    labeled.Facelets[index] = (char)index;
                labeled.ApplyMoves(algorithm);
                // new[i] = old[permutation[i]] maps each destination back to its
                // source, giving the inverse algorithm needed by reverse BFS.
                return labeled.Facelets.Select(value => (int)value).ToArray();
            }
        }
    }
}
