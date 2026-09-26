using VirtualRubiksCube.Solver;
using static VirtualRubiksCube.Tests.CubeTestSupport;

namespace VirtualRubiksCube.Tests;

[TestFixture]
public class ControllerTests
{
    private static IEnumerable<TestCaseData> LayerTurns()
    {
        foreach (var layer in Enum.GetValues<RubiksCube.Layer>().Where(layer => layer != RubiksCube.Layer.None))
        foreach (var direction in Enum.GetValues<Move.RotationType>())
            yield return new TestCaseData(layer, direction).SetName($"Animation_matches_scramble_{layer}_{direction}");
    }

    [TestCaseSource(nameof(LayerTurns))]
    public void Animation_InterpolatesAndFinishesAtTheSameGeometryAsScramble(
        RubiksCube.Layer layer, Move.RotationType direction)
    {
        var (animatedCube, controller) = CreateCube(animationTime: 200);
        var (instantCube, instantController) = CreateCube();
        var move = new Move(layer, direction);
        var movingCorner = animatedCube.Cubelets.First(cubelet => cubelet.CurrentLayers.HasFlag(layer));
        var original = movingCorner.Vertices.Select(vertex => vertex.Clone()).ToArray();
        instantController.Scramble(new List<Move> { move });

        controller.StartRotation(move);
        Assert.That(controller.CurrentRotatingLayer, Has.Count.EqualTo(9));
        Assert.That(controller.CurrentRotationInfo.NumberOfSteps, Is.GreaterThan(1));
        controller.RotateStep();

        Assert.That(controller.CurrentRotationInfo.IsRotating, Is.True);
        Assert.That(movingCorner.Vertices.Select(VertexCoordinates), Is.Not.EqualTo(original.Select(VertexCoordinates)));
        var target = instantCube.Cubelets.Single(cubelet => cubelet.OriginalPosition == movingCorner.OriginalPosition);
        Assert.That(movingCorner.Vertices.Select(VertexCoordinates), Is.Not.EqualTo(target.Vertices.Select(VertexCoordinates)));

        FinishAnimation(controller);

        AssertCubesEqual(instantCube, animatedCube);
        controller.StartRotation(move.GetCounterMove());
        FinishAnimation(controller);
        AssertSame(FaceletCube.Solved(), FaceletCube.FromControllerState(animatedCube));
        Assert.That(RubiksCubeController.IsSolved(Snapshot(animatedCube)), Is.True);
    }

    [Test]
    public void ExecuteMoveQueue_PreservesOrderAndRepeatedMoveReferencesAndRaisesOneEventPair()
    {
        var (cube, controller) = CreateCube();
        var repeated = Scramble("R").Single();
        var moves = new List<Move> { repeated, repeated };
        moves.AddRange(Scramble("U F' L D2 B"));
        controller.MoveQueue.AddRange(moves);
        var events = new List<string>();
        controller.RotationStarted += sender =>
        {
            Assert.That(sender, Is.SameAs(controller));
            Assert.That(controller.CurrentRotationInfo.IsRotating, Is.True);
            events.Add("started");
        };
        controller.RotationFinished += sender =>
        {
            Assert.That(sender, Is.SameAs(controller));
            Assert.That(controller.CurrentRotationInfo.IsRotating, Is.False);
            Assert.That(controller.CurrentRotationInfo.IsExecutingMoveQueue, Is.False);
            Assert.That(controller.MoveQueue, Is.Empty);
            events.Add("finished");
        };

        controller.ExecuteMoveQueue();
        controller.ExecuteMoveQueue(); // A second click must not restart the queue.
        var startedIndices = new List<int>();
        while (controller.CurrentRotationInfo.IsRotating && startedIndices.Count <= moves.Count)
        {
            var index = controller.CurrentRotationInfo.CurrentMoveIndex;
            startedIndices.Add(index);
            Assert.That(controller.CurrentRotationInfo.Move, Is.SameAs(moves[index]));
            for (var step = 0; step < 100 && controller.CurrentRotationInfo.IsRotating &&
                 controller.CurrentRotationInfo.CurrentMoveIndex == index; step++)
                controller.RotateStep();
        }

        Assert.That(controller.CurrentRotationInfo.IsRotating, Is.False);
        Assert.That(startedIndices, Is.EqualTo(Enumerable.Range(0, moves.Count)));
        Assert.That(events, Is.EqualTo(new[] { "started", "finished" }));
        var expected = FaceletCube.Solved();
        expected.ApplyMoves(SolverMove.ParseSequence("R2 U F' L D2 B"));
        AssertSame(expected, FaceletCube.FromControllerState(cube));
        var (expectedCube, expectedController) = CreateCube();
        expectedController.Scramble(moves);
        AssertCubesEqual(expectedCube, cube);
    }

    [Test]
    public void ExecuteEmptyQueueAndRotateWhenIdle_DoNotRaiseEventsOrChangeCube()
    {
        var (cube, controller) = CreateCube();
        var eventCount = 0;
        controller.RotationStarted += _ => eventCount++;
        controller.RotationFinished += _ => eventCount++;

        controller.ExecuteMoveQueue();
        controller.RotateStep();

        Assert.That(eventCount, Is.Zero);
        Assert.That(controller.CurrentRotationInfo.IsRotating, Is.False);
        Assert.That(controller.CurrentRotationInfo.IsExecutingMoveQueue, Is.False);
        AssertSame(FaceletCube.Solved(), FaceletCube.FromControllerState(cube));
    }

    [Test]
    public void SingleRotation_RaisesOneStartAndFinishEvent()
    {
        var (_, controller) = CreateCube();
        var events = new List<string>();
        controller.RotationStarted += _ => events.Add("started");
        controller.RotationFinished += _ => events.Add("finished");

        controller.StartRotation(Scramble("U").Single());
        Assert.That(events, Is.EqualTo(new[] { "started" }));
        FinishAnimation(controller);
        controller.RotateStep();

        Assert.That(events, Is.EqualTo(new[] { "started", "finished" }));
    }

    [Test]
    public async Task CommandsWhileRotating_DoNotReplaceActiveMoveOrPendingQueue()
    {
        var (cube, controller) = CreateCube();
        var pending = Scramble("F").Single();
        controller.MoveQueue.Add(pending);
        controller.StartRotation(Scramble("U").Single());

        controller.StartRotation(Scramble("R").Single());
        controller.Scramble(Scramble("B D"));
        controller.GetSolutionMoves();
        Assert.That(controller.GetSolverMoves(), Is.False);
        Assert.That(await controller.GetSolverMovesAsync(), Is.False);
        Assert.That(controller.MoveQueue, Is.EqualTo(new[] { pending }));
        FinishAnimation(controller);

        var expected = FaceletCube.Solved();
        expected.ApplyMove(new SolverMove('U', 1));
        AssertSame(expected, FaceletCube.FromControllerState(cube));
    }

    [TestCase("R U F", "F' U' R'")]
    [TestCase("R U U' F", "F' R'")]
    [TestCase("R U U U F", "F' U R'")]
    [TestCase("R U F F F F U' L", "L' R'")]
    [TestCase("R R' U", "U'")]
    public void GetSolutionMoves_ReversesHistoryAndReducesRedundantTurns(string scramble, string expectedSolution)
    {
        var (cube, controller) = CreateCube();
        controller.Scramble(Scramble(scramble));
        var before = FaceletCube.FromControllerState(cube);
        controller.MoveQueue.AddRange(Scramble("B D"));

        controller.GetSolutionMoves();

        Assert.That(controller.MoveQueue.Select(MoveCoordinates), Is.EqualTo(Scramble(expectedSolution).Select(MoveCoordinates)));
        AssertSame(before, FaceletCube.FromControllerState(cube));
        controller.ExecuteMoveQueue();
        FinishAnimation(controller);
        Assert.That(RubiksCubeController.IsSolved(Snapshot(cube)), Is.True);
        controller.GetSolutionMoves();
        Assert.That(controller.MoveQueue, Is.Empty);
    }

    [Test]
    public void GetSolutionMoves_AnimatedHistoryCanBeReversed()
    {
        var (cube, controller) = CreateCube();
        controller.MoveQueue.AddRange(Scramble("R U R' F2"));
        controller.ExecuteMoveQueue();
        FinishAnimation(controller);

        controller.GetSolutionMoves();
        controller.ExecuteMoveQueue();
        FinishAnimation(controller);

        AssertSame(FaceletCube.Solved(), FaceletCube.FromControllerState(cube));
    }

    [TestCase(RubiksCube.Axis.X)]
    [TestCase(RubiksCube.Axis.Y)]
    [TestCase(RubiksCube.Axis.Z)]
    public void WholeCubeRotation_RemainsSolvedAndClearsHistory(RubiksCube.Axis axis)
    {
        var (cube, controller) = CreateCube();
        var layers = axis switch
        {
            RubiksCube.Axis.X => new[] { RubiksCube.Layer.Left, RubiksCube.Layer.MiddleX, RubiksCube.Layer.Right },
            RubiksCube.Axis.Y => new[] { RubiksCube.Layer.Up, RubiksCube.Layer.MiddleY, RubiksCube.Layer.Down },
            _ => new[] { RubiksCube.Layer.Front, RubiksCube.Layer.MiddleZ, RubiksCube.Layer.Back }
        };
        var moves = layers.Select(layer => new Move(layer, Move.RotationType.Clockwise))
            .Select(move => move.TargetAngle == 90 ? move : move.GetCounterMove()).ToList();

        controller.Scramble(moves);

        Assert.That(RubiksCubeController.IsSolved(Snapshot(cube)), Is.True);
        AssertSame(FaceletCube.Solved(), FaceletCube.FromControllerState(cube));
        controller.GetSolutionMoves();
        Assert.That(controller.MoveQueue, Is.Empty);
        Assert.That(controller.GetSolverMoves(), Is.True);
        Assert.That(controller.MoveQueue, Is.Empty);
    }

    [Test]
    public void Reset_AfterPartialAnimationRestoresGeometrySelectionsAndCanRotateAgain()
    {
        var (cube, controller) = CreateCube(animationTime: 200);
        var (initialCube, _) = CreateCube();
        controller.Scramble(Scramble("R U F2 L'"));
        cube.Cubelets[0].SetSelectionMode((byte)0, Face3D.SelectionMode.Selected);
        cube.Cubelets[1].SetSelectionMode(1, Face3D.SelectionMode.SecondarySelection);
        controller.MoveQueue.AddRange(Scramble("B D"));
        controller.ExecuteMoveQueue();
        controller.RotateStep();
        controller.RotateStep();
        Assert.That(controller.CurrentRotationInfo.IsRotating, Is.True);

        controller.Reset();
        controller.RotateStep(); // A render tick after reset must not resume the old rotation.

        Assert.That(controller.CurrentRotationInfo.IsRotating, Is.False);
        Assert.That(controller.CurrentRotationInfo.IsExecutingMoveQueue, Is.False);
        Assert.That(controller.CurrentRotationInfo.AnimationTime, Is.EqualTo(200));
        Assert.That(controller.CurrentRotatingLayer, Is.Empty);
        Assert.That(controller.MoveQueue, Is.Empty);
        AssertCubesEqual(initialCube, cube);
        Assert.That(cube.Cubelets.SelectMany(cubelet => Enumerable.Range(0, 6)
            .Select(slot => cubelet.GetSelectionMode((byte)slot))), Is.All.EqualTo(Face3D.SelectionMode.None));
        controller.GetSolutionMoves();
        Assert.That(controller.MoveQueue, Is.Empty);

        controller.StartRotation(Scramble("U'").Single());
        FinishAnimation(controller);
        var expected = FaceletCube.Solved();
        expected.ApplyMove(new SolverMove('U', -1));
        AssertSame(expected, FaceletCube.FromControllerState(cube));
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task Solver_ProducesExecutableSolutionWithoutMovingCubeUntilExecution(bool asynchronous)
    {
        var (cube, controller) = CreateCube();
        controller.Scramble(Scramble("F R U2 L' B D2"));
        controller.Scramble(new List<Move> { new(RubiksCube.Layer.MiddleX, Move.RotationType.Clockwise) });
        var before = FaceletCube.FromControllerState(cube);
        controller.MoveQueue.AddRange(Scramble("R R R"));

        var success = asynchronous ? await controller.GetSolverMovesAsync() : controller.GetSolverMoves();

        Assert.That(success, Is.True, controller.SolverError);
        Assert.That(controller.SolverError, Is.Null);
        Assert.That(controller.MoveQueue, Is.Not.Empty);
        AssertSame(before, FaceletCube.FromControllerState(cube));
        controller.ExecuteMoveQueue();
        FinishAnimation(controller);
        Assert.That(RubiksCubeController.IsSolved(Snapshot(cube)), Is.True);
        AssertSame(FaceletCube.Solved(), FaceletCube.FromControllerState(cube));
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task Solver_InvalidStateClearsPreviousQueueReportsErrorAndCanRecover(bool asynchronous)
    {
        var (cube, controller) = CreateCube();
        var corner = cube.Cubelets.Single(cubelet => cubelet.CurrentPosition == ((sbyte)1, (sbyte)-1, (sbyte)1));
        (corner.CurrentFaces[0], corner.CurrentFaces[3], corner.CurrentFaces[4]) =
            (corner.CurrentFaces[3], corner.CurrentFaces[4], corner.CurrentFaces[0]);
        controller.MoveQueue.AddRange(Scramble("R U"));

        var success = asynchronous ? await controller.GetSolverMovesAsync() : controller.GetSolverMoves();

        Assert.That(success, Is.False);
        Assert.That(controller.MoveQueue, Is.Empty);
        Assert.That(controller.SolverError, Is.Not.Null.And.Not.Empty);
        controller.Reset();
        Assert.That(controller.SolverError, Is.Null);
        success = asynchronous ? await controller.GetSolverMovesAsync() : controller.GetSolverMoves();
        Assert.That(success, Is.True);
        Assert.That(controller.SolverError, Is.Null);
        Assert.That(controller.MoveQueue, Is.Empty);
    }

    [TestCase(false)]
    [TestCase(true)]
    public void Solver_PreCancelledTokenThrowsAndDoesNotLeaveOldMovesQueued(bool asynchronous)
    {
        var (cube, controller) = CreateCube();
        controller.Scramble(Scramble("R U F"));
        var before = FaceletCube.FromControllerState(cube);
        controller.MoveQueue.AddRange(Scramble("R U"));
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        if (asynchronous)
            Assert.That((Func<Task>)(async () => { await controller.GetSolverMovesAsync(cancellation.Token); }),
                Throws.InstanceOf<OperationCanceledException>());
        else
            Assert.That((Action)(() => { controller.GetSolverMoves(cancellation.Token); }),
                Throws.InstanceOf<OperationCanceledException>());

        Assert.That(controller.MoveQueue, Is.Empty);
        Assert.That(controller.SolverError, Is.Null);
        AssertSame(before, FaceletCube.FromControllerState(cube));
    }

    [TestCase("reset")]
    [TestCase("scramble")]
    [TestCase("rotation")]
    public async Task AsyncSolver_StateChangedBeforeCompletionRejectsStaleMoves(string change)
    {
        var (cube, controller) = CreateCube();
        controller.Scramble(Scramble("R"));
        var capturedBeforeChange = FaceletCube.FromControllerState(cube);
        FaceletCube? capturedByWorker = null;
        var completion = new TaskCompletionSource<List<Move>>(TaskCreationOptions.RunContinuationsAsynchronously);
        var solve = controller.GetSolverMovesAsync((snapshot, _) =>
        {
            capturedByWorker = snapshot;
            return completion.Task;
        });
        Assert.That(solve.IsCompleted, Is.False);

        switch (change)
        {
            case "reset": controller.Reset(); break;
            case "scramble": controller.Scramble(Scramble("U")); break;
            case "rotation": controller.StartRotation(Scramble("U").Single()); break;
        }
        var stateAfterChange = FaceletCube.FromControllerState(cube);
        Assert.That(capturedByWorker, Is.Not.Null);
        AssertSame(capturedBeforeChange, capturedByWorker!);
        completion.SetResult(Scramble("R'"));

        Assert.That(await solve, Is.False);
        Assert.That(controller.MoveQueue, Is.Empty);
        Assert.That(controller.SolverError, Does.Contain("changed"));
        AssertSame(stateAfterChange, FaceletCube.FromControllerState(cube));
    }

    [Test]
    public void AsyncSolver_CancelledAfterWorkerStartsDoesNotPublishItsCompletedMoves()
    {
        var (cube, controller) = CreateCube();
        controller.Scramble(Scramble("R"));
        var before = FaceletCube.FromControllerState(cube);
        var completion = new TaskCompletionSource<List<Move>>(TaskCreationOptions.RunContinuationsAsynchronously);
        using var cancellation = new CancellationTokenSource();
        var solve = controller.GetSolverMovesAsync((_, token) =>
        {
            Assert.That(token, Is.EqualTo(cancellation.Token));
            return completion.Task;
        }, cancellation.Token);
        Assert.That(solve.IsCompleted, Is.False);

        cancellation.Cancel();
        completion.SetResult(Scramble("R'"));

        Assert.That((Func<Task>)(async () => { await solve; }), Throws.InstanceOf<OperationCanceledException>());
        Assert.That(controller.MoveQueue, Is.Empty);
        Assert.That(controller.SolverError, Is.Null);
        AssertSame(before, FaceletCube.FromControllerState(cube));
    }

    [Test]
    public async Task AsyncSolver_WorkerFailureReportsErrorAndClearsItOnNextSuccessfulSolve()
    {
        var (_, controller) = CreateCube();
        controller.Scramble(Scramble("R"));
        controller.MoveQueue.AddRange(Scramble("U"));
        var completion = new TaskCompletionSource<List<Move>>(TaskCreationOptions.RunContinuationsAsynchronously);
        var solve = controller.GetSolverMovesAsync((_, _) => completion.Task);

        completion.SetException(new InvalidOperationException("Injected worker failure"));

        Assert.That(await solve, Is.False);
        Assert.That(controller.SolverError, Is.EqualTo("Injected worker failure"));
        Assert.That(controller.MoveQueue, Is.Empty);
        Assert.That(await controller.GetSolverMovesAsync(), Is.True);
        Assert.That(controller.SolverError, Is.Null);
        Assert.That(controller.MoveQueue, Is.Not.Empty);
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task AsyncSolver_OlderRequestCannotOverwriteOrAppendToNewerResult(bool olderWorkerFails)
    {
        var (cube, controller) = CreateCube();
        controller.Scramble(Scramble("R"));
        var olderCompletion = new TaskCompletionSource<List<Move>>(TaskCreationOptions.RunContinuationsAsynchronously);
        var newerCompletion = new TaskCompletionSource<List<Move>>(TaskCreationOptions.RunContinuationsAsynchronously);
        var olderSolve = controller.GetSolverMovesAsync((_, _) => olderCompletion.Task);
        var newerSolve = controller.GetSolverMovesAsync((_, _) => newerCompletion.Task);
        Assert.That(olderSolve.IsCompleted, Is.False);
        Assert.That(newerSolve.IsCompleted, Is.False);

        newerCompletion.SetResult(Scramble("R'"));
        Assert.That(await newerSolve, Is.True);
        Assert.That(controller.SolverError, Is.Null);
        Assert.That(controller.MoveQueue.Select(MoveCoordinates), Is.EqualTo(Scramble("R'").Select(MoveCoordinates)));

        if (olderWorkerFails)
            olderCompletion.SetException(new InvalidOperationException("Obsolete worker failure"));
        else
            olderCompletion.SetResult(Scramble("R'"));

        Assert.That(await olderSolve, Is.False);
        Assert.That(controller.SolverError, Is.Null, "An obsolete request must preserve the latest request's error state.");
        Assert.That(controller.MoveQueue.Select(MoveCoordinates), Is.EqualTo(Scramble("R'").Select(MoveCoordinates)));
        controller.ExecuteMoveQueue();
        FinishAnimation(controller);
        AssertSame(FaceletCube.Solved(), FaceletCube.FromControllerState(cube));
    }

    [Test]
    public async Task AsyncSolver_OlderRequestCannotAppendAfterSynchronousSolve()
    {
        var (_, controller) = CreateCube();
        controller.Scramble(Scramble("R"));
        var completion = new TaskCompletionSource<List<Move>>(TaskCreationOptions.RunContinuationsAsynchronously);
        var olderSolve = controller.GetSolverMovesAsync((_, _) => completion.Task);

        Assert.That(controller.GetSolverMoves(), Is.True);
        var latestQueue = controller.MoveQueue.ToArray();
        completion.SetResult(Scramble("R'"));

        Assert.That(await olderSolve, Is.False);
        Assert.That(controller.MoveQueue, Is.EqualTo(latestQueue));
        Assert.That(controller.SolverError, Is.Null);
    }

    private static (double X, double Y, double Z) VertexCoordinates(Point3D point) => (point.X, point.Y, point.Z);

    private static (RubiksCube.Layer Layer, Move.RotationType Type) MoveCoordinates(Move move) => (move.Layer, move.Type);

    private static void AssertCubesEqual(RubiksCube expected, RubiksCube actual)
    {
        foreach (var expectedCubelet in expected.Cubelets)
        {
            var actualCubelet = actual.Cubelets.Single(cubelet => cubelet.OriginalPosition == expectedCubelet.OriginalPosition);
            Assert.That(actualCubelet.CurrentPosition, Is.EqualTo(expectedCubelet.CurrentPosition));
            Assert.That(actualCubelet.CurrentFaces, Is.EquivalentTo(expectedCubelet.CurrentFaces));
            for (var index = 0; index < expectedCubelet.Vertices.Length; index++)
            {
                var expectedVertex = expectedCubelet.Vertices[index];
                var actualVertex = actualCubelet.Vertices[index];
                Assert.That(actualVertex.X, Is.EqualTo(expectedVertex.X).Within(1e-9));
                Assert.That(actualVertex.Y, Is.EqualTo(expectedVertex.Y).Within(1e-9));
                Assert.That(actualVertex.Z, Is.EqualTo(expectedVertex.Z).Within(1e-9));
            }
        }
    }
}
