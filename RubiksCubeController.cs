namespace VirtualRubiksCube
{
    public class RubiksCubeController
    {
        #region Fields
        private RubiksCubeState currentState;
        private List<Move> moveHistory = new();  // history for reversal; reset when cube reaches solved state
        private List<Move> moveQueue = new();
        private RotationInfo currentRotationInfo;
        private List<Cubelet> currentRotatingLayer = new();
        private Dictionary<Cubelet, Point3D[]>? targetCubeletsPosition;
        private int solverRequestVersion;
        #endregion

        #region Properties
        public RubiksCube RubiksCube { get; private set; }
        public RotationInfo CurrentRotationInfo { get { return currentRotationInfo; } }
        public List<Move> MoveQueue { get { return moveQueue; } }
        public IReadOnlyList<Cubelet> CurrentRotatingLayer { get { return currentRotatingLayer; } }
        public string? SolverError { get; private set; }
        #endregion

        #region Events
        public delegate void RotationStartedHandler(object sender);
        public event RotationStartedHandler? RotationStarted;
        public delegate void RotationFinishedHandler(object sender);
        public event RotationFinishedHandler? RotationFinished;
        #endregion

        #region Constructor
        public RubiksCubeController(RubiksCube rubiksCube, RotationInfo rotationInfo)
        {
            RubiksCube = rubiksCube;
            rubiksCube.Controller = this;
            currentRotationInfo = rotationInfo;

            Dictionary<Cubelet, (sbyte, sbyte, sbyte)> initialCubeletsPosition = new();
            foreach (Cubelet cubelet in rubiksCube.Cubelets)
                initialCubeletsPosition.Add(cubelet, cubelet.OriginalPosition);
            currentState = new RubiksCubeState(initialCubeletsPosition);
        }
        #endregion

        #region Methods
        public void RotateStep()
        {
            if (!currentRotationInfo.IsRotating)
                return;

            if (currentRotationInfo.CurrentStep < currentRotationInfo.NumberOfSteps)
            {
                double xAngle = 0;
                double yAngle = 0;
                double zAngle = 0;
                switch (currentRotationInfo.Axis)
                {
                    case RubiksCube.Axis.X:
                        xAngle = currentRotationInfo.RotationStep;
                        break;
                    case RubiksCube.Axis.Y:
                        yAngle = currentRotationInfo.RotationStep;
                        break;
                    case RubiksCube.Axis.Z:
                        zAngle = currentRotationInfo.RotationStep;
                        break;
                }

                foreach (Cubelet cubelet in currentRotatingLayer)
                    for (int i = 0; i < cubelet.Vertices.Length; i++)
                        cubelet.Vertices[i] = cubelet.Vertices[i].Rotate(xAngle, yAngle, zAngle);

                currentRotationInfo.CurrentStep += 1;
            }
            else
            {
                foreach (Cubelet cubelet in currentRotatingLayer)
                    for (int i = 0; i < cubelet.Vertices.Length; i++)
                        if (targetCubeletsPosition != null)
                            cubelet.Vertices[i] = targetCubeletsPosition[cubelet][i];

                if (IsSolved(currentState))
                    moveHistory.Clear();

                if (currentRotationInfo.IsExecutingMoveQueue && currentRotationInfo.CurrentMoveIndex + 1 < moveQueue.Count)
                {
                    currentRotationInfo.CurrentMoveIndex += 1;
                    BeginRotation(moveQueue[currentRotationInfo.CurrentMoveIndex]);
                }
                else
                {
                    if (currentRotationInfo.IsExecutingMoveQueue)
                    {
                        moveQueue.Clear();
                        currentRotationInfo.IsExecutingMoveQueue = false;
                    }

                    currentRotationInfo.IsRotating = false;
                    RotationFinished?.Invoke(this);
                }
            }
        }

        // on render thread
        public void StartRotation(Move move)
        {
            if (currentRotationInfo.IsRotating)
                return;

            BeginRotation(move);
        }

        private void BeginRotation(Move move)
        {
            RubiksCubeState newState = currentState.Clone();

            currentRotatingLayer = new List<Cubelet>();
            currentRotationInfo.Move = move;

            foreach (KeyValuePair<Cubelet, (sbyte, sbyte, sbyte)> cubeletPosition in currentState.CubeletsPosition)
            {
                Cubelet cubelet = cubeletPosition.Key;
                (sbyte oldX, sbyte oldY, sbyte oldZ) = cubeletPosition.Value;
                sbyte newX = oldX;
                sbyte newY = oldY;
                sbyte newZ = oldZ;

                if (cubelet.CurrentLayers.HasFlag(currentRotationInfo.Move.Layer))
                {
                    switch (currentRotationInfo.Axis)
                    {
                        case RubiksCube.Axis.X:
                            newY = (sbyte)(-1 * oldZ * Math.Sin(currentRotationInfo.TargetAngle * Math.PI / 180));
                            newZ = (sbyte)(oldY * Math.Sin(currentRotationInfo.TargetAngle * Math.PI / 180));
                            break;
                        case RubiksCube.Axis.Y:
                            newX = (sbyte)(oldZ * Math.Sin(currentRotationInfo.TargetAngle * Math.PI / 180));
                            newZ = (sbyte)(-1 * oldX * Math.Sin(currentRotationInfo.TargetAngle * Math.PI / 180));
                            break;
                        case RubiksCube.Axis.Z:
                            newX = (sbyte)(-1 * oldY * Math.Sin(currentRotationInfo.TargetAngle * Math.PI / 180));
                            newY = (sbyte)(oldX * Math.Sin(currentRotationInfo.TargetAngle * Math.PI / 180));
                            break;
                    }

                    for (byte i = 0; i < 6; i++)
                        newState.CubeletsFaces[cubelet][i] = cubelet.CurrentFaces[i] = RotateFace(cubelet.CurrentFaces[i], move);

                    currentRotatingLayer.Add(cubelet);
                }
                    
                cubelet.CurrentPosition = (newX, newY, newZ);
                newState.CubeletsPosition[cubelet] = cubelet.CurrentPosition;
            }

            currentState = newState;
            moveHistory.Add(move);

            double frameRate = 60.0;
            currentRotationInfo.RotationStep = currentRotationInfo.TargetAngle / ((currentRotationInfo.AnimationTime / 1000.0) * frameRate);
            targetCubeletsPosition = GetTargetVertices();
            currentRotationInfo.IsRotating = true;

            if (!currentRotationInfo.IsExecutingMoveQueue ||
                currentRotationInfo.CurrentMoveIndex == 0)
                // Raise the event
                RotationStarted?.Invoke(this);
        }

        public static RubiksCube.Face RotateFace(RubiksCube.Face face, Move move)
        {
            RubiksCube.Face rotatedFace = face;
            if (move.Axis == RubiksCube.Axis.X)
            {
                if ((move.Type == Move.RotationType.Clockwise && move.Layer != RubiksCube.Layer.Left) ||
                    (move.Type == Move.RotationType.Counterclockwise && move.Layer == RubiksCube.Layer.Left))
                {
                    if (face == RubiksCube.Face.Top)
                        rotatedFace = RubiksCube.Face.Back;
                    else if (face == RubiksCube.Face.Back)
                        rotatedFace = RubiksCube.Face.Bottom;
                    else if (face == RubiksCube.Face.Bottom)
                        rotatedFace = RubiksCube.Face.Front;
                    else if (face == RubiksCube.Face.Front)
                        rotatedFace = RubiksCube.Face.Top;
                }
                else
                {
                    if (face == RubiksCube.Face.Top)
                        rotatedFace = RubiksCube.Face.Front;
                    else if (face == RubiksCube.Face.Front)
                        rotatedFace = RubiksCube.Face.Bottom;
                    else if (face == RubiksCube.Face.Bottom)
                        rotatedFace = RubiksCube.Face.Back;
                    else if (face == RubiksCube.Face.Back)
                        rotatedFace = RubiksCube.Face.Top;
                }
            }
            else if (move.Axis == RubiksCube.Axis.Y)
            {
                if ((move.Type == Move.RotationType.Clockwise && move.Layer != RubiksCube.Layer.Down) ||
                    (move.Type == Move.RotationType.Counterclockwise && move.Layer == RubiksCube.Layer.Down))
                {
                    if (face == RubiksCube.Face.Front)
                        rotatedFace = RubiksCube.Face.Left;
                    else if (face == RubiksCube.Face.Left)
                        rotatedFace = RubiksCube.Face.Back;
                    else if (face == RubiksCube.Face.Back)
                        rotatedFace = RubiksCube.Face.Right;
                    else if (face == RubiksCube.Face.Right)
                        rotatedFace = RubiksCube.Face.Front;
                }
                else
                {
                    if (face == RubiksCube.Face.Front)
                        rotatedFace = RubiksCube.Face.Right;
                    else if (face == RubiksCube.Face.Right)
                        rotatedFace = RubiksCube.Face.Back;
                    else if (face == RubiksCube.Face.Back)
                        rotatedFace = RubiksCube.Face.Left;
                    else if (face == RubiksCube.Face.Left)
                        rotatedFace = RubiksCube.Face.Front;
                }
            }
            else
            {
                if ((move.Type == Move.RotationType.Clockwise && move.Layer != RubiksCube.Layer.Back) ||
                    (move.Type == Move.RotationType.Counterclockwise && move.Layer == RubiksCube.Layer.Back))
                {
                    if (face == RubiksCube.Face.Top)
                        rotatedFace = RubiksCube.Face.Right;
                    else if (face == RubiksCube.Face.Right)
                        rotatedFace = RubiksCube.Face.Bottom;
                    else if (face == RubiksCube.Face.Bottom)
                        rotatedFace = RubiksCube.Face.Left;
                    else if (face == RubiksCube.Face.Left)
                        rotatedFace = RubiksCube.Face.Top;
                }
                else
                {
                    if (face == RubiksCube.Face.Top)
                        rotatedFace = RubiksCube.Face.Left;
                    else if (face == RubiksCube.Face.Left)
                        rotatedFace = RubiksCube.Face.Bottom;
                    else if (face == RubiksCube.Face.Bottom)
                        rotatedFace = RubiksCube.Face.Right;
                    else if (face == RubiksCube.Face.Right)
                        rotatedFace = RubiksCube.Face.Top;
                }
            }

            return rotatedFace;
        }

        private Dictionary<Cubelet, Point3D[]> GetTargetVertices()
        {
            double xAngle = 0;
            double yAngle = 0;
            double zAngle = 0;
            switch (currentRotationInfo.Axis)
            {
                case RubiksCube.Axis.X:
                    xAngle = currentRotationInfo.TargetAngle;
                    break;
                case RubiksCube.Axis.Y:
                    yAngle = currentRotationInfo.TargetAngle;
                    break;
                case RubiksCube.Axis.Z:
                    zAngle = currentRotationInfo.TargetAngle;
                    break;
            }

            Dictionary<Cubelet, Point3D[]> targetCubeletsPosition = new();
            foreach (Cubelet cubelet in currentRotatingLayer)
            {
                Point3D[] targetVertices = new Point3D[cubelet.Vertices.Length];
                for (int i = 0; i < cubelet.Vertices.Length; i++)
                    targetVertices[i] = cubelet.Vertices[i].Rotate(xAngle, yAngle, zAngle);

                targetCubeletsPosition.Add(cubelet, targetVertices);
            }

            return targetCubeletsPosition;
        }

        public void ExecuteMoveQueue()
        {
            if (currentRotationInfo.IsRotating || moveQueue.Count == 0)
                return;

            currentRotationInfo.IsExecutingMoveQueue = true;
            currentRotationInfo.CurrentMoveIndex = 0;
            StartRotation(moveQueue[0]);
        }

        public void Scramble(List<Move> moves)
        {
            if (currentRotationInfo.IsRotating)
                return;

            foreach (Move move in moves)
            {
                RubiksCubeState newState = currentState.Clone();
                int targetAngle = move.TargetAngle;

                List<Cubelet> rotatingLayer = new();
                foreach (KeyValuePair<Cubelet, (sbyte, sbyte, sbyte)> cubeletPosition in currentState.CubeletsPosition)
                {
                    Cubelet cubelet = cubeletPosition.Key;
                    (sbyte oldX, sbyte oldY, sbyte oldZ) = cubeletPosition.Value;
                    sbyte newX = oldX;
                    sbyte newY = oldY;
                    sbyte newZ = oldZ;

                    if (cubelet.CurrentLayers.HasFlag(move.Layer))
                    {
                        switch (move.Axis)
                        {
                            case RubiksCube.Axis.X:
                                newY = (sbyte)(-1 * oldZ * Math.Sin(targetAngle * Math.PI / 180));
                                newZ = (sbyte)(oldY * Math.Sin(targetAngle * Math.PI / 180));
                                break;
                            case RubiksCube.Axis.Y:
                                newX = (sbyte)(oldZ * Math.Sin(targetAngle * Math.PI / 180));
                                newZ = (sbyte)(-1 * oldX * Math.Sin(targetAngle * Math.PI / 180));
                                break;
                            case RubiksCube.Axis.Z:
                                newX = (sbyte)(-1 * oldY * Math.Sin(targetAngle * Math.PI / 180));
                                newY = (sbyte)(oldX * Math.Sin(targetAngle * Math.PI / 180));
                                break;
                        }

                        for (byte i = 0; i < 6; i++)
                            newState.CubeletsFaces[cubelet][i] = cubelet.CurrentFaces[i] = RotateFace(cubelet.CurrentFaces[i], move);

                        rotatingLayer.Add(cubelet);
                    }

                    cubelet.CurrentPosition = (newX, newY, newZ);
                    newState.CubeletsPosition[cubelet] = cubelet.CurrentPosition;
                }

                currentState = newState;
                moveHistory.Add(move);
                if (IsSolved(currentState))
                    moveHistory.Clear();

                double xAngle = 0, yAngle = 0, zAngle = 0;
                switch (move.Axis)
                {
                    case RubiksCube.Axis.X: xAngle = targetAngle; break;
                    case RubiksCube.Axis.Y: yAngle = targetAngle; break;
                    case RubiksCube.Axis.Z: zAngle = targetAngle; break;
                }

                foreach (Cubelet cubelet in rotatingLayer)
                    for (int i = 0; i < cubelet.Vertices.Length; i++)
                        cubelet.Vertices[i] = cubelet.Vertices[i].Rotate(xAngle, yAngle, zAngle);
            }
        }

        public void GetSolutionMoves()
        {
            if (currentRotationInfo.IsRotating)
                return;

            moveQueue.Clear();
            if (IsSolved(currentState))
                return;

            var raw = new List<Move>();
            int i = moveHistory.Count - 1;
            while (i >= 0)
            {
                if (i >= 1 && moveHistory[i].IsCounterMove(moveHistory[i - 1]))
                {
                    i -= 2;
                    continue;
                }
                raw.Add(moveHistory[i].GetCounterMove());
                i--;
            }

            foreach (var m in CollapseTriples(raw))
                moveQueue.Add(m);
        }

        // Collapse the tail of s repeatedly:
        //   3 consecutive identical moves  →  their single counter move
        //   2 consecutive counter moves    →  both removed
        // Cascades until no more reductions are possible at the tail.
        private static IEnumerable<Move> CollapseTriples(IEnumerable<Move> moves)
        {
            var s = new List<Move>();
            foreach (var m in moves)
            {
                s.Add(m);
                bool changed = true;
                while (changed)
                {
                    changed = false;
                    int n = s.Count;
                    if (n >= 3)
                    {
                        var a = s[n - 3]; var b = s[n - 2]; var c = s[n - 1];
                        if (a.Layer == b.Layer && b.Layer == c.Layer && a.Type == b.Type && b.Type == c.Type)
                        {
                            s.RemoveRange(n - 3, 3);
                            s.Add(a.GetCounterMove());
                            changed = true;
                            continue;
                        }
                    }
                    if (n >= 2 && s[n - 2].IsCounterMove(s[n - 1]))
                    {
                        s.RemoveRange(n - 2, 2);
                        changed = true;
                    }
                }
            }
            return s;
        }

        // Returns true when solver succeeds and the move queue is populated; false when it fails
        // (queue is left empty — the caller decides how to notify the user).
        public bool GetSolverMoves(CancellationToken cancellationToken = default)
        {
            if (currentRotationInfo.IsRotating)
                return false;

            solverRequestVersion++;
            moveQueue.Clear();
            SolverError = null;

            try
            {
                var facelet = Solver.FaceletCube.FromControllerState(RubiksCube);
                moveQueue.AddRange(BuildSolverMoves(facelet, cancellationToken));
                return true;
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                SolverError = ex.Message;
                return false;
            }
        }

        public Task<bool> GetSolverMovesAsync(CancellationToken cancellationToken = default) =>
            GetSolverMovesAsync((facelet, token) => Task.Run(() => BuildSolverMoves(facelet, token), token), cancellationToken);

        // Isolate background calculation from result handling so cancellation and stale-state
        // behavior can be tested with a controlled completion, without relying on timing.
        internal async Task<bool> GetSolverMovesAsync(
            Func<Solver.FaceletCube, CancellationToken, Task<List<Move>>> solveAsync,
            CancellationToken cancellationToken = default)
        {
            if (currentRotationInfo.IsRotating)
                return false;

            int requestVersion = ++solverRequestVersion;
            moveQueue.Clear();
            SolverError = null;
            var stateAtStart = currentState;
            try
            {
                // Capture the live model on the UI thread. The worker only sees this snapshot.
                var facelet = Solver.FaceletCube.FromControllerState(RubiksCube);
                var moves = await solveAsync(facelet, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                if (requestVersion != solverRequestVersion)
                    return false;
                if (!ReferenceEquals(currentState, stateAtStart))
                {
                    SolverError = "The cube changed while the solution was being calculated.";
                    return false;
                }
                moveQueue.AddRange(moves);
                return true;
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                if (requestVersion == solverRequestVersion)
                    SolverError = ex.Message;
                return false;
            }
        }

        private static List<Move> BuildSolverMoves(Solver.FaceletCube facelet, CancellationToken cancellationToken)
        {
            var solverMoves = Solver.LayerByLayerSolver.Solve(facelet.Clone(), cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            facelet.ApplyMoves(solverMoves);
            if (!facelet.IsSolved())
                throw new InvalidOperationException("The solver returned a sequence that does not solve the cube.");

            return solverMoves.SelectMany(move => move.ToControllerMoves()).ToList();
        }

        public static bool IsSolved(RubiksCubeState rubiksCubeState)
        {
            // Compare the visible stickers, including their orientation. A solved
            // cube may have been rotated as a whole using the middle slices.
            var faceColors = new Dictionary<RubiksCube.Face, byte>();
            var faceCounts = new Dictionary<RubiksCube.Face, int>();
            RubiksCube.Layer[] stickerLayers =
            {
                RubiksCube.Layer.Up, RubiksCube.Layer.Down, RubiksCube.Layer.Left,
                RubiksCube.Layer.Right, RubiksCube.Layer.Front, RubiksCube.Layer.Back
            };

            foreach (var entry in rubiksCubeState.CubeletsPosition)
            {
                var cubelet = entry.Key;
                var (x, y, z) = entry.Value;
                for (byte slot = 0; slot < stickerLayers.Length; slot++)
                {
                    if (!cubelet.OriginalLayers.HasFlag(stickerLayers[slot]))
                        continue;

                    var face = rubiksCubeState.CubeletsFaces[cubelet][slot];
                    bool isOutside = face switch
                    {
                        RubiksCube.Face.Top => y == -1,
                        RubiksCube.Face.Bottom => y == 1,
                        RubiksCube.Face.Left => x == -1,
                        RubiksCube.Face.Right => x == 1,
                        RubiksCube.Face.Front => z == 1,
                        RubiksCube.Face.Back => z == -1,
                        _ => false
                    };
                    if (!isOutside || (faceColors.TryGetValue(face, out byte color) && color != slot))
                        return false;

                    faceColors[face] = slot;
                    faceCounts[face] = faceCounts.GetValueOrDefault(face) + 1;
                }
            }

            return faceCounts.Count == 6 && faceCounts.Values.All(count => count == 9);
        }

        public void Reset()
        {
            SolverError = null;
            moveHistory.Clear();
            moveQueue.Clear();
            currentRotationInfo = new RotationInfo { AnimationTime = currentRotationInfo.AnimationTime };
            currentRotatingLayer.Clear();
            targetCubeletsPosition = null;
            foreach (Cubelet cubelet in RubiksCube.Cubelets)
            {
                var (x, y, z) = cubelet.OriginalPosition;
                var initialCubelet = new Cubelet(RubiksCube, cubelet.Size, x, y, z);
                cubelet.CurrentPosition = cubelet.OriginalPosition;
                Array.Copy(initialCubelet.Vertices, cubelet.Vertices, cubelet.Vertices.Length);
                cubelet.CurrentFaces[0] = RubiksCube.Face.Top;
                cubelet.CurrentFaces[1] = RubiksCube.Face.Bottom;
                cubelet.CurrentFaces[2] = RubiksCube.Face.Left;
                cubelet.CurrentFaces[3] = RubiksCube.Face.Right;
                cubelet.CurrentFaces[4] = RubiksCube.Face.Front;
                cubelet.CurrentFaces[5] = RubiksCube.Face.Back;
                for (byte slot = 0; slot < 6; slot++)
                    cubelet.SetSelectionMode(slot, Face3D.SelectionMode.None);
            }
            currentState = new RubiksCubeState(RubiksCube.Cubelets.ToDictionary(
                cubelet => cubelet, cubelet => cubelet.CurrentPosition));
        }

        public void SetRotationInfo(RotationInfo rotationInfo)
        {
            currentRotationInfo = rotationInfo;
        }
        #endregion
    }
}
