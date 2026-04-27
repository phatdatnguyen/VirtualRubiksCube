using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace VirtualRubiksCube
{
    public class RubiksCubeRenderer
    {
        #region Fields
        private RubiksCube rubiksCube;
        private CancellationTokenSource cancellationTokenSource;
        private Task? renderTask;
        private List<double> frameTimes;
        private RenderInfo currentRenderInfo;

        // flags
        private bool isPaused;
        private bool isTerminating;
        #endregion

        #region Properties
        public bool IsRunning { get; private set; }
        public double FrameRate { get; private set; } // fps
        #endregion

        #region Event
        public delegate void RenderHandler(object sender, RenderEventArgs e);
        public event RenderHandler? OnRender;
        #endregion

        #region Constructor
        public RubiksCubeRenderer(RubiksCube rubiksCube, RenderInfo initialCommand)
        {
            this.rubiksCube = rubiksCube;
            rubiksCube.Renderer = this;

            frameTimes = new List<double>();
            IsRunning = false;

            currentRenderInfo = initialCommand;

            cancellationTokenSource = new CancellationTokenSource();
        }
        #endregion

        #region Methods
        public void Start()
        {
            if (!IsRunning)
            {
                IsRunning = true;
                renderTask = Task.Run(async () => await RenderLoop(cancellationTokenSource.Token));
            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                isPaused = true;
            }
        }

        public void Resume()
        {
            if (!IsRunning)
            {
                IsRunning = true;
                isPaused = false;
            }
        }

        public void Abort()
        {
            if (IsRunning)
            {
                IsRunning = false;
                isTerminating = true;
                cancellationTokenSource?.Cancel();
                try { renderTask?.Wait(1000); } catch { }
            }
        }

        private async Task RenderLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                // Check for termination request
                if (isTerminating)
                    return;

                // Check for pausing
                while (isPaused)
                {
                    await Task.Delay(100, cancellationToken);
                    if (isTerminating)
                        return;
                }

                // Rendering work
                Stopwatch stopwatch = new();
                stopwatch.Start();

                // Animate move
                if (rubiksCube.Controller != null)
                {
                    RotationInfo currentRotationInfo = rubiksCube.Controller.CurrentRotationInfo;
                    if (currentRotationInfo.IsRotating)
                    {
                        rubiksCube.Controller.SetRotationInfo(currentRotationInfo);
                        rubiksCube.Controller.RotateStep();
                    }
                }

                // Raise the event
                OnRender?.Invoke(this, new RenderEventArgs(currentRenderInfo));

                int targetFrameMs = (int)(1000.0 / 60);
                int delay = Math.Max(targetFrameMs - (int)stopwatch.ElapsedMilliseconds, 1);
                await Task.Delay(delay, cancellationToken);

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
                FrameRate = frameTimes[0] > 0 ? counter + ((1000 - ms) / frameTimes[0]) : counter;

                // Check for cancellation and exit the loop if requested
                if (cancellationToken.IsCancellationRequested)
                    break;
            }
        }

        private Face3D? GetMouseHoveredFace(List<Face3D> projectedFaces)
        {
            Face3D? mouseHoverFace = null;

            for (int i = projectedFaces.Count - 1; i >= 0; i--)
            {
                PointF[] vertices = projectedFaces[i].Vertices.Select(p => new PointF((float)p.X, (float)p.Y)).ToArray();
                using GraphicsPath graphicsPath = new();
                graphicsPath.AddPolygon(vertices);
                if (graphicsPath.IsVisible(currentRenderInfo.MousePosition))
                {
                    mouseHoverFace = projectedFaces[i];
                    break;
                }
            }

            return mouseHoverFace;
        }

        public Face3D? RenderFaces(Graphics graphics, RenderInfo renderInfo)
        {
            currentRenderInfo = renderInfo;

            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Rotate faces
            List<Face3D> rotatedFaces = new();
            foreach (Face3D face in rubiksCube.Faces)
                rotatedFaces.Add(face.Rotate(renderInfo.RotationX, renderInfo.RotationY, renderInfo.RotationZ));
            
            // Project faces
            List<Face3D> projectedFaces = new();
            foreach (Face3D face in rotatedFaces)
                projectedFaces.Add(face.Project(currentRenderInfo.ViewWidth, currentRenderInfo.ViewHeight, renderInfo.ImageDistance, currentRenderInfo.ViewDistance));

            projectedFaces = projectedFaces.OrderBy(p => p.MinZ).ToList();

            // Render
            Face3D? mouseHoveredFace = GetMouseHoveredFace(projectedFaces);
            foreach (Face3D projectedFace in projectedFaces)
            {
                if (projectedFace != mouseHoveredFace) // render last
                {
                    PointF[] vertices = projectedFace.Vertices.Select(p => new PointF((float)p.X, (float)p.Y)).ToArray();

                    if (projectedFace.SelectionStatus == Face3D.SelectionMode.Selected)
                    {
                        using var brush = new HatchBrush(HatchStyle.Percent90, Color.Black, GetFaceColor(projectedFace.ColorIndex));
                        graphics.FillPolygon(brush, vertices);
                    }
                    else if (projectedFace.SelectionStatus == Face3D.SelectionMode.SecondarySelection)
                    {
                        using var brush = new HatchBrush(HatchStyle.Percent60, Color.Black, GetFaceColor(projectedFace.ColorIndex));
                        graphics.FillPolygon(brush, vertices);
                    }
                    else
                    {
                        using var brush = new SolidBrush(GetFaceColor(projectedFace.ColorIndex));
                        graphics.FillPolygon(brush, vertices);
                    }
                    graphics.DrawPolygon(Pens.Black, vertices);
                }
            }
            if (mouseHoveredFace != null)
            {
                PointF[] vertices = mouseHoveredFace.Vertices.Select(p => new PointF((float)p.X, (float)p.Y)).ToArray();
                using var hoverBrush = new HatchBrush(HatchStyle.Percent25, Color.Gold, GetFaceColor(mouseHoveredFace.ColorIndex));
                using var hoverPen = new Pen(Color.Gold, 3f);
                graphics.FillPolygon(hoverBrush, vertices);
                graphics.DrawPolygon(hoverPen, vertices);
            }

            return mouseHoveredFace;
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
        #endregion
    }
}
