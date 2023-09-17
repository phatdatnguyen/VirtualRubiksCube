using System.Reflection;

namespace VirtualRubiksCube
{
    public partial class MainForm : Form
    {
        #region Fields
        private RubiksCube rubiksCube;
        private RubiksCubeController controller;
        private RubiksCubeRenderer renderer;

        private RenderInfo currentRenderInfo = new();
        private int viewWidth = 600;
        private int viewHeight = 500;
        private int defaultImageDistance = 80;
        private int viewDistance = 100;
        private double defaultRotationX = -10;
        private double defaultRotationY = -20;
        private double defaultRotationZ = 0;

        private bool isRotating = false;
        private Point oldMousePosition;
        private BindingSource moveQueueBindingSource = new();
        private SettingDialog settingDialog = new();
        private Face3D? mouseHoveredFace;
        private Face3D? selectedFace;
        #endregion

        #region Constructors
        public MainForm()
        {
            InitializeComponent();

            // Enable double buffering for the diagramPanel
            typeof(Panel).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, renderPanel, new object[] { true });

            // Initial render info
            currentRenderInfo.ViewWidth = viewWidth;
            currentRenderInfo.ViewHeight = viewHeight;
            currentRenderInfo.ViewDistance = viewDistance;
            currentRenderInfo.TopFaceColor = settingDialog.TopFaceColor;
            currentRenderInfo.BottomFaceColor = settingDialog.BottomFaceColor;
            currentRenderInfo.LeftFaceColor = settingDialog.LeftFaceColor;
            currentRenderInfo.RightFaceColor = settingDialog.RightFaceColor;
            currentRenderInfo.FrontFaceColor = settingDialog.FrontFaceColor;
            currentRenderInfo.BackFaceColor = settingDialog.BackFaceColor;

            // Initialize the cube
            rubiksCube = new RubiksCube(100);

            RotationInfo rotationInfo = new();
            rotationInfo.IsRotating = false;
            rotationInfo.AnimationTime = settingDialog.AnimationTime;
            controller = new RubiksCubeController(rubiksCube, rotationInfo);
            controller.RotationStarted += new RubiksCubeController.RotationStartedHandler(OnRotationStarted);
            controller.RotationFinished += new RubiksCubeController.RotationFinishedHandler(OnRotationFinished);
            moveQueueBindingSource.DataSource = controller.MoveQueue;
            moveQueueListBox.DataSource = moveQueueBindingSource;

            currentRenderInfo.RotationX = defaultRotationX;
            currentRenderInfo.RotationY = defaultRotationY;
            currentRenderInfo.RotationZ = defaultRotationZ;
            currentRenderInfo.ImageDistance = defaultImageDistance;

            renderer = new RubiksCubeRenderer(rubiksCube, currentRenderInfo);
            renderer.OnRender += new RubiksCubeRenderer.RenderHandler(RenderCube);

            renderer.Start();

            // Register event handler
            renderPanel.MouseWheel += renderPanel_MouseWheel;
        }
        #endregion

        #region Methods
        // Form event handlers
        private void MainForm_Load(object sender, EventArgs e)
        {
            viewComboBox.SelectedIndex = 0;
        }

        private void ResetCube()
        {
            try
            {
                renderer?.Abort();

                rubiksCube = new RubiksCube(100);

                RotationInfo rotationInfo = new();
                rotationInfo.IsRotating = false;
                rotationInfo.AnimationTime = settingDialog.AnimationTime;
                controller = new RubiksCubeController(rubiksCube, rotationInfo);
                controller.RotationStarted += new RubiksCubeController.RotationStartedHandler(OnRotationStarted);
                controller.RotationFinished += new RubiksCubeController.RotationFinishedHandler(OnRotationFinished);
                moveQueueBindingSource.DataSource = controller.MoveQueue;
                moveQueueListBox.DataSource = moveQueueBindingSource;

                currentRenderInfo.RotationX = defaultRotationX;
                currentRenderInfo.RotationY = defaultRotationY;
                currentRenderInfo.RotationZ = defaultRotationZ;
                currentRenderInfo.ImageDistance = defaultImageDistance;

                renderer = new RubiksCubeRenderer(rubiksCube, currentRenderInfo);
                renderer.OnRender += RenderCube;

                renderer.Start();
            }
            catch { }
        }

        // pass info to render thread
        public void RenderCube(object sender, RenderEventArgs e)
        {
            currentRenderInfo = e.RenderInfo;

            renderPanel.Invalidate();
        }

        // pass info to render thread
        private void OnRotationStarted(object sender)
        {
            isRotating = true;
        }

        // pass info to render thread
        private void OnRotationFinished(object sender)
        {
            moveQueueListBox.DataSource ??= moveQueueBindingSource;

            isRotating = false;
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        // Menu
        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Do you want to reset the Rubik's cube?", "Virtual Rubik's Cube", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
                ResetCube();
        }

        private void scrambleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ScrambleDialog scrambleDialog = new();
            scrambleDialog.ShowDialog(this);
            if (scrambleDialog.DialogResult == DialogResult.OK)
            {
                int numberOfMoves = scrambleDialog.NumberOfMoves;
                List<Move> randomMoves = new();
                Random random = new();
                if (scrambleDialog.IncludeMiddleLayerRotation)
                {
                    List<Move> moves = new();
                    moves.Add(new(RubiksCube.Layer.Up, VirtualRubiksCube.Move.RotationType.Clockwise));
                    moves.Add(new(RubiksCube.Layer.Up, VirtualRubiksCube.Move.RotationType.Counterclockwise));
                    moves.Add(new(RubiksCube.Layer.MiddleY, VirtualRubiksCube.Move.RotationType.Clockwise));
                    moves.Add(new(RubiksCube.Layer.MiddleY, VirtualRubiksCube.Move.RotationType.Counterclockwise));
                    moves.Add(new(RubiksCube.Layer.Down, VirtualRubiksCube.Move.RotationType.Clockwise));
                    moves.Add(new(RubiksCube.Layer.Down, VirtualRubiksCube.Move.RotationType.Counterclockwise));
                    moves.Add(new(RubiksCube.Layer.Left, VirtualRubiksCube.Move.RotationType.Clockwise));
                    moves.Add(new(RubiksCube.Layer.Left, VirtualRubiksCube.Move.RotationType.Counterclockwise));
                    moves.Add(new(RubiksCube.Layer.MiddleX, VirtualRubiksCube.Move.RotationType.Clockwise));
                    moves.Add(new(RubiksCube.Layer.MiddleX, VirtualRubiksCube.Move.RotationType.Counterclockwise));
                    moves.Add(new(RubiksCube.Layer.Right, VirtualRubiksCube.Move.RotationType.Clockwise));
                    moves.Add(new(RubiksCube.Layer.Right, VirtualRubiksCube.Move.RotationType.Counterclockwise));
                    moves.Add(new(RubiksCube.Layer.Front, VirtualRubiksCube.Move.RotationType.Clockwise));
                    moves.Add(new(RubiksCube.Layer.Front, VirtualRubiksCube.Move.RotationType.Counterclockwise));
                    moves.Add(new(RubiksCube.Layer.MiddleZ, VirtualRubiksCube.Move.RotationType.Clockwise));
                    moves.Add(new(RubiksCube.Layer.MiddleZ, VirtualRubiksCube.Move.RotationType.Counterclockwise));
                    moves.Add(new(RubiksCube.Layer.Back, VirtualRubiksCube.Move.RotationType.Clockwise));
                    moves.Add(new(RubiksCube.Layer.Back, VirtualRubiksCube.Move.RotationType.Counterclockwise));


                    for (int i = 0; i < numberOfMoves; i++)
                    {
                        int index = random.Next(moves.Count);
                        randomMoves.Add(moves[index]);
                    }
                }
                else
                {
                    List<Move> moves = new();
                    moves.Add(new(RubiksCube.Layer.Up, VirtualRubiksCube.Move.RotationType.Clockwise));
                    moves.Add(new(RubiksCube.Layer.Up, VirtualRubiksCube.Move.RotationType.Counterclockwise));
                    moves.Add(new(RubiksCube.Layer.Down, VirtualRubiksCube.Move.RotationType.Clockwise));
                    moves.Add(new(RubiksCube.Layer.Down, VirtualRubiksCube.Move.RotationType.Counterclockwise));
                    moves.Add(new(RubiksCube.Layer.Left, VirtualRubiksCube.Move.RotationType.Clockwise));
                    moves.Add(new(RubiksCube.Layer.Left, VirtualRubiksCube.Move.RotationType.Counterclockwise));
                    moves.Add(new(RubiksCube.Layer.Right, VirtualRubiksCube.Move.RotationType.Clockwise));
                    moves.Add(new(RubiksCube.Layer.Right, VirtualRubiksCube.Move.RotationType.Counterclockwise));
                    moves.Add(new(RubiksCube.Layer.Front, VirtualRubiksCube.Move.RotationType.Clockwise));
                    moves.Add(new(RubiksCube.Layer.Front, VirtualRubiksCube.Move.RotationType.Counterclockwise));
                    moves.Add(new(RubiksCube.Layer.Back, VirtualRubiksCube.Move.RotationType.Clockwise));
                    moves.Add(new(RubiksCube.Layer.Back, VirtualRubiksCube.Move.RotationType.Counterclockwise));


                    for (int i = 0; i < numberOfMoves; i++)
                    {
                        int index = random.Next(moves.Count);
                        randomMoves.Add(moves[index]);
                    }
                }

                ResetCube();
                controller.Scramble(randomMoves);
            }
        }

        private void solveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!renderer.IsRunning || controller.CurrentRotationInfo.IsRotating)
                return;

            controller.GetSolutionMoves();
            controller.ExecuteMoveQueue();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!renderer.IsRunning || controller.CurrentRotationInfo.IsRotating)
                return;

            if (settingDialog.ShowDialog(this) == DialogResult.OK)
            {
                currentRenderInfo.TopFaceColor = settingDialog.TopFaceColor;
                currentRenderInfo.BottomFaceColor = settingDialog.BottomFaceColor;
                currentRenderInfo.LeftFaceColor = settingDialog.LeftFaceColor;
                currentRenderInfo.RightFaceColor = settingDialog.RightFaceColor;
                currentRenderInfo.FrontFaceColor = settingDialog.FrontFaceColor;
                currentRenderInfo.BackFaceColor = settingDialog.BackFaceColor;
                renderer.SetRenderInfo(currentRenderInfo);

                RotationInfo rotationInfo = controller.CurrentRotationInfo;
                rotationInfo.AnimationTime = settingDialog.AnimationTime;
                controller.SetRotationInfo(rotationInfo);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox aboutBox = new();
            aboutBox.ShowDialog(this);
        }

        // Render
        private void renderPanel_Paint(object sender, PaintEventArgs e)
        {
            if (!renderer.IsRunning)
                return;

            if (isRotating)
            {
                statusLabel.Text = "Status: Rotating";
                Cursor = Cursors.WaitCursor;
            }
            else
            {
                Cursor = Cursors.Default;
                statusLabel.Text = "Status: Ready";
            }

            rotationLabel.Text = "Rotation: x = " + currentRenderInfo.RotationX.ToString("F2") + "°; y = " + currentRenderInfo.RotationY.ToString("F2") + "°; z = " + currentRenderInfo.RotationZ.ToString("F2") + "°";
            if (mouseHoveredFace != null)
                faceLabel.Text = "Face: " + mouseHoveredFace.CurrentFace.ToString();
            else
                faceLabel.Text = "Face: ";

            mouseHoveredFace = renderer.RenderFaces(e.Graphics, currentRenderInfo);
        }

        // Mouse control
        private void renderPanel_MouseWheel(object? sender, MouseEventArgs e)
        {
            if (!renderer.IsRunning || e.X < 0 || e.X > currentRenderInfo.ViewWidth || e.Y < 0 || e.Y > currentRenderInfo.ViewHeight)
                return;

            int imageDistance = currentRenderInfo.ImageDistance;
            imageDistance += e.Delta / 30;
            if (imageDistance > 100)
                imageDistance = 100;
            if (imageDistance < 10)
                imageDistance = 10;

            zoomLabel.Text = "Zoom: " + imageDistance.ToString() + "%";

            RenderInfo newRenderInfo = currentRenderInfo;
            newRenderInfo.ImageDistance = imageDistance;
            renderer.SetRenderInfo(newRenderInfo);
        }

        private void renderPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (!renderer.IsRunning || e.X < 0 || e.X > currentRenderInfo.ViewWidth || e.Y < 0 || e.Y > currentRenderInfo.ViewHeight)
            {
                oldMousePosition = new Point(-1, -1);
                return;
            }

            if (oldMousePosition.X != -1 && oldMousePosition.Y != -1)
            {
                RenderInfo newRenderInfo = currentRenderInfo;
                newRenderInfo.MousePosition = e.Location;

                if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Middle)
                {
                    Cursor = Cursors.SizeAll;

                    int dX = e.X - oldMousePosition.X;
                    int dY = e.Y - oldMousePosition.Y;

                    if (Math.Abs(dX) > Math.Abs(dY))
                    {
                        newRenderInfo.RotationY = currentRenderInfo.RotationY + dX * 5;
                        if (ModifierKeys != Keys.Shift)
                            newRenderInfo.RotationX = currentRenderInfo.RotationX - dY * 5;
                    }
                    else
                    {
                        newRenderInfo.RotationX = currentRenderInfo.RotationX - dY * 5;
                        if (ModifierKeys != Keys.Shift)
                            newRenderInfo.RotationY = currentRenderInfo.RotationY + dX * 5;
                    }
                }
                else
                {
                    Cursor = Cursors.Arrow;
                }

                renderer.SetRenderInfo(newRenderInfo);
            }
            oldMousePosition = e.Location;
        }

        private void renderPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (!renderer.IsRunning || controller.CurrentRotationInfo.IsRotating || e.Button != MouseButtons.Left || mouseHoveredFace == null || e.X < 0 || e.X > 600 || e.Y < 0 || e.Y > 500)
                return;

            if (mouseHoveredFace.SelectionStatus == Face3D.SelectionMode.None)
            {
                if (selectedFace != null)
                    DeselectFace(selectedFace);

                SelectFace(mouseHoveredFace);
                selectedFace = mouseHoveredFace;
            }
            else if (mouseHoveredFace.SelectionStatus == Face3D.SelectionMode.Selected)
            {
                if (selectedFace != null)
                    DeselectFace(selectedFace);
                selectedFace = null;
            }
            else if (mouseHoveredFace.SelectionStatus == Face3D.SelectionMode.SecondarySelection)
            {
                if (selectedFace != null)
                {
                    DeselectFace(selectedFace);
                    ExecuteSelection(selectedFace, mouseHoveredFace);
                }
                selectedFace = null;
            }
        }

        private void SelectFace(Face3D face)
        {
            // Select the first face
            face.SetSelectionMode(Face3D.SelectionMode.Selected);

            // Find all possible secondary options
            foreach (Cubelet cubelet in rubiksCube.Cubelets)
            {
                if (cubelet == face.Cubelet)
                    continue;

                if (cubelet.CurrentLayers.HasFlag(RubiksCube.GetLayerFromFace(face.CurrentFace)))
                {
                    sbyte selectedX = face.Cubelet.CurrentPosition.Item1;
                    sbyte selectedY = face.Cubelet.CurrentPosition.Item2;
                    sbyte selectedZ = face.Cubelet.CurrentPosition.Item3;
                    sbyte xPosition = cubelet.CurrentPosition.Item1;
                    sbyte yPosition = cubelet.CurrentPosition.Item2;
                    sbyte zPosition = cubelet.CurrentPosition.Item3;

                    if ((face.CurrentFace == RubiksCube.Face.Top || face.CurrentFace == RubiksCube.Face.Bottom) &&
                        ((xPosition == selectedX && zPosition != selectedZ) || (xPosition != selectedX && zPosition == selectedZ)))
                        cubelet.SetSelectionMode(face.CurrentFace, Face3D.SelectionMode.SecondarySelection);

                    if ((face.CurrentFace == RubiksCube.Face.Left || face.CurrentFace == RubiksCube.Face.Right) &&
                        ((yPosition == selectedY && zPosition != selectedZ) || (yPosition != selectedY && zPosition == selectedZ)))
                        cubelet.SetSelectionMode(face.CurrentFace, Face3D.SelectionMode.SecondarySelection);

                    if ((face.CurrentFace == RubiksCube.Face.Front || face.CurrentFace == RubiksCube.Face.Back) &&
                        ((xPosition == selectedX && yPosition != selectedY) || (xPosition != selectedX && yPosition == selectedY)))
                        cubelet.SetSelectionMode(face.CurrentFace, Face3D.SelectionMode.SecondarySelection);
                }
            }
        }

        public void DeselectFace(Face3D face)
        {
            // Deselect the first face
            face.SetSelectionMode(Face3D.SelectionMode.None);

            // Find all possible secondary options
            foreach (Cubelet cubelet in rubiksCube.Cubelets)
            {
                if (cubelet == face.Cubelet)
                    continue;

                if (cubelet.CurrentLayers.HasFlag(RubiksCube.GetLayerFromFace(face.CurrentFace)))
                {
                    sbyte selectedX = face.Cubelet.CurrentPosition.Item1;
                    sbyte selectedY = face.Cubelet.CurrentPosition.Item2;
                    sbyte selectedZ = face.Cubelet.CurrentPosition.Item3;
                    sbyte xPosition = cubelet.CurrentPosition.Item1;
                    sbyte yPosition = cubelet.CurrentPosition.Item2;
                    sbyte zPosition = cubelet.CurrentPosition.Item3;

                    if ((face.CurrentFace == RubiksCube.Face.Top || face.CurrentFace == RubiksCube.Face.Bottom) &&
                        ((xPosition == selectedX && zPosition != selectedZ) || (xPosition != selectedX && zPosition == selectedZ)))
                        cubelet.SetSelectionMode(face.CurrentFace, Face3D.SelectionMode.None);

                    if ((face.CurrentFace == RubiksCube.Face.Left || face.CurrentFace == RubiksCube.Face.Right) &&
                        ((yPosition == selectedY && zPosition != selectedZ) || (yPosition != selectedY && zPosition == selectedZ)))
                        cubelet.SetSelectionMode(face.CurrentFace, Face3D.SelectionMode.None);

                    if ((face.CurrentFace == RubiksCube.Face.Front || face.CurrentFace == RubiksCube.Face.Back) &&
                        ((xPosition == selectedX && yPosition != selectedY) || (xPosition != selectedX && yPosition == selectedY)))
                        cubelet.SetSelectionMode(face.CurrentFace, Face3D.SelectionMode.None);
                }
            }
        }

        public void ExecuteSelection(Face3D firstFace, Face3D secondFace)
        {
            sbyte firstX = firstFace.Cubelet.CurrentPosition.Item1;
            sbyte firstY = firstFace.Cubelet.CurrentPosition.Item2;
            sbyte firstZ = firstFace.Cubelet.CurrentPosition.Item3;
            sbyte secondX = secondFace.Cubelet.CurrentPosition.Item1;
            sbyte secondY = secondFace.Cubelet.CurrentPosition.Item2;
            sbyte secondZ = secondFace.Cubelet.CurrentPosition.Item3;

            RubiksCube.Layer layer;
            Move.RotationType rotationType;
            // Front face
            if (firstFace.CurrentFace == RubiksCube.Face.Front)
            {
                if (firstX == -1 && secondX == -1) // left
                {
                    layer = RubiksCube.Layer.Left;

                    if (firstY < secondY)
                        rotationType = VirtualRubiksCube.Move.RotationType.Clockwise;
                    else
                        rotationType = VirtualRubiksCube.Move.RotationType.Counterclockwise;
                }
                else if (firstX == 0 && secondX == 0) // middle x
                {
                    layer = RubiksCube.Layer.MiddleX;

                    if (firstY < secondY)
                        rotationType = VirtualRubiksCube.Move.RotationType.Counterclockwise;
                    else
                        rotationType = VirtualRubiksCube.Move.RotationType.Clockwise;
                }
                else if (firstX == 1 && secondX == 1) // right
                {
                    layer = RubiksCube.Layer.Right;

                    if (firstY < secondY)
                        rotationType = VirtualRubiksCube.Move.RotationType.Counterclockwise;
                    else
                        rotationType = VirtualRubiksCube.Move.RotationType.Clockwise;
                }
                else if (firstY == -1 && secondY == -1) // up
                {
                    layer = RubiksCube.Layer.Up;

                    if (firstX < secondX)
                        rotationType = VirtualRubiksCube.Move.RotationType.Counterclockwise;
                    else
                        rotationType = VirtualRubiksCube.Move.RotationType.Clockwise;
                }
                else if (firstY == 0 && secondY == 0) // middle y
                {
                    layer = RubiksCube.Layer.MiddleY;

                    if (firstX < secondX)
                        rotationType = VirtualRubiksCube.Move.RotationType.Counterclockwise;
                    else
                        rotationType = VirtualRubiksCube.Move.RotationType.Clockwise;
                }
                else // down
                {
                    layer = RubiksCube.Layer.Down;

                    if (firstX < secondX)
                        rotationType = VirtualRubiksCube.Move.RotationType.Clockwise;
                    else
                        rotationType = VirtualRubiksCube.Move.RotationType.Counterclockwise;
                }

                Move move = new(layer, rotationType);
                controller.StartRotation(move);
                return;
            }

            // Back face
            if (firstFace.CurrentFace == RubiksCube.Face.Back)
            {
                if (firstX == -1 && secondX == -1) // left
                {
                    layer = RubiksCube.Layer.Left;

                    if (firstY < secondY)
                        rotationType = VirtualRubiksCube.Move.RotationType.Counterclockwise;
                    else
                        rotationType = VirtualRubiksCube.Move.RotationType.Clockwise;
                }
                else if (firstX == 0 && secondX == 0) // middle x
                {
                    layer = RubiksCube.Layer.MiddleX;

                    if (firstY < secondY)
                        rotationType = VirtualRubiksCube.Move.RotationType.Clockwise;
                    else
                        rotationType = VirtualRubiksCube.Move.RotationType.Counterclockwise;
                }
                else if (firstX == 1 && secondX == 1) // right
                {
                    layer = RubiksCube.Layer.Right;

                    if (firstY < secondY)
                        rotationType = VirtualRubiksCube.Move.RotationType.Clockwise;
                    else
                        rotationType = VirtualRubiksCube.Move.RotationType.Counterclockwise;
                }
                else if (firstY == -1 && secondY == -1) // up
                {
                    layer = RubiksCube.Layer.Up;

                    if (firstX < secondX)
                        rotationType = VirtualRubiksCube.Move.RotationType.Clockwise;
                    else
                        rotationType = VirtualRubiksCube.Move.RotationType.Counterclockwise;
                }
                else if (firstY == 0 && secondY == 0) // middle y
                {
                    layer = RubiksCube.Layer.MiddleY;

                    if (firstX < secondX)
                        rotationType = VirtualRubiksCube.Move.RotationType.Clockwise;
                    else
                        rotationType = VirtualRubiksCube.Move.RotationType.Counterclockwise;
                }
                else // down
                {
                    layer = RubiksCube.Layer.Down;

                    if (firstX < secondX)
                        rotationType = VirtualRubiksCube.Move.RotationType.Counterclockwise;
                    else
                        rotationType = VirtualRubiksCube.Move.RotationType.Clockwise;
                }

                Move move = new(layer, rotationType);
                controller.StartRotation(move);
                return;
            }

            // Right face
            if (firstFace.CurrentFace == RubiksCube.Face.Right)
            {
                if (firstZ == -1 && secondZ == -1) // back
                {
                    layer = RubiksCube.Layer.Back;

                    if (firstY < secondY)
                        rotationType = VirtualRubiksCube.Move.RotationType.Counterclockwise;
                    else
                        rotationType = VirtualRubiksCube.Move.RotationType.Clockwise;
                }
                else if (firstZ == 0 && secondZ == 0) // middle z
                {
                    layer = RubiksCube.Layer.MiddleZ;

                    if (firstY < secondY)
                        rotationType = VirtualRubiksCube.Move.RotationType.Clockwise;
                    else
                        rotationType = VirtualRubiksCube.Move.RotationType.Counterclockwise;
                }
                else if (firstZ == 1 && secondZ == 1) // front
                {
                    layer = RubiksCube.Layer.Front;

                    if (firstY < secondY)
                        rotationType = VirtualRubiksCube.Move.RotationType.Clockwise;
                    else
                        rotationType = VirtualRubiksCube.Move.RotationType.Counterclockwise;
                }
                else if (firstY == -1 && secondY == -1) // up
                {
                    layer = RubiksCube.Layer.Up;

                    if (firstZ < secondZ)
                        rotationType = VirtualRubiksCube.Move.RotationType.Clockwise;
                    else
                        rotationType = VirtualRubiksCube.Move.RotationType.Counterclockwise;
                }
                else if (firstY == 0 && secondY == 0) // middle y
                {
                    layer = RubiksCube.Layer.MiddleY;

                    if (firstZ < secondZ)
                        rotationType = VirtualRubiksCube.Move.RotationType.Clockwise;
                    else
                        rotationType = VirtualRubiksCube.Move.RotationType.Counterclockwise;
                }
                else // down
                {
                    layer = RubiksCube.Layer.Down;

                    if (firstZ < secondZ)
                        rotationType = VirtualRubiksCube.Move.RotationType.Counterclockwise;
                    else
                        rotationType = VirtualRubiksCube.Move.RotationType.Clockwise;
                }

                Move move = new(layer, rotationType);
                controller.StartRotation(move);
                return;
            }

            // Left face
            if (firstFace.CurrentFace == RubiksCube.Face.Left)
            {
                if (firstZ == -1 && secondZ == -1) // back
                {
                    layer = RubiksCube.Layer.Back;

                    if (firstY < secondY)
                        rotationType = VirtualRubiksCube.Move.RotationType.Clockwise;
                    else
                        rotationType = VirtualRubiksCube.Move.RotationType.Counterclockwise;
                }
                else if (firstZ == 0 && secondZ == 0) // middle z
                {
                    layer = RubiksCube.Layer.MiddleZ;

                    if (firstY < secondY)
                        rotationType = VirtualRubiksCube.Move.RotationType.Counterclockwise;
                    else
                        rotationType = VirtualRubiksCube.Move.RotationType.Clockwise;
                }
                else if (firstZ == 1 && secondZ == 1) // front
                {
                    layer = RubiksCube.Layer.Front;

                    if (firstY < secondY)
                        rotationType = VirtualRubiksCube.Move.RotationType.Counterclockwise;
                    else
                        rotationType = VirtualRubiksCube.Move.RotationType.Clockwise;
                }
                else if (firstY == -1 && secondY == -1) // up
                {
                    layer = RubiksCube.Layer.Up;

                    if (firstZ < secondZ)
                        rotationType = VirtualRubiksCube.Move.RotationType.Counterclockwise;
                    else
                        rotationType = VirtualRubiksCube.Move.RotationType.Clockwise;
                }
                else if (firstY == 0 && secondY == 0) // middle y
                {
                    layer = RubiksCube.Layer.MiddleY;

                    if (firstZ < secondZ)
                        rotationType = VirtualRubiksCube.Move.RotationType.Counterclockwise;
                    else
                        rotationType = VirtualRubiksCube.Move.RotationType.Clockwise;
                }
                else // down
                {
                    layer = RubiksCube.Layer.Down;

                    if (firstZ < secondZ)
                        rotationType = VirtualRubiksCube.Move.RotationType.Clockwise;
                    else
                        rotationType = VirtualRubiksCube.Move.RotationType.Counterclockwise;
                }

                Move move = new(layer, rotationType);
                controller.StartRotation(move);
                return;
            }

            // Top face
            if (firstFace.CurrentFace == RubiksCube.Face.Top)
            {
                if (firstZ == -1 && secondZ == -1) // back
                {
                    layer = RubiksCube.Layer.Back;

                    if (firstX < secondX)
                        rotationType = VirtualRubiksCube.Move.RotationType.Counterclockwise;
                    else
                        rotationType = VirtualRubiksCube.Move.RotationType.Clockwise;
                }
                else if (firstZ == 0 && secondZ == 0) // middle z
                {
                    layer = RubiksCube.Layer.MiddleZ;

                    if (firstX < secondX)
                        rotationType = VirtualRubiksCube.Move.RotationType.Clockwise;
                    else
                        rotationType = VirtualRubiksCube.Move.RotationType.Counterclockwise;
                }
                else if (firstZ == 1 && secondZ == 1) // front
                {
                    layer = RubiksCube.Layer.Front;

                    if (firstX < secondX)
                        rotationType = VirtualRubiksCube.Move.RotationType.Clockwise;
                    else
                        rotationType = VirtualRubiksCube.Move.RotationType.Counterclockwise;
                }
                else if (firstX == -1 && secondX == -1) // left
                {
                    layer = RubiksCube.Layer.Left;

                    if (firstZ < secondZ)
                        rotationType = VirtualRubiksCube.Move.RotationType.Clockwise;
                    else
                        rotationType = VirtualRubiksCube.Move.RotationType.Counterclockwise;
                }
                else if (firstX == 0 && secondX == 0) // middle x
                {
                    layer = RubiksCube.Layer.MiddleX;

                    if (firstZ < secondZ)
                        rotationType = VirtualRubiksCube.Move.RotationType.Counterclockwise;
                    else
                        rotationType = VirtualRubiksCube.Move.RotationType.Clockwise;
                }
                else // right
                {
                    layer = RubiksCube.Layer.Right; // right

                    if (firstZ < secondZ)
                        rotationType = VirtualRubiksCube.Move.RotationType.Counterclockwise;
                    else
                        rotationType = VirtualRubiksCube.Move.RotationType.Clockwise;
                }

                Move move = new(layer, rotationType);
                controller.StartRotation(move);
                return;
            }

            // Bottom face
            if (firstFace.CurrentFace == RubiksCube.Face.Bottom)
            {
                if (firstZ == -1 && secondZ == -1) // back
                {
                    layer = RubiksCube.Layer.Back;

                    if (firstX < secondX)
                        rotationType = VirtualRubiksCube.Move.RotationType.Clockwise;
                    else
                        rotationType = VirtualRubiksCube.Move.RotationType.Counterclockwise;
                }
                else if (firstZ == 0 && secondZ == 0) // middle z
                {
                    layer = RubiksCube.Layer.MiddleZ;

                    if (firstX < secondX)
                        rotationType = VirtualRubiksCube.Move.RotationType.Counterclockwise;
                    else
                        rotationType = VirtualRubiksCube.Move.RotationType.Clockwise;
                }
                else if (firstZ == 1 && secondZ == 1) // front
                {
                    layer = RubiksCube.Layer.Front;

                    if (firstX < secondX)
                        rotationType = VirtualRubiksCube.Move.RotationType.Counterclockwise;
                    else
                        rotationType = VirtualRubiksCube.Move.RotationType.Clockwise;
                }
                else if (firstX == -1 && secondX == -1) // left
                {
                    layer = RubiksCube.Layer.Left;

                    if (firstZ < secondZ)
                        rotationType = VirtualRubiksCube.Move.RotationType.Counterclockwise;
                    else
                        rotationType = VirtualRubiksCube.Move.RotationType.Clockwise;
                }
                else if (firstX == 0 && secondX == 0) // middle x
                {
                    layer = RubiksCube.Layer.MiddleX;

                    if (firstZ < secondZ)
                        rotationType = VirtualRubiksCube.Move.RotationType.Clockwise;
                    else
                        rotationType = VirtualRubiksCube.Move.RotationType.Counterclockwise;
                }
                else // right
                {
                    layer = RubiksCube.Layer.Right; // right

                    if (firstZ < secondZ)
                        rotationType = VirtualRubiksCube.Move.RotationType.Clockwise;
                    else
                        rotationType = VirtualRubiksCube.Move.RotationType.Counterclockwise;
                }

                Move move = new(layer, rotationType);
                controller.StartRotation(move);
                return;
            }
        }

        // Keyboard control
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Shift)
            {
                if (e.KeyCode == Keys.U)
                    uPrimeButton_Click(uPrimeButton, EventArgs.Empty);
                if (e.KeyCode == Keys.Y)
                    yPrimeButton_Click(yPrimeButton, EventArgs.Empty);
                if (e.KeyCode == Keys.D)
                    dPrimeButton_Click(dPrimeButton, EventArgs.Empty);
                if (e.KeyCode == Keys.L)
                    lPrimeButton_Click(lPrimeButton, EventArgs.Empty);
                if (e.KeyCode == Keys.X)
                    xPrimeButton_Click(xPrimeButton, EventArgs.Empty);
                if (e.KeyCode == Keys.R)
                    rPrimeButton_Click(rPrimeButton, EventArgs.Empty);
                if (e.KeyCode == Keys.F)
                    fPrimeButton_Click(fPrimeButton, EventArgs.Empty);
                if (e.KeyCode == Keys.Z)
                    zPrimeButton_Click(zPrimeButton, EventArgs.Empty);
                if (e.KeyCode == Keys.B)
                    bPrimeButton_Click(bPrimeButton, EventArgs.Empty);
            }
            else
            {
                if (e.KeyCode == Keys.U)
                    uButton_Click(uButton, EventArgs.Empty);
                if (e.KeyCode == Keys.Y)
                    yButton_Click(yButton, EventArgs.Empty);
                if (e.KeyCode == Keys.D)
                    dButton_Click(dButton, EventArgs.Empty);
                if (e.KeyCode == Keys.L)
                    lButton_Click(lButton, EventArgs.Empty);
                if (e.KeyCode == Keys.X)
                    xButton_Click(xButton, EventArgs.Empty);
                if (e.KeyCode == Keys.R)
                    rButton_Click(rButton, EventArgs.Empty);
                if (e.KeyCode == Keys.F)
                    fButton_Click(fButton, EventArgs.Empty);
                if (e.KeyCode == Keys.Z)
                    zButton_Click(zButton, EventArgs.Empty);
                if (e.KeyCode == Keys.B)
                    bButton_Click(bButton, EventArgs.Empty);
            }
        }

        // View
        private void viewComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (viewComboBox.Items.Count == 0)
                return;

            showViewButton_Click(showViewButton, EventArgs.Empty);
        }

        private void showViewButton_Click(object sender, EventArgs e)
        {
            if (!renderer.IsRunning || controller.CurrentRotationInfo.IsRotating)
                return;

            switch (viewComboBox.SelectedIndex)
            {
                case 0:
                    currentRenderInfo.RotationX = -10;
                    currentRenderInfo.RotationY = -20;
                    currentRenderInfo.RotationZ = 0;
                    break;
                case 1:
                    currentRenderInfo.RotationX = -90;
                    currentRenderInfo.RotationY = 0;
                    currentRenderInfo.RotationZ = 0;
                    break;
                case 2:
                    currentRenderInfo.RotationX = 90;
                    currentRenderInfo.RotationY = 0;
                    currentRenderInfo.RotationZ = 0;
                    break;
                case 3:
                    currentRenderInfo.RotationX = 0;
                    currentRenderInfo.RotationY = 90;
                    currentRenderInfo.RotationZ = 0;
                    break;
                case 4:
                    currentRenderInfo.RotationX = 0;
                    currentRenderInfo.RotationY = -90;
                    currentRenderInfo.RotationZ = 0;
                    break;
                case 5:
                    currentRenderInfo.RotationX = 0;
                    currentRenderInfo.RotationY = 0;
                    currentRenderInfo.RotationZ = 0;
                    break;
                case 6:
                    currentRenderInfo.RotationX = 0;
                    currentRenderInfo.RotationY = 180;
                    currentRenderInfo.RotationZ = 0;
                    break;
            }

            renderer.SetRenderInfo(currentRenderInfo);
        }

        // Moves and Queue
        private void uButton_Click(object sender, EventArgs e)
        {
            if (!renderer.IsRunning || controller.CurrentRotationInfo.IsRotating)
                return;

            Move newMove = new(RubiksCube.Layer.Up, VirtualRubiksCube.Move.RotationType.Clockwise);
            if (rotateRadioButton.Checked)
            {
                controller.StartRotation(newMove);
            }
            else
            {
                controller.MoveQueue.Add(newMove);
                moveQueueBindingSource.ResetBindings(false);
                moveQueueListBox.SelectedIndex = moveQueueListBox.Items.Count - 1;
            }
        }

        private void uPrimeButton_Click(object sender, EventArgs e)
        {
            if (!renderer.IsRunning || controller.CurrentRotationInfo.IsRotating)
                return;

            Move newMove = new(RubiksCube.Layer.Up, VirtualRubiksCube.Move.RotationType.Counterclockwise);
            if (rotateRadioButton.Checked)
            {
                controller.StartRotation(newMove);
            }
            else
            {
                controller.MoveQueue.Add(newMove);
                moveQueueBindingSource.ResetBindings(false);
                moveQueueListBox.SelectedIndex = moveQueueListBox.Items.Count - 1;
            }
        }

        private void yButton_Click(object sender, EventArgs e)
        {
            if (!renderer.IsRunning || controller.CurrentRotationInfo.IsRotating)
                return;

            Move newMove = new(RubiksCube.Layer.MiddleY, VirtualRubiksCube.Move.RotationType.Clockwise);
            if (rotateRadioButton.Checked)
            {
                controller.StartRotation(newMove);
            }
            else
            {
                controller.MoveQueue.Add(newMove);
                moveQueueBindingSource.ResetBindings(false);
                moveQueueListBox.SelectedIndex = moveQueueListBox.Items.Count - 1;
            }
        }

        private void yPrimeButton_Click(object sender, EventArgs e)
        {
            if (!renderer.IsRunning || controller.CurrentRotationInfo.IsRotating)
                return;

            Move newMove = new(RubiksCube.Layer.MiddleY, VirtualRubiksCube.Move.RotationType.Counterclockwise);
            if (rotateRadioButton.Checked)
            {
                controller.StartRotation(newMove);
            }
            else
            {
                controller.MoveQueue.Add(newMove);
                moveQueueBindingSource.ResetBindings(false);
                moveQueueListBox.SelectedIndex = moveQueueListBox.Items.Count - 1;
            }
        }

        private void dButton_Click(object sender, EventArgs e)
        {
            if (!renderer.IsRunning || controller.CurrentRotationInfo.IsRotating)
                return;

            Move newMove = new(RubiksCube.Layer.Down, VirtualRubiksCube.Move.RotationType.Clockwise);
            if (rotateRadioButton.Checked)
            {
                controller.StartRotation(newMove);
            }
            else
            {
                controller.MoveQueue.Add(newMove);
                moveQueueBindingSource.ResetBindings(false);
                moveQueueListBox.SelectedIndex = moveQueueListBox.Items.Count - 1;
            }
        }

        private void dPrimeButton_Click(object sender, EventArgs e)
        {
            if (!renderer.IsRunning || controller.CurrentRotationInfo.IsRotating)
                return;

            Move newMove = new(RubiksCube.Layer.Down, VirtualRubiksCube.Move.RotationType.Counterclockwise);
            if (rotateRadioButton.Checked)
            {
                controller.StartRotation(newMove);
            }
            else
            {
                controller.MoveQueue.Add(newMove);
                moveQueueBindingSource.ResetBindings(false);
                moveQueueListBox.SelectedIndex = moveQueueListBox.Items.Count - 1;
            }
        }

        private void lButton_Click(object sender, EventArgs e)
        {
            if (!renderer.IsRunning || controller.CurrentRotationInfo.IsRotating)
                return;

            Move newMove = new(RubiksCube.Layer.Left, VirtualRubiksCube.Move.RotationType.Clockwise);
            if (rotateRadioButton.Checked)
            {
                controller.StartRotation(newMove);
            }
            else
            {
                controller.MoveQueue.Add(newMove);
                moveQueueBindingSource.ResetBindings(false);
                moveQueueListBox.SelectedIndex = moveQueueListBox.Items.Count - 1;
            }
        }

        private void lPrimeButton_Click(object sender, EventArgs e)
        {
            if (!renderer.IsRunning || controller.CurrentRotationInfo.IsRotating)
                return;

            Move newMove = new(RubiksCube.Layer.Left, VirtualRubiksCube.Move.RotationType.Counterclockwise);
            if (rotateRadioButton.Checked)
            {
                controller.StartRotation(newMove);
            }
            else
            {
                controller.MoveQueue.Add(newMove);
                moveQueueBindingSource.ResetBindings(false);
                moveQueueListBox.SelectedIndex = moveQueueListBox.Items.Count - 1;
            }
        }

        private void xButton_Click(object sender, EventArgs e)
        {
            if (!renderer.IsRunning || controller.CurrentRotationInfo.IsRotating)
                return;

            Move newMove = new(RubiksCube.Layer.MiddleX, VirtualRubiksCube.Move.RotationType.Clockwise);
            if (rotateRadioButton.Checked)
            {
                controller.StartRotation(newMove);
            }
            else
            {
                controller.MoveQueue.Add(newMove);
                moveQueueBindingSource.ResetBindings(false);
                moveQueueListBox.SelectedIndex = moveQueueListBox.Items.Count - 1;
            }
        }

        private void xPrimeButton_Click(object sender, EventArgs e)
        {
            if (!renderer.IsRunning || controller.CurrentRotationInfo.IsRotating)
                return;

            Move newMove = new(RubiksCube.Layer.MiddleX, VirtualRubiksCube.Move.RotationType.Counterclockwise);
            if (rotateRadioButton.Checked)
            {
                controller.StartRotation(newMove);
            }
            else
            {
                controller.MoveQueue.Add(newMove);
                moveQueueBindingSource.ResetBindings(false);
                moveQueueListBox.SelectedIndex = moveQueueListBox.Items.Count - 1;
            }
        }

        private void rButton_Click(object sender, EventArgs e)
        {
            if (!renderer.IsRunning || controller.CurrentRotationInfo.IsRotating)
                return;

            Move newMove = new(RubiksCube.Layer.Right, VirtualRubiksCube.Move.RotationType.Clockwise);
            if (rotateRadioButton.Checked)
            {
                controller.StartRotation(newMove);
            }
            else
            {
                controller.MoveQueue.Add(newMove);
                moveQueueBindingSource.ResetBindings(false);
                moveQueueListBox.SelectedIndex = moveQueueListBox.Items.Count - 1;
            }
        }

        private void rPrimeButton_Click(object sender, EventArgs e)
        {
            if (!renderer.IsRunning || controller.CurrentRotationInfo.IsRotating)
                return;

            Move newMove = new(RubiksCube.Layer.Right, VirtualRubiksCube.Move.RotationType.Counterclockwise);
            if (rotateRadioButton.Checked)
            {
                controller.StartRotation(newMove);
            }
            else
            {
                controller.MoveQueue.Add(newMove);
                moveQueueBindingSource.ResetBindings(false);
                moveQueueListBox.SelectedIndex = moveQueueListBox.Items.Count - 1;
            }
        }

        private void fButton_Click(object sender, EventArgs e)
        {
            if (!renderer.IsRunning || controller.CurrentRotationInfo.IsRotating)
                return;

            Move newMove = new(RubiksCube.Layer.Front, VirtualRubiksCube.Move.RotationType.Clockwise);
            if (rotateRadioButton.Checked)
            {
                controller.StartRotation(newMove);
            }
            else
            {
                controller.MoveQueue.Add(newMove);
                moveQueueBindingSource.ResetBindings(false);
                moveQueueListBox.SelectedIndex = moveQueueListBox.Items.Count - 1;
            }
        }

        private void fPrimeButton_Click(object sender, EventArgs e)
        {
            if (!renderer.IsRunning || controller.CurrentRotationInfo.IsRotating)
                return;

            Move newMove = new(RubiksCube.Layer.Front, VirtualRubiksCube.Move.RotationType.Counterclockwise);
            if (rotateRadioButton.Checked)
            {
                controller.StartRotation(newMove);
            }
            else
            {
                controller.MoveQueue.Add(newMove);
                moveQueueBindingSource.ResetBindings(false);
                moveQueueListBox.SelectedIndex = moveQueueListBox.Items.Count - 1;
            }
        }

        private void zButton_Click(object sender, EventArgs e)
        {
            if (!renderer.IsRunning || controller.CurrentRotationInfo.IsRotating)
                return;

            Move newMove = new(RubiksCube.Layer.MiddleZ, VirtualRubiksCube.Move.RotationType.Clockwise);
            if (rotateRadioButton.Checked)
            {
                controller.StartRotation(newMove);
            }
            else
            {
                controller.MoveQueue.Add(newMove);
                moveQueueBindingSource.ResetBindings(false);
                moveQueueListBox.SelectedIndex = moveQueueListBox.Items.Count - 1;
            }
        }

        private void zPrimeButton_Click(object sender, EventArgs e)
        {
            if (!renderer.IsRunning || controller.CurrentRotationInfo.IsRotating)
                return;

            Move newMove = new(RubiksCube.Layer.MiddleZ, VirtualRubiksCube.Move.RotationType.Counterclockwise);
            if (rotateRadioButton.Checked)
            {
                controller.StartRotation(newMove);
            }
            else
            {
                controller.MoveQueue.Add(newMove);
                moveQueueBindingSource.ResetBindings(false);
                moveQueueListBox.SelectedIndex = moveQueueListBox.Items.Count - 1;
            }
        }

        private void bButton_Click(object sender, EventArgs e)
        {
            if (!renderer.IsRunning || controller.CurrentRotationInfo.IsRotating)
                return;

            Move newMove = new(RubiksCube.Layer.Back, VirtualRubiksCube.Move.RotationType.Clockwise);
            if (rotateRadioButton.Checked)
            {
                controller.StartRotation(newMove);
            }
            else
            {
                controller.MoveQueue.Add(newMove);
                moveQueueBindingSource.ResetBindings(false);
                moveQueueListBox.SelectedIndex = moveQueueListBox.Items.Count - 1;
            }
        }

        private void bPrimeButton_Click(object sender, EventArgs e)
        {
            if (!renderer.IsRunning || controller.CurrentRotationInfo.IsRotating)
                return;

            Move newMove = new(RubiksCube.Layer.Back, VirtualRubiksCube.Move.RotationType.Counterclockwise);
            if (rotateRadioButton.Checked)
            {
                controller.StartRotation(newMove);
            }
            else
            {
                controller.MoveQueue.Add(newMove);
                moveQueueBindingSource.ResetBindings(false);
                moveQueueListBox.SelectedIndex = moveQueueListBox.Items.Count - 1;
            }
        }

        private void executeQueueButton_Click(object sender, EventArgs e)
        {
            if (!renderer.IsRunning || controller.CurrentRotationInfo.IsRotating || controller.MoveQueue.Count == 0)
                return;

            moveQueueListBox.DataSource = null;
            moveQueueListBox.Items.Clear();
            controller.ExecuteMoveQueue();
        }

        private void clearQueueButton_Click(object sender, EventArgs e)
        {
            if (!renderer.IsRunning || controller.CurrentRotationInfo.IsRotating || controller.MoveQueue.Count == 0)
                return;

            controller.MoveQueue.Clear();
            moveQueueBindingSource.ResetBindings(false);
        }

        private void setQueueToSolutionButton_Click(object sender, EventArgs e)
        {
            if (!renderer.IsRunning || controller.CurrentRotationInfo.IsRotating)
                return;

            controller.GetSolutionMoves();
            moveQueueBindingSource.ResetBindings(false);
        }
        #endregion
    }
}
