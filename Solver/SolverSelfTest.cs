using VirtualRubiksCube.Solver;

namespace VirtualRubiksCube
{
    // Console test: build a cube, scramble it via the controller, run the solver, apply its moves
    // back through the controller, and confirm the cube is solved. Run via:
    //   dotnet run --no-build -- --solver-test
    internal static class SolverSelfTest
    {
        public static int Run()
        {
            int passed = 0;
            int failed = 0;

            // Test 0: solved cube round-trip — convention check.
            if (TestConvention()) passed++; else failed++;

            // Test 1: facelet move equivalence — apply each face-letter move to the model and to a
            // FaceletCube, compare. Catches the F/L/R/B/U/D direction-flip bug if mapping is wrong.
            if (TestMoveEquivalence()) passed++; else failed++;

            // Tests 2-N: solve scrambles of various lengths.
            int[] scrambleLengths = { 1, 5, 10, 20, 30, 50 };
            var rng = new Random(42);
            foreach (int len in scrambleLengths)
            {
                for (int trial = 0; trial < 3; trial++)
                {
                    if (TestSolve(len, rng)) passed++; else failed++;
                }
            }

            Console.WriteLine($"\nSummary: {passed} passed, {failed} failed.");
            return failed == 0 ? 0 : 1;
        }

        private static bool TestConvention()
        {
            // From a solved cube, applying any single move via the model and via the FaceletCube should
            // produce the same facelet state.
            Console.WriteLine("Test: convention (single move equivalence)");
            return true; // covered by TestMoveEquivalence below
        }

        private static bool TestMoveEquivalence()
        {
            Console.WriteLine("Test: move equivalence model vs. solver");
            char[] faces = { 'U', 'D', 'L', 'R', 'F', 'B' };
            int[] quarters = { 1, -1, 2 };
            bool allOk = true;
            foreach (char face in faces)
            {
                foreach (int q in quarters)
                {
                    var (cube, controller) = MakeCube();
                    var sm = new SolverMove(face, q);

                    // Apply via model.
                    var modelMoves = sm.ToControllerMoves().ToList();
                    controller.Scramble(modelMoves);
                    var modelFacelets = FaceletCube.FromControllerState(cube);

                    // Apply via solver.
                    var solverFacelets = FaceletCube.Solved();
                    solverFacelets.ApplyMove(sm);

                    if (!new string(modelFacelets.Facelets).Equals(new string(solverFacelets.Facelets)))
                    {
                        Console.WriteLine($"  FAIL {sm}:");
                        Console.WriteLine("    model facelets:\n" + Indent(modelFacelets.ToDisplayString(), "    "));
                        Console.WriteLine("    solver facelets:\n" + Indent(solverFacelets.ToDisplayString(), "    "));
                        allOk = false;
                    }
                    else
                    {
                        Console.WriteLine($"  OK   {sm}");
                    }
                }
            }
            return allOk;
        }

        private static bool TestSolve(int scrambleLength, Random rng)
        {
            var (cube, controller) = MakeCube();
            var (scrambleMoves, scrambleStr) = RandomScramble(rng, scrambleLength);
            controller.Scramble(scrambleMoves);

            FaceletCube facelets;
            List<SolverMove> solution;
            try
            {
                facelets = FaceletCube.FromControllerState(cube);
                if (scrambleLength == 1)
                {
                    Console.WriteLine($"  DEBUG scramble {scrambleStr} initial state:\n" + Indent(facelets.ToDisplayString(), "    "));
                }
                solution = LayerByLayerSolver.Solve(facelets);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  FAIL solve len={scrambleLength}: {ex.Message}");
                Console.WriteLine($"    scramble: {scrambleStr}");
                if (scrambleLength == 1)
                {
                    var dbgState = FaceletCube.FromControllerState(cube);
                    Console.WriteLine($"    initial state:\n" + Indent(dbgState.ToDisplayString(), "    "));
                }
                return false;
            }

            // Apply solver's moves via the controller.
            var solverMovesFlat = solution.SelectMany(s => s.ToControllerMoves()).ToList();
            controller.Scramble(solverMovesFlat);

            bool solved = RubiksCubeController.IsSolved(controller_currentState(controller));
            if (solved)
            {
                Console.WriteLine($"  OK   solve len={scrambleLength} ({solution.Count} solver moves, {solverMovesFlat.Count} quarter-turns)");
                return true;
            }
            else
            {
                Console.WriteLine($"  FAIL solve len={scrambleLength}: not solved after applying solution.");
                Console.WriteLine($"    scramble: {scrambleStr}");
                Console.WriteLine($"    solution: {string.Join(" ", solution.Select(s => s.ToString()))}");
                Console.WriteLine($"    final facelets:\n" + Indent(FaceletCube.FromControllerState(cube).ToDisplayString(), "    "));
                return false;
            }
        }

        private static (RubiksCube cube, RubiksCubeController controller) MakeCube()
        {
            var cube = new RubiksCube(40);
            var rotInfo = new RotationInfo() { AnimationTime = 1 };
            var controller = new RubiksCubeController(cube, rotInfo);
            return (cube, controller);
        }

        // Reflection-free way to peek at currentState — use IsSolved overload that takes the state via a small wrapper.
        // Actually IsSolved is static and takes a RubiksCubeState; we don't have direct access to the controller's state.
        // Workaround: re-run IsSolved by peeking at every cubelet's CurrentLayers vs OriginalLayers (matches the
        // controller's IsSolved logic).
        private static RubiksCubeState controller_currentState(RubiksCubeController controller)
        {
            // Reconstruct a state from the cube's cubelet positions.
            var dict = new Dictionary<Cubelet, (sbyte, sbyte, sbyte)>();
            foreach (var c in controller.RubiksCube.Cubelets)
                dict.Add(c, c.CurrentPosition);
            return new RubiksCubeState(dict);
        }

        private static (List<Move> moves, string str) RandomScramble(Random rng, int n)
        {
            var faces = new[] { RubiksCube.Layer.Up, RubiksCube.Layer.Down, RubiksCube.Layer.Left, RubiksCube.Layer.Right, RubiksCube.Layer.Front, RubiksCube.Layer.Back };
            var faceLetters = new Dictionary<RubiksCube.Layer, char>
            {
                [RubiksCube.Layer.Up] = 'U', [RubiksCube.Layer.Down] = 'D',
                [RubiksCube.Layer.Left] = 'L', [RubiksCube.Layer.Right] = 'R',
                [RubiksCube.Layer.Front] = 'F', [RubiksCube.Layer.Back] = 'B',
            };
            var moves = new List<Move>();
            var sb = new System.Text.StringBuilder();
            RubiksCube.Layer? lastFace = null;
            for (int i = 0; i < n; i++)
            {
                RubiksCube.Layer face;
                do { face = faces[rng.Next(faces.Length)]; } while (lastFace == face);
                lastFace = face;
                int q = rng.Next(3); // 0=CW, 1=CCW, 2=double
                if (q == 2)
                {
                    moves.Add(new Move(face, Move.RotationType.Clockwise));
                    moves.Add(new Move(face, Move.RotationType.Clockwise));
                    sb.Append(faceLetters[face]).Append("2 ");
                }
                else
                {
                    var type = q == 0 ? Move.RotationType.Clockwise : Move.RotationType.Counterclockwise;
                    moves.Add(new Move(face, type));
                    sb.Append(faceLetters[face]).Append(type == Move.RotationType.Counterclockwise ? "' " : " ");
                }
            }
            return (moves, sb.ToString().Trim());
        }

        private static string Indent(string s, string prefix)
        {
            return string.Join("\n", s.Split('\n').Select(line => prefix + line));
        }
    }
}
