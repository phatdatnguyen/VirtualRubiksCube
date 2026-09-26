namespace VirtualRubiksCube.Tests;

[TestFixture]
public class GeometryTests
{
    [TestCase(90, 0, 0, 1, -3, 2)]
    [TestCase(-90, 0, 0, 1, 3, -2)]
    [TestCase(0, 90, 0, 3, 2, -1)]
    [TestCase(0, -90, 0, -3, 2, 1)]
    [TestCase(0, 0, 90, -2, 1, 3)]
    [TestCase(0, 0, -90, 2, -1, 3)]
    [TestCase(90, 90, 90, 1, -3, 2)]
    public void Rotate_ProducesExpectedCoordinatesWithoutMutatingInput(
        double xAngle, double yAngle, double zAngle, double x, double y, double z)
    {
        var point = new Point3D(1, 2, 3);

        var rotated = point.Rotate(xAngle, yAngle, zAngle);

        AssertPoint(rotated, x, y, z);
        AssertPoint(point, 1, 2, 3);
        Assert.That(rotated, Is.Not.SameAs(point));
    }

    [Test]
    public void Rotate_AndReverseRotationsRestorePoint()
    {
        var point = new Point3D(13, -7, 2);
        var rotated = point.Rotate(31, 47, -19);

        Assert.That(rotated.X * rotated.X + rotated.Y * rotated.Y + rotated.Z * rotated.Z,
            Is.EqualTo(222).Within(1e-9));
        var restored = rotated.Rotate(0, -47, 0).Rotate(-31, 0, 0).Rotate(0, 0, 19);
        AssertPoint(restored, point.X, point.Y, point.Z);
    }

    [Test]
    public void Clone_HasIndependentCoordinates()
    {
        var point = new Point3D(1, 2, 3);
        var clone = point.Clone();
        clone.X = 99;

        AssertPoint(point, 1, 2, 3);
        AssertPoint(clone, 99, 2, 3);
    }

    [Test]
    public void Project_ScalesAndCentersScreenCoordinatesWhileKeepingDepth()
    {
        var point = new Point3D(10, -20, 30);

        var projected = point.Project(800, 600, 80, 100);

        AssertPoint(projected, 408, 284, 30);
        AssertPoint(point, 10, -20, 30);
    }

    [Test]
    public void FaceTransforms_PreserveStickerMetadataAndDoNotReplaceSourceGeometry()
    {
        var (cube, _) = CubeTestSupport.CreateCube();
        var face = cube.Cubelets[0].Faces[0];
        var original = face.Vertices.Select(vertex => vertex.Clone()).ToArray();

        var rotated = face.Rotate(0, 0, 90);
        var projected = face.Project(800, 600, 80, 100);

        foreach (var transformed in new[] { rotated, projected })
        {
            Assert.That(transformed.Cubelet, Is.SameAs(face.Cubelet));
            Assert.That(transformed.CubeletFaceIndex, Is.EqualTo(face.CubeletFaceIndex));
            Assert.That(transformed.ColorIndex, Is.EqualTo(face.ColorIndex));
            Assert.That(transformed.Vertices, Is.Not.SameAs(face.Vertices));
        }
        for (int i = 0; i < original.Length; i++)
        {
            AssertPoint(face.Vertices[i], original[i].X, original[i].Y, original[i].Z);
            AssertPoint(rotated.Vertices[i], -original[i].Y, original[i].X, original[i].Z);
            AssertPoint(projected.Vertices[i], original[i].X * 0.8 + 400, original[i].Y * 0.8 + 300, original[i].Z);
        }
        rotated.SelectionStatus = Face3D.SelectionMode.Selected;
        Assert.That(face.SelectionStatus, Is.EqualTo(Face3D.SelectionMode.Selected));
    }

    private static void AssertPoint(Point3D point, double x, double y, double z)
    {
        Assert.That(point.X, Is.EqualTo(x).Within(1e-9));
        Assert.That(point.Y, Is.EqualTo(y).Within(1e-9));
        Assert.That(point.Z, Is.EqualTo(z).Within(1e-9));
    }
}
