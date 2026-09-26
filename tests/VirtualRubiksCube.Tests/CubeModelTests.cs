namespace VirtualRubiksCube.Tests;

[TestFixture]
public class CubeModelTests
{
    [Test]
    public void NewCube_HasAll27DistinctPositionsAnd54ColoredStickers()
    {
        var (cube, _) = CubeTestSupport.CreateCube();

        Assert.That(cube.Cubelets, Has.Count.EqualTo(27));
        Assert.That(cube.Cubelets.Select(c => c.CurrentPosition).Distinct().Count(), Is.EqualTo(27));
        Assert.That(cube.Cubelets.All(c => c.CurrentPosition == c.OriginalPosition), Is.True);
        Assert.That(cube.Faces.Count(face => face.ColorIndex != 0), Is.EqualTo(54));
    }

    [Test]
    public void PieceClassification_ExcludesInternalCoreFromEdges()
    {
        var (cube, _) = CubeTestSupport.CreateCube();

        Assert.That(cube.Cubelets.Count(c => c.IsCorner), Is.EqualTo(8));
        Assert.That(cube.Cubelets.Count(c => c.IsCenter), Is.EqualTo(6));
        Assert.That(cube.Cubelets.Count(c => c.IsEdge), Is.EqualTo(12));
        var core = cube.Cubelets.Single(c => c.CurrentPosition == ((sbyte)0, (sbyte)0, (sbyte)0));
        Assert.That(core.IsEdge || core.IsCorner || core.IsCenter, Is.False);
    }

    [TestCase(RubiksCube.Layer.Up, RubiksCube.Face.Top)]
    [TestCase(RubiksCube.Layer.Down, RubiksCube.Face.Bottom)]
    [TestCase(RubiksCube.Layer.Left, RubiksCube.Face.Left)]
    [TestCase(RubiksCube.Layer.Right, RubiksCube.Face.Right)]
    [TestCase(RubiksCube.Layer.Front, RubiksCube.Face.Front)]
    [TestCase(RubiksCube.Layer.Back, RubiksCube.Face.Back)]
    public void OuterLayerAndFaceMappings_AreInverses(RubiksCube.Layer layer, RubiksCube.Face face)
    {
        Assert.That(RubiksCube.GetFaceFromLayer(layer), Is.EqualTo(face));
        Assert.That(RubiksCube.GetLayerFromFace(face), Is.EqualTo(layer));
    }

    [TestCase(RubiksCube.Layer.None)]
    [TestCase(RubiksCube.Layer.MiddleX)]
    [TestCase(RubiksCube.Layer.MiddleY)]
    [TestCase(RubiksCube.Layer.MiddleZ)]
    public void InnerLayers_HaveNoOuterFace(RubiksCube.Layer layer) =>
        Assert.That(RubiksCube.GetFaceFromLayer(layer), Is.EqualTo(RubiksCube.Face.None));

    [Test]
    public void EachLayer_HasNineCubelets_AfterMixedSliceAndFaceTurns()
    {
        var (cube, controller) = CubeTestSupport.CreateCube();
        controller.Scramble(CubeTestSupport.Scramble("R U F' L2 B"));
        controller.Scramble(new List<Move> { new(RubiksCube.Layer.MiddleX, Move.RotationType.Clockwise) });

        foreach (var layer in Enum.GetValues<RubiksCube.Layer>().Where(layer => layer != RubiksCube.Layer.None))
            Assert.That(cube.Cubelets.Count(c => c.CurrentLayers.HasFlag(layer)), Is.EqualTo(9), layer.ToString());
    }

    [Test]
    public void GetCurrentFace_WorksBeforeFacesAreEnumerated()
    {
        var (cube, _) = CubeTestSupport.CreateCube();
        var cubelet = cube.Cubelets[0];

        var face = cubelet.GetCurrentFace(RubiksCube.Face.Top);

        Assert.That(face, Is.Not.Null);
        Assert.That(face.CurrentFace, Is.EqualTo(RubiksCube.Face.Top));
        Assert.That(face.Cubelet, Is.SameAs(cubelet));
    }

    [Test]
    public void GetCurrentFace_UsesCurrentVerticesAfterAnimation()
    {
        var (cube, controller) = CubeTestSupport.CreateCube();
        var corner = cube.Cubelets.Single(c => c.CurrentPosition == ((sbyte)1, (sbyte)-1, (sbyte)1));
        _ = corner.Faces;
        controller.StartRotation(new Move(RubiksCube.Layer.Right, Move.RotationType.Clockwise));
        CubeTestSupport.FinishAnimation(controller);

        var face = corner.GetCurrentFace(RubiksCube.Face.Back);

        Assert.That(face.CurrentFace, Is.EqualTo(RubiksCube.Face.Back));
        Assert.That(face.Vertices.All(vertex => corner.Vertices.Any(current => ReferenceEquals(current, vertex))), Is.True);
    }

    [Test]
    public void SelectingWorldFace_AfterTurnSelectsTheCorrespondingSticker()
    {
        var (cube, controller) = CubeTestSupport.CreateCube();
        var corner = cube.Cubelets.Single(c => c.CurrentPosition == ((sbyte)1, (sbyte)-1, (sbyte)1));
        controller.Scramble(CubeTestSupport.Scramble("R"));

        corner.SetSelectionMode(RubiksCube.Face.Back, Face3D.SelectionMode.Selected);

        Assert.That(corner.GetSelectionMode(0), Is.EqualTo(Face3D.SelectionMode.Selected));
        Assert.That(corner.Faces.Single(f => f.CurrentFace == RubiksCube.Face.Back).SelectionStatus,
            Is.EqualTo(Face3D.SelectionMode.Selected));
    }
}
