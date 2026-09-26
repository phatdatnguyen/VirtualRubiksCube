namespace VirtualRubiksCube.Tests;

[TestFixture]
public class MoveTests
{
    [TestCase(RubiksCube.Layer.Up, RubiksCube.Axis.Y, -90)]
    [TestCase(RubiksCube.Layer.MiddleY, RubiksCube.Axis.Y, -90)]
    [TestCase(RubiksCube.Layer.Down, RubiksCube.Axis.Y, 90)]
    [TestCase(RubiksCube.Layer.Left, RubiksCube.Axis.X, -90)]
    [TestCase(RubiksCube.Layer.MiddleX, RubiksCube.Axis.X, 90)]
    [TestCase(RubiksCube.Layer.Right, RubiksCube.Axis.X, 90)]
    [TestCase(RubiksCube.Layer.Front, RubiksCube.Axis.Z, 90)]
    [TestCase(RubiksCube.Layer.MiddleZ, RubiksCube.Axis.Z, 90)]
    [TestCase(RubiksCube.Layer.Back, RubiksCube.Axis.Z, -90)]
    public void MoveAndCounterMove_HaveExpectedAxisAndOppositeAngles(RubiksCube.Layer layer, RubiksCube.Axis axis, int angle)
    {
        var move = new Move(layer, Move.RotationType.Clockwise);
        var counter = move.GetCounterMove();

        Assert.That(move.Axis, Is.EqualTo(axis));
        Assert.That(move.TargetAngle, Is.EqualTo(angle));
        Assert.That(counter.Axis, Is.EqualTo(axis));
        Assert.That(counter.Layer, Is.EqualTo(layer));
        Assert.That(counter.TargetAngle, Is.EqualTo(-angle));
        Assert.That(counter.Type, Is.EqualTo(Move.RotationType.Counterclockwise));
        Assert.That(counter.GetCounterMove().Type, Is.EqualTo(move.Type));
        Assert.That(move.IsCounterMove(counter), Is.True);
        Assert.That(counter.IsCounterMove(move), Is.True);
        Assert.That(move.IsCounterMove(move), Is.False);
    }

    [Test]
    public void OppositeDirectionOnDifferentLayer_IsNotCounterMove() =>
        Assert.That(new Move(RubiksCube.Layer.Up, Move.RotationType.Clockwise)
            .IsCounterMove(new Move(RubiksCube.Layer.Down, Move.RotationType.Counterclockwise)), Is.False);

    [TestCase(90, 15, 6)]
    [TestCase(-90, -15, 6)]
    public void RotationInfo_TracksSignedProgressAndClampsAtTarget(int target, double step, int expectedSteps)
    {
        var rotation = new RotationInfo
        {
            Move = new Move(RubiksCube.Layer.Front, target > 0 ? Move.RotationType.Clockwise : Move.RotationType.Counterclockwise),
            RotationStep = step,
        };

        Assert.That(rotation.Axis, Is.EqualTo(RubiksCube.Axis.Z));
        Assert.That(rotation.TargetAngle, Is.EqualTo(target));
        Assert.That(rotation.NumberOfSteps, Is.EqualTo(expectedSteps));
        rotation.CurrentStep = 3;
        Assert.That(rotation.CurrentAngle, Is.EqualTo(step * 3));
        rotation.CurrentStep = 100;
        Assert.That(rotation.CurrentAngle, Is.EqualTo(target));
    }
}
