namespace VirtualRubiksCube
{
    public class Cubelet
    {
        #region Fields
        private Point3D[] vertices = new Point3D[8];
        private Face3D[] faces = new Face3D[6];
        private Dictionary<byte, Face3D.SelectionMode> facesSelectionMode = new();
        private Dictionary<byte, RubiksCube.Face> currentFaces = new();
        #endregion

        #region Properties
        public RubiksCube RubiksCube { get; private set; }
        public byte Size { get; private set; }
        public Point3D[] Vertices { get { return vertices; } }
        public Face3D[] Faces
        {
            get
            {
                // top face
                if (OriginalLayers.HasFlag(RubiksCube.Layer.Up))
                    faces[0] = new Face3D(this, new Point3D[] { vertices[0], vertices[1], vertices[2], vertices[3] }, 0, facesSelectionMode[0], 1);
                else
                    faces[0] = new Face3D(this, new Point3D[] { vertices[0], vertices[1], vertices[2], vertices[3] }, 0, facesSelectionMode[0], 0);

                // bottom face
                if (OriginalLayers.HasFlag(RubiksCube.Layer.Down))
                    faces[1] = new Face3D(this, new Point3D[] { vertices[4], vertices[5], vertices[6], vertices[7] }, 1, facesSelectionMode[1], 2);
                else
                    faces[1] = new Face3D(this, new Point3D[] { vertices[4], vertices[5], vertices[6], vertices[7] }, 1, facesSelectionMode[1], 0);

                // left face
                if (OriginalLayers.HasFlag(RubiksCube.Layer.Left))
                    faces[2] = new Face3D(this, new Point3D[] { vertices[0], vertices[3], vertices[7], vertices[4] }, 2, facesSelectionMode[2], 3);
                else
                    faces[2] = new Face3D(this, new Point3D[] { vertices[0], vertices[3], vertices[7], vertices[4] }, 2, facesSelectionMode[2], 0);

                // right face
                if (OriginalLayers.HasFlag(RubiksCube.Layer.Right))
                    faces[3] = new Face3D(this, new Point3D[] { vertices[1], vertices[2], vertices[6], vertices[5] }, 3, facesSelectionMode[3], 4);
                else
                    faces[3] = new Face3D(this, new Point3D[] { vertices[1], vertices[2], vertices[6], vertices[5] }, 3, facesSelectionMode[3], 0);

                // front face
                if (OriginalLayers.HasFlag(RubiksCube.Layer.Front))
                    faces[4] = new Face3D(this, new Point3D[] { vertices[3], vertices[2], vertices[6], vertices[7] }, 4, facesSelectionMode[4], 5);
                else
                    faces[4] = new Face3D(this, new Point3D[] { vertices[3], vertices[2], vertices[6], vertices[7] }, 4, facesSelectionMode[4], 0);

                // back face
                if (OriginalLayers.HasFlag(RubiksCube.Layer.Back))
                    faces[5] = new Face3D(this, new Point3D[] { vertices[0], vertices[1], vertices[5], vertices[4] }, 5, facesSelectionMode[5], 6);
                else
                    faces[5] = new Face3D(this, new Point3D[] { vertices[0], vertices[1], vertices[5], vertices[4] }, 5, facesSelectionMode[5], 0);

                return faces;
            }
        }
        public Dictionary<byte, RubiksCube.Face> CurrentFaces { get { return currentFaces; } }
        public (sbyte, sbyte, sbyte) OriginalPosition { get; private set; }
        public (sbyte, sbyte, sbyte) CurrentPosition { get; set; }
        public RubiksCube.Layer OriginalLayers
        {
            get
            {
                (sbyte x, sbyte y, sbyte z) = OriginalPosition;

                RubiksCube.Layer yLayer;
                if (y == -1)
                    yLayer = RubiksCube.Layer.Up;
                else if (y == 0)
                    yLayer = RubiksCube.Layer.MiddleY;
                else
                    yLayer = RubiksCube.Layer.Down;

                RubiksCube.Layer xLayer;
                if (x == 1)
                    xLayer = RubiksCube.Layer.Right;
                else if (x == 0)
                    xLayer = RubiksCube.Layer.MiddleX;
                else
                    xLayer = RubiksCube.Layer.Left;

                RubiksCube.Layer zLayer;
                if (z == 1)
                    zLayer = RubiksCube.Layer.Front;
                else if (z == 0)
                    zLayer = RubiksCube.Layer.MiddleZ;
                else
                    zLayer = RubiksCube.Layer.Back;

                return xLayer | yLayer | zLayer;
            }
        }
        public RubiksCube.Layer CurrentLayers
        {
            get
            {
                (sbyte x, sbyte y, sbyte z) = CurrentPosition;

                RubiksCube.Layer yLayer;
                if (y == -1)
                    yLayer = RubiksCube.Layer.Up;
                else if (y == 0)
                    yLayer = RubiksCube.Layer.MiddleY;
                else
                    yLayer = RubiksCube.Layer.Down;

                RubiksCube.Layer xLayer;
                if (x == 1)
                    xLayer = RubiksCube.Layer.Right;
                else if (x == 0)
                    xLayer = RubiksCube.Layer.MiddleX;
                else
                    xLayer = RubiksCube.Layer.Left;

                RubiksCube.Layer zLayer;
                if (z == 1)
                    zLayer = RubiksCube.Layer.Front;
                else if (z == 0)
                    zLayer = RubiksCube.Layer.MiddleZ;
                else
                    zLayer = RubiksCube.Layer.Back;

                return xLayer | yLayer | zLayer;
            }
        }
        public bool IsCenter
        {
            get
            {
                (sbyte x, sbyte y, sbyte z) = CurrentPosition;

                return (x == 0 && y == 0 && z == -1) ||
                    (x == 0 && y == 0 && z == 1) ||
                    (x == 0 && y == -1 && z == 0) ||
                    (x == 0 && y == 1 && z == 0) ||
                    (x == -1 && y == 0 && z == 0) ||
                    (x == 1 && y == 0 && z == 0);
            }
        }
        public bool IsCorner
        {
            get
            {
                (sbyte x, sbyte y, sbyte z) = CurrentPosition;

                return (x == -1 && y == -1 && z == -1) ||
                    (x == -1 && y == -1 && z == 1) ||
                    (x == -1 && y == 1 && z == -1) ||
                    (x == -1 && y == 1 && z == 1) ||
                    (x == 1 && y == -1 && z == -1) ||
                    (x == 1 && y == -1 && z == 1) ||
                    (x == 1 && y == 1 && z == -1) ||
                    (x == 1 && y == 1 && z == 1);
            }
        }
        public bool IsEdge
        {
            get
            {
                return !(IsCenter || IsCorner);
            }
        }
        #endregion

        #region Constructor
        public Cubelet(RubiksCube rubiksCube, byte size, sbyte xPosition, sbyte yPosition, sbyte zPosition)
        {
            RubiksCube = rubiksCube;
            Size = size;

            OriginalPosition = (xPosition, yPosition, zPosition);
            CurrentPosition = (xPosition, yPosition, zPosition);

            vertices[0] = new Point3D(-0.5 * size + xPosition * size, -0.5 * size + yPosition * size, -0.5 * size + zPosition * size);
            vertices[1] = new Point3D(0.5 * size + xPosition * size, -0.5 * size + yPosition * size, -0.5 * size + zPosition * size);
            vertices[2] = new Point3D(0.5 * size + xPosition * size, -0.5 * size + yPosition * size, 0.5 * size + zPosition * size);
            vertices[3] = new Point3D(-0.5 * size + xPosition * size, -0.5 * size + yPosition * size, 0.5 * size + zPosition * size);
            vertices[4] = new Point3D(-0.5 * size + xPosition * size, 0.5 * size + yPosition * size, -0.5 * size + zPosition * size);
            vertices[5] = new Point3D(0.5 * size + xPosition * size, 0.5 * size + yPosition * size, -0.5 * size + zPosition * size);
            vertices[6] = new Point3D(0.5 * size + xPosition * size, 0.5 * size + yPosition * size, 0.5 * size + zPosition * size);
            vertices[7] = new Point3D(-0.5 * size + xPosition * size, 0.5 * size + yPosition * size, 0.5 * size + zPosition * size);

            facesSelectionMode.Add(0, Face3D.SelectionMode.None);
            facesSelectionMode.Add(1, Face3D.SelectionMode.None);
            facesSelectionMode.Add(2, Face3D.SelectionMode.None);
            facesSelectionMode.Add(3, Face3D.SelectionMode.None);
            facesSelectionMode.Add(4, Face3D.SelectionMode.None);
            facesSelectionMode.Add(5, Face3D.SelectionMode.None);

            currentFaces.Add(0, RubiksCube.Face.Top);
            currentFaces.Add(1, RubiksCube.Face.Bottom);
            currentFaces.Add(2, RubiksCube.Face.Left);
            currentFaces.Add(3, RubiksCube.Face.Right);
            currentFaces.Add(4, RubiksCube.Face.Front);
            currentFaces.Add(5, RubiksCube.Face.Back);
        }
        #endregion

        #region Methods
        public void SetSelectionMode(byte cubeletFaceIndex, Face3D.SelectionMode selectionMode)
        {
            facesSelectionMode[cubeletFaceIndex] = selectionMode;
        }

        public void SetSelectionMode(RubiksCube.Face face, Face3D.SelectionMode selectionMode)
        {
            byte faceIndex = 0;
            for (byte i = 0; i < currentFaces.Count; i++)
                if (currentFaces[i] == face)
                {
                    faceIndex = i;
                    break;
                }

            facesSelectionMode[faceIndex] = selectionMode;
        }

        public Face3D GetCurrentFace(RubiksCube.Face face)
        {
            byte faceIndex = 0;
            for (byte i = 0; i < currentFaces.Count; i++)
                if (currentFaces[i] == face)
                {
                    faceIndex = i;
                    break;
                }

            return faces[faceIndex];
        }
        #endregion
    }
}
