using static VirtualRubiksCube.Tests.CubeTestSupport;

namespace VirtualRubiksCube.Tests;

[TestFixture]
public class CubeStateTests
{
    [Test]
    public void Constructor_CopiesPositionsAndFacesFromTheLiveCube()
    {
        var (cube, controller) = CreateCube();
        controller.Scramble(Scramble("R U F'"));
        var positions = cube.Cubelets.ToDictionary(cubelet => cubelet, cubelet => cubelet.CurrentPosition);
        var state = new RubiksCubeState(positions);

        foreach (var cubelet in cube.Cubelets)
        {
            Assert.That(state.CubeletsPosition[cubelet], Is.EqualTo(cubelet.CurrentPosition));
            Assert.That(state.CubeletsFaces[cubelet], Is.EqualTo(
                Enumerable.Range(0, 6).Select(slot => cubelet.CurrentFaces[(byte)slot])));
        }

        var corner = cube.Cubelets[0];
        var capturedPosition = state.CubeletsPosition[corner];
        var capturedFaces = state.CubeletsFaces[corner].ToArray();
        positions[corner] = (0, 0, 0);
        positions.Remove(cube.Cubelets[1]);
        controller.Scramble(Scramble("L D B"));

        Assert.That(state.CubeletsPosition, Has.Count.EqualTo(27));
        Assert.That(state.CubeletsPosition[corner], Is.EqualTo(capturedPosition));
        Assert.That(state.CubeletsFaces[corner], Is.EqualTo(capturedFaces));
    }

    [Test]
    public void Clone_ChangingCloneDoesNotChangeSnapshotOrLiveCube()
    {
        var (cube, _) = CreateCube();
        var state = Snapshot(cube);
        var clone = state.Clone();
        var corner = cube.Cubelets[0];
        clone.CubeletsPosition[corner] = (0, 0, 0);
        clone.CubeletsFaces[corner][0] = RubiksCube.Face.Bottom;
        clone.CubeletsPosition.Remove(cube.Cubelets[1]);

        Assert.That(state.CubeletsPosition, Has.Count.EqualTo(27));
        Assert.That(state.CubeletsPosition[corner], Is.EqualTo(corner.OriginalPosition));
        Assert.That(state.CubeletsFaces[corner][0], Is.EqualTo(RubiksCube.Face.Top));
        Assert.That(corner.CurrentPosition, Is.EqualTo(corner.OriginalPosition));
        Assert.That(corner.CurrentFaces[0], Is.EqualTo(RubiksCube.Face.Top));
    }

    [Test]
    public void Clone_CapturesSnapshotFacesEvenWhenLiveCubeHasMovedSinceSnapshot()
    {
        var (cube, controller) = CreateCube();
        var state = Snapshot(cube);
        controller.Scramble(Scramble("R U F"));
        var clone = state.Clone();

        Assert.That(RubiksCubeController.IsSolved(Snapshot(cube)), Is.False);
        Assert.That(RubiksCubeController.IsSolved(clone), Is.True);
        foreach (var cubelet in cube.Cubelets)
        {
            Assert.That(clone.CubeletsPosition[cubelet], Is.EqualTo(state.CubeletsPosition[cubelet]));
            Assert.That(clone.CubeletsFaces[cubelet], Is.EqualTo(state.CubeletsFaces[cubelet]));
            Assert.That(clone.CubeletsFaces[cubelet], Is.Not.SameAs(state.CubeletsFaces[cubelet]));
        }
    }

    [Test]
    public void Clone_ChangingOriginalSnapshotDoesNotChangeClone()
    {
        var (cube, _) = CreateCube();
        var state = Snapshot(cube);
        var clone = state.Clone();
        var corner = cube.Cubelets[0];
        state.CubeletsPosition[corner] = (0, 0, 0);
        state.CubeletsFaces[corner][0] = RubiksCube.Face.Bottom;

        Assert.That(clone.CubeletsPosition[corner], Is.EqualTo(corner.OriginalPosition));
        Assert.That(clone.CubeletsFaces[corner][0], Is.EqualTo(RubiksCube.Face.Top));
        Assert.That(RubiksCubeController.IsSolved(clone), Is.True);
    }

    [Test]
    public void IsSolved_TwistedCornerAtItsOriginalPositionIsNotSolved()
    {
        var (cube, _) = CreateCube();
        var state = Snapshot(cube);
        var corner = cube.Cubelets.Single(cubelet => cubelet.OriginalPosition == ((sbyte)1, (sbyte)-1, (sbyte)1));
        var faces = state.CubeletsFaces[corner];
        (faces[0], faces[3], faces[4]) = (faces[3], faces[4], faces[0]);

        Assert.That(RubiksCubeController.IsSolved(state), Is.False);
    }

    [Test]
    public void IsSolved_EmptySnapshotIsNotSolved()
    {
        var empty = new RubiksCubeState(new Dictionary<Cubelet, (sbyte, sbyte, sbyte)>());

        Assert.That(RubiksCubeController.IsSolved(empty), Is.False);
    }
}
