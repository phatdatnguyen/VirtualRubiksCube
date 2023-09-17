namespace VirtualRubiksCube
{
    public class RubiksCubeController
    {
        #region Fields
        private RubiksCubeState currentState;
        private List<RubiksCubeState> states = new();
        private List<Move> executedMoves = new();
        private List<Move> moveQueue = new();
        private RotationInfo currentRotationInfo; // shared with animation thread
        private List<Cubelet> currentRotatingLayer = new();
        private Dictionary<Cubelet, Point3D[]>? targetCubeletsPosition;
        #endregion

        #region Properties
        public RubiksCube RubiksCube { get; private set; }
        public RotationInfo CurrentRotationInfo { get { return currentRotationInfo; } }
        public List<Move> MoveQueue { get { return moveQueue; } }
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
        // on render thread
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
            else // last rotation step
            {
                foreach (Cubelet cubelet in currentRotatingLayer)
                    for (int i = 0; i < cubelet.Vertices.Length; i++)
                        if (targetCubeletsPosition != null)
                            cubelet.Vertices[i] = targetCubeletsPosition[cubelet][i];

                if (IsSolved(currentState))
                    Reset();

                if (currentRotationInfo.IsExecutingMoveQueue && currentRotationInfo.Move != moveQueue.Last())
                {
                    currentRotationInfo.CurrentMoveIndex += 1;
                    StartRotation(moveQueue[currentRotationInfo.CurrentMoveIndex]);
                }
                else
                {
                    if (currentRotationInfo.IsExecutingMoveQueue)
                    {
                        moveQueue.Clear();
                        currentRotationInfo.IsExecutingMoveQueue = false;
                    }

                    currentRotationInfo.IsRotating = false;
                    // Raise the event
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

            if (RubiksCube.Renderer != null)
                currentRotationInfo.RotationStep = currentRotationInfo.TargetAngle / ((double)(currentRotationInfo.AnimationTime / 1000.0) * RubiksCube.Renderer.FrameRate);
            targetCubeletsPosition = GetTargetVertices();
            currentRotationInfo.IsRotating = true; // Rotation info will be sent to animation thread of the renderer.

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

                List<Cubelet> currentRotatingLayer = new();
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

                        currentRotatingLayer.Add(cubelet);
                    }

                    cubelet.CurrentPosition = (newX, newY, newZ);
                    newState.CubeletsPosition[cubelet] = cubelet.CurrentPosition;
                }

                currentState = newState;
                states.Add(currentState);
                executedMoves.Add(move);

                double xAngle = 0;
                double yAngle = 0;
                double zAngle = 0;
                switch (move.Axis)
                {
                    case RubiksCube.Axis.X:
                        xAngle = targetAngle;
                        break;
                    case RubiksCube.Axis.Y:
                        yAngle = targetAngle;
                        break;
                    case RubiksCube.Axis.Z:
                        zAngle = targetAngle;
                        break;
                }

                foreach (Cubelet cubelet in currentRotatingLayer)
                    for (int i = 0; i < cubelet.Vertices.Length; i++)
                        cubelet.Vertices[i] = cubelet.Vertices[i].Rotate(xAngle, yAngle, zAngle);
            }
        }

        public void GetSolutionMoves()
        {
            moveQueue.Clear();
            if (IsSolved(currentState))
                return;

            int i = executedMoves.Count - 1;
            while (i >= 0)
            {
                // Skip a pair of counter moves
                if (i >= 1)
                    if (executedMoves[i].IsCounterMove(executedMoves[i - 1]))
                    {
                        i -= 2;
                        continue;
                    }

                moveQueue.Add(executedMoves[i].GetCounterMove());

                i--;
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
