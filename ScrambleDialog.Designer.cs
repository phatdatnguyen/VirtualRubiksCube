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
            this.numberOfMovesLabel = new System.Windows.Forms.Label();
            this.numberOfMovesNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.middleLayerCheckBox = new System.Windows.Forms.CheckBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numberOfMovesNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // numberOfMovesLabel
            // 
            this.numberOfMovesLabel.AutoSize = true;
            this.numberOfMovesLabel.Location = new System.Drawing.Point(33, 25);
            this.numberOfMovesLabel.Name = "numberOfMovesLabel";
            this.numberOfMovesLabel.Size = new System.Drawing.Size(90, 13);
            this.numberOfMovesLabel.TabIndex = 0;
            this.numberOfMovesLabel.Text = "Number of moves";
            // 
            // numberOfMovesNumericUpDown
            // 
            this.numberOfMovesNumericUpDown.Location = new System.Drawing.Point(129, 23);
            this.numberOfMovesNumericUpDown.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numberOfMovesNumericUpDown.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numberOfMovesNumericUpDown.Name = "numberOfMovesNumericUpDown";
            this.numberOfMovesNumericUpDown.Size = new System.Drawing.Size(120, 20);
            this.numberOfMovesNumericUpDown.TabIndex = 0;
            this.numberOfMovesNumericUpDown.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // middleLayerCheckBox
            // 
            this.middleLayerCheckBox.AutoSize = true;
            this.middleLayerCheckBox.Location = new System.Drawing.Point(36, 65);
            this.middleLayerCheckBox.Name = "middleLayerCheckBox";
            this.middleLayerCheckBox.Size = new System.Drawing.Size(157, 17);
            this.middleLayerCheckBox.TabIndex = 1;
            this.middleLayerCheckBox.Text = "Include middle layer rotation";
            this.middleLayerCheckBox.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(116, 111);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(197, 111);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // ScrambleDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(284, 146);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.middleLayerCheckBox);
            this.Controls.Add(this.numberOfMovesNumericUpDown);
            this.Controls.Add(this.numberOfMovesLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ScrambleDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Scramble";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ScrambleDialog_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.numberOfMovesNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label numberOfMovesLabel;
        private System.Windows.Forms.NumericUpDown numberOfMovesNumericUpDown;
        private System.Windows.Forms.CheckBox middleLayerCheckBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
    }
}