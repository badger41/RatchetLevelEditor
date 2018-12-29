namespace RatchetLevelEditor.MobyPVarUserControls
{
    partial class IntegerControl
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
            this.intValueControl = new System.Windows.Forms.NumericUpDown();
            this.intControlGroup = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.intValueControl)).BeginInit();
            this.intControlGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // intValueControl
            // 
            this.intValueControl.Location = new System.Drawing.Point(6, 19);
            this.intValueControl.Name = "intValueControl";
            this.intValueControl.Size = new System.Drawing.Size(120, 20);
            this.intValueControl.TabIndex = 0;
            this.intValueControl.ValueChanged += new System.EventHandler(this.floatValueControl_ValueChanged);
            // 
            // intControlGroup
            // 
            this.intControlGroup.Controls.Add(this.intValueControl);
            this.intControlGroup.Location = new System.Drawing.Point(3, 3);
            this.intControlGroup.Name = "intControlGroup";
            this.intControlGroup.Size = new System.Drawing.Size(235, 50);
            this.intControlGroup.TabIndex = 1;
            this.intControlGroup.TabStop = false;
            this.intControlGroup.Text = "Integer";
            // 
            // IntegerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.intControlGroup);
            this.Name = "IntegerControl";
            this.Size = new System.Drawing.Size(302, 57);
            ((System.ComponentModel.ISupportInitialize)(this.intValueControl)).EndInit();
            this.intControlGroup.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NumericUpDown intValueControl;
        private System.Windows.Forms.GroupBox intControlGroup;
    }
}
