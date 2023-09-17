namespace VirtualRubiksCube
{
    public partial class ScrambleDialog : Form
    {
        #region Properties
        public int NumberOfMoves = 20;
        public bool IncludeMiddleLayerRotation = false;
        #endregion

        #region Constructor
        public ScrambleDialog()
        {
            InitializeComponent();
        }
        #endregion

        #region Method
        private void ScrambleDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                NumberOfMoves = Convert.ToInt32(numberOfMovesNumericUpDown.Value);
                IncludeMiddleLayerRotation = middleLayerCheckBox.Checked;
            }
        }
        #endregion
    }
}
