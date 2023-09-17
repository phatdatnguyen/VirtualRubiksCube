namespace VirtualRubiksCube
{
    public struct RenderInfo
    {
        #region Fields
        private double rotationX;
        private double rotationY;
        private double rotationZ;
        #endregion

        #region Properties
        public Point MousePosition { get; set; }

        public double RotationX
        {
            readonly get
            {
                return rotationX;
            }
            set
            {
                rotationX = value;
                while (rotationX > 360)
                    rotationX -= 360;
                while (rotationX <= -360)
                    rotationX += 360;
            }
        }
        public double RotationY
        {
            readonly get
            {
                return rotationY;
            }
            set
            {
                rotationY = value;
                while (rotationY > 360)
                    rotationY -= 360;
                while (rotationY <= -360)
                    rotationY += 360;
            }
        }
        public double RotationZ
        {
            readonly get
            {
                return rotationZ;
            }
            set
            {
                rotationZ = value;
                while (rotationZ > 360)
                    rotationZ -= 360;
                while (rotationZ <= -360)
                    rotationZ += 360;
            }
        }

        public int ViewWidth { get; set; }
        public int ViewHeight { get; set; }
        public int ImageDistance { get; set; }
        public int ViewDistance { get; set; }

        public Color TopFaceColor { get; set; }
        public Color BottomFaceColor { get; set; }
        public Color LeftFaceColor { get; set; }
        public Color RightFaceColor { get; set; }
        public Color FrontFaceColor { get; set; }
        public Color BackFaceColor { get; set; }
        #endregion
    }
}
