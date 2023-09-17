namespace VirtualRubiksCube
{
    public class Point3D
    {
        #region Properties
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        #endregion

        #region Constructor
        public Point3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        #endregion

        #region Methods
        public Point3D Clone()
        {
            return new Point3D(X, Y, Z);
        }

        public Point3D Rotate(double xAngle, double yAngle, double zAngle) // Real axis rotation, not arbitrary axis rotation
        {
            Point3D startPoint = Clone();
            Point3D endPoint = Clone();

            if (zAngle != 0) // not used for projection
            {
                endPoint.X = startPoint.X * Math.Cos(zAngle * Math.PI / 180) - startPoint.Y * Math.Sin(zAngle * Math.PI / 180);
                endPoint.Y = startPoint.X * Math.Sin(zAngle * Math.PI / 180) + startPoint.Y * Math.Cos(zAngle * Math.PI / 180);
                startPoint = endPoint.Clone();
            }
            if (xAngle != 0)
            {
                endPoint.Y = startPoint.Y * Math.Cos(xAngle * Math.PI / 180) - startPoint.Z * Math.Sin(xAngle * Math.PI / 180);
                endPoint.Z = startPoint.Y * Math.Sin(xAngle * Math.PI / 180) + startPoint.Z * Math.Cos(xAngle * Math.PI / 180);
                startPoint = endPoint.Clone();
            }
            if (yAngle != 0)
            {
                endPoint.X = startPoint.Z * Math.Sin(yAngle * Math.PI / 180) + startPoint.X * Math.Cos(yAngle * Math.PI / 180);
                endPoint.Z = startPoint.Z * Math.Cos(yAngle * Math.PI / 180) - startPoint.X * Math.Sin(yAngle * Math.PI / 180);
            }

            return endPoint;
        }

        public Point3D Project(int viewWidth, int viewHeight, int imageDistance, int viewDistance)
        {
            double x = X * Convert.ToDouble(imageDistance) / viewDistance + viewWidth / 2;
            double y = Y * Convert.ToDouble(imageDistance) / viewDistance + viewHeight / 2;
            return new Point3D(x, y, Z); // only x and y are projected, z is used for ordering the faces
        }
        #endregion
    }
}
