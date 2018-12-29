namespace RatchetLevelEditor
{
    partial class ModelViewer
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
            this.modelsListTree = new System.Windows.Forms.TreeView();
            this.modelDataList = new System.Windows.Forms.ListBox();
            this.glControl1 = new OpenTK.GLControl();
            this.rotBar = new System.Windows.Forms.TrackBar();
            this.zoomBar = new System.Windows.Forms.TrackBar();
            this.exportBtn = new System.Windows.Forms.Button();
            this.objSaveDialog = new System.Windows.Forms.SaveFileDialog();
            this.animListBox = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.rotBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zoomBar)).BeginInit();
            this.SuspendLayout();
            // 
            // modelsListTree
            // 
            this.modelsListTree.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.modelsListTree.Location = new System.Drawing.Point(12, 12);
            this.modelsListTree.Name = "modelsListTree";
            this.modelsListTree.Size = new System.Drawing.Size(347, 759);
            this.modelsListTree.TabIndex = 10;
            this.modelsListTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.modelsListTree_AfterSelect);
            // 
            // modelDataList
            // 
            this.modelDataList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.modelDataList.FormattingEnabled = true;
            this.modelDataList.Location = new System.Drawing.Point(365, 624);
            this.modelDataList.Name = "modelDataList";
            this.modelDataList.Size = new System.Drawing.Size(120, 147);
            this.modelDataList.TabIndex = 11;
            // 
            // glControl1
            // 
            this.glControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.glControl1.BackColor = System.Drawing.Color.Black;
            this.glControl1.Location = new System.Drawing.Point(365, 12);
            this.glControl1.Name = "glControl1";
            this.glControl1.Size = new System.Drawing.Size(640, 480);
            this.glControl1.TabIndex = 12;
            this.glControl1.VSync = false;
            this.glControl1.Paint += new System.Windows.Forms.PaintEventHandler(this.glControl1_Paint);
            this.glControl1.Resize += new System.EventHandler(this.glControl1_Resize);
            // 
            // rotBar
            // 
            this.rotBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rotBar.Location = new System.Drawing.Point(365, 498);
            this.rotBar.Maximum = 314;
            this.rotBar.Minimum = -314;
            this.rotBar.Name = "rotBar";
            this.rotBar.Size = new System.Drawing.Size(640, 45);
            this.rotBar.TabIndex = 13;
            this.rotBar.Scroll += new System.EventHandler(this.rotBar_Scroll);
            // 
            // zoomBar
            // 
            this.zoomBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.zoomBar.Location = new System.Drawing.Point(365, 549);
            this.zoomBar.Maximum = 100;
            this.zoomBar.Name = "zoomBar";
            this.zoomBar.Size = new System.Drawing.Size(640, 45);
            this.zoomBar.TabIndex = 14;
            this.zoomBar.Value = 75;
            this.zoomBar.Scroll += new System.EventHandler(this.zoomBar_Scroll);
            // 
            // exportBtn
            // 
            this.exportBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.exportBtn.Location = new System.Drawing.Point(881, 735);
            this.exportBtn.Name = "exportBtn";
            this.exportBtn.Size = new System.Drawing.Size(124, 36);
            this.exportBtn.TabIndex = 15;
            this.exportBtn.Text = "Export to .OBJ";
            this.exportBtn.UseVisualStyleBackColor = true;
            this.exportBtn.Click += new System.EventHandler(this.exportBtn_Click);
            // 
            // objSaveDialog
            // 
            this.objSaveDialog.Filter = "OBJ file|*.obj";
            // 
            // animListBox
            // 
            this.animListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.animListBox.FormattingEnabled = true;
            this.animListBox.Location = new System.Drawing.Point(491, 624);
            this.animListBox.Name = "animListBox";
            this.animListBox.Size = new System.Drawing.Size(120, 147);
            this.animListBox.TabIndex = 16;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(365, 608);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "Data:";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(488, 608);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "Animations:";
            // 
            // ModelViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1017, 783);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.animListBox);
            this.Controls.Add(this.exportBtn);
            this.Controls.Add(this.zoomBar);
            this.Controls.Add(this.rotBar);
            this.Controls.Add(this.glControl1);
            this.Controls.Add(this.modelDataList);
            this.Controls.Add(this.modelsListTree);
            this.MinimumSize = new System.Drawing.Size(16, 39);
            this.Name = "ModelViewer";
            this.Text = "Models";
            this.Load += new System.EventHandler(this.ModelViewer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.rotBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zoomBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView modelsListTree;
        private System.Windows.Forms.ListBox modelDataList;
        private OpenTK.GLControl glControl1;
        private System.Windows.Forms.TrackBar rotBar;
        private System.Windows.Forms.TrackBar zoomBar;
        private System.Windows.Forms.Button exportBtn;
        private System.Windows.Forms.SaveFileDialog objSaveDialog;
        private System.Windows.Forms.ListBox animListBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}