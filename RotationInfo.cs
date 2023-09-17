namespace VirtualRubiksCube
{
    public struct RotationInfo
    {
        #region Fields
        private Move move;
        private double rotationStep;
        #endregion

        #region Properties
        public int AnimationTime { get; set; } //in ms
        public bool IsRotating { get; set; }
        public bool IsExecutingMoveQueue { get; set; }
        public Move Move
        {
            readonly get { return move; }
            set
            {
                move = value;

                Axis = move.Axis;
                TargetAngle = move.TargetAngle;
            }
        }
        public RubiksCube.Axis Axis { get; private set; }
        public int TargetAngle { get; private set; }
        public double RotationStep
        {
            readonly get { return rotationStep; }
            set
            {
                rotationStep = value;
                NumberOfSteps = Convert.ToInt32(TargetAngle / rotationStep);
                CurrentStep = 1;
            }
        }
        public int NumberOfSteps { get; private set; }
        public int CurrentStep { get; set; }
        public int CurrentMoveIndex { get; set; }
        #endregion
    }
}
