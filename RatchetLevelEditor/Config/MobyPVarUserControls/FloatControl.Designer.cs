namespace RatchetLevelEditor.MobyPVarUserControls
{
    partial class FloatControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.floatValueControl = new System.Windows.Forms.NumericUpDown();
            this.floatControlGroup = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.floatValueControl)).BeginInit();
            this.floatControlGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // floatValueControl
            // 
            this.floatValueControl.DecimalPlaces = 2;
            this.floatValueControl.Location = new System.Drawing.Point(6, 19);
            this.floatValueControl.Name = "floatValueControl";
            this.floatValueControl.Size = new System.Drawing.Size(120, 20);
            this.floatValueControl.TabIndex = 0;
            this.floatValueControl.ValueChanged += new System.EventHandler(this.floatValueControl_ValueChanged);
            // 
            // floatControlGroup
            // 
            this.floatControlGroup.Controls.Add(this.floatValueControl);
            this.floatControlGroup.Location = new System.Drawing.Point(3, 3);
            this.floatControlGroup.Name = "floatControlGroup";
            this.floatControlGroup.Size = new System.Drawing.Size(133, 50);
            this.floatControlGroup.TabIndex = 1;
            this.floatControlGroup.TabStop = false;
            this.floatControlGroup.Text = "Float";
            // 
            // FloatControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.floatControlGroup);
            this.Name = "FloatControl";
            this.Size = new System.Drawing.Size(140, 57);
            ((System.ComponentModel.ISupportInitialize)(this.floatValueControl)).EndInit();
            this.floatControlGroup.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NumericUpDown floatValueControl;
        private System.Windows.Forms.GroupBox floatControlGroup;
    }
}
