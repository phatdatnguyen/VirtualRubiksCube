namespace VirtualRubiksCube
{
    public class RubiksCubeController
    {
        #region Fields
        private RubiksCubeState currentState;
        private List<RubiksCubeState> states = new();
        private List<Move> executedMoves = new();
        private List<Move> moveHistory = new();  // history for reversal; reset when cube reaches solved state
        private List<Move> moveQueue = new();
        private RotationInfo currentRotationInfo;
        private List<Cubelet> currentRotatingLayer = new();
        private Dictionary<Cubelet, Point3D[]>? targetCubeletsPosition;
        #endregion

        #region Properties
        public RubiksCube RubiksCube { get; private set; }
        public RotationInfo CurrentRotationInfo { get { return currentRotationInfo; } }
        public List<Move> MoveQueue { get { return moveQueue; } }
        public IReadOnlyList<Cubelet> CurrentRotatingLayer { get { return currentRotatingLayer; } }
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
            states.Add(new RubiksCubeState(initialCubeletsPosition));

            currentState = states[0];
        }
        #endregion

        #region Methods
        public void RotateStep()
        {
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

                if (currentRotationInfo.IsExecutingMoveQueue && currentRotationInfo.Move != moveQueue.Last())
                {
                    currentRotationInfo.CurrentMoveIndex += 1;
                    // Between moves: if the cube is solved, reset moveHistory so the
                    // remaining queue moves build a fresh history for reversal.
                    if (IsSolved(currentState))
                        moveHistory.Clear();
                    StartRotation(moveQueue[currentRotationInfo.CurrentMoveIndex]);
                }
                else
                {
                    if (currentRotationInfo.IsExecutingMoveQueue)
                    {
                        moveQueue.Clear();
                        currentRotationInfo.IsExecutingMoveQueue = false;
                        // Last queue move just finished in solved state — fresh history start.
                        if (IsSolved(currentState))
                            moveHistory.Clear();
                    }

                    currentRotationInfo.IsRotating = false;
                    RotationFinished?.Invoke(this);
                }
            }
        }

        // on render thread
        public void StartRotation(Move move)
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
                        cubelet.CurrentFaces[i] = RotateFace(cubelet.CurrentFaces[i], move);

                    currentRotatingLayer.Add(cubelet);
                }
                    
                cubelet.CurrentPosition = (newX, newY, newZ);
                newState.CubeletsPosition[cubelet] = cubelet.CurrentPosition;
            }

            currentState = newState;
            states.Add(currentState);
            executedMoves.Add(move);
            moveHistory.Add(move);

            double frameRate = 60.0;
            currentRotationInfo.RotationStep = currentRotationInfo.TargetAngle / ((currentRotationInfo.AnimationTime / 1000.0) * frameRate);
            targetCubeletsPosition = GetTargetVertices();
            currentRotationInfo.IsRotating = true;

            if (!currentRotationInfo.IsExecutingMoveQueue ||
                (currentRotationInfo.IsExecutingMoveQueue && move == moveQueue[0]))
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
            if (moveQueue.Count == 0)
                return;

            currentRotationInfo.IsExecutingMoveQueue = true;
            currentRotationInfo.CurrentMoveIndex = 0;
            StartRotation(moveQueue[0]);
        }

        public void Scramble(List<Move> moves)
        {
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
                            cubelet.CurrentFaces[i] = RotateFace(cubelet.CurrentFaces[i], move);

                        rotatingLayer.Add(cubelet);
                    }

                    cubelet.CurrentPosition = (newX, newY, newZ);
                    newState.CubeletsPosition[cubelet] = cubelet.CurrentPosition;
                }

                currentState = newState;
                states.Add(currentState);
                executedMoves.Add(move);
                moveHistory.Add(move);

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
        public bool GetSolverMoves()
        {
            moveQueue.Clear();
            if (IsSolved(currentState))
                return true;

            try
            {
                var facelet = Solver.FaceletCube.FromControllerState(RubiksCube);
                var solverMoves = Solver.LayerByLayerSolver.Solve(facelet);
                foreach (var sm in solverMoves)
                    foreach (var m in sm.ToControllerMoves())
                        moveQueue.Add(m);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsSolved(RubiksCubeState rubiksCubeState)
        {
            bool isSolved = true;

            foreach (Cubelet cubelet in rubiksCubeState.CubeletsPosition.Keys)
                if (cubelet.CurrentLayers != cubelet.OriginalLayers)
                    isSolved = false;

            return isSolved;
        }

        public void Reset()
        {
            executedMoves.Clear();
            moveHistory.Clear();
            states.RemoveRange(1, states.Count - 1);
            currentState = states[0];
            foreach (Cubelet cubelet in RubiksCube.Cubelets)
            {
                cubelet.CurrentFaces[0] = RubiksCube.Face.Top;
                cubelet.CurrentFaces[1] = RubiksCube.Face.Bottom;
                cubelet.CurrentFaces[2] = RubiksCube.Face.Left;
                cubelet.CurrentFaces[3] = RubiksCube.Face.Right;
                cubelet.CurrentFaces[4] = RubiksCube.Face.Front;
                cubelet.CurrentFaces[5] = RubiksCube.Face.Back;
            }
        }

        public void SetRotationInfo(RotationInfo rotationInfo)
        {
            currentRotationInfo = rotationInfo;
        }
        #endregion
    }
}
