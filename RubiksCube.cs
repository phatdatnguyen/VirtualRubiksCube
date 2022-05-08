using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualRubiksCube
{
    public class RubiksCube
    {
        // Fields
        private List<Cubelet> cubelets = new List<Cubelet>();
        private List<Face3D> faces = new List<Face3D>();

        // Enumerations
        [Flags]
        public enum Layer
        {
            None = 0,
            Up = 1,
            MiddleY = 2,
            Down = 4,
            Left = 8,
            MiddleX = 16,
            Right = 32,
            Front = 64,
            MiddleZ = 128,
            Back = 256
        }
        public enum Face { None, Top, Bottom, Left, Right, Front, Back }
        public enum Axis { X, Y, Z };

        // Properties
        public byte CubeletSize { get; private set; }
        public List<Cubelet> Cubelets { get { return cubelets; } }
        public List<Face3D> Faces
        {
            get
            {
                faces = new List<Face3D>();
                foreach (var cubelet in cubelets)
                    foreach (var face in cubelet.Faces)
                        faces.Add(face);

                return faces;
            }
        }
        public RubiksCubeRenderer Renderer { get; set; }
        public RubiksCubeController Controller { get; set; }

        // Constructor
        public RubiksCube(byte cubeletSize)
        {
            CubeletSize = cubeletSize;

            for (sbyte i = -1; i <= 1; i++)
                for (sbyte j = -1; j <= 1; j++)
                    for (sbyte k = -1; k <= 1; k++)
                        cubelets.Add(new Cubelet(this, cubeletSize, i, j, k));
        }

        // Methods
        public static Face GetFaceFromLayer(Layer layer)
        {
            if (layer == Layer.Up)
                return Face.Top;
            else if (layer == Layer.Down)
                return Face.Bottom;
            else if (layer == Layer.Left)
                return Face.Left;
            else if (layer == Layer.Right)
                return Face.Right;
            else if (layer == Layer.Front)
                return Face.Front;
            else if (layer == Layer.Back)
                return Face.Back;
            else
                return Face.None;
        }

        public static Layer GetLayerFromFace(Face face)
        {
            if (face == Face.Top)
                return Layer.Up;
            else if (face == Face.Bottom)
                return Layer.Down;
            else if (face == Face.Left)
                return Layer.Left;
            else if (face == Face.Right)
                return Layer.Right;
            else if (face == Face.Front)
                return Layer.Front;
            else if (face == Face.Back)
                return Layer.Back;
            else
                return Layer.None;
        }
    }
}
