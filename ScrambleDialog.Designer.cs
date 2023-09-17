namespace VirtualRubiksCube
{
    partial class ScrambleDialog
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
            numberOfMovesLabel = new Label();
            numberOfMovesNumericUpDown = new NumericUpDown();
            middleLayerCheckBox = new CheckBox();
            okButton = new Button();
            cancelButton = new Button();
            ((System.ComponentModel.ISupportInitialize)numberOfMovesNumericUpDown).BeginInit();
            SuspendLayout();
            // 
            // numberOfMovesLabel
            // 
            numberOfMovesLabel.AutoSize = true;
            numberOfMovesLabel.Location = new Point(38, 29);
            numberOfMovesLabel.Margin = new Padding(4, 0, 4, 0);
            numberOfMovesLabel.Name = "numberOfMovesLabel";
            numberOfMovesLabel.Size = new Size(103, 15);
            numberOfMovesLabel.TabIndex = 0;
            numberOfMovesLabel.Text = "Number of moves";
            // 
            // numberOfMovesNumericUpDown
            // 
            numberOfMovesNumericUpDown.Location = new Point(150, 27);
            numberOfMovesNumericUpDown.Margin = new Padding(4, 3, 4, 3);
            numberOfMovesNumericUpDown.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
            numberOfMovesNumericUpDown.Name = "numberOfMovesNumericUpDown";
            numberOfMovesNumericUpDown.Size = new Size(140, 23);
            numberOfMovesNumericUpDown.TabIndex = 0;
            numberOfMovesNumericUpDown.Value = new decimal(new int[] { 20, 0, 0, 0 });
            // 
            // middleLayerCheckBox
            // 
            middleLayerCheckBox.AutoSize = true;
            middleLayerCheckBox.Location = new Point(42, 75);
            middleLayerCheckBox.Margin = new Padding(4, 3, 4, 3);
            middleLayerCheckBox.Name = "middleLayerCheckBox";
            middleLayerCheckBox.Size = new Size(178, 19);
            middleLayerCheckBox.TabIndex = 1;
            middleLayerCheckBox.Text = "Include middle layer rotation";
            middleLayerCheckBox.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            okButton.DialogResult = DialogResult.OK;
            okButton.Location = new Point(135, 128);
            okButton.Margin = new Padding(4, 3, 4, 3);
            okButton.Name = "okButton";
            okButton.Size = new Size(88, 27);
            okButton.TabIndex = 2;
            okButton.Text = "OK";
            okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            cancelButton.DialogResult = DialogResult.Cancel;
            cancelButton.Location = new Point(230, 128);
            cancelButton.Margin = new Padding(4, 3, 4, 3);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(88, 27);
            cancelButton.TabIndex = 3;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            // 
            // ScrambleDialog
            // 
            AcceptButton = okButton;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = cancelButton;
            ClientSize = new Size(331, 168);
            Controls.Add(cancelButton);
            Controls.Add(okButton);
            Controls.Add(middleLayerCheckBox);
            Controls.Add(numberOfMovesNumericUpDown);
            Controls.Add(numberOfMovesLabel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ScrambleDialog";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Scramble";
            FormClosed += ScrambleDialog_FormClosed;
            ((System.ComponentModel.ISupportInitialize)numberOfMovesNumericUpDown).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label numberOfMovesLabel;
        private NumericUpDown numberOfMovesNumericUpDown;
        private CheckBox middleLayerCheckBox;
        private Button okButton;
        private Button cancelButton;
    }
}