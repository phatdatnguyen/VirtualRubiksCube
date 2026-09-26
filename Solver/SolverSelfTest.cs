using System.Diagnostics;
using VirtualRubiksCube.Solver;

namespace VirtualRubiksCube
{
    // Runs without creating a window or an OpenGL context:
    //   dotnet run -- --solver-test
    internal static class SolverSelfTest
    {
        private static readonly char[] Faces = { 'U', 'D', 'L', 'R', 'F', 'B' };
        private static readonly int[] Quarters = { 1, -1, 2 };

        public static int Run()
        {
            int passed = 0, failed = 0;
            var elapsed = Stopwatch.StartNew();
            void Check(string name, Action test)
            {
                try
                {
                    test();
                    passed++;
                    Console.WriteLine($"  OK   {name}");
                }
                catch (Exception ex)
                {
                    failed++;
                    Console.WriteLine($"  FAIL {name}: {ex.Message}");
                }
            }

            Check("solved cube and empty solution", TestSolved);
            Check("cancelled table initialization can be retried", TestCancellation);
            Check("18 face moves match the controller", TestMoveEquivalence);
            Check("400 mixed moves match the controller", TestMixedMoveEquivalence);
            Check("24 orientations and move translations", TestOrientations);
            Check("move notation validation", TestMoveParsing);
            Check("impossible states rejected", TestInvalidStates);
            Check("solved detection includes sticker orientation", TestSolvedDetection);
            Check("repeated move instance in animation queue", TestRepeatedQueuedMove);
            Check("reset during animation and subsequent move", TestReset);
            Check("async solver success, failure, cancellation and reset", TestAsyncController);

            foreach (char face in Faces)
                foreach (int quarter in Quarters)
                {
                    var move = new SolverMove(face, quarter);
                    Check($"solve {move}", () => TestSolve(new[] { move }));
                }

            string[] patterns =
            {
                "U2 D2 L2 R2 F2 B2", // Checkerboard.
                "R U R' U' R U R' U'",
                "F R U R' U' F'",
                "R U R' U R U2 R'",
                "R U' R U R U R U' R' U' R2",
                "U R2 F B R B2 R U2 L B2 R U' D' R2 F R' L B2 U2 F2", // Superflip.
                "B2 F' D F F2 D' R2 D2 U2 B L U' R2 U R' D2 L2 L D2 D", // Legacy cross phase left an edge unsolved.
            };
            foreach (string pattern in patterns)
                Check($"solve pattern {pattern}", () => TestSolve(SolverMove.ParseSequence(pattern)));

            // Independent seeds keep failures reproducible as cases are added or reordered.
            for (int seed = 0; seed < 128; seed++)
                foreach (int length in new[] { 5, 20, 50, 100 })
                {
                    var scramble = RandomScramble(new Random(seed * 101 + length), length);
                    Check($"solve seed={seed} length={length}", () => TestSolve(scramble));
                }

            foreach (var layer in new[] { RubiksCube.Layer.MiddleX, RubiksCube.Layer.MiddleY, RubiksCube.Layer.MiddleZ })
                foreach (int quarter in Quarters)
                {
                    var moves = Enumerable.Repeat(new Move(layer, quarter == -1
                        ? Move.RotationType.Counterclockwise : Move.RotationType.Clockwise), quarter == 2 ? 2 : 1).ToList();
                    Check($"solve slice {layer} quarter={quarter}", () => TestControllerSolve(moves));
                }

            foreach (int seed in new[] { 7, 42, 1701, 2026 })
            {
                var rng = new Random(seed);
                var layers = Enum.GetValues<RubiksCube.Layer>().Where(layer => layer != RubiksCube.Layer.None).ToArray();
                var moves = Enumerable.Range(0, 60).Select(_ => new Move(layers[rng.Next(layers.Length)],
                    rng.Next(2) == 0 ? Move.RotationType.Clockwise : Move.RotationType.Counterclockwise)).ToList();
                Check($"solve mixed face/slice seed={seed}", () => TestControllerSolve(moves));
            }

            Console.WriteLine($"\nSummary: {passed} passed, {failed} failed ({elapsed.Elapsed.TotalSeconds:F2}s).");
            return failed == 0 ? 0 : 1;
        }

        private static void TestSolved()
        {
            var (cube, _) = MakeCube();
            var facelets = FaceletCube.FromControllerState(cube);
            AssertSame(FaceletCube.Solved(), facelets, "Solved model conversion");
            Require(LayerByLayerSolver.Solve(facelets).Count == 0, "Solved cube should need no moves.");
        }

        private static void TestMoveEquivalence()
        {
            foreach (char face in Faces)
                foreach (int quarter in Quarters)
                {
                    var (cube, controller) = MakeCube();
                    var move = new SolverMove(face, quarter);
                    var expected = FaceletCube.Solved();
                    expected.ApplyMove(move);
                    controller.Scramble(move.ToControllerMoves().ToList());
                    AssertSame(expected, FaceletCube.FromControllerState(cube), $"Move {move}");
                    expected.ApplyMove(move.Inverse());
                    AssertSame(FaceletCube.Solved(), expected, $"Inverse of {move}");
                }
        }

        private static void TestCancellation()
        {
            using var alreadyCancelled = new CancellationTokenSource();
            alreadyCancelled.Cancel();
            ExpectException<OperationCanceledException>(() =>
                LayerByLayerSolver.Solve(FaceletCube.Solved(), alreadyCancelled.Token), "Pre-cancelled solved cube");

            var cube = FaceletCube.Solved();
            cube.ApplyMoves(SolverMove.ParseSequence("F R U2 L' B D2"));
            var before = cube.Clone();
            using var cancelDuringBuild = new CancellationTokenSource();
            cancelDuringBuild.CancelAfter(1);
            ExpectException<OperationCanceledException>(() => LayerByLayerSolver.Solve(cube, cancelDuringBuild.Token),
                "Cancelled first lookup-table build");
            AssertSame(before, cube, "Cancelled solve must preserve its input");
            var solution = LayerByLayerSolver.Solve(cube);
            cube.ApplyMoves(solution);
            Require(cube.IsSolved(), "Retry must succeed after interrupted table initialization.");

            var (_, controller) = MakeCube();
            ExpectException<OperationCanceledException>(() => controller.GetSolverMoves(alreadyCancelled.Token),
                "Cancelled controller solve");
            Require(controller.MoveQueue.Count == 0, "Cancelled solve must not enqueue moves.");
        }

        private static void TestMixedMoveEquivalence()
        {
            var (cube, controller) = MakeCube();
            var expected = FaceletCube.Solved();
            var moves = RandomScramble(new Random(83917), 400);
            for (int i = 0; i < moves.Count; i++)
            {
                expected.ApplyMove(moves[i]);
                controller.Scramble(moves[i].ToControllerMoves().ToList());
                AssertSame(expected, FaceletCube.FromControllerState(cube), $"After mixed move {i}: {moves[i]}");
            }
            expected.ApplyMoves(moves.AsEnumerable().Reverse().Select(move => move.Inverse()));
            AssertSame(FaceletCube.Solved(), expected, "Inverse mixed sequence");
        }

        private static void TestOrientations()
        {
            Require(FaceletCube.Orientations.Length == 24, "Expected 24 orientations.");
            Require(FaceletCube.Orientations.Select(orientation => string.Join(",", orientation.Perm)).Distinct().Count() == 24,
                "Orientations must be distinct.");
            // Unique labels expose incorrect permutations even when neighboring colors match.
            var labels = FaceletCube.Solved();
            for (int i = 0; i < FaceletCube.FaceletCount; i++) labels.Facelets[i] = (char)(i + 128);
            var scramble = FaceletCube.Solved();
            scramble.ApplyMoves(RandomScramble(new Random(741), 50));
            foreach (var orientation in FaceletCube.Orientations)
            {
                AssertSame(labels, labels.ApplyOrientation(orientation.Perm).ApplyOrientation(orientation.InvPerm),
                    "Orientation inverse");
                AssertSame(FaceletCube.Solved(), FaceletCube.Solved().ApplyOrientationWithRelabel(orientation),
                    "Orientation relabeling");
                foreach (char face in Faces)
                    foreach (int quarter in Quarters)
                    {
                        var move = new SolverMove(face, quarter);
                        var rotated = scramble.ApplyOrientationWithRelabel(orientation);
                        rotated.ApplyMove(move);
                        var translated = scramble.Clone();
                        translated.ApplyMove(FaceletCube.TranslateMove(move, orientation));
                        AssertSame(rotated, translated.ApplyOrientationWithRelabel(orientation), $"Translated move {move}");
                    }
            }
        }

        private static void TestMoveParsing()
        {
            var moves = SolverMove.ParseSequence(" \tU R'\r\nF2  ");
            Require(string.Join(" ", moves) == "U R' F2", "Notation must accept whitespace separators.");
            foreach (string token in new[] { "", "X", "u", "U3", "Ufoo", "R2'", "F''" })
                ExpectException<FormatException>(() => SolverMove.Parse(token), $"Invalid token '{token}'");
            ExpectException<ArgumentException>(() => new SolverMove('X', 1), "Invalid face");
            foreach (int quarter in new[] { -2, 0, 3, 4 })
                ExpectException<ArgumentException>(() => new SolverMove('U', quarter), $"Invalid quarter {quarter}");
        }

        private static void TestInvalidStates()
        {
            void Reject(string name, Action<char[]> corrupt)
            {
                var cube = FaceletCube.Solved();
                corrupt(cube.Facelets);
                ExpectException<InvalidOperationException>(() => cube.Validate(), name);
                ExpectException<InvalidOperationException>(() => LayerByLayerSolver.Solve(cube), name);
            }
            Reject("Unknown sticker", f => f[0] = '?');
            Reject("Wrong color count", f => f[0] = 'R');
            Reject("Duplicate centers", f => (f[4], f[9]) = (f[9], f[4]));
            Reject("Flipped UF edge", f => (f[7], f[19]) = (f[19], f[7]));
            Reject("Twisted UFR corner", f => (f[8], f[9], f[20]) = (f[9], f[20], f[8]));
            Reject("Reflected UFR corner", f => (f[9], f[20]) = (f[20], f[9]));
            Reject("Swapped UF and UR edges", f => (f[19], f[10]) = (f[10], f[19]));
            Reject("Duplicate UF/DB and missing UB/DF edges", f => (f[46], f[25]) = (f[25], f[46]));
        }

        private static void TestSolvedDetection()
        {
            var (cube, _) = MakeCube();
            var corner = cube.Cubelets.Single(c => c.CurrentPosition == ((sbyte)1, (sbyte)-1, (sbyte)1));
            (corner.CurrentFaces[0], corner.CurrentFaces[3], corner.CurrentFaces[4]) =
                (corner.CurrentFaces[3], corner.CurrentFaces[4], corner.CurrentFaces[0]);
            Require(!RubiksCubeController.IsSolved(CurrentState(cube)), "A twisted corner at home must be unsolved.");

            var (rotatedCube, rotatedController) = MakeCube();
            rotatedController.Scramble(new List<Move>
            {
                new(RubiksCube.Layer.Left, Move.RotationType.Counterclockwise),
                new(RubiksCube.Layer.MiddleX, Move.RotationType.Clockwise),
                new(RubiksCube.Layer.Right, Move.RotationType.Clockwise),
            });
            Require(FaceletCube.FromControllerState(rotatedCube).IsSolved(), "Whole rotation must preserve solved stickers.");
            Require(RubiksCubeController.IsSolved(CurrentState(rotatedCube)), "Whole rotation must count as solved.");
            Require(rotatedController.GetSolverMoves() && rotatedController.MoveQueue.Count == 0,
                "Rotated solved cube must need no queued moves.");
        }

        private static void TestRepeatedQueuedMove()
        {
            var (cube, controller) = MakeCube();
            var move = new Move(RubiksCube.Layer.Up, Move.RotationType.Clockwise);
            controller.MoveQueue.AddRange(new[] { move, move });
            controller.ExecuteMoveQueue();
            FinishAnimation(controller);
            var expected = FaceletCube.Solved();
            expected.ApplyMove(new SolverMove('U', 2));
            AssertSame(expected, FaceletCube.FromControllerState(cube), "Queue with reused move object");
            Require(controller.MoveQueue.Count == 0, "Completed queue must be empty.");
        }

        private static void TestReset()
        {
            var (cube, controller) = MakeCube();
            var initialVertices = cube.Cubelets.Select(c => c.Vertices.Select(v => v.Clone()).ToArray()).ToArray();
            controller.Scramble(SolverMove.ParseSequence("R U F2 L'").SelectMany(m => m.ToControllerMoves()).ToList());
            controller.MoveQueue.AddRange(SolverMove.ParseSequence("B D").SelectMany(m => m.ToControllerMoves()));
            controller.ExecuteMoveQueue();
            controller.Reset();
            Require(!controller.CurrentRotationInfo.IsRotating && !controller.CurrentRotationInfo.IsExecutingMoveQueue,
                "Reset must stop animation.");
            Require(controller.MoveQueue.Count == 0, "Reset must discard queued moves.");
            AssertSame(FaceletCube.Solved(), FaceletCube.FromControllerState(cube), "Reset stickers");
            for (int i = 0; i < cube.Cubelets.Count; i++)
            {
                Require(cube.Cubelets[i].CurrentPosition == cube.Cubelets[i].OriginalPosition, "Reset cubie position");
                for (int j = 0; j < initialVertices[i].Length; j++)
                {
                    var actual = cube.Cubelets[i].Vertices[j];
                    var expected = initialVertices[i][j];
                    Require(Math.Abs(actual.X - expected.X) < 1e-9 && Math.Abs(actual.Y - expected.Y) < 1e-9 &&
                        Math.Abs(actual.Z - expected.Z) < 1e-9, "Reset must restore rendered geometry.");
                }
            }
            controller.StartRotation(new Move(RubiksCube.Layer.Up, Move.RotationType.Counterclockwise));
            FinishAnimation(controller);
            var afterMove = FaceletCube.Solved();
            afterMove.ApplyMove(new SolverMove('U', -1));
            AssertSame(afterMove, FaceletCube.FromControllerState(cube), "Move after reset");
        }

        private static void TestSolve(IEnumerable<SolverMove> scramble)
        {
            var moves = scramble.ToList();
            try
            {
                TestControllerSolve(moves.SelectMany(move => move.ToControllerMoves()).ToList());
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"{ex.Message}\n    scramble: {string.Join(" ", moves)}", ex);
            }
        }

        private static void TestAsyncController()
        {
            var (cube, controller) = MakeCube();
            controller.Scramble(SolverMove.ParseSequence("F R U2 L' B D2").SelectMany(m => m.ToControllerMoves()).ToList());
            Require(controller.GetSolverMovesAsync().GetAwaiter().GetResult(), "Async solver should produce moves.");
            Require(controller.MoveQueue.Count > 0, "Async solver must enqueue its solution.");
            controller.ExecuteMoveQueue();
            FinishAnimation(controller);
            Require(FaceletCube.FromControllerState(cube).IsSolved(), "Animated async solution must solve all stickers.");

            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();
            ExpectException<OperationCanceledException>(() => controller.GetSolverMovesAsync(cancellation.Token).GetAwaiter().GetResult(),
                "Cancelled async solve");
            Require(controller.MoveQueue.Count == 0, "Cancelled async solve must leave no queued moves.");

            var (invalidCube, invalidController) = MakeCube();
            var corner = invalidCube.Cubelets.Single(c => c.CurrentPosition == ((sbyte)1, (sbyte)-1, (sbyte)1));
            (corner.CurrentFaces[0], corner.CurrentFaces[3], corner.CurrentFaces[4]) =
                (corner.CurrentFaces[3], corner.CurrentFaces[4], corner.CurrentFaces[0]);
            Require(!invalidController.GetSolverMovesAsync().GetAwaiter().GetResult(), "Invalid cube must fail asynchronously.");
            Require(!string.IsNullOrWhiteSpace(invalidController.SolverError) && invalidController.MoveQueue.Count == 0,
                "Failed async solve must report a reason and leave no queue.");

            // Hold the UI continuation until after Reset, so background results cannot overwrite it.
            var previousContext = SynchronizationContext.Current;
            var context = new QueuedSynchronizationContext();
            try
            {
                SynchronizationContext.SetSynchronizationContext(context);
                controller.Scramble(SolverMove.ParseSequence("B2 U F L2 D R'").SelectMany(m => m.ToControllerMoves()).ToList());
                var task = controller.GetSolverMovesAsync();
                bool wasPending = !task.IsCompleted;
                controller.Reset();
                context.Complete(task);
                if (wasPending) Require(!task.GetAwaiter().GetResult(), "Reset must reject an outstanding solver result.");
                Require(controller.MoveQueue.Count == 0, "Reset must not be followed by a stale solution queue.");
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(previousContext);
            }
        }

        private sealed class QueuedSynchronizationContext : SynchronizationContext
        {
            private readonly System.Collections.Concurrent.BlockingCollection<(SendOrPostCallback Callback, object? State)> callbacks = new();

            public override void Post(SendOrPostCallback callback, object? state) => callbacks.Add((callback, state));

            public void Complete(Task task)
            {
                var timeout = Stopwatch.StartNew();
                while (!task.IsCompleted && timeout.Elapsed < TimeSpan.FromSeconds(5))
                    if (callbacks.TryTake(out var callback, 10)) callback.Callback(callback.State);
                Require(task.IsCompleted, "Async controller did not complete within five seconds.");
            }
        }

        private static void TestControllerSolve(List<Move> scramble)
        {
            var (cube, controller) = MakeCube();
            controller.Scramble(scramble);
            var facelets = FaceletCube.FromControllerState(cube);
            var before = facelets.Clone();
            var solution = LayerByLayerSolver.Solve(facelets);
            AssertSame(before, facelets, "Solve must not mutate its input");
            var checkedSolution = facelets.Clone();
            checkedSolution.ApplyMoves(solution);
            Require(checkedSolution.IsSolved(), $"Solution leaves unsolved stickers: {string.Join(" ", solution)}");
            controller.Scramble(solution.SelectMany(move => move.ToControllerMoves()).ToList());
            Require(FaceletCube.FromControllerState(cube).IsSolved(), "Solution must solve all model stickers.");
            Require(RubiksCubeController.IsSolved(CurrentState(cube)), "Controller must recognize the solved model.");
        }

        private static List<SolverMove> RandomScramble(Random rng, int length)
        {
            var moves = new List<SolverMove>();
            char previous = '\0';
            for (int i = 0; i < length; i++)
            {
                char face;
                do { face = Faces[rng.Next(Faces.Length)]; } while (face == previous);
                moves.Add(new SolverMove(face, Quarters[rng.Next(Quarters.Length)]));
                previous = face;
            }
            return moves;
        }

        private static void FinishAnimation(RubiksCubeController controller)
        {
            for (int step = 0; step < 1000 && controller.CurrentRotationInfo.IsRotating; step++)
                controller.RotateStep();
            Require(!controller.CurrentRotationInfo.IsRotating, "Animation queue did not complete.");
        }

        private static (RubiksCube cube, RubiksCubeController controller) MakeCube()
        {
            var cube = new RubiksCube(40);
            return (cube, new RubiksCubeController(cube, new RotationInfo { AnimationTime = 1 }));
        }

        private static RubiksCubeState CurrentState(RubiksCube cube) =>
            new(cube.Cubelets.ToDictionary(c => c, c => c.CurrentPosition));

        private static void AssertSame(FaceletCube expected, FaceletCube actual, string context) =>
            Require(expected.Facelets.SequenceEqual(actual.Facelets),
                $"{context}\n    expected: {new string(expected.Facelets)}\n    actual:   {new string(actual.Facelets)}");

        private static void ExpectException<T>(Action action, string context) where T : Exception
        {
            try { action(); }
            catch (T) { return; }
            throw new InvalidOperationException($"{context} must throw {typeof(T).Name}.");
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }
    }
}
