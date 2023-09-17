namespace VirtualRubiksCube
{
    public partial class SettingDialog : Form
    {
        #region Properties
        public Color TopFaceColor { get; set; }
        public Color BottomFaceColor { get; set; }
        public Color LeftFaceColor { get; set; }
        public Color RightFaceColor { get; set; }
        public Color FrontFaceColor { get; set; }
        public Color BackFaceColor { get; set; }
        public int AnimationTime { get; set; }
        #endregion

        #region Constructor
        public SettingDialog()
        {
            InitializeComponent();

            TopFaceColor = Color.White;
            BottomFaceColor = Color.Yellow;
            LeftFaceColor = Color.Orange;
            RightFaceColor = Color.Red;
            FrontFaceColor = Color.Green;
            BackFaceColor = Color.Blue;
            AnimationTime = 200;
        }
        #endregion

        #region Methods
        private void topButton_Click(object sender, EventArgs e)
        {
            colorDialog.Color = TopFaceColor;
            if (colorDialog.ShowDialog(this) == DialogResult.OK)
                topButton.BackColor = colorDialog.Color;
        }

        private void leftButton_Click(object sender, EventArgs e)
        {
            colorDialog.Color = LeftFaceColor;
            if (colorDialog.ShowDialog(this) == DialogResult.OK)
                leftButton.BackColor = colorDialog.Color;
        }

        private void frontButton_Click(object sender, EventArgs e)
        {
            colorDialog.Color = FrontFaceColor;
            if (colorDialog.ShowDialog(this) == DialogResult.OK)
                frontButton.BackColor = colorDialog.Color;
        }

        private void bottomButton_Click(object sender, EventArgs e)
        {
            colorDialog.Color = BottomFaceColor;
            if (colorDialog.ShowDialog(this) == DialogResult.OK)
                bottomButton.BackColor = colorDialog.Color;
        }

        private void rightButton_Click(object sender, EventArgs e)
        {
            colorDialog.Color = RightFaceColor;
            if (colorDialog.ShowDialog(this) == DialogResult.OK)
                rightButton.BackColor = colorDialog.Color;
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            colorDialog.Color = BackFaceColor;
            if (colorDialog.ShowDialog(this) == DialogResult.OK)
                backButton.BackColor = colorDialog.Color;
        }

        private void animationTimeTrackBar_Scroll(object sender, EventArgs e)
        {
            animationTimeLabel.Text = ((double)animationTimeTrackBar.Value * 10 / 1000).ToString() + " s";
        }

        private void defaultButton_Click(object sender, EventArgs e)
        {
            topButton.BackColor = Color.White;
            bottomButton.BackColor = Color.Yellow;
            leftButton.BackColor = Color.Red;
            rightButton.BackColor = Color.Orange;
            frontButton.BackColor = Color.Green;
            backButton.BackColor = Color.Blue;
            animationTimeTrackBar.Value = 20;
            animationTimeLabel.Text = "0.2 s";
        }

        private void SettingDialog_Load(object sender, EventArgs e)
        {
            topButton.BackColor = TopFaceColor;
            bottomButton.BackColor = BottomFaceColor;
            leftButton.BackColor = LeftFaceColor;
            rightButton.BackColor = RightFaceColor;
            frontButton.BackColor = FrontFaceColor;
            backButton.BackColor = BackFaceColor;
            animationTimeTrackBar.Value = AnimationTime / 10;
            animationTimeLabel.Text = ((double)AnimationTime / 1000).ToString() + " s";
        }

        private void SettingDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                TopFaceColor = topButton.BackColor;
                BottomFaceColor = bottomButton.BackColor;
                LeftFaceColor = leftButton.BackColor;
                RightFaceColor = rightButton.BackColor;
                FrontFaceColor = frontButton.BackColor;
                BackFaceColor = backButton.BackColor;
                AnimationTime = animationTimeTrackBar.Value * 10;
            }
        }
        #endregion
    }
}
