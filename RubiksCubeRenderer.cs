using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VirtualRubiksCube
{
    public class RubiksCubeRenderer
    {
        // Fields
        private RubiksCube rubiksCube;
        private Thread animateThread;
        private Thread renderThread;
        private AutoResetEvent[] animateHandle;
        private AutoResetEvent[] renderHandle;
        //private RenderInfo[] renderInfos;
        private List<double> frameTimes;
        private RenderInfo currentRenderInfo;

        // Properties
        public bool IsRunning { get; private set; }
        public double FrameRate { get; private set; } // fps

        // Event
        public delegate void RenderHandler(object sender, RenderEventArgs e);
        public event RenderHandler OnRender;

        // Constructor
        public RubiksCubeRenderer(RubiksCube rubiksCube, RenderInfo initialCommand)
        {
            this.rubiksCube = rubiksCube;
            rubiksCube.Renderer = this;

            frameTimes = new List<double>();
            IsRunning = false;

            animateHandle = new AutoResetEvent[2];
            for (int i = 0; i < animateHandle.Length; i++)
                animateHandle[i] = new AutoResetEvent(false);

            renderHandle = new AutoResetEvent[2];
            for (int i = 0; i < renderHandle.Length; i++)
                renderHandle[i] = new AutoResetEvent(true);

            currentRenderInfo = initialCommand;
        }

        // Methods
        public void Start()
        {
            if (!IsRunning)
            {
                IsRunning = true;
                animateThread = new Thread(AnimateLoop);
                animateThread.Start();
                renderThread = new Thread(RenderLoop);
                renderThread.Start();
            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                animateThread.Join();
                renderThread.Join();
                FrameRate = 0;
                frameTimes.Clear();
            }
        }

        public void Abort()
        {
            if (IsRunning)
            {
                IsRunning = false;
                animateThread.Abort();
                renderThread.Abort();
            }
        }

        private void AnimateLoop()
        {
            Stopwatch stopwatch = new Stopwatch();
            int bufferIndex = 0x0;

            while (IsRunning)
            {
                stopwatch.Start();
                Animate(bufferIndex);
                bufferIndex ^= 0x1;
                stopwatch.Reset();
            }
        }

        private void Animate(int bufferIndex)
        {
            renderHandle[bufferIndex].WaitOne();

            //Update
            RotationInfo currentRotationInfo = rubiksCube.Controller.CurrentRotationInfo;
            if (currentRotationInfo.IsRotating)
            {
                rubiksCube.Controller.SetRotationInfo(currentRotationInfo);
                rubiksCube.Controller.RotateStep();
            }

            animateHandle[bufferIndex].Set();
        }

        private void RenderLoop()
        {
            Stopwatch stopwatch = new Stopwatch();
            int bufferIndex = 0x0;

            while (IsRunning)
            {
                stopwatch.Restart();
                BroadcastRender(bufferIndex);
                bufferIndex ^= 0x1;

                if (stopwatch.ElapsedMilliseconds < 15)
                    Thread.Sleep(Math.Max(15 - (int)stopwatch.ElapsedMilliseconds, 0));
                while (stopwatch.Elapsed.TotalMilliseconds < 1000.0 / 60) { }

                stopwatch.Stop();

                frameTimes.Add(stopwatch.Elapsed.TotalMilliseconds);
                int counter = 0;
                int index = frameTimes.Count - 1;
                double ms = 0;
                while (index >= 0 && ms + frameTimes[index] <= 1000)
                {
                    ms += frameTimes[index];
                    counter++;
                    index--;
                }
                if (index > 0) frameTimes.RemoveRange(0, index);
                FrameRate = counter + ((1000 - ms) / frameTimes[0]);
            }
        }

        private void BroadcastRender(int bufferIndex)
        {
            animateHandle[bufferIndex].WaitOne();

            if (OnRender == null)
                return;
            //OnRender(this, new RenderEventArgs(currentRenderInfos[currentBufferIndex]));
            OnRender(this, new RenderEventArgs(currentRenderInfo));

            renderHandle[bufferIndex].Set();
        }

        public Face3D Render(Graphics graphics, RenderInfo renderInfo) // return the face that has mouse cursor on it
        {
            currentRenderInfo = renderInfo;

            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Rotate faces
            List<Face3D> rotatedFaces = new List<Face3D>();
            foreach (Face3D face in rubiksCube.Faces)
                rotatedFaces.Add(face.Rotate(renderInfo.RotationX, renderInfo.RotationY, renderInfo.RotationZ));
            
            // Project faces
            List<Face3D> projectedFaces = new List<Face3D>();
            foreach (Face3D face in rotatedFaces)
                projectedFaces.Add(face.Project(currentRenderInfo.ViewWidth, currentRenderInfo.ViewHeight, renderInfo.ImageDistance, currentRenderInfo.ViewDistance));

            projectedFaces = projectedFaces.OrderBy(p => p.MinZ).ToList();

            // Render
            Face3D mouseHoveredFace = GetMoveHoveredFace(projectedFaces);
            foreach (Face3D projectedFace in projectedFaces)
            {
                if (projectedFace != mouseHoveredFace) // render last
                {
                    PointF[] vertices = projectedFace.Vertices.Select(p => new PointF((float)p.X, (float)p.Y)).ToArray();

                    if (projectedFace.SelectionStatus == Face3D.SelectionMode.Selected)
                        graphics.FillPolygon(new HatchBrush(HatchStyle.Percent90, Color.Black, GetFaceColor(projectedFace.ColorIndex)), vertices);
                    else if (projectedFace.SelectionStatus == Face3D.SelectionMode.SecondarySelection)
                        graphics.FillPolygon(new HatchBrush(HatchStyle.Percent60, Color.Black, GetFaceColor(projectedFace.ColorIndex)), vertices);
                    else
                        graphics.FillPolygon(new SolidBrush(GetFaceColor(projectedFace.ColorIndex)), vertices);
                    graphics.DrawPolygon(Pens.Black, vertices);
                }
            }
            if (mouseHoveredFace != null)
            {
                PointF[] vertices = mouseHoveredFace.Vertices.Select(p => new PointF((float)p.X, (float)p.Y)).ToArray();
                graphics.FillPolygon(new HatchBrush(HatchStyle.Percent25, Color.Gold, GetFaceColor(mouseHoveredFace.ColorIndex)), vertices);
                graphics.DrawPolygon(new Pen(Color.Gold, 3f), vertices);
            }

            return mouseHoveredFace;
        }

        private Face3D GetMoveHoveredFace(List<Face3D> projectedFaces)
        {
            Face3D mouseHoverFace = null;

            for (int i = projectedFaces.Count - 1; i >= 0; i--)
            {
                PointF[] vertices = projectedFaces[i].Vertices.Select(p => new PointF((float)p.X, (float)p.Y)).ToArray();
                GraphicsPath graphicsPath = new GraphicsPath();
                graphicsPath.AddPolygon(vertices);
                if (graphicsPath.IsVisible(currentRenderInfo.MousePosition))
                {
                    mouseHoverFace = projectedFaces[i];
                    break;
                }
            }

            return mouseHoverFace;
        }

        public void SetRenderInfo(RenderInfo renderInfo)
        {
            currentRenderInfo = renderInfo;
        }

        private Color GetFaceColor(byte colorIndex)
        {
            Color faceColor = Color.Black;
            if (colorIndex == 1)
                faceColor = currentRenderInfo.TopFaceColor;
            if (colorIndex == 2)
                faceColor = currentRenderInfo.BottomFaceColor;
            if (colorIndex == 3)
                faceColor = currentRenderInfo.LeftFaceColor;
            if (colorIndex == 4)
                faceColor = currentRenderInfo.RightFaceColor;
            if (colorIndex == 5)
                faceColor = currentRenderInfo.FrontFaceColor;
            if (colorIndex == 6)
                faceColor = currentRenderInfo.BackFaceColor;

            return faceColor;
        }
    }

    public class RenderEventArgs : EventArgs
    {
        public RenderInfo RenderInfo { get; private set; }
        public RenderEventArgs(RenderInfo renderInfo)
        {
            RenderInfo = renderInfo;
        }
    }

    public struct RenderInfo
    {
        // Fields
        private double rotationX;
        private double rotationY;
        private double rotationZ;

        // Properties
        public Point MousePosition { get; set; }

        public double RotationX
        {
            get
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
            get
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
            get
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
    }
}
