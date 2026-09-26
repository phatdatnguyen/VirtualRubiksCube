using VirtualRubiksCube.Solver;

namespace VirtualRubiksCube.Tests;

[TestFixture]
[Category("Solver")]
public class LayerByLayerSolverTests
{
    private const string LegacyCrossScramble = "B2 F' D F F2 D' R2 D2 U2 B L U' R2 U R' D2 L2 L D2 D";

    [Test]
    public void Solve_SolvedCube_ReturnsEmptySolutionWithoutChangingInput()
    {
        var cube = FaceletCube.Solved();
        var before = cube.Clone();

        var solution = LayerByLayerSolver.Solve(cube);

        Assert.That(solution, Is.Empty);
        CubeTestSupport.AssertSame(before, cube);
    }

    [Test]
    public void Solve_NullInput_ThrowsArgumentNullException()
    {
        Assert.That((Action)(() => { LayerByLayerSolver.Solve(null!); }),
            Throws.ArgumentNullException.With.Property("ParamName").EqualTo("start"));
    }

    [TestCase("flipped edge")]
    [TestCase("twisted corner")]
    [TestCase("swapped edges")]
    public void Solve_ImpossibleCube_RejectsInputWithoutChangingIt(string invalidState)
    {
        var cube = FaceletCube.Solved();
        var stickers = cube.Facelets;
        switch (invalidState)
        {
            case "flipped edge":
                (stickers[7], stickers[19]) = (stickers[19], stickers[7]);
                break;
            case "twisted corner":
                (stickers[8], stickers[9], stickers[20]) = (stickers[9], stickers[20], stickers[8]);
                break;
            case "swapped edges":
                (stickers[19], stickers[10]) = (stickers[10], stickers[19]);
                break;
        }
        var before = cube.Clone();

        Assert.That((Action)(() => { LayerByLayerSolver.Solve(cube); }),
            Throws.TypeOf<InvalidOperationException>());

        CubeTestSupport.AssertSame(before, cube);
    }

    [TestCase("")]
    [TestCase("F R U2 L' B D2")]
    public void Solve_PreCancelledToken_ThrowsPreservesInputAndAllowsRetry(string notation)
    {
        var scramble = SolverMove.ParseSequence(notation);
        var cube = FaceletCube.Solved();
        cube.ApplyMoves(scramble);
        var before = cube.Clone();
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        // Deterministic whether the shared lookup tables are cold or already populated.
        Assert.That((Action)(() => { LayerByLayerSolver.Solve(cube, cancellation.Token); }),
            Throws.InstanceOf<OperationCanceledException>()
                .With.Property(nameof(OperationCanceledException.CancellationToken)).EqualTo(cancellation.Token));
        CubeTestSupport.AssertSame(before, cube);

        var solution = LayerByLayerSolver.Solve(cube);

        CubeTestSupport.AssertSame(before, cube);
        AssertSolutionSolvesBothModels(scramble, before, solution);
    }

    [TestCaseSource(nameof(SingleFaceMoves))]
    public void Solve_SingleFaceMove_SolvesCube(string notation)
    {
        AssertScrambleIsSolved(SolverMove.ParseSequence(notation));
    }

    [TestCaseSource(nameof(Patterns))]
    [Category("Regression")]
    public void Solve_KnownPattern_SolvesCube(string notation)
    {
        AssertScrambleIsSolved(SolverMove.ParseSequence(notation));
    }

    [TestCaseSource(nameof(SeededScrambles))]
    [Category("Regression")]
    public void Solve_SeededScramble_SolvesCube(int seed, int length)
    {
        // Preserve the original self-test corpus, with each case individually discoverable.
        AssertScrambleIsSolved(CubeTestSupport.RandomScramble(seed * 101 + length, length));
    }

    [Test]
    public async Task Solve_ConcurrentCalls_KeepInputsAndSolutionsIndependent()
    {
        var scrambles = new[] { 7, 42, 1701, 2026 }
            .Select(seed => CubeTestSupport.RandomScramble(seed, 50)).ToArray();
        var inputs = scrambles.Select(scramble =>
        {
            var cube = FaceletCube.Solved();
            cube.ApplyMoves(scramble);
            return cube;
        }).ToArray();
        var before = inputs.Select(cube => cube.Clone()).ToArray();

        var solutions = await Task.WhenAll(inputs.Select(cube =>
            Task.Run(() => LayerByLayerSolver.Solve(cube))));

        for (int i = 0; i < inputs.Length; i++)
        {
            CubeTestSupport.AssertSame(before[i], inputs[i]);
            AssertSolutionSolvesBothModels(scrambles[i], before[i], solutions[i]);
        }
    }

    private static IEnumerable<TestCaseData> SingleFaceMoves()
    {
        foreach (char face in "UDLRFB")
            foreach (string suffix in new[] { "", "'", "2" })
            {
                string notation = face + suffix;
                yield return new TestCaseData(notation).SetName($"Solve_SingleFaceMove_{notation}");
            }
    }

    private static IEnumerable<TestCaseData> Patterns()
    {
        yield return new TestCaseData("U2 D2 L2 R2 F2 B2").SetName("Solve_Checkerboard");
        yield return new TestCaseData("R U R' U' R U R' U'").SetName("Solve_RepeatedCornerCommutator");
        yield return new TestCaseData("F R U R' U' F'").SetName("Solve_LastLayerEdgeOrientation");
        yield return new TestCaseData("R U R' U R U2 R'").SetName("Solve_LastLayerCornerOrientation");
        yield return new TestCaseData("R U' R U R U R U' R' U' R2").SetName("Solve_LastLayerEdgeCycle");
        yield return new TestCaseData("U R2 F B R B2 R U2 L B2 R U' D' R2 F R' L B2 U2 F2")
            .SetName("Solve_Superflip");
        yield return new TestCaseData(LegacyCrossScramble).SetName("Solve_LegacyCrossLeftAnEdgeUnsolved");
    }

    private static IEnumerable<TestCaseData> SeededScrambles()
    {
        for (int seed = 0; seed < 128; seed++)
            foreach (int length in new[] { 5, 20, 50, 100 })
                yield return new TestCaseData(seed, length)
                    .SetName($"Solve_SeededScramble_Seed{seed:D3}_Length{length:D3}");
    }

    private static void AssertScrambleIsSolved(List<SolverMove> scramble)
    {
        var cube = FaceletCube.Solved();
        cube.ApplyMoves(scramble);
        var before = cube.Clone();

        var solution = LayerByLayerSolver.Solve(cube);

        CubeTestSupport.AssertSame(before, cube);
        AssertSolutionSolvesBothModels(scramble, before, solution);
    }

    private static void AssertSolutionSolvesBothModels(
        List<SolverMove> scramble, FaceletCube initial, List<SolverMove> solution)
    {
        string context = $"Scramble: {string.Join(" ", scramble)}\nSolution: {string.Join(" ", solution)}";
        var replay = initial.Clone();
        replay.ApplyMoves(solution);
        Assert.That(replay.IsSolved(), Is.True, $"Facelet replay failed. {context}");

        // Independently replay through the app's cubie positions and sticker orientations.
        // A solver and its own verifier could otherwise agree on the same incorrect move model.
        var (cube, controller) = CubeTestSupport.CreateCube();
        controller.Scramble(scramble.SelectMany(move => move.ToControllerMoves()).ToList());
        CubeTestSupport.AssertSame(initial, FaceletCube.FromControllerState(cube));
        controller.Scramble(solution.SelectMany(move => move.ToControllerMoves()).ToList());
        Assert.That(FaceletCube.FromControllerState(cube).IsSolved(), Is.True,
            $"Controller sticker replay failed. {context}");
        Assert.That(RubiksCubeController.IsSolved(CubeTestSupport.Snapshot(cube)), Is.True,
            $"Controller solved-state detection failed. {context}");
    }
}
