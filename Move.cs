namespace VirtualRubiksCube
{
    public class Move
    {
        #region Enumeration
        public enum RotationType { Clockwise, Counterclockwise }
        #endregion

        #region Properties
        public RubiksCube.Layer Layer { get; private set; }
        public RotationType Type { get; private set; }
        public RubiksCube.Axis Axis
        {
            get
            {
                if (Layer == RubiksCube.Layer.Up ||
                    Layer == RubiksCube.Layer.MiddleY ||
                    Layer == RubiksCube.Layer.Down)
                    return RubiksCube.Axis.Y;
                else if (Layer == RubiksCube.Layer.Left ||
                    Layer == RubiksCube.Layer.MiddleX ||
                    Layer == RubiksCube.Layer.Right)
                    return RubiksCube.Axis.X;
                else
                    return RubiksCube.Axis.Z;
            }
        }
        public int TargetAngle
        {
            get
            {
                if (Type == Move.RotationType.Clockwise)
                {
                    if (Layer == RubiksCube.Layer.Down ||
                        Layer == RubiksCube.Layer.MiddleX ||
                        Layer == RubiksCube.Layer.Right ||
                        Layer == RubiksCube.Layer.Front ||
                        Layer == RubiksCube.Layer.MiddleZ)
                        return 90;
                    else
                        return -90;
                }
                else
                {
                    if (Layer == RubiksCube.Layer.Down ||
                        Layer == RubiksCube.Layer.MiddleX ||
                        Layer == RubiksCube.Layer.Right ||
                        Layer == RubiksCube.Layer.Front ||
                        Layer == RubiksCube.Layer.MiddleZ)
                        return -90;
                    else
                        return 90;
                }
            }
        }
        #endregion

        #region Constructor
        public Move(RubiksCube.Layer layer, RotationType type)
        {
            Type = type;
            Layer = layer;
        }
        #endregion

        #region Methods
        public Move GetCounterMove()
        {
            if (Type == RotationType.Clockwise)
                return new Move(Layer, RotationType.Counterclockwise);
            else
                return new Move(Layer, RotationType.Clockwise);
        }

        public bool IsCounterMove(Move move)
        {
            return Layer == move.Layer && Type != move.Type;
        }

        public override string ToString()
        {
            return Layer.ToString() + " - " + Type.ToString();
        }
        #endregion
    }
}
