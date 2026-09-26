using VirtualRubiksCube.Solver;

namespace VirtualRubiksCube.Tests;

internal static class CubeTestSupport
{
    public static (RubiksCube Cube, RubiksCubeController Controller) CreateCube(int animationTime = 200)
    {
        var cube = new RubiksCube(40);
        return (cube, new RubiksCubeController(cube, new RotationInfo { AnimationTime = animationTime }));
    }

    public static List<Move> Scramble(string notation) => SolverMove.ParseSequence(notation)
        .SelectMany(move => move.ToControllerMoves()).ToList();

    public static RubiksCubeState Snapshot(RubiksCube cube) =>
        new(cube.Cubelets.ToDictionary(cubelet => cubelet, cubelet => cubelet.CurrentPosition));

    public static void AssertSame(FaceletCube expected, FaceletCube actual) =>
        Assert.That(new string(actual.Facelets), Is.EqualTo(new string(expected.Facelets)));

    public static void FinishAnimation(RubiksCubeController controller)
    {
        // Advancing frames directly avoids timers, windows, OpenGL, and wall-clock sleeps.
        for (int frame = 0; frame < 10000 && controller.CurrentRotationInfo.IsRotating; frame++)
            controller.RotateStep();
        Assert.That(controller.CurrentRotationInfo.IsRotating, Is.False, "Animation did not finish within 10,000 frames.");
    }

    public static List<SolverMove> RandomScramble(int seed, int length)
    {
        var random = new Random(seed);
        const string faces = "UDLRFB";
        int[] quarters = { 1, -1, 2 };
        var moves = new List<SolverMove>(length);
        char previous = '\0';
        for (int i = 0; i < length; i++)
        {
            char face;
            do { face = faces[random.Next(faces.Length)]; } while (face == previous);
            moves.Add(new SolverMove(face, quarters[random.Next(quarters.Length)]));
            previous = face;
        }
        return moves;
    }
}
