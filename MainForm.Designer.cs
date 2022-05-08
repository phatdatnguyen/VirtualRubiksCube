namespace VirtualRubiksCube
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.moveQueueListBox = new System.Windows.Forms.ListBox();
            this.zPrimeButton = new System.Windows.Forms.Button();
            this.zButton = new System.Windows.Forms.Button();
            this.xPrimeButton = new System.Windows.Forms.Button();
            this.xButton = new System.Windows.Forms.Button();
            this.yPrimeButton = new System.Windows.Forms.Button();
            this.yButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.setQueueToSolutionButton = new System.Windows.Forms.Button();
            this.clearQueueButton = new System.Windows.Forms.Button();
            this.executeQueueButton = new System.Windows.Forms.Button();
            this.bPrimeButton = new System.Windows.Forms.Button();
            this.bButton = new System.Windows.Forms.Button();
            this.fPrimeButton = new System.Windows.Forms.Button();
            this.fButton = new System.Windows.Forms.Button();
            this.lPrimeButton = new System.Windows.Forms.Button();
            this.lButton = new System.Windows.Forms.Button();
            this.rPrimeButton = new System.Windows.Forms.Button();
            this.rButton = new System.Windows.Forms.Button();
            this.dPrimeButton = new System.Windows.Forms.Button();
            this.dButton = new System.Windows.Forms.Button();
            this.uPrimeButton = new System.Windows.Forms.Button();
            this.uButton = new System.Windows.Forms.Button();
            this.addToQueueRadioButton = new System.Windows.Forms.RadioButton();
            this.rotateRadioButton = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.faceLabel = new System.Windows.Forms.Label();
            this.zoomLabel = new System.Windows.Forms.Label();
            this.rotationLabel = new System.Windows.Forms.Label();
            this.statusLabel = new System.Windows.Forms.Label();
            this.showViewButton = new System.Windows.Forms.Button();
            this.viewGroupBox = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.viewComboBox = new System.Windows.Forms.ComboBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scrambleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.solveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.viewGroupBox.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.moveQueueListBox);
            this.groupBox1.Controls.Add(this.zPrimeButton);
            this.groupBox1.Controls.Add(this.zButton);
            this.groupBox1.Controls.Add(this.xPrimeButton);
            this.groupBox1.Controls.Add(this.xButton);
            this.groupBox1.Controls.Add(this.yPrimeButton);
            this.groupBox1.Controls.Add(this.yButton);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.setQueueToSolutionButton);
            this.groupBox1.Controls.Add(this.clearQueueButton);
            this.groupBox1.Controls.Add(this.executeQueueButton);
            this.groupBox1.Controls.Add(this.bPrimeButton);
            this.groupBox1.Controls.Add(this.bButton);
            this.groupBox1.Controls.Add(this.fPrimeButton);
            this.groupBox1.Controls.Add(this.fButton);
            this.groupBox1.Controls.Add(this.lPrimeButton);
            this.groupBox1.Controls.Add(this.lButton);
            this.groupBox1.Controls.Add(this.rPrimeButton);
            this.groupBox1.Controls.Add(this.rButton);
            this.groupBox1.Controls.Add(this.dPrimeButton);
            this.groupBox1.Controls.Add(this.dButton);
            this.groupBox1.Controls.Add(this.uPrimeButton);
            this.groupBox1.Controls.Add(this.uButton);
            this.groupBox1.Controls.Add(this.addToQueueRadioButton);
            this.groupBox1.Controls.Add(this.rotateRadioButton);
            this.groupBox1.Location = new System.Drawing.Point(618, 85);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(354, 482);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Moves";
            // 
            // moveQueueListBox
            // 
            this.moveQueueListBox.FormattingEnabled = true;
            this.moveQueueListBox.Location = new System.Drawing.Point(81, 311);
            this.moveQueueListBox.Name = "moveQueueListBox";
            this.moveQueueListBox.Size = new System.Drawing.Size(267, 134);
            this.moveQueueListBox.TabIndex = 20;
            // 
            // zPrimeButton
            // 
            this.zPrimeButton.Location = new System.Drawing.Point(177, 250);
            this.zPrimeButton.Name = "zPrimeButton";
            this.zPrimeButton.Size = new System.Drawing.Size(171, 23);
            this.zPrimeButton.TabIndex = 17;
            this.zPrimeButton.Text = "Middle Z - Counterclockwise (Z\')";
            this.toolTip.SetToolTip(this.zPrimeButton, "[Shift + Z]");
            this.zPrimeButton.UseVisualStyleBackColor = true;
            this.zPrimeButton.Click += new System.EventHandler(this.zPrimeButton_Click);
            // 
            // zButton
            // 
            this.zButton.Location = new System.Drawing.Point(11, 250);
            this.zButton.Name = "zButton";
            this.zButton.Size = new System.Drawing.Size(160, 23);
            this.zButton.TabIndex = 16;
            this.zButton.Text = "Middle Z - Clockwise (Z)";
            this.toolTip.SetToolTip(this.zButton, "[Z]");
            this.zButton.UseVisualStyleBackColor = true;
            this.zButton.Click += new System.EventHandler(this.zButton_Click);
            // 
            // xPrimeButton
            // 
            this.xPrimeButton.Location = new System.Drawing.Point(177, 163);
            this.xPrimeButton.Name = "xPrimeButton";
            this.xPrimeButton.Size = new System.Drawing.Size(171, 23);
            this.xPrimeButton.TabIndex = 11;
            this.xPrimeButton.Text = "Middle  - Counterclockwise (X\')";
            this.toolTip.SetToolTip(this.xPrimeButton, "[Shift + Y]");
            this.xPrimeButton.UseVisualStyleBackColor = true;
            this.xPrimeButton.Click += new System.EventHandler(this.xPrimeButton_Click);
            // 
            // xButton
            // 
            this.xButton.Location = new System.Drawing.Point(11, 163);
            this.xButton.Name = "xButton";
            this.xButton.Size = new System.Drawing.Size(160, 23);
            this.xButton.TabIndex = 10;
            this.xButton.Text = "Middle X - Clockwise (X)";
            this.toolTip.SetToolTip(this.xButton, "[X]");
            this.xButton.UseVisualStyleBackColor = true;
            this.xButton.Click += new System.EventHandler(this.xButton_Click);
            // 
            // yPrimeButton
            // 
            this.yPrimeButton.Location = new System.Drawing.Point(177, 76);
            this.yPrimeButton.Name = "yPrimeButton";
            this.yPrimeButton.Size = new System.Drawing.Size(171, 23);
            this.yPrimeButton.TabIndex = 5;
            this.yPrimeButton.Text = "Middle Y - Counterclockwise (Y\')";
            this.toolTip.SetToolTip(this.yPrimeButton, "[Shift + Y]");
            this.yPrimeButton.UseVisualStyleBackColor = true;
            this.yPrimeButton.Click += new System.EventHandler(this.yPrimeButton_Click);
            // 
            // yButton
            // 
            this.yButton.Location = new System.Drawing.Point(11, 76);
            this.yButton.Name = "yButton";
            this.yButton.Size = new System.Drawing.Size(160, 23);
            this.yButton.TabIndex = 4;
            this.yButton.Text = "Middle Y - Clockwise (Y)";
            this.toolTip.SetToolTip(this.yButton, "[Y]");
            this.yButton.UseVisualStyleBackColor = true;
            this.yButton.Click += new System.EventHandler(this.yButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 311);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Move queue";
            // 
            // setQueueToSolutionButton
            // 
            this.setQueueToSolutionButton.Location = new System.Drawing.Point(218, 448);
            this.setQueueToSolutionButton.Name = "setQueueToSolutionButton";
            this.setQueueToSolutionButton.Size = new System.Drawing.Size(130, 23);
            this.setQueueToSolutionButton.TabIndex = 23;
            this.setQueueToSolutionButton.Text = "Set queue to solution";
            this.setQueueToSolutionButton.UseVisualStyleBackColor = true;
            this.setQueueToSolutionButton.Click += new System.EventHandler(this.setQueueToSolutionButton_Click);
            // 
            // clearQueueButton
            // 
            this.clearQueueButton.Location = new System.Drawing.Point(112, 448);
            this.clearQueueButton.Name = "clearQueueButton";
            this.clearQueueButton.Size = new System.Drawing.Size(100, 23);
            this.clearQueueButton.TabIndex = 22;
            this.clearQueueButton.Text = "Clear queue";
            this.clearQueueButton.UseVisualStyleBackColor = true;
            this.clearQueueButton.Click += new System.EventHandler(this.clearQueueButton_Click);
            // 
            // executeQueueButton
            // 
            this.executeQueueButton.Location = new System.Drawing.Point(6, 448);
            this.executeQueueButton.Name = "executeQueueButton";
            this.executeQueueButton.Size = new System.Drawing.Size(100, 23);
            this.executeQueueButton.TabIndex = 21;
            this.executeQueueButton.Text = "Execute queue";
            this.executeQueueButton.UseVisualStyleBackColor = true;
            this.executeQueueButton.Click += new System.EventHandler(this.executeQueueButton_Click);
            // 
            // bPrimeButton
            // 
            this.bPrimeButton.Location = new System.Drawing.Point(177, 279);
            this.bPrimeButton.Name = "bPrimeButton";
            this.bPrimeButton.Size = new System.Drawing.Size(171, 23);
            this.bPrimeButton.TabIndex = 19;
            this.bPrimeButton.Text = "Back - Counterclockwise (B\')";
            this.toolTip.SetToolTip(this.bPrimeButton, "[Shift + B]");
            this.bPrimeButton.UseVisualStyleBackColor = true;
            this.bPrimeButton.Click += new System.EventHandler(this.bPrimeButton_Click);
            // 
            // bButton
            // 
            this.bButton.Location = new System.Drawing.Point(11, 279);
            this.bButton.Name = "bButton";
            this.bButton.Size = new System.Drawing.Size(160, 23);
            this.bButton.TabIndex = 18;
            this.bButton.Text = "Back - Clockwise (B)";
            this.toolTip.SetToolTip(this.bButton, "[B]");
            this.bButton.UseVisualStyleBackColor = true;
            this.bButton.Click += new System.EventHandler(this.bButton_Click);
            // 
            // fPrimeButton
            // 
            this.fPrimeButton.Location = new System.Drawing.Point(177, 221);
            this.fPrimeButton.Name = "fPrimeButton";
            this.fPrimeButton.Size = new System.Drawing.Size(171, 23);
            this.fPrimeButton.TabIndex = 15;
            this.fPrimeButton.Text = "Front - Counterclockwise (F\')";
            this.toolTip.SetToolTip(this.fPrimeButton, "[Shift + F]");
            this.fPrimeButton.UseVisualStyleBackColor = true;
            this.fPrimeButton.Click += new System.EventHandler(this.fPrimeButton_Click);
            // 
            // fButton
            // 
            this.fButton.Location = new System.Drawing.Point(11, 221);
            this.fButton.Name = "fButton";
            this.fButton.Size = new System.Drawing.Size(160, 23);
            this.fButton.TabIndex = 14;
            this.fButton.Text = "Front - Clockwise (F)";
            this.toolTip.SetToolTip(this.fButton, "[F]");
            this.fButton.UseVisualStyleBackColor = true;
            this.fButton.Click += new System.EventHandler(this.fButton_Click);
            // 
            // lPrimeButton
            // 
            this.lPrimeButton.Location = new System.Drawing.Point(177, 134);
            this.lPrimeButton.Name = "lPrimeButton";
            this.lPrimeButton.Size = new System.Drawing.Size(171, 23);
            this.lPrimeButton.TabIndex = 9;
            this.lPrimeButton.Text = "Left  - Counterclockwise (L\')";
            this.toolTip.SetToolTip(this.lPrimeButton, "[Shift + L]");
            this.lPrimeButton.UseVisualStyleBackColor = true;
            this.lPrimeButton.Click += new System.EventHandler(this.lPrimeButton_Click);
            // 
            // lButton
            // 
            this.lButton.Location = new System.Drawing.Point(11, 134);
            this.lButton.Name = "lButton";
            this.lButton.Size = new System.Drawing.Size(160, 23);
            this.lButton.TabIndex = 8;
            this.lButton.Text = "Left - Clockwise (L)";
            this.toolTip.SetToolTip(this.lButton, "[L]");
            this.lButton.UseVisualStyleBackColor = true;
            this.lButton.Click += new System.EventHandler(this.lButton_Click);
            // 
            // rPrimeButton
            // 
            this.rPrimeButton.Location = new System.Drawing.Point(177, 192);
            this.rPrimeButton.Name = "rPrimeButton";
            this.rPrimeButton.Size = new System.Drawing.Size(171, 23);
            this.rPrimeButton.TabIndex = 13;
            this.rPrimeButton.Text = "Right - Counterclockwise (R\')";
            this.toolTip.SetToolTip(this.rPrimeButton, "[Shift + R]");
            this.rPrimeButton.UseVisualStyleBackColor = true;
            this.rPrimeButton.Click += new System.EventHandler(this.rPrimeButton_Click);
            // 
            // rButton
            // 
            this.rButton.Location = new System.Drawing.Point(11, 192);
            this.rButton.Name = "rButton";
            this.rButton.Size = new System.Drawing.Size(160, 23);
            this.rButton.TabIndex = 12;
            this.rButton.Text = "Right - Clockwise (R)";
            this.toolTip.SetToolTip(this.rButton, "[R]");
            this.rButton.UseVisualStyleBackColor = true;
            this.rButton.Click += new System.EventHandler(this.rButton_Click);
            // 
            // dPrimeButton
            // 
            this.dPrimeButton.Location = new System.Drawing.Point(177, 105);
            this.dPrimeButton.Name = "dPrimeButton";
            this.dPrimeButton.Size = new System.Drawing.Size(171, 23);
            this.dPrimeButton.TabIndex = 7;
            this.dPrimeButton.Text = "Down - Counterclockwise (D\')";
            this.toolTip.SetToolTip(this.dPrimeButton, "[Shift + D]");
            this.dPrimeButton.UseVisualStyleBackColor = true;
            this.dPrimeButton.Click += new System.EventHandler(this.dPrimeButton_Click);
            // 
            // dButton
            // 
            this.dButton.Location = new System.Drawing.Point(11, 105);
            this.dButton.Name = "dButton";
            this.dButton.Size = new System.Drawing.Size(160, 23);
            this.dButton.TabIndex = 6;
            this.dButton.Text = "Down - Clockwise (D)";
            this.toolTip.SetToolTip(this.dButton, "[D]");
            this.dButton.UseVisualStyleBackColor = true;
            this.dButton.Click += new System.EventHandler(this.dButton_Click);
            // 
            // uPrimeButton
            // 
            this.uPrimeButton.Location = new System.Drawing.Point(177, 47);
            this.uPrimeButton.Name = "uPrimeButton";
            this.uPrimeButton.Size = new System.Drawing.Size(171, 23);
            this.uPrimeButton.TabIndex = 3;
            this.uPrimeButton.Text = "Up - Counterclockwise (U\')";
            this.toolTip.SetToolTip(this.uPrimeButton, "[Shift + U]");
            this.uPrimeButton.UseVisualStyleBackColor = true;
            this.uPrimeButton.Click += new System.EventHandler(this.uPrimeButton_Click);
            // 
            // uButton
            // 
            this.uButton.Location = new System.Drawing.Point(11, 47);
            this.uButton.Name = "uButton";
            this.uButton.Size = new System.Drawing.Size(160, 23);
            this.uButton.TabIndex = 2;
            this.uButton.Text = "Up - Clockwise (U)";
            this.toolTip.SetToolTip(this.uButton, "[U]");
            this.uButton.UseVisualStyleBackColor = true;
            this.uButton.Click += new System.EventHandler(this.uButton_Click);
            // 
            // addToQueueRadioButton
            // 
            this.addToQueueRadioButton.AutoSize = true;
            this.addToQueueRadioButton.Location = new System.Drawing.Point(177, 19);
            this.addToQueueRadioButton.Name = "addToQueueRadioButton";
            this.addToQueueRadioButton.Size = new System.Drawing.Size(89, 17);
            this.addToQueueRadioButton.TabIndex = 1;
            this.addToQueueRadioButton.TabStop = true;
            this.addToQueueRadioButton.Text = "Add to queue";
            this.addToQueueRadioButton.UseVisualStyleBackColor = true;
            // 
            // rotateRadioButton
            // 
            this.rotateRadioButton.AutoSize = true;
            this.rotateRadioButton.Checked = true;
            this.rotateRadioButton.Location = new System.Drawing.Point(73, 19);
            this.rotateRadioButton.Name = "rotateRadioButton";
            this.rotateRadioButton.Size = new System.Drawing.Size(57, 17);
            this.rotateRadioButton.TabIndex = 0;
            this.rotateRadioButton.TabStop = true;
            this.rotateRadioButton.Text = "Rotate";
            this.rotateRadioButton.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.faceLabel);
            this.panel1.Controls.Add(this.zoomLabel);
            this.panel1.Controls.Add(this.rotationLabel);
            this.panel1.Controls.Add(this.statusLabel);
            this.panel1.Location = new System.Drawing.Point(12, 537);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(600, 30);
            this.panel1.TabIndex = 0;
            // 
            // faceLabel
            // 
            this.faceLabel.AutoSize = true;
            this.faceLabel.Location = new System.Drawing.Point(507, 6);
            this.faceLabel.Name = "faceLabel";
            this.faceLabel.Size = new System.Drawing.Size(34, 13);
            this.faceLabel.TabIndex = 0;
            this.faceLabel.Text = "Face:";
            // 
            // zoomLabel
            // 
            this.zoomLabel.AutoSize = true;
            this.zoomLabel.Location = new System.Drawing.Point(399, 6);
            this.zoomLabel.Name = "zoomLabel";
            this.zoomLabel.Size = new System.Drawing.Size(60, 13);
            this.zoomLabel.TabIndex = 0;
            this.zoomLabel.Text = "Zoom: 80%";
            // 
            // rotationLabel
            // 
            this.rotationLabel.AutoSize = true;
            this.rotationLabel.Location = new System.Drawing.Point(145, 6);
            this.rotationLabel.Name = "rotationLabel";
            this.rotationLabel.Size = new System.Drawing.Size(134, 13);
            this.rotationLabel.TabIndex = 0;
            this.rotationLabel.Text = "Rotation: x = 0; y = 0; z = 0";
            this.rotationLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(3, 6);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(74, 13);
            this.statusLabel.TabIndex = 0;
            this.statusLabel.Text = "Status: Ready";
            // 
            // showViewButton
            // 
            this.showViewButton.Location = new System.Drawing.Point(262, 17);
            this.showViewButton.Name = "showViewButton";
            this.showViewButton.Size = new System.Drawing.Size(75, 23);
            this.showViewButton.TabIndex = 1;
            this.showViewButton.Text = "Show";
            this.showViewButton.UseVisualStyleBackColor = true;
            this.showViewButton.Click += new System.EventHandler(this.showViewButton_Click);
            // 
            // viewGroupBox
            // 
            this.viewGroupBox.Controls.Add(this.label2);
            this.viewGroupBox.Controls.Add(this.showViewButton);
            this.viewGroupBox.Controls.Add(this.viewComboBox);
            this.viewGroupBox.Location = new System.Drawing.Point(618, 27);
            this.viewGroupBox.Name = "viewGroupBox";
            this.viewGroupBox.Size = new System.Drawing.Size(354, 52);
            this.viewGroupBox.TabIndex = 1;
            this.viewGroupBox.TabStop = false;
            this.viewGroupBox.Text = "View";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(45, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "View";
            // 
            // viewComboBox
            // 
            this.viewComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.viewComboBox.FormattingEnabled = true;
            this.viewComboBox.Items.AddRange(new object[] {
            "Defauft",
            "Top face",
            "Bottom face",
            "Left face",
            "Right face",
            "Front face",
            "Back face"});
            this.viewComboBox.Location = new System.Drawing.Point(81, 19);
            this.viewComboBox.Name = "viewComboBox";
            this.viewComboBox.Size = new System.Drawing.Size(175, 21);
            this.viewComboBox.TabIndex = 0;
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetToolStripMenuItem,
            this.scrambleToolStripMenuItem,
            this.solveToolStripMenuItem,
            this.toolStripMenuItem1,
            this.settingsToolStripMenuItem,
            this.toolStripMenuItem2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(88, 20);
            this.fileToolStripMenuItem.Text = "&Rubik\'s Cube";
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.resetToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.resetToolStripMenuItem.Text = "&Reset";
            this.resetToolStripMenuItem.Click += new System.EventHandler(this.resetToolStripMenuItem_Click);
            // 
            // scrambleToolStripMenuItem
            // 
            this.scrambleToolStripMenuItem.Name = "scrambleToolStripMenuItem";
            this.scrambleToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.scrambleToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.scrambleToolStripMenuItem.Text = "&Scramble";
            this.scrambleToolStripMenuItem.Click += new System.EventHandler(this.scrambleToolStripMenuItem_Click);
            // 
            // solveToolStripMenuItem
            // 
            this.solveToolStripMenuItem.Name = "solveToolStripMenuItem";
            this.solveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.solveToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.solveToolStripMenuItem.Text = "Sol&ve";
            this.solveToolStripMenuItem.Click += new System.EventHandler(this.solveToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(160, 6);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.settingsToolStripMenuItem.Text = "S&ettings...";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(160, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.aboutToolStripMenuItem.Text = "&About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(984, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 579);
            this.Controls.Add(this.viewGroupBox);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Virtual Rubik\'s Cube";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MainForm_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseClick);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseMove);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.viewGroupBox.ResumeLayout(false);
            this.viewGroupBox.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button setQueueToSolutionButton;
        private System.Windows.Forms.Button clearQueueButton;
        private System.Windows.Forms.Button executeQueueButton;
        private System.Windows.Forms.Button bPrimeButton;
        private System.Windows.Forms.Button bButton;
        private System.Windows.Forms.Button fPrimeButton;
        private System.Windows.Forms.Button fButton;
        private System.Windows.Forms.Button lPrimeButton;
        private System.Windows.Forms.Button lButton;
        private System.Windows.Forms.Button rPrimeButton;
        private System.Windows.Forms.Button rButton;
        private System.Windows.Forms.Button dPrimeButton;
        private System.Windows.Forms.Button dButton;
        private System.Windows.Forms.Button uPrimeButton;
        private System.Windows.Forms.Button uButton;
        private System.Windows.Forms.RadioButton addToQueueRadioButton;
        private System.Windows.Forms.RadioButton rotateRadioButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button zPrimeButton;
        private System.Windows.Forms.Button zButton;
        private System.Windows.Forms.Button xPrimeButton;
        private System.Windows.Forms.Button xButton;
        private System.Windows.Forms.Button yPrimeButton;
        private System.Windows.Forms.Button yButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label faceLabel;
        private System.Windows.Forms.Label zoomLabel;
        private System.Windows.Forms.Label rotationLabel;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Button showViewButton;
        private System.Windows.Forms.GroupBox viewGroupBox;
        private System.Windows.Forms.ComboBox viewComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox moveQueueListBox;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scrambleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem solveToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip;
    }
}

