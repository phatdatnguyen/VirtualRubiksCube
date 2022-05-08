namespace VirtualRubiksCube
{
    partial class SettingDialog
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
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.topButton = new System.Windows.Forms.Button();
            this.colorsGroupBox = new System.Windows.Forms.GroupBox();
            this.backLabel = new System.Windows.Forms.Label();
            this.backButton = new System.Windows.Forms.Button();
            this.frontLabel = new System.Windows.Forms.Label();
            this.frontButton = new System.Windows.Forms.Button();
            this.rightLabel = new System.Windows.Forms.Label();
            this.rightButton = new System.Windows.Forms.Button();
            this.leftLabel = new System.Windows.Forms.Label();
            this.leftButton = new System.Windows.Forms.Button();
            this.bottomLabel = new System.Windows.Forms.Label();
            this.bottomButton = new System.Windows.Forms.Button();
            this.topLabel = new System.Windows.Forms.Label();
            this.rotationSpeedLabel = new System.Windows.Forms.Label();
            this.animationTimeTrackBar = new System.Windows.Forms.TrackBar();
            this.defaultButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.animationTimeLabel = new System.Windows.Forms.Label();
            this.colorsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.animationTimeTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // colorDialog
            // 
            this.colorDialog.AnyColor = true;
            this.colorDialog.Color = System.Drawing.Color.White;
            // 
            // topButton
            // 
            this.topButton.BackColor = System.Drawing.Color.White;
            this.topButton.Location = new System.Drawing.Point(91, 27);
            this.topButton.Name = "topButton";
            this.topButton.Size = new System.Drawing.Size(23, 23);
            this.topButton.TabIndex = 0;
            this.topButton.UseVisualStyleBackColor = false;
            this.topButton.Click += new System.EventHandler(this.topButton_Click);
            // 
            // colorsGroupBox
            // 
            this.colorsGroupBox.Controls.Add(this.backLabel);
            this.colorsGroupBox.Controls.Add(this.backButton);
            this.colorsGroupBox.Controls.Add(this.frontLabel);
            this.colorsGroupBox.Controls.Add(this.frontButton);
            this.colorsGroupBox.Controls.Add(this.rightLabel);
            this.colorsGroupBox.Controls.Add(this.rightButton);
            this.colorsGroupBox.Controls.Add(this.leftLabel);
            this.colorsGroupBox.Controls.Add(this.leftButton);
            this.colorsGroupBox.Controls.Add(this.bottomLabel);
            this.colorsGroupBox.Controls.Add(this.bottomButton);
            this.colorsGroupBox.Controls.Add(this.topLabel);
            this.colorsGroupBox.Controls.Add(this.topButton);
            this.colorsGroupBox.Location = new System.Drawing.Point(12, 12);
            this.colorsGroupBox.Name = "colorsGroupBox";
            this.colorsGroupBox.Size = new System.Drawing.Size(371, 115);
            this.colorsGroupBox.TabIndex = 0;
            this.colorsGroupBox.TabStop = false;
            this.colorsGroupBox.Text = "Colors";
            // 
            // backLabel
            // 
            this.backLabel.AutoSize = true;
            this.backLabel.Location = new System.Drawing.Point(253, 73);
            this.backLabel.Name = "backLabel";
            this.backLabel.Size = new System.Drawing.Size(56, 13);
            this.backLabel.TabIndex = 0;
            this.backLabel.Text = "Back face";
            // 
            // backButton
            // 
            this.backButton.BackColor = System.Drawing.Color.Blue;
            this.backButton.Location = new System.Drawing.Point(315, 68);
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(23, 23);
            this.backButton.TabIndex = 5;
            this.backButton.UseVisualStyleBackColor = false;
            this.backButton.Click += new System.EventHandler(this.backButton_Click);
            // 
            // frontLabel
            // 
            this.frontLabel.AutoSize = true;
            this.frontLabel.Location = new System.Drawing.Point(254, 32);
            this.frontLabel.Name = "frontLabel";
            this.frontLabel.Size = new System.Drawing.Size(55, 13);
            this.frontLabel.TabIndex = 0;
            this.frontLabel.Text = "Front face";
            // 
            // frontButton
            // 
            this.frontButton.BackColor = System.Drawing.Color.Green;
            this.frontButton.Location = new System.Drawing.Point(315, 27);
            this.frontButton.Name = "frontButton";
            this.frontButton.Size = new System.Drawing.Size(23, 23);
            this.frontButton.TabIndex = 4;
            this.frontButton.UseVisualStyleBackColor = false;
            this.frontButton.Click += new System.EventHandler(this.frontButton_Click);
            // 
            // rightLabel
            // 
            this.rightLabel.AutoSize = true;
            this.rightLabel.Location = new System.Drawing.Point(141, 73);
            this.rightLabel.Name = "rightLabel";
            this.rightLabel.Size = new System.Drawing.Size(56, 13);
            this.rightLabel.TabIndex = 0;
            this.rightLabel.Text = "Right face";
            // 
            // rightButton
            // 
            this.rightButton.BackColor = System.Drawing.Color.Red;
            this.rightButton.Location = new System.Drawing.Point(203, 68);
            this.rightButton.Name = "rightButton";
            this.rightButton.Size = new System.Drawing.Size(23, 23);
            this.rightButton.TabIndex = 3;
            this.rightButton.UseVisualStyleBackColor = false;
            this.rightButton.Click += new System.EventHandler(this.rightButton_Click);
            // 
            // leftLabel
            // 
            this.leftLabel.AutoSize = true;
            this.leftLabel.Location = new System.Drawing.Point(148, 32);
            this.leftLabel.Name = "leftLabel";
            this.leftLabel.Size = new System.Drawing.Size(49, 13);
            this.leftLabel.TabIndex = 0;
            this.leftLabel.Text = "Left face";
            // 
            // leftButton
            // 
            this.leftButton.BackColor = System.Drawing.Color.Orange;
            this.leftButton.Location = new System.Drawing.Point(203, 27);
            this.leftButton.Name = "leftButton";
            this.leftButton.Size = new System.Drawing.Size(23, 23);
            this.leftButton.TabIndex = 2;
            this.leftButton.UseVisualStyleBackColor = false;
            this.leftButton.Click += new System.EventHandler(this.leftButton_Click);
            // 
            // bottomLabel
            // 
            this.bottomLabel.AutoSize = true;
            this.bottomLabel.Location = new System.Drawing.Point(21, 73);
            this.bottomLabel.Name = "bottomLabel";
            this.bottomLabel.Size = new System.Drawing.Size(64, 13);
            this.bottomLabel.TabIndex = 0;
            this.bottomLabel.Text = "Bottom face";
            // 
            // bottomButton
            // 
            this.bottomButton.BackColor = System.Drawing.Color.Yellow;
            this.bottomButton.Location = new System.Drawing.Point(91, 68);
            this.bottomButton.Name = "bottomButton";
            this.bottomButton.Size = new System.Drawing.Size(23, 23);
            this.bottomButton.TabIndex = 1;
            this.bottomButton.UseVisualStyleBackColor = false;
            this.bottomButton.Click += new System.EventHandler(this.bottomButton_Click);
            // 
            // topLabel
            // 
            this.topLabel.AutoSize = true;
            this.topLabel.Location = new System.Drawing.Point(35, 32);
            this.topLabel.Name = "topLabel";
            this.topLabel.Size = new System.Drawing.Size(50, 13);
            this.topLabel.TabIndex = 0;
            this.topLabel.Text = "Top face";
            // 
            // rotationSpeedLabel
            // 
            this.rotationSpeedLabel.AutoSize = true;
            this.rotationSpeedLabel.Location = new System.Drawing.Point(18, 135);
            this.rotationSpeedLabel.Name = "rotationSpeedLabel";
            this.rotationSpeedLabel.Size = new System.Drawing.Size(79, 13);
            this.rotationSpeedLabel.TabIndex = 0;
            this.rotationSpeedLabel.Text = "Rotation speed";
            // 
            // animationTimeTrackBar
            // 
            this.animationTimeTrackBar.Location = new System.Drawing.Point(103, 133);
            this.animationTimeTrackBar.Maximum = 100;
            this.animationTimeTrackBar.Minimum = 10;
            this.animationTimeTrackBar.Name = "animationTimeTrackBar";
            this.animationTimeTrackBar.Size = new System.Drawing.Size(235, 45);
            this.animationTimeTrackBar.TabIndex = 1;
            this.animationTimeTrackBar.TickFrequency = 10;
            this.animationTimeTrackBar.Value = 20;
            this.animationTimeTrackBar.Scroll += new System.EventHandler(this.animationTimeTrackBar_Scroll);
            // 
            // defaultButton
            // 
            this.defaultButton.Location = new System.Drawing.Point(155, 184);
            this.defaultButton.Name = "defaultButton";
            this.defaultButton.Size = new System.Drawing.Size(75, 23);
            this.defaultButton.TabIndex = 2;
            this.defaultButton.Text = "Default";
            this.defaultButton.UseVisualStyleBackColor = true;
            this.defaultButton.Click += new System.EventHandler(this.defaultButton_Click);
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(236, 184);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(317, 184);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // animationTimeLabel
            // 
            this.animationTimeLabel.AutoSize = true;
            this.animationTimeLabel.Location = new System.Drawing.Point(353, 135);
            this.animationTimeLabel.Name = "animationTimeLabel";
            this.animationTimeLabel.Size = new System.Drawing.Size(30, 13);
            this.animationTimeLabel.TabIndex = 0;
            this.animationTimeLabel.Text = "0.2 s";
            // 
            // SettingDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(404, 220);
            this.Controls.Add(this.animationTimeLabel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.defaultButton);
            this.Controls.Add(this.animationTimeTrackBar);
            this.Controls.Add(this.rotationSpeedLabel);
            this.Controls.Add(this.colorsGroupBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Setting";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SettingDialog_FormClosed);
            this.Load += new System.EventHandler(this.SettingDialog_Load);
            this.colorsGroupBox.ResumeLayout(false);
            this.colorsGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.animationTimeTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.Button topButton;
        private System.Windows.Forms.GroupBox colorsGroupBox;
        private System.Windows.Forms.Label topLabel;
        private System.Windows.Forms.Label backLabel;
        private System.Windows.Forms.Button backButton;
        private System.Windows.Forms.Label frontLabel;
        private System.Windows.Forms.Button frontButton;
        private System.Windows.Forms.Label rightLabel;
        private System.Windows.Forms.Button rightButton;
        private System.Windows.Forms.Label leftLabel;
        private System.Windows.Forms.Button leftButton;
        private System.Windows.Forms.Label bottomLabel;
        private System.Windows.Forms.Button bottomButton;
        private System.Windows.Forms.Label rotationSpeedLabel;
        private System.Windows.Forms.TrackBar animationTimeTrackBar;
        private System.Windows.Forms.Button defaultButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label animationTimeLabel;
    }
}