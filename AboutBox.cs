namespace VirtualRubiksCube
{
    partial class AboutBox : Form
    {
        #region Constructor
        public AboutBox()
        {
            InitializeComponent();
            Text = "About Virtual Rubik's Cube";
            labelProductName.Text = "Virtual Rubik's Cube";
            labelVersion.Text = "Version 1.0";
            labelCopyright.Text = "Copyright © Dat Nguyen";
            labelCompanyName.Text = "Dat Nguyen";
            textBoxDescription.Text = "Virtual Rubik's Cube";
        }
        #endregion
    }
}
