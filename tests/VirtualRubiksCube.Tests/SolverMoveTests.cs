using VirtualRubiksCube.Solver;

namespace VirtualRubiksCube.Tests;

[TestFixture]
public class SolverMoveTests
{
    private static IEnumerable<TestCaseData> Moves()
    {
        foreach (char face in "UDLRFB")
            foreach (int quarter in new[] { 1, -1, 2 })
                yield return new TestCaseData(face, quarter);
    }

    [TestCaseSource(nameof(Moves))]
    public void NotationRoundTrips(char face, int quarter)
    {
        string notation = face + (quarter == 1 ? "" : quarter == -1 ? "'" : "2");
        var parsed = SolverMove.Parse(notation);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(parsed.Face, Is.EqualTo(face));
            Assert.That(parsed.Quarter, Is.EqualTo(quarter));
            Assert.That(parsed.ToString(), Is.EqualTo(notation));
        }
    }

    [TestCaseSource(nameof(Moves))]
    public void InverseHasTheOppositeTurnAndIsAnInvolution(char face, int quarter)
    {
        var move = new SolverMove(face, quarter);
        var inverse = move.Inverse();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(inverse.Face, Is.EqualTo(face));
            Assert.That(inverse.Quarter, Is.EqualTo(quarter == 2 ? 2 : -quarter));
            Assert.That(inverse.Inverse(), Is.EqualTo(move));
        }
    }

    [TestCaseSource(nameof(Moves))]
    public void ControllerMappingUsesTheCorrectLayerDirectionAndTurnCount(char face, int quarter)
    {
        RubiksCube.Layer layer = face switch
        {
            'U' => RubiksCube.Layer.Up,
            'D' => RubiksCube.Layer.Down,
            'L' => RubiksCube.Layer.Left,
            'R' => RubiksCube.Layer.Right,
            'F' => RubiksCube.Layer.Front,
            'B' => RubiksCube.Layer.Back,
            _ => throw new AssertionException("Unexpected test face")
        };
        var controllerMoves = new SolverMove(face, quarter).ToControllerMoves().ToArray();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(controllerMoves, Has.Length.EqualTo(quarter == 2 ? 2 : 1));
            Assert.That(controllerMoves.Select(move => move.Layer), Is.All.EqualTo(layer));
            Assert.That(controllerMoves.Select(move => move.Type), Is.All.EqualTo(
                quarter == -1 ? Move.RotationType.Counterclockwise : Move.RotationType.Clockwise));
        }
    }

    [TestCase("")]
    [TestCase(" ")]
    [TestCase("X")]
    [TestCase("u")]
    [TestCase("U3")]
    [TestCase("U0")]
    [TestCase("Ufoo")]
    [TestCase("R2'")]
    [TestCase("F''")]
    [TestCase(" U")]
    [TestCase("U ")]
    [TestCase("U R")]
    public void ParseRejectsMalformedTokens(string token)
    {
        Assert.That((Action)(() => { SolverMove.Parse(token); }), Throws.TypeOf<FormatException>());
    }

    [Test]
    public void ParseRejectsNull()
    {
        Assert.That((Action)(() => { SolverMove.Parse(null!); }), Throws.TypeOf<ArgumentNullException>());
    }

    [TestCase('X')]
    [TestCase('u')]
    [TestCase(' ')]
    [TestCase('\0')]
    public void ConstructorRejectsInvalidFaces(char face)
    {
        Assert.That((Action)(() => { new SolverMove(face, 1); }), Throws.TypeOf<ArgumentException>());
    }

    [TestCase(-2)]
    [TestCase(0)]
    [TestCase(3)]
    [TestCase(4)]
    [TestCase(int.MinValue)]
    [TestCase(int.MaxValue)]
    public void ConstructorRejectsInvalidQuarterTurns(int quarter)
    {
        Assert.That((Action)(() => { new SolverMove('U', quarter); }), Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [TestCase("U R' F2")]
    [TestCase(" \tU R'\r\nF2  ")]
    [TestCase("U\nR'\tF2")]
    public void ParseSequenceAcceptsWhitespaceSeparators(string sequence)
    {
        Assert.That(SolverMove.ParseSequence(sequence).Select(move => move.ToString()),
            Is.EqualTo(new[] { "U", "R'", "F2" }));
    }

    [TestCase("")]
    [TestCase(" \t\r\n")]
    public void EmptySequenceHasNoMoves(string sequence)
    {
        Assert.That(SolverMove.ParseSequence(sequence), Is.Empty);
    }

    [TestCase("U R3 F")]
    [TestCase("U,R,F")]
    [TestCase("U r F")]
    public void ParseSequenceRejectsMalformedMoves(string sequence)
    {
        Assert.That((Action)(() => { SolverMove.ParseSequence(sequence); }), Throws.TypeOf<FormatException>());
    }

    [Test]
    public void ParseSequenceRejectsNull()
    {
        Assert.That((Action)(() => { SolverMove.ParseSequence(null!); }), Throws.TypeOf<ArgumentNullException>());
    }
}
