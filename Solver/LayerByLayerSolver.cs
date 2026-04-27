using System.Text;

namespace VirtualRubiksCube.Solver
{
    // Beginner's layer-by-layer solver. Operates on a FaceletCube clone, returns the move list.
    // Solutions are 80-200 moves; not optimal but correct from any valid 3x3 state.
    //
    // Each phase iterates until a sub-goal is met. Algorithms are written in the "target = F" frame
    // (i.e., assume the piece we're working on belongs at the front face); for other targets we
    // rotate the algorithm string by substituting F/R/B/L letters cyclically. U and D are invariant
    // under y-rotation so they pass through unchanged.
    public static class LayerByLayerSolver
    {
        public static List<SolverMove> Solve(FaceletCube start)
        {
            var c = start.Clone();
            var moves = new List<SolverMove>();
            SolveCross(c, moves);
            SolveBottomCorners(c, moves);
            SolveMiddleEdges(c, moves);
            OrientYellowEdges(c, moves);
            PermuteYellowEdges(c, moves);
            PermuteYellowCorners(c, moves);
            OrientYellowCorners(c, moves);
            if (!c.IsSolved())
                throw new InvalidOperationException("LBL solver did not finish; final state:\n" + c.ToDisplayString());
            return moves;
        }

        // ---------- helpers ----------

        private static void Apply(FaceletCube c, List<SolverMove> moves, string seq)
        {
            foreach (var m in SolverMove.ParseSequence(seq))
            {
                c.ApplyMove(m);
                moves.Add(m);
            }
        }

        // BFS over an abstract phase-state (defined by the hash function).
        // moveSequences are tokenized strings; each is treated as ONE BFS edge (executed atomically).
        // Applies the found path to the live cube + moves list.
        private static void BfsApply(FaceletCube c, List<SolverMove> moves,
            Func<FaceletCube, long> hash, Func<FaceletCube, bool> goal, string[] moveSequences, int maxDepth, string phaseName)
        {
            if (goal(c)) return;

            var parsedSeqs = moveSequences.Select(s => SolverMove.ParseSequence(s).ToArray()).ToArray();
            var queue = new Queue<(FaceletCube state, List<int> path)>();
            var visited = new HashSet<long> { hash(c) };
            queue.Enqueue((c.Clone(), new List<int>()));

            while (queue.Count > 0)
            {
                var (state, path) = queue.Dequeue();
                if (path.Count >= maxDepth) continue;
                for (int seqIdx = 0; seqIdx < parsedSeqs.Length; seqIdx++)
                {
                    var ns = state.Clone();
                    foreach (var m in parsedSeqs[seqIdx]) ns.ApplyMove(m);
                    long h = hash(ns);
                    if (visited.Contains(h)) continue;
                    visited.Add(h);
                    var np = new List<int>(path) { seqIdx };
                    if (goal(ns))
                    {
                        foreach (int idx in np)
                            foreach (var m in parsedSeqs[idx])
                            {
                                c.ApplyMove(m);
                                moves.Add(m);
                            }
                        return;
                    }
                    queue.Enqueue((ns, np));
                }
            }

            throw new InvalidOperationException($"{phaseName} did not converge.");
        }

        // Pack a facelet character (U/D/L/R/F/B) into 3 bits.
        private static long FaceletBits(char c) => c switch
        {
            'U' => 0L, 'D' => 1L, 'L' => 2L, 'R' => 3L, 'F' => 4L, 'B' => 5L, _ => 7L,
        };

        // Substitute side-face letters to rotate an algorithm into a different target frame.
        // ccwY is the number of CCW y-rotations applied to the cube before running the algo.
        private static string Rotate(string algo, int ccwY)
        {
            char[] map = (((ccwY % 4) + 4) % 4) switch
            {
                0 => new[] { 'F', 'R', 'B', 'L' },
                1 => new[] { 'L', 'F', 'R', 'B' },
                2 => new[] { 'B', 'L', 'F', 'R' },
                3 => new[] { 'R', 'B', 'L', 'F' },
                _ => new[] { 'F', 'R', 'B', 'L' },
            };
            var sb = new StringBuilder();
            foreach (char ch in algo)
            {
                sb.Append(ch switch
                {
                    'F' => map[0],
                    'R' => map[1],
                    'B' => map[2],
                    'L' => map[3],
                    _ => ch,
                });
            }
            return sb.ToString();
        }

        // 12 edge slots: indexed UF=0, UR=1, UB=2, UL=3, FR=4, FL=5, BR=6, BL=7, DF=8, DR=9, DB=10, DL=11.
        // Each tuple = the two facelet positions (A on U/D face for top/bottom slots; otherwise both side faces).
        private static readonly (int A, int B)[] EdgeFacelets =
        {
            (7, 19),    // UF: U7, F1
            (5, 10),    // UR: U5, R1
            (1, 46),    // UB: U1, B1
            (3, 37),    // UL: U3, L1
            (23, 12),   // FR: F5, R3
            (21, 41),   // FL: F3, L5
            (14, 48),   // BR: R5, B3
            (39, 50),   // BL: L3, B5
            (28, 25),   // DF: D1, F7
            (32, 16),   // DR: D5, R7
            (34, 52),   // DB: D7, B7
            (30, 43),   // DL: D3, L7
        };

        private static (int slot, bool aFirst) FindEdge(FaceletCube c, char a, char b)
        {
            for (int i = 0; i < 12; i++)
            {
                char x = c[EdgeFacelets[i].A], y = c[EdgeFacelets[i].B];
                if (x == a && y == b) return (i, true);
                if (x == b && y == a) return (i, false);
            }
            throw new InvalidOperationException($"Edge {a}{b} not found");
        }

        // 8 corner slots. Each tuple has 3 facelet positions in a fixed canonical order:
        //   - .A = U or D face sticker
        //   - .B = first side-face sticker (clockwise around top/bottom)
        //   - .C = second side-face sticker
        // Corner names: UFL=0, UFR=1, UBR=2, UBL=3, DFL=4, DFR=5, DBR=6, DBL=7.
        private static readonly (int A, int B, int C)[] CornerFacelets =
        {
            (6, 18, 38),    // UFL: U6, F0, L2
            (8, 20, 9),     // UFR: U8, F2, R0
            (2, 11, 45),    // UBR: U2, R2, B0
            (0, 47, 36),    // UBL: U0, B2, L0
            (27, 24, 44),   // DFL: D0, F6, L8
            (29, 26, 15),   // DFR: D2, F8, R6
            (35, 17, 51),   // DBR: D8, R8, B6
            (33, 53, 42),   // DBL: D6, B8, L6
        };

        // Find a corner with the given color triple (any order), returning slot and a 3-element rotation offset.
        // The offset r ∈ {0, 1, 2} indicates how the target's A-color is positioned on the slot:
        //   r=0: target A on slot.A, B on slot.B, C on slot.C
        //   r=1: target A on slot.B, B on slot.C, C on slot.A
        //   r=2: target A on slot.C, B on slot.A, C on slot.B
        // (a, b, c) is the natural ordering for the target corner.
        private static (int slot, int rot) FindCorner(FaceletCube c, char a, char b, char ccol)
        {
            for (int i = 0; i < 8; i++)
            {
                char x = c[CornerFacelets[i].A];
                char y = c[CornerFacelets[i].B];
                char z = c[CornerFacelets[i].C];
                // Check if {x,y,z} == {a,b,c} as multisets, and find the rotation that aligns a to A.
                if (x == a && y == b && z == ccol) return (i, 0);
                if (z == a && x == b && y == ccol) return (i, 1);
                if (y == a && z == b && x == ccol) return (i, 2);
                // Mirror orderings (the corner's other 3 cyclic positions). Standard cubies have only one chirality, so these shouldn't occur on a valid cube — but handle for safety.
                if (x == a && z == b && y == ccol) return (i, 0);
                if (y == a && x == b && z == ccol) return (i, 2);
                if (z == a && y == b && x == ccol) return (i, 1);
            }
            throw new InvalidOperationException($"Corner {a}{b}{ccol} not found");
        }

        // ---------- Phase 1: cross (D layer edges) ----------

        private static char NextCw(char face) => face switch
        {
            'F' => 'R', 'R' => 'B', 'B' => 'L', 'L' => 'F',
            _ => face,
        };

        private static int CrossEdgeFacelet_D(char target) => target switch
        {
            'F' => 28, 'R' => 32, 'B' => 34, 'L' => 30, _ => -1,
        };

        private static int CrossEdgeFacelet_Side(char target) => target switch
        {
            'F' => 25, 'R' => 16, 'B' => 52, 'L' => 43, _ => -1,
        };

        // Map target letter F/R/B/L to its U-layer slot index (UF=0, UR=1, UB=2, UL=3).
        private static int UTargetSlot(char target) => target switch
        {
            'F' => 0, 'R' => 1, 'B' => 2, 'L' => 3, _ => -1,
        };

        private static int DTargetSlot(char target) => target switch
        {
            'F' => 8, 'R' => 9, 'B' => 10, 'L' => 11, _ => -1,
        };

        private static void SolveCross(FaceletCube c, List<SolverMove> moves)
        {
            foreach (char target in new[] { 'F', 'R', 'B', 'L' })
                SolveCrossEdge(c, moves, target);
        }

        private static void SolveCrossEdge(FaceletCube c, List<SolverMove> moves, char target)
        {
            int dF = CrossEdgeFacelet_D(target);
            int sF = CrossEdgeFacelet_Side(target);
            int targetUSlot = UTargetSlot(target);
            int targetDSlot = DTargetSlot(target);
            char rightFace = NextCw(target);

            for (int it = 0; it < 30; it++)
            {
                if (c[dF] == 'D' && c[sF] == target) return;

                var (slot, dFirst) = FindEdge(c, 'D', target);

                if (slot == targetDSlot)
                {
                    // In target slot but flipped — bring up with target+"2", will re-handle as UF flipped.
                    Apply(c, moves, target + "2");
                    continue;
                }
                if (slot >= 8 && slot <= 11)
                {
                    // Wrong D slot. Bring up with that slot's face turn (twice).
                    char face = slot switch { 8 => 'F', 9 => 'R', 10 => 'B', 11 => 'L', _ => 'F' };
                    Apply(c, moves, face + "2");
                    continue;
                }
                if (slot >= 4 && slot <= 7)
                {
                    // Middle layer. One move brings to U.
                    string m = slot switch
                    {
                        4 => "R",   // FR -> UR
                        5 => "L'",  // FL -> UL
                        6 => "R'",  // BR -> UR
                        7 => "L",   // BL -> UL
                        _ => "",
                    };
                    Apply(c, moves, m);
                    continue;
                }

                // U layer (slot 0..3). Bring to target U slot via U turns.
                int relIdx = ((slot - targetUSlot) + 4) % 4;
                if (relIdx > 0)
                {
                    string uMoves = relIdx switch { 1 => "U", 2 => "U2", 3 => "U'", _ => "" };
                    Apply(c, moves, uMoves);
                    continue;
                }

                // Now at target U slot. Place based on orient.
                // dFirst => D is on the slot's A facelet (which is U-face for U slots) => D on top.
                if (dFirst)
                    Apply(c, moves, target + "2");
                else
                    Apply(c, moves, $"U' {rightFace}' {target} {rightFace}");
            }
            throw new InvalidOperationException($"Cross edge for target {target} did not solve.");
        }

        // ---------- Phase 2: bottom (D) corners ----------

        // Corner targets in order: DFR=5, DFL=4, DBR=6, DBL=7.
        // Each target has an associated y-rotation count to bring it conceptually to DFR.
        // DFR: y0. DFL: y1 (CCW). DBL: y2. DBR: y3.
        private static readonly (int slot, int yRot)[] BottomCornerTargets =
        {
            (5, 0),  // DFR (in F frame)
            (4, 1),  // DFL (rotate cube CCW once to make L face become F)
            (7, 2),  // DBL
            (6, 3),  // DBR
        };

        private static void SolveBottomCorners(FaceletCube c, List<SolverMove> moves)
        {
            // For each of the 4 D corners in fixed order.
            foreach (var (targetSlot, yRot) in BottomCornerTargets)
                SolveBottomCorner(c, moves, targetSlot, yRot);
        }

        // The colors of the corner at slot s (in the corner's canonical (A, B, C) facelet order on a solved cube).
        // We use this to identify the cubie regardless of where it currently sits.
        private static (char a, char b, char ccol) CornerCanonicalColors(int slot) => slot switch
        {
            0 => ('U', 'F', 'L'),
            1 => ('U', 'F', 'R'),
            2 => ('U', 'R', 'B'),
            3 => ('U', 'B', 'L'),
            4 => ('D', 'F', 'L'),
            5 => ('D', 'F', 'R'),
            6 => ('D', 'R', 'B'),
            7 => ('D', 'B', 'L'),
            _ => ('?', '?', '?'),
        };

        private static bool CornerSolved(FaceletCube c, int targetSlot)
        {
            var (a, b, cc) = CornerCanonicalColors(targetSlot);
            var (sA, sB, sC) = CornerFacelets[targetSlot];
            return c[sA] == a && c[sB] == b && c[sC] == cc;
        }

        private static void SolveBottomCorner(FaceletCube c, List<SolverMove> moves, int targetSlot, int yRot)
        {
            // Algorithms below are written for target = DFR (yRot=0). They are rotated for other targets.
            // Beginner method: get corner above its slot in the U layer at UFR (slot 1), then apply
            // "R U R' U'" repeatedly until placed.
            var (a, b, cc) = CornerCanonicalColors(targetSlot);

            for (int it = 0; it < 30; it++)
            {
                if (CornerSolved(c, targetSlot)) return;

                var (slot, _) = FindCorner(c, a, b, cc);

                if (slot == targetSlot)
                {
                    // In correct slot but wrong orient — pop it out via one rotated R U R' U'.
                    Apply(c, moves, Rotate("R U R' U'", yRot));
                    continue;
                }
                if (slot >= 4 && slot <= 7)
                {
                    // In some other D slot. Pop out by applying the slot's R U R' U' (in that slot's frame).
                    // Compute that slot's yRot relative to F-frame:
                    int wrongYRot = slot switch { 4 => 1, 5 => 0, 6 => 3, 7 => 2, _ => 0 };
                    Apply(c, moves, Rotate("R U R' U'", wrongYRot));
                    continue;
                }

                // In U layer (slot 0..3). Rotate U to bring it above target.
                // Target's "above" U slot is targetSlot - 4 (e.g., DFR=5 → UFR=1).
                int targetUSlot_corner = targetSlot - 4;
                int relIdx = ((slot - targetUSlot_corner) + 4) % 4;
                if (relIdx > 0)
                {
                    string uMoves = relIdx switch { 1 => "U", 2 => "U2", 3 => "U'", _ => "" };
                    Apply(c, moves, uMoves);
                    continue;
                }

                // Now above target. Apply R U R' U' (rotated to target frame).
                Apply(c, moves, Rotate("R U R' U'", yRot));
            }
            throw new InvalidOperationException($"Bottom corner at slot {targetSlot} did not solve.");
        }

        // ---------- Phase 3: middle layer edges ----------

        // 4 middle edges: FR=4, BR=6, BL=7, FL=5. Each has a y-rotation:
        //   FR: yRot=0. BR: yRot=3 (clockwise once). BL: yRot=2. FL: yRot=1.
        // (slot, yRot, a, b) where a is the color expected on EdgeFacelets[slot].A and b on .B.
        private static readonly (int slot, int yRot, char a, char b)[] MiddleEdgeTargets =
        {
            (4, 0, 'F', 'R'),  // FR: F5='F', R3='R'
            (6, 3, 'R', 'B'),  // BR: R5='R', B3='B'
            (7, 2, 'L', 'B'),  // BL: L3='L', B5='B'
            (5, 1, 'F', 'L'),  // FL: F3='F', L5='L'
        };

        private static void SolveMiddleEdges(FaceletCube c, List<SolverMove> moves)
        {
            foreach (var (slot, yRot, a, b) in MiddleEdgeTargets)
                SolveMiddleEdge(c, moves, slot, yRot, a, b);
        }

        private static bool MiddleEdgeSolved(FaceletCube c, int slot, char a, char b)
        {
            var (sA, sB) = EdgeFacelets[slot];
            return c[sA] == a && c[sB] == b;
        }

        private static void SolveMiddleEdge(FaceletCube c, List<SolverMove> moves, int targetSlot, int yRot, char a, char b)
        {
            for (int it = 0; it < 25; it++)
            {
                if (MiddleEdgeSolved(c, targetSlot, a, b)) return;

                var (slot, aFirst) = FindEdge(c, a, b);

                if (slot >= 4 && slot <= 7)
                {
                    // In a middle slot (possibly the target, but wrong orient if not solved).
                    // Pop it out to U with right-insert in that slot's frame.
                    int wrongYRot = slot switch { 4 => 0, 5 => 1, 6 => 3, 7 => 2, _ => 0 };
                    Apply(c, moves, Rotate("U R U' R' U' F' U F", wrongYRot));
                    continue;
                }
                if (slot >= 8 && slot <= 11)
                {
                    // Wrong D-layer slot — shouldn't happen if cross/bottom corners are correct, but if it does,
                    // pull it up via face2.
                    char face = slot switch { 8 => 'F', 9 => 'R', 10 => 'B', 11 => 'L', _ => 'F' };
                    Apply(c, moves, face + "2");
                    continue;
                }

                // In U layer. Find the side-color sticker (the one not facing top).
                // EdgeFacelets[slot=0..3] = (U_facelet, side_facelet). aFirst=true means a is on U facelet.
                char sideColor = aFirst ? b : a;

                // Rotate U so the side-color sticker faces its matching center.
                // sideColor is one of F, R, B, L.
                int sideIndex = sideColor switch { 'F' => 0, 'R' => 1, 'B' => 2, 'L' => 3, _ => -1 };
                int relIdx = ((slot - sideIndex) + 4) % 4;
                if (relIdx > 0)
                {
                    string uMoves = relIdx switch { 1 => "U", 2 => "U2", 3 => "U'", _ => "" };
                    Apply(c, moves, uMoves);
                    continue;
                }

                // Now side sticker faces its center. Decide right or left insertion based on where target is.
                // The U-facing sticker is the OTHER color. Target slot's edge: need this top color matches the
                // CW or CCW neighbor's center.
                char topColor = aFirst ? a : b;
                char rightNeighbor = NextCw(sideColor);
                char leftNeighbor = NextCw(NextCw(NextCw(sideColor)));
                if (topColor == rightNeighbor)
                {
                    // Insert to the right: in side's frame, U R U' R' U' F' U F.
                    int sideYRot = sideColor switch { 'F' => 0, 'L' => 1, 'B' => 2, 'R' => 3, _ => 0 };
                    Apply(c, moves, Rotate("U R U' R' U' F' U F", sideYRot));
                }
                else if (topColor == leftNeighbor)
                {
                    // Insert to the left: U' L' U L U F U' F'.
                    int sideYRot = sideColor switch { 'F' => 0, 'L' => 1, 'B' => 2, 'R' => 3, _ => 0 };
                    Apply(c, moves, Rotate("U' L' U L U F U' F'", sideYRot));
                }
                else
                {
                    // Shouldn't happen — top color must be a neighbor.
                    throw new InvalidOperationException($"Middle edge insertion: unexpected top color {topColor} side {sideColor}.");
                }
            }
            throw new InvalidOperationException($"Middle edge {a}{b} did not solve.");
        }

        // ---------- Phase 4: orient last (U) layer edges ----------
        // Goal: U face's 4 edge stickers are all 'U'.
        private static void OrientYellowEdges(FaceletCube c, List<SolverMove> moves)
        {
            BfsApply(c, moves,
                hash: cube => (long)(
                    ((cube.Facelets[1] == 'U' ? 1 : 0) << 0) |
                    ((cube.Facelets[3] == 'U' ? 1 : 0) << 1) |
                    ((cube.Facelets[5] == 'U' ? 1 : 0) << 2) |
                    ((cube.Facelets[7] == 'U' ? 1 : 0) << 3)),
                goal: cube => cube.Facelets[1] == 'U' && cube.Facelets[3] == 'U' && cube.Facelets[5] == 'U' && cube.Facelets[7] == 'U',
                moveSequences: new[] { "U", "U'", "U2", "F R U R' U' F'" },
                maxDepth: 8,
                phaseName: "Yellow cross orientation");
        }

        // ---------- Phase 5: permute U edges ----------
        private static void PermuteYellowEdges(FaceletCube c, List<SolverMove> moves)
        {
            // BFS over (4 U-side facelets) state. Each is one of {F,R,B,L} after phase 4 ensures U on top.
            BfsApply(c, moves,
                hash: cube => (FaceletBits(cube.Facelets[19]) << 0)   // F1
                            | (FaceletBits(cube.Facelets[10]) << 3)   // R1
                            | (FaceletBits(cube.Facelets[46]) << 6)   // B1
                            | (FaceletBits(cube.Facelets[37]) << 9),  // L1
                goal: cube => cube.Facelets[19] == 'F' && cube.Facelets[10] == 'R' && cube.Facelets[46] == 'B' && cube.Facelets[37] == 'L',
                moveSequences: new[] { "U", "U'", "U2", "R U R' U R U2 R'", "L' U' L U' L' U2 L" },
                maxDepth: 14,
                phaseName: "Yellow edge permutation");
        }

        // ---------- Phase 6: permute U corners ----------
        private static void PermuteYellowCorners(FaceletCube c, List<SolverMove> moves)
        {
            // Hash: at each U-corner slot, identify the corner by its non-U color set.
            // 4 possible color pairs: {F,L}=0, {F,R}=1, {R,B}=2, {B,L}=3.
            // Plus we need to keep edges solved — hash includes side-edge facelets too.
            BfsApply(c, moves,
                hash: cube =>
                {
                    long h = 0;
                    for (int s = 0; s < 4; s++)
                    {
                        char x = cube.Facelets[CornerFacelets[s].A];
                        char y = cube.Facelets[CornerFacelets[s].B];
                        char z = cube.Facelets[CornerFacelets[s].C];
                        // The non-U colors among {x,y,z}.
                        char p = '?', q = '?';
                        if (x != 'U') { p = x; }
                        if (y != 'U') { if (p == '?') p = y; else q = y; }
                        if (z != 'U') { if (p == '?') p = z; else q = z; }
                        // Sort lexicographically.
                        if (p > q) { var t = p; p = q; q = t; }
                        long pair = (p, q) switch
                        {
                            ('F', 'L') => 0L,
                            ('F', 'R') => 1L,
                            ('B', 'R') => 2L,
                            ('B', 'L') => 3L,
                            _ => 7L,
                        };
                        h |= pair << (s * 3);
                    }
                    // Include U-side-edge facelets so phase 5 progress isn't lost.
                    h |= FaceletBits(cube.Facelets[19]) << 12;
                    h |= FaceletBits(cube.Facelets[10]) << 15;
                    h |= FaceletBits(cube.Facelets[46]) << 18;
                    h |= FaceletBits(cube.Facelets[37]) << 21;
                    return h;
                },
                goal: cube =>
                {
                    for (int s = 0; s < 4; s++)
                        if (!UCornerCorrectPos(cube, s)) return false;
                    return cube.Facelets[19] == 'F' && cube.Facelets[10] == 'R' && cube.Facelets[46] == 'B' && cube.Facelets[37] == 'L';
                },
                moveSequences: new[] { "U", "U'", "U2", "U R U' L' U R' U' L", "U' L' U R U' L U R'" },
                maxDepth: 12,
                phaseName: "Yellow corner permutation");
        }

        private static bool UCornerCorrectPos(FaceletCube cube, int slot)
        {
            var (a, b, cc) = CornerCanonicalColors(slot);
            var (sA, sB, sC) = CornerFacelets[slot];
            char x = cube[sA], y = cube[sB], z = cube[sC];
            char[] expected = { a, b, cc };
            char[] actual = { x, y, z };
            Array.Sort(expected); Array.Sort(actual);
            return expected[0] == actual[0] && expected[1] == actual[1] && expected[2] == actual[2];
        }

        // ---------- Phase 7: orient U corners ----------
        private static void OrientYellowCorners(FaceletCube c, List<SolverMove> moves)
        {
            // Hash: U-face stickers at the 4 U-corner positions + side-edge alignment.
            // For each corner, encode whether U-color is on top, on first side, or on second side (3 states).
            BfsApply(c, moves,
                hash: cube =>
                {
                    long h = 0;
                    for (int s = 0; s < 4; s++)
                    {
                        var (sA, sB, sC) = CornerFacelets[s];
                        long ori;
                        if (cube.Facelets[sA] == 'U') ori = 0;
                        else if (cube.Facelets[sB] == 'U') ori = 1;
                        else ori = 2;
                        h |= ori << (s * 2);
                    }
                    h |= FaceletBits(cube.Facelets[19]) << 8;
                    h |= FaceletBits(cube.Facelets[10]) << 11;
                    h |= FaceletBits(cube.Facelets[46]) << 14;
                    h |= FaceletBits(cube.Facelets[37]) << 17;
                    return h;
                },
                goal: cube => cube.IsSolved(),
                moveSequences: new[] { "U", "U'", "U2", "R' D' R D", "R' D' R D R' D' R D" },
                maxDepth: 28,
                phaseName: "Yellow corner orientation");
        }
    }
}
