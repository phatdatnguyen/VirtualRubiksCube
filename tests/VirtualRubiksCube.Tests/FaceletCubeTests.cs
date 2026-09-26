using VirtualRubiksCube.Solver;

namespace VirtualRubiksCube.Tests;

[TestFixture]
public class FaceletCubeTests
{
    private static IEnumerable<TestCaseData> Moves()
    {
        foreach (char face in "UDLRFB")
            foreach (int quarter in new[] { 1, -1, 2 })
                yield return new TestCaseData(face, quarter);
    }

    private static IEnumerable<TestCaseData> Orientations() =>
        Enumerable.Range(0, 24).Select(index => new TestCaseData(index));

    private static IEnumerable<TestCaseData> OrientationMoves()
    {
        for (int orientation = 0; orientation < 24; orientation++)
            foreach (char face in "UDLRFB")
                foreach (int quarter in new[] { 1, -1, 2 })
                    yield return new TestCaseData(orientation, face, quarter);
    }

    private static IEnumerable<TestCaseData> SliceMoves()
    {
        foreach (var layer in new[] { RubiksCube.Layer.MiddleX, RubiksCube.Layer.MiddleY, RubiksCube.Layer.MiddleZ })
            foreach (int quarter in new[] { 1, -1, 2 })
                yield return new TestCaseData(layer, quarter);
    }

    private static IEnumerable<TestCaseData> WholeCubeOrientations()
    {
        foreach (string baseRotation in new[] { "", "X", "X2", "X'", "Z", "Z'" })
            for (int yaw = 0; yaw < 4; yaw++)
                yield return new TestCaseData(baseRotation, yaw);
    }

    [Test]
    public void SolvedCubeContainsNineStickersPerFaceAndCanonicalCenters()
    {
        var cube = FaceletCube.Solved();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(cube.Facelets, Has.Length.EqualTo(54));
            Assert.That(new string(cube.Facelets), Is.EqualTo(
                "UUUUUUUUURRRRRRRRRFFFFFFFFFDDDDDDDDDLLLLLLLLLBBBBBBBBB"));
            Assert.That(cube.IsSolved(), Is.True);
            Assert.That((Action)(() => { cube.Validate(); }), Throws.Nothing);
        }
    }

    [Test]
    public void CloneAndSolvedFactoryDoNotShareStickerStorage()
    {
        var original = FaceletCube.Solved();
        var clone = original.Clone();
        clone.ApplyMove(new SolverMove('R', 1));
        clone.Facelets[0] = '?';

        using (Assert.EnterMultipleScope())
        {
            Assert.That(clone.Facelets, Is.Not.SameAs(original.Facelets));
            Assert.That(original.IsSolved(), Is.True);
            Assert.That(FaceletCube.Solved().Facelets, Is.EqualTo(original.Facelets));
            Assert.That(clone.IsSolved(), Is.False);
        }
    }

    [TestCaseSource(nameof(Moves))]
    public void MoveAndInverseRestoreEverySticker(char face, int quarter)
    {
        var original = UniqueStickers();
        var actual = original.Clone();
        var move = new SolverMove(face, quarter);

        actual.ApplyMove(move);
        actual.ApplyMove(move.Inverse());

        CubeTestSupport.AssertSame(original, actual);
    }

    [TestCase('U')]
    [TestCase('D')]
    [TestCase('L')]
    [TestCase('R')]
    [TestCase('F')]
    [TestCase('B')]
    public void FourQuarterTurnsRestoreEverySticker(char face)
    {
        var original = UniqueStickers();
        var actual = original.Clone();

        actual.ApplyMoves(Enumerable.Repeat(new SolverMove(face, 1), 4));

        CubeTestSupport.AssertSame(original, actual);
    }

    [TestCaseSource(nameof(Moves))]
    public void MovesMatchTheControllerAfterAnExistingScramble(char face, int quarter)
    {
        var (cube, controller) = CubeTestSupport.CreateCube();
        controller.Scramble(CubeTestSupport.Scramble("R U F2 L' D B2"));
        var expected = FaceletCube.FromControllerState(cube);
        var move = new SolverMove(face, quarter);

        expected.ApplyMove(move);
        controller.Scramble(move.ToControllerMoves().ToList());

        CubeTestSupport.AssertSame(expected, FaceletCube.FromControllerState(cube));
        Assert.That((Action)(() => { expected.Validate(); }), Throws.Nothing);
    }

    [TestCase(83917, 400)]
    [TestCase(741, 100)]
    [TestCase(2026, 100)]
    public void MixedMoveSequencesMatchTheControllerAtEveryStep(int seed, int length)
    {
        var (cube, controller) = CubeTestSupport.CreateCube();
        var expected = FaceletCube.Solved();

        foreach (var move in CubeTestSupport.RandomScramble(seed, length))
        {
            expected.ApplyMove(move);
            controller.Scramble(move.ToControllerMoves().ToList());
            CubeTestSupport.AssertSame(expected, FaceletCube.FromControllerState(cube));
        }
    }

    [Test]
    public void OrientationsContainAllTwentyFourDistinctRotations()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(FaceletCube.Orientations, Has.Length.EqualTo(24));
            Assert.That(FaceletCube.Orientations.Select(orientation => string.Join(",", orientation.Perm)), Is.Unique);
        }
    }

    [TestCaseSource(nameof(Orientations))]
    public void OrientationAndItsInverseRestoreEveryStickerWithoutChangingInput(int orientationIndex)
    {
        var original = UniqueStickers();
        var before = original.Clone();
        var orientation = FaceletCube.Orientations[orientationIndex];

        var rotated = original.ApplyOrientation(orientation.Perm);
        var restored = rotated.ApplyOrientation(orientation.InvPerm);

        CubeTestSupport.AssertSame(before, original);
        CubeTestSupport.AssertSame(original, restored);
        Assert.That(rotated.Facelets, Is.Not.SameAs(original.Facelets));
    }

    [TestCaseSource(nameof(Orientations))]
    public void OrientationRelabelingPreservesSolvedCube(int orientationIndex)
    {
        var rotated = FaceletCube.Solved().ApplyOrientationWithRelabel(FaceletCube.Orientations[orientationIndex]);

        CubeTestSupport.AssertSame(FaceletCube.Solved(), rotated);
        Assert.That((Action)(() => { rotated.Validate(); }), Throws.Nothing);
    }

    [TestCaseSource(nameof(OrientationMoves))]
    public void TranslatedMoveHasTheSameEffectInTheOriginalOrientation(int orientationIndex, char face, int quarter)
    {
        var original = FaceletCube.Solved();
        original.ApplyMoves(CubeTestSupport.RandomScramble(741, 50));
        var orientation = FaceletCube.Orientations[orientationIndex];
        var move = new SolverMove(face, quarter);
        var oriented = original.ApplyOrientationWithRelabel(orientation);
        var translated = original.Clone();

        oriented.ApplyMove(move);
        translated.ApplyMove(FaceletCube.TranslateMove(move, orientation));

        CubeTestSupport.AssertSame(oriented, translated.ApplyOrientationWithRelabel(orientation));
    }

    [TestCase("unknown sticker")]
    [TestCase("wrong color count")]
    [TestCase("unnormalized centers")]
    [TestCase("single edge flip")]
    [TestCase("single corner twist")]
    [TestCase("reflected corner")]
    [TestCase("edge permutation parity")]
    [TestCase("duplicate edge")]
    [TestCase("duplicate corner")]
    public void ValidateRejectsPhysicallyImpossibleStates(string corruption)
    {
        var cube = FaceletCube.Solved();
        var stickers = cube.Facelets;
        switch (corruption)
        {
            case "unknown sticker": stickers[0] = '?'; break;
            case "wrong color count": stickers[0] = 'R'; break;
            case "unnormalized centers": Swap(stickers, 4, 9); break;
            case "single edge flip": Swap(stickers, 7, 19); break; // UF
            case "single corner twist": Cycle(stickers, 8, 9, 20); break; // UFR
            case "reflected corner": Swap(stickers, 9, 20); break;
            case "edge permutation parity": Swap(stickers, 19, 10); break; // UF / UR
            case "duplicate edge": Swap(stickers, 46, 25); break; // UB / DF -> UF / DB
            case "duplicate corner":
                // Replace UFL and UBR with UFR and UBL, preserving all color counts.
                stickers[18] = 'R'; stickers[38] = 'F'; stickers[45] = 'L'; stickers[11] = 'B';
                break;
            default: throw new AssertionException("Unknown corruption");
        }

        Assert.That((Action)(() => { cube.Validate(); }), Throws.TypeOf<InvalidOperationException>());
    }

    [TestCase("two flipped edges")]
    [TestCase("opposite corner twists")]
    [TestCase("matching edge and corner parity")]
    public void ValidateAcceptsBalancedPieceOrientationsAndPermutations(string arrangement)
    {
        var cube = FaceletCube.Solved();
        var stickers = cube.Facelets;
        switch (arrangement)
        {
            case "two flipped edges": Swap(stickers, 7, 19); Swap(stickers, 1, 46); break;
            case "opposite corner twists": Cycle(stickers, 8, 9, 20); Cycle(stickers, 6, 38, 18); break;
            case "matching edge and corner parity":
                Swap(stickers, 19, 10);
                Swap(stickers, 8, 6); Swap(stickers, 9, 18); Swap(stickers, 20, 38);
                break;
            default: throw new AssertionException("Unknown arrangement");
        }

        Assert.That((Action)(() => { cube.Validate(); }), Throws.Nothing);
        Assert.That(cube.IsSolved(), Is.False);
    }

    [Test]
    public void ControllerConversionRejectsNull()
    {
        Assert.That((Action)(() => { FaceletCube.FromControllerState(null!); }), Throws.TypeOf<ArgumentNullException>());
    }

    [TestCase("missing cubelet")]
    [TestCase("duplicate current position")]
    [TestCase("duplicate original position")]
    [TestCase("out of range position")]
    [TestCase("missing direction")]
    [TestCase("duplicate direction")]
    [TestCase("invalid direction")]
    [TestCase("exposed internal face")]
    public void ControllerConversionRejectsInvalidStructures(string corruption)
    {
        var (cube, _) = CubeTestSupport.CreateCube();
        var first = cube.Cubelets[0];
        switch (corruption)
        {
            case "missing cubelet": cube.Cubelets.RemoveAt(0); break;
            case "duplicate current position": cube.Cubelets[1].CurrentPosition = first.CurrentPosition; break;
            case "duplicate original position":
                var (x, y, z) = first.OriginalPosition;
                cube.Cubelets[1] = new Cubelet(cube, first.Size, x, y, z)
                {
                    CurrentPosition = cube.Cubelets[1].CurrentPosition
                };
                break;
            case "out of range position": first.CurrentPosition = (2, -1, -1); break;
            case "missing direction": first.CurrentFaces.Remove(0); break;
            case "duplicate direction": first.CurrentFaces[0] = first.CurrentFaces[1]; break;
            case "invalid direction": first.CurrentFaces[0] = RubiksCube.Face.None; break;
            case "exposed internal face":
                (first.CurrentFaces[0], first.CurrentFaces[1]) = (first.CurrentFaces[1], first.CurrentFaces[0]);
                break;
            default: throw new AssertionException("Unknown corruption");
        }

        Assert.That((Action)(() => { FaceletCube.FromControllerState(cube); }), Throws.TypeOf<InvalidOperationException>());
    }

    [TestCaseSource(nameof(SliceMoves))]
    public void SliceTurnsNormalizeCentersAndRemainValid(RubiksCube.Layer layer, int quarter)
    {
        var (cube, controller) = CubeTestSupport.CreateCube();
        var turn = new Move(layer, quarter == -1 ? Move.RotationType.Counterclockwise : Move.RotationType.Clockwise);
        controller.Scramble(Enumerable.Repeat(turn, quarter == 2 ? 2 : 1).ToList());

        var actual = FaceletCube.FromControllerState(cube);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(new string(Enumerable.Range(0, 6).Select(face => actual.Facelets[face * 9 + 4]).ToArray()),
                Is.EqualTo("URFDLB"));
            Assert.That((Action)(() => { actual.Validate(); }), Throws.Nothing);
            Assert.That(actual.IsSolved(), Is.False);
        }
        controller.Scramble(Enumerable.Repeat(turn.GetCounterMove(), quarter == 2 ? 2 : 1).ToList());
        CubeTestSupport.AssertSame(FaceletCube.Solved(), FaceletCube.FromControllerState(cube));
    }

    [TestCase(RubiksCube.Axis.X, 1)]
    [TestCase(RubiksCube.Axis.X, -1)]
    [TestCase(RubiksCube.Axis.X, 2)]
    [TestCase(RubiksCube.Axis.Y, 1)]
    [TestCase(RubiksCube.Axis.Y, -1)]
    [TestCase(RubiksCube.Axis.Y, 2)]
    [TestCase(RubiksCube.Axis.Z, 1)]
    [TestCase(RubiksCube.Axis.Z, -1)]
    [TestCase(RubiksCube.Axis.Z, 2)]
    public void WholeCubeRotationNormalizesBackToSolved(RubiksCube.Axis axis, int quarter)
    {
        var (cube, controller) = CubeTestSupport.CreateCube();
        RotateWholeCube(controller, axis, quarter);

        CubeTestSupport.AssertSame(FaceletCube.Solved(), FaceletCube.FromControllerState(cube));
    }

    [TestCaseSource(nameof(WholeCubeOrientations))]
    public void EveryWholeCubeOrientationNormalizesToTheCurrentCenters(string baseRotation, int yaw)
    {
        var (cube, controller) = CubeTestSupport.CreateCube();
        if (baseRotation.Length > 0)
        {
            var axis = baseRotation[0] == 'X' ? RubiksCube.Axis.X : RubiksCube.Axis.Z;
            int quarter = baseRotation.EndsWith('2') ? 2 : baseRotation.EndsWith('\'') ? -1 : 1;
            RotateWholeCube(controller, axis, quarter);
        }
        for (int turn = 0; turn < yaw; turn++) RotateWholeCube(controller, RubiksCube.Axis.Y, 1);

        CubeTestSupport.AssertSame(FaceletCube.Solved(), FaceletCube.FromControllerState(cube));
    }

    private static void RotateWholeCube(RubiksCubeController controller, RubiksCube.Axis axis, int quarter)
    {
        var layers = axis switch
        {
            RubiksCube.Axis.X => new[] { RubiksCube.Layer.Left, RubiksCube.Layer.MiddleX, RubiksCube.Layer.Right },
            RubiksCube.Axis.Y => new[] { RubiksCube.Layer.Up, RubiksCube.Layer.MiddleY, RubiksCube.Layer.Down },
            _ => new[] { RubiksCube.Layer.Front, RubiksCube.Layer.MiddleZ, RubiksCube.Layer.Back }
        };
        int targetAngle = quarter == -1 ? -90 : 90;
        foreach (var layer in layers)
        {
            var move = new Move(layer, Move.RotationType.Clockwise);
            if (move.TargetAngle != targetAngle) move = move.GetCounterMove();
            controller.Scramble(Enumerable.Repeat(move, quarter == 2 ? 2 : 1).ToList());
        }

    }

    private static FaceletCube UniqueStickers()
    {
        var cube = FaceletCube.Solved();
        for (int index = 0; index < cube.Facelets.Length; index++) cube.Facelets[index] = (char)(128 + index);
        return cube;
    }

    private static void Swap(char[] stickers, int first, int second) =>
        (stickers[first], stickers[second]) = (stickers[second], stickers[first]);

    private static void Cycle(char[] stickers, int first, int second, int third) =>
        (stickers[first], stickers[second], stickers[third]) = (stickers[second], stickers[third], stickers[first]);
}
