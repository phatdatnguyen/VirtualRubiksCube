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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            groupBox1 = new GroupBox();
            moveQueueListBox = new ListBox();
            zPrimeButton = new Button();
            zButton = new Button();
            xPrimeButton = new Button();
            xButton = new Button();
            yPrimeButton = new Button();
            yButton = new Button();
            label1 = new Label();
            setQueueToSolutionButton = new Button();
            clearQueueButton = new Button();
            executeQueueButton = new Button();
            bPrimeButton = new Button();
            bButton = new Button();
            fPrimeButton = new Button();
            fButton = new Button();
            lPrimeButton = new Button();
            lButton = new Button();
            rPrimeButton = new Button();
            rButton = new Button();
            dPrimeButton = new Button();
            dButton = new Button();
            uPrimeButton = new Button();
            uButton = new Button();
            addToQueueRadioButton = new RadioButton();
            rotateRadioButton = new RadioButton();
            statusPanel = new Panel();
            faceLabel = new Label();
            zoomLabel = new Label();
            rotationLabel = new Label();
            statusLabel = new Label();
            showViewButton = new Button();
            viewGroupBox = new GroupBox();
            label2 = new Label();
            viewComboBox = new ComboBox();
            toolTip = new ToolTip(components);
            fileToolStripMenuItem = new ToolStripMenuItem();
            resetToolStripMenuItem = new ToolStripMenuItem();
            scrambleToolStripMenuItem = new ToolStripMenuItem();
            solveToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripSeparator();
            settingsToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem2 = new ToolStripSeparator();
            exitToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            menuStrip = new MenuStrip();
            renderPanel = new Panel();
            groupBox1.SuspendLayout();
            statusPanel.SuspendLayout();
            viewGroupBox.SuspendLayout();
            menuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(moveQueueListBox);
            groupBox1.Controls.Add(zPrimeButton);
            groupBox1.Controls.Add(zButton);
            groupBox1.Controls.Add(xPrimeButton);
            groupBox1.Controls.Add(xButton);
            groupBox1.Controls.Add(yPrimeButton);
            groupBox1.Controls.Add(yButton);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(setQueueToSolutionButton);
            groupBox1.Controls.Add(clearQueueButton);
            groupBox1.Controls.Add(executeQueueButton);
            groupBox1.Controls.Add(bPrimeButton);
            groupBox1.Controls.Add(bButton);
            groupBox1.Controls.Add(fPrimeButton);
            groupBox1.Controls.Add(fButton);
            groupBox1.Controls.Add(lPrimeButton);
            groupBox1.Controls.Add(lButton);
            groupBox1.Controls.Add(rPrimeButton);
            groupBox1.Controls.Add(rButton);
            groupBox1.Controls.Add(dPrimeButton);
            groupBox1.Controls.Add(dButton);
            groupBox1.Controls.Add(uPrimeButton);
            groupBox1.Controls.Add(uButton);
            groupBox1.Controls.Add(addToQueueRadioButton);
            groupBox1.Controls.Add(rotateRadioButton);
            groupBox1.Location = new Point(721, 98);
            groupBox1.Margin = new Padding(4, 3, 4, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(4, 3, 4, 3);
            groupBox1.Size = new Size(413, 556);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "Moves";
            // 
            // moveQueueListBox
            // 
            moveQueueListBox.FormattingEnabled = true;
            moveQueueListBox.ItemHeight = 15;
            moveQueueListBox.Location = new Point(94, 359);
            moveQueueListBox.Margin = new Padding(4, 3, 4, 3);
            moveQueueListBox.Name = "moveQueueListBox";
            moveQueueListBox.Size = new Size(311, 154);
            moveQueueListBox.TabIndex = 20;
            // 
            // zPrimeButton
            // 
            zPrimeButton.Location = new Point(206, 288);
            zPrimeButton.Margin = new Padding(4, 3, 4, 3);
            zPrimeButton.Name = "zPrimeButton";
            zPrimeButton.Size = new Size(200, 27);
            zPrimeButton.TabIndex = 17;
            zPrimeButton.Text = "Middle Z - Counterclockwise (Z')";
            toolTip.SetToolTip(zPrimeButton, "[Shift + Z]");
            zPrimeButton.UseVisualStyleBackColor = true;
            zPrimeButton.Click += zPrimeButton_Click;
            // 
            // zButton
            // 
            zButton.Location = new Point(13, 288);
            zButton.Margin = new Padding(4, 3, 4, 3);
            zButton.Name = "zButton";
            zButton.Size = new Size(187, 27);
            zButton.TabIndex = 16;
            zButton.Text = "Middle Z - Clockwise (Z)";
            toolTip.SetToolTip(zButton, "[Z]");
            zButton.UseVisualStyleBackColor = true;
            zButton.Click += zButton_Click;
            // 
            // xPrimeButton
            // 
            xPrimeButton.Location = new Point(206, 188);
            xPrimeButton.Margin = new Padding(4, 3, 4, 3);
            xPrimeButton.Name = "xPrimeButton";
            xPrimeButton.Size = new Size(200, 27);
            xPrimeButton.TabIndex = 11;
            xPrimeButton.Text = "Middle  - Counterclockwise (X')";
            toolTip.SetToolTip(xPrimeButton, "[Shift + Y]");
            xPrimeButton.UseVisualStyleBackColor = true;
            xPrimeButton.Click += xPrimeButton_Click;
            // 
            // xButton
            // 
            xButton.Location = new Point(13, 188);
            xButton.Margin = new Padding(4, 3, 4, 3);
            xButton.Name = "xButton";
            xButton.Size = new Size(187, 27);
            xButton.TabIndex = 10;
            xButton.Text = "Middle X - Clockwise (X)";
            toolTip.SetToolTip(xButton, "[X]");
            xButton.UseVisualStyleBackColor = true;
            xButton.Click += xButton_Click;
            // 
            // yPrimeButton
            // 
            yPrimeButton.Location = new Point(206, 88);
            yPrimeButton.Margin = new Padding(4, 3, 4, 3);
            yPrimeButton.Name = "yPrimeButton";
            yPrimeButton.Size = new Size(200, 27);
            yPrimeButton.TabIndex = 5;
            yPrimeButton.Text = "Middle Y - Counterclockwise (Y')";
            toolTip.SetToolTip(yPrimeButton, "[Shift + Y]");
            yPrimeButton.UseVisualStyleBackColor = true;
            yPrimeButton.Click += yPrimeButton_Click;
            // 
            // yButton
            // 
            yButton.Location = new Point(13, 88);
            yButton.Margin = new Padding(4, 3, 4, 3);
            yButton.Name = "yButton";
            yButton.Size = new Size(187, 27);
            yButton.TabIndex = 4;
            yButton.Text = "Middle Y - Clockwise (Y)";
            toolTip.SetToolTip(yButton, "[Y]");
            yButton.UseVisualStyleBackColor = true;
            yButton.Click += yButton_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(9, 359);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(73, 15);
            label1.TabIndex = 0;
            label1.Text = "Move queue";
            // 
            // setQueueToSolutionButton
            // 
            setQueueToSolutionButton.Location = new Point(254, 517);
            setQueueToSolutionButton.Margin = new Padding(4, 3, 4, 3);
            setQueueToSolutionButton.Name = "setQueueToSolutionButton";
            setQueueToSolutionButton.Size = new Size(152, 27);
            setQueueToSolutionButton.TabIndex = 23;
            setQueueToSolutionButton.Text = "Set queue to solution";
            setQueueToSolutionButton.UseVisualStyleBackColor = true;
            setQueueToSolutionButton.Click += setQueueToSolutionButton_Click;
            // 
            // clearQueueButton
            // 
            clearQueueButton.Location = new Point(131, 517);
            clearQueueButton.Margin = new Padding(4, 3, 4, 3);
            clearQueueButton.Name = "clearQueueButton";
            clearQueueButton.Size = new Size(117, 27);
            clearQueueButton.TabIndex = 22;
            clearQueueButton.Text = "Clear queue";
            clearQueueButton.UseVisualStyleBackColor = true;
            clearQueueButton.Click += clearQueueButton_Click;
            // 
            // executeQueueButton
            // 
            executeQueueButton.Location = new Point(7, 517);
            executeQueueButton.Margin = new Padding(4, 3, 4, 3);
            executeQueueButton.Name = "executeQueueButton";
            executeQueueButton.Size = new Size(117, 27);
            executeQueueButton.TabIndex = 21;
            executeQueueButton.Text = "Execute queue";
            executeQueueButton.UseVisualStyleBackColor = true;
            executeQueueButton.Click += executeQueueButton_Click;
            // 
            // bPrimeButton
            // 
            bPrimeButton.Location = new Point(206, 322);
            bPrimeButton.Margin = new Padding(4, 3, 4, 3);
            bPrimeButton.Name = "bPrimeButton";
            bPrimeButton.Size = new Size(200, 27);
            bPrimeButton.TabIndex = 19;
            bPrimeButton.Text = "Back - Counterclockwise (B')";
            toolTip.SetToolTip(bPrimeButton, "[Shift + B]");
            bPrimeButton.UseVisualStyleBackColor = true;
            bPrimeButton.Click += bPrimeButton_Click;
            // 
            // bButton
            // 
            bButton.Location = new Point(13, 322);
            bButton.Margin = new Padding(4, 3, 4, 3);
            bButton.Name = "bButton";
            bButton.Size = new Size(187, 27);
            bButton.TabIndex = 18;
            bButton.Text = "Back - Clockwise (B)";
            toolTip.SetToolTip(bButton, "[B]");
            bButton.UseVisualStyleBackColor = true;
            bButton.Click += bButton_Click;
            // 
            // fPrimeButton
            // 
            fPrimeButton.Location = new Point(206, 255);
            fPrimeButton.Margin = new Padding(4, 3, 4, 3);
            fPrimeButton.Name = "fPrimeButton";
            fPrimeButton.Size = new Size(200, 27);
            fPrimeButton.TabIndex = 15;
            fPrimeButton.Text = "Front - Counterclockwise (F')";
            toolTip.SetToolTip(fPrimeButton, "[Shift + F]");
            fPrimeButton.UseVisualStyleBackColor = true;
            fPrimeButton.Click += fPrimeButton_Click;
            // 
            // fButton
            // 
            fButton.Location = new Point(13, 255);
            fButton.Margin = new Padding(4, 3, 4, 3);
            fButton.Name = "fButton";
            fButton.Size = new Size(187, 27);
            fButton.TabIndex = 14;
            fButton.Text = "Front - Clockwise (F)";
            toolTip.SetToolTip(fButton, "[F]");
            fButton.UseVisualStyleBackColor = true;
            fButton.Click += fButton_Click;
            // 
            // lPrimeButton
            // 
            lPrimeButton.Location = new Point(206, 155);
            lPrimeButton.Margin = new Padding(4, 3, 4, 3);
            lPrimeButton.Name = "lPrimeButton";
            lPrimeButton.Size = new Size(200, 27);
            lPrimeButton.TabIndex = 9;
            lPrimeButton.Text = "Left  - Counterclockwise (L')";
            toolTip.SetToolTip(lPrimeButton, "[Shift + L]");
            lPrimeButton.UseVisualStyleBackColor = true;
            lPrimeButton.Click += lPrimeButton_Click;
            // 
            // lButton
            // 
            lButton.Location = new Point(13, 155);
            lButton.Margin = new Padding(4, 3, 4, 3);
            lButton.Name = "lButton";
            lButton.Size = new Size(187, 27);
            lButton.TabIndex = 8;
            lButton.Text = "Left - Clockwise (L)";
            toolTip.SetToolTip(lButton, "[L]");
            lButton.UseVisualStyleBackColor = true;
            lButton.Click += lButton_Click;
            // 
            // rPrimeButton
            // 
            rPrimeButton.Location = new Point(206, 222);
            rPrimeButton.Margin = new Padding(4, 3, 4, 3);
            rPrimeButton.Name = "rPrimeButton";
            rPrimeButton.Size = new Size(200, 27);
            rPrimeButton.TabIndex = 13;
            rPrimeButton.Text = "Right - Counterclockwise (R')";
            toolTip.SetToolTip(rPrimeButton, "[Shift + R]");
            rPrimeButton.UseVisualStyleBackColor = true;
            rPrimeButton.Click += rPrimeButton_Click;
            // 
            // rButton
            // 
            rButton.Location = new Point(13, 222);
            rButton.Margin = new Padding(4, 3, 4, 3);
            rButton.Name = "rButton";
            rButton.Size = new Size(187, 27);
            rButton.TabIndex = 12;
            rButton.Text = "Right - Clockwise (R)";
            toolTip.SetToolTip(rButton, "[R]");
            rButton.UseVisualStyleBackColor = true;
            rButton.Click += rButton_Click;
            // 
            // dPrimeButton
            // 
            dPrimeButton.Location = new Point(206, 121);
            dPrimeButton.Margin = new Padding(4, 3, 4, 3);
            dPrimeButton.Name = "dPrimeButton";
            dPrimeButton.Size = new Size(200, 27);
            dPrimeButton.TabIndex = 7;
            dPrimeButton.Text = "Down - Counterclockwise (D')";
            toolTip.SetToolTip(dPrimeButton, "[Shift + D]");
            dPrimeButton.UseVisualStyleBackColor = true;
            dPrimeButton.Click += dPrimeButton_Click;
            // 
            // dButton
            // 
            dButton.Location = new Point(13, 121);
            dButton.Margin = new Padding(4, 3, 4, 3);
            dButton.Name = "dButton";
            dButton.Size = new Size(187, 27);
            dButton.TabIndex = 6;
            dButton.Text = "Down - Clockwise (D)";
            toolTip.SetToolTip(dButton, "[D]");
            dButton.UseVisualStyleBackColor = true;
            dButton.Click += dButton_Click;
            // 
            // uPrimeButton
            // 
            uPrimeButton.Location = new Point(206, 54);
            uPrimeButton.Margin = new Padding(4, 3, 4, 3);
            uPrimeButton.Name = "uPrimeButton";
            uPrimeButton.Size = new Size(200, 27);
            uPrimeButton.TabIndex = 3;
            uPrimeButton.Text = "Up - Counterclockwise (U')";
            toolTip.SetToolTip(uPrimeButton, "[Shift + U]");
            uPrimeButton.UseVisualStyleBackColor = true;
            uPrimeButton.Click += uPrimeButton_Click;
            // 
            // uButton
            // 
            uButton.Location = new Point(13, 54);
            uButton.Margin = new Padding(4, 3, 4, 3);
            uButton.Name = "uButton";
            uButton.Size = new Size(187, 27);
            uButton.TabIndex = 2;
            uButton.Text = "Up - Clockwise (U)";
            toolTip.SetToolTip(uButton, "[U]");
            uButton.UseVisualStyleBackColor = true;
            uButton.Click += uButton_Click;
            // 
            // addToQueueRadioButton
            // 
            addToQueueRadioButton.AutoSize = true;
            addToQueueRadioButton.Location = new Point(206, 22);
            addToQueueRadioButton.Margin = new Padding(4, 3, 4, 3);
            addToQueueRadioButton.Name = "addToQueueRadioButton";
            addToQueueRadioButton.Size = new Size(97, 19);
            addToQueueRadioButton.TabIndex = 1;
            addToQueueRadioButton.TabStop = true;
            addToQueueRadioButton.Text = "Add to queue";
            addToQueueRadioButton.UseVisualStyleBackColor = true;
            // 
            // rotateRadioButton
            // 
            rotateRadioButton.AutoSize = true;
            rotateRadioButton.Checked = true;
            rotateRadioButton.Location = new Point(85, 22);
            rotateRadioButton.Margin = new Padding(4, 3, 4, 3);
            rotateRadioButton.Name = "rotateRadioButton";
            rotateRadioButton.Size = new Size(59, 19);
            rotateRadioButton.TabIndex = 0;
            rotateRadioButton.TabStop = true;
            rotateRadioButton.Text = "Rotate";
            rotateRadioButton.UseVisualStyleBackColor = true;
            // 
            // statusPanel
            // 
            statusPanel.Controls.Add(faceLabel);
            statusPanel.Controls.Add(zoomLabel);
            statusPanel.Controls.Add(rotationLabel);
            statusPanel.Controls.Add(statusLabel);
            statusPanel.Location = new Point(14, 620);
            statusPanel.Margin = new Padding(4, 3, 4, 3);
            statusPanel.Name = "statusPanel";
            statusPanel.Size = new Size(700, 35);
            statusPanel.TabIndex = 0;
            // 
            // faceLabel
            // 
            faceLabel.AutoSize = true;
            faceLabel.Location = new Point(592, 7);
            faceLabel.Margin = new Padding(4, 0, 4, 0);
            faceLabel.Name = "faceLabel";
            faceLabel.Size = new Size(34, 15);
            faceLabel.TabIndex = 0;
            faceLabel.Text = "Face:";
            // 
            // zoomLabel
            // 
            zoomLabel.AutoSize = true;
            zoomLabel.Location = new Point(465, 7);
            zoomLabel.Margin = new Padding(4, 0, 4, 0);
            zoomLabel.Name = "zoomLabel";
            zoomLabel.Size = new Size(67, 15);
            zoomLabel.TabIndex = 0;
            zoomLabel.Text = "Zoom: 80%";
            // 
            // rotationLabel
            // 
            rotationLabel.AutoSize = true;
            rotationLabel.Location = new Point(169, 7);
            rotationLabel.Margin = new Padding(4, 0, 4, 0);
            rotationLabel.Name = "rotationLabel";
            rotationLabel.Size = new Size(147, 15);
            rotationLabel.TabIndex = 0;
            rotationLabel.Text = "Rotation: x = 0; y = 0; z = 0";
            rotationLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // statusLabel
            // 
            statusLabel.AutoSize = true;
            statusLabel.Location = new Point(4, 7);
            statusLabel.Margin = new Padding(4, 0, 4, 0);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(77, 15);
            statusLabel.TabIndex = 0;
            statusLabel.Text = "Status: Ready";
            // 
            // showViewButton
            // 
            showViewButton.Location = new Point(306, 20);
            showViewButton.Margin = new Padding(4, 3, 4, 3);
            showViewButton.Name = "showViewButton";
            showViewButton.Size = new Size(88, 27);
            showViewButton.TabIndex = 1;
            showViewButton.Text = "Show";
            showViewButton.UseVisualStyleBackColor = true;
            showViewButton.Click += showViewButton_Click;
            // 
            // viewGroupBox
            // 
            viewGroupBox.Controls.Add(label2);
            viewGroupBox.Controls.Add(showViewButton);
            viewGroupBox.Controls.Add(viewComboBox);
            viewGroupBox.Location = new Point(721, 31);
            viewGroupBox.Margin = new Padding(4, 3, 4, 3);
            viewGroupBox.Name = "viewGroupBox";
            viewGroupBox.Padding = new Padding(4, 3, 4, 3);
            viewGroupBox.Size = new Size(413, 60);
            viewGroupBox.TabIndex = 1;
            viewGroupBox.TabStop = false;
            viewGroupBox.Text = "View";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(52, 25);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(32, 15);
            label2.TabIndex = 0;
            label2.Text = "View";
            // 
            // viewComboBox
            // 
            viewComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            viewComboBox.FormattingEnabled = true;
            viewComboBox.Items.AddRange(new object[] { "Defauft", "Top face", "Bottom face", "Left face", "Right face", "Front face", "Back face" });
            viewComboBox.Location = new Point(94, 22);
            viewComboBox.Margin = new Padding(4, 3, 4, 3);
            viewComboBox.Name = "viewComboBox";
            viewComboBox.Size = new Size(204, 23);
            viewComboBox.TabIndex = 0;
            viewComboBox.SelectedIndexChanged += viewComboBox_SelectedIndexChanged;
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { resetToolStripMenuItem, scrambleToolStripMenuItem, solveToolStripMenuItem, toolStripMenuItem1, settingsToolStripMenuItem, toolStripMenuItem2, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(88, 20);
            fileToolStripMenuItem.Text = "&Rubik's Cube";
            // 
            // resetToolStripMenuItem
            // 
            resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            resetToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.R;
            resetToolStripMenuItem.Size = new Size(163, 22);
            resetToolStripMenuItem.Text = "&Reset";
            resetToolStripMenuItem.Click += resetToolStripMenuItem_Click;
            // 
            // scrambleToolStripMenuItem
            // 
            scrambleToolStripMenuItem.Name = "scrambleToolStripMenuItem";
            scrambleToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.S;
            scrambleToolStripMenuItem.Size = new Size(163, 22);
            scrambleToolStripMenuItem.Text = "&Scramble";
            scrambleToolStripMenuItem.Click += scrambleToolStripMenuItem_Click;
            // 
            // solveToolStripMenuItem
            // 
            solveToolStripMenuItem.Name = "solveToolStripMenuItem";
            solveToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.V;
            solveToolStripMenuItem.Size = new Size(163, 22);
            solveToolStripMenuItem.Text = "Sol&ve";
            solveToolStripMenuItem.Click += solveToolStripMenuItem_Click;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(160, 6);
            // 
            // settingsToolStripMenuItem
            // 
            settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            settingsToolStripMenuItem.Size = new Size(163, 22);
            settingsToolStripMenuItem.Text = "S&ettings...";
            settingsToolStripMenuItem.Click += settingsToolStripMenuItem_Click;
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new Size(160, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.ShortcutKeys = Keys.Alt | Keys.F4;
            exitToolStripMenuItem.Size = new Size(163, 22);
            exitToolStripMenuItem.Text = "E&xit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aboutToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(44, 20);
            helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.H;
            aboutToolStripMenuItem.Size = new Size(150, 22);
            aboutToolStripMenuItem.Text = "&About";
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            // 
            // menuStrip
            // 
            menuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, helpToolStripMenuItem });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Padding = new Padding(7, 2, 0, 2);
            menuStrip.Size = new Size(1148, 24);
            menuStrip.TabIndex = 0;
            menuStrip.Text = "menuStrip1";
            // 
            // renderPanel
            // 
            renderPanel.Location = new Point(12, 31);
            renderPanel.Name = "renderPanel";
            renderPanel.Size = new Size(702, 583);
            renderPanel.TabIndex = 3;
            renderPanel.Paint += renderPanel_Paint;
            renderPanel.MouseClick += renderPanel_MouseClick;
            renderPanel.MouseMove += renderPanel_MouseMove;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1148, 668);
            Controls.Add(renderPanel);
            Controls.Add(viewGroupBox);
            Controls.Add(statusPanel);
            Controls.Add(groupBox1);
            Controls.Add(menuStrip);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            MainMenuStrip = menuStrip;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            Name = "MainForm";
            Text = "Virtual Rubik's Cube";
            FormClosed += MainForm_FormClosed;
            Load += MainForm_Load;
            KeyDown += MainForm_KeyDown;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            statusPanel.ResumeLayout(false);
            statusPanel.PerformLayout();
            viewGroupBox.ResumeLayout(false);
            viewGroupBox.PerformLayout();
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private GroupBox groupBox1;
        private Button setQueueToSolutionButton;
        private Button clearQueueButton;
        private Button executeQueueButton;
        private Button bPrimeButton;
        private Button bButton;
        private Button fPrimeButton;
        private Button fButton;
        private Button lPrimeButton;
        private Button lButton;
        private Button rPrimeButton;
        private Button rButton;
        private Button dPrimeButton;
        private Button dButton;
        private Button uPrimeButton;
        private Button uButton;
        private RadioButton addToQueueRadioButton;
        private RadioButton rotateRadioButton;
        private Label label1;
        private Button zPrimeButton;
        private Button zButton;
        private Button xPrimeButton;
        private Button xButton;
        private Button yPrimeButton;
        private Button yButton;
        private Panel statusPanel;
        private Label faceLabel;
        private Label zoomLabel;
        private Label rotationLabel;
        private Label statusLabel;
        private Button showViewButton;
        private GroupBox viewGroupBox;
        private ComboBox viewComboBox;
        private Label label2;
        private ListBox moveQueueListBox;
        private ToolTip toolTip;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem resetToolStripMenuItem;
        private ToolStripMenuItem scrambleToolStripMenuItem;
        private ToolStripMenuItem solveToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem2;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private MenuStrip menuStrip;
        private Panel renderPanel;
    }
}

