using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VirtualRubiksCube
{
    public partial class ScrambleDialog : Form
    {
        // Property
        public int NumberOfMoves = 20;
        public bool IncludeMiddleLayerRotation = false;

        // Constructor
        public ScrambleDialog()
        {
            InitializeComponent();
        }

        // Methods
        private void ScrambleDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                NumberOfMoves = Convert.ToInt32(numberOfMovesNumericUpDown.Value);
                IncludeMiddleLayerRotation = middleLayerCheckBox.Checked;
            }
        }
    }
}
