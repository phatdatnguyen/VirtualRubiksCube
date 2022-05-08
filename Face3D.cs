using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualRubiksCube
{
    public class Face3D
    {
        // Enumeration
        public enum SelectionMode { None, Selected, SecondarySelection }

        // Properties
        public Cubelet Cubelet { get; private set; }
        public Point3D[] Vertices { get; set; }
        public byte CubeletFaceIndex { get; private set; } //0: top; 1: bottom; 2: left; 3: right; 4: front; 5: back
        public SelectionMode SelectionStatus { get; set; }
        public byte ColorIndex { get; private set; } // 0: none; 1: top; 2: bottom; 3: left; 4: right; 5: front; 6: back
        public double MinZ
        { 
            get
            {
                return Vertices.Min(x => x.Z);
            }
        } // used for ordering the faces
        public RubiksCube.Face CurrentFace
        {
            get
            {
                return Cubelet.CurrentFaces[CubeletFaceIndex];
            }
        }

        // Constructor
        public Face3D(Cubelet cubelet, Point3D[] vertices, byte cubeletFaceIndex, SelectionMode selectionStatus, byte colorIndex = 0)
        {
            Cubelet = cubelet;
            Vertices = vertices;
            CubeletFaceIndex = cubeletFaceIndex;
            SelectionStatus = selectionStatus;
            ColorIndex = colorIndex;
        }

        // Methods
        public Face3D Rotate(double xAngle, double yAngle, double zAngle)
        {
            Point3D[] rotatedVertices = Vertices.Select(p => p.Rotate(xAngle, yAngle, zAngle)).ToArray();
            return new Face3D(Cubelet, rotatedVertices, CubeletFaceIndex, SelectionStatus, ColorIndex);
        }
        
        public Face3D Project(int viewWidth, int viewHeight, int imageDistance, int viewDistance)
        {
            Point3D[] projectedVertices = Vertices.Select(p => p.Project(viewWidth, viewHeight,imageDistance, viewDistance)).ToArray();
            return new Face3D(Cubelet, projectedVertices, CubeletFaceIndex, SelectionStatus, ColorIndex);
        }

        public void SetSelectionMode(SelectionMode selectionMode)
        {
            Cubelet.SetSelectionMode(CubeletFaceIndex, selectionMode);
        }
    }
}
