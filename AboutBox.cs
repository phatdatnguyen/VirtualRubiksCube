using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VirtualRubiksCube
{
    partial class AboutBox : Form
    {
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
    }
}
