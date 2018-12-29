namespace RatchetLevelEditor
{
    partial class TextureViewer
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.texAmountLabel = new System.Windows.Forms.Label();
            this.textureList = new System.Windows.Forms.ListBox();
            this.textureContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.textureReplaceCM = new System.Windows.Forms.ToolStripMenuItem();
            this.texImgGroupBox = new System.Windows.Forms.GroupBox();
            this.textureImage = new System.Windows.Forms.PictureBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.texListView = new System.Windows.Forms.ListView();
            this.texImages = new System.Windows.Forms.ImageList(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pngToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dXT5ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dDSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportAllFolderSelect = new System.Windows.Forms.FolderBrowserDialog();
            this.addTextureDialog = new System.Windows.Forms.OpenFileDialog();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.textureContext.SuspendLayout();
            this.texImgGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textureImage)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.texImgGroupBox);
            this.groupBox1.Location = new System.Drawing.Point(0, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(424, 295);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.texAmountLabel);
            this.groupBox2.Controls.Add(this.textureList);
            this.groupBox2.Location = new System.Drawing.Point(6, 8);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(135, 280);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Textures";
            // 
            // texAmountLabel
            // 
            this.texAmountLabel.AutoSize = true;
            this.texAmountLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.texAmountLabel.Location = new System.Drawing.Point(6, 16);
            this.texAmountLabel.Name = "texAmountLabel";
            this.texAmountLabel.Size = new System.Drawing.Size(89, 13);
            this.texAmountLabel.TabIndex = 0;
            this.texAmountLabel.Text = "Texture Count: -1";
            // 
            // textureList
            // 
            this.textureList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.textureList.ContextMenuStrip = this.textureContext;
            this.textureList.FormattingEnabled = true;
            this.textureList.Location = new System.Drawing.Point(9, 31);
            this.textureList.Name = "textureList";
            this.textureList.Size = new System.Drawing.Size(117, 238);
            this.textureList.TabIndex = 1;
            this.textureList.SelectedIndexChanged += new System.EventHandler(this.textureList_SelectedIndexChanged);
            // 
            // textureContext
            // 
            this.textureContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.textureReplaceCM});
            this.textureContext.Name = "textureContext";
            this.textureContext.Size = new System.Drawing.Size(116, 26);
            // 
            // textureReplaceCM
            // 
            this.textureReplaceCM.Name = "textureReplaceCM";
            this.textureReplaceCM.Size = new System.Drawing.Size(115, 22);
            this.textureReplaceCM.Text = "Replace";
            this.textureReplaceCM.Click += new System.EventHandler(this.textureReplaceCM_Click);
            // 
            // texImgGroupBox
            // 
            this.texImgGroupBox.Controls.Add(this.textureImage);
            this.texImgGroupBox.Location = new System.Drawing.Point(147, 8);
            this.texImgGroupBox.Name = "texImgGroupBox";
            this.texImgGroupBox.Size = new System.Drawing.Size(271, 280);
            this.texImgGroupBox.TabIndex = 2;
            this.texImgGroupBox.TabStop = false;
            // 
            // textureImage
            // 
            this.textureImage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textureImage.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.textureImage.Location = new System.Drawing.Point(6, 11);
            this.textureImage.Name = "textureImage";
            this.textureImage.Size = new System.Drawing.Size(256, 256);
            this.textureImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.textureImage.TabIndex = 0;
            this.textureImage.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.texListView);
            this.groupBox3.Location = new System.Drawing.Point(436, 27);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(794, 295);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Texture Grid";
            // 
            // texListView
            // 
            this.texListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.texListView.LargeImageList = this.texImages;
            this.texListView.Location = new System.Drawing.Point(6, 19);
            this.texListView.Name = "texListView";
            this.texListView.Size = new System.Drawing.Size(782, 269);
            this.texListView.TabIndex = 9;
            this.texListView.UseCompatibleStateImageBehavior = false;
            this.texListView.VirtualMode = true;
            this.texListView.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.texListView_RetrieveVirtualItem);
            this.texListView.SelectedIndexChanged += new System.EventHandler(this.texListView_SelectedIndexChanged);
            // 
            // texImages
            // 
            this.texImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.texImages.ImageSize = new System.Drawing.Size(64, 64);
            this.texImages.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // menuStrip1
            // 
            this.menuStrip1.AllowMerge = false;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1230, 24);
            this.menuStrip1.TabIndex = 11;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dToolStripMenuItem,
            this.exportAsToolStripMenuItem,
            this.exportAllToolStripMenuItem});
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.testToolStripMenuItem.Text = "File";
            // 
            // dToolStripMenuItem
            // 
            this.dToolStripMenuItem.Name = "dToolStripMenuItem";
            this.dToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.dToolStripMenuItem.Text = "Add New...";
            this.dToolStripMenuItem.Click += new System.EventHandler(this.dToolStripMenuItem_Click);
            // 
            // exportAsToolStripMenuItem
            // 
            this.exportAsToolStripMenuItem.Name = "exportAsToolStripMenuItem";
            this.exportAsToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.exportAsToolStripMenuItem.Text = "Export As...";
            this.exportAsToolStripMenuItem.Click += new System.EventHandler(this.exportAsToolStripMenuItem_Click);
            // 
            // exportAllToolStripMenuItem
            // 
            this.exportAllToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pngToolStripMenuItem,
            this.dXT5ToolStripMenuItem,
            this.dDSToolStripMenuItem});
            this.exportAllToolStripMenuItem.Name = "exportAllToolStripMenuItem";
            this.exportAllToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.exportAllToolStripMenuItem.Text = "Export All";
            // 
            // pngToolStripMenuItem
            // 
            this.pngToolStripMenuItem.Name = "pngToolStripMenuItem";
            this.pngToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.pngToolStripMenuItem.Tag = "0";
            this.pngToolStripMenuItem.Text = "PNG";
            this.pngToolStripMenuItem.Click += new System.EventHandler(this.exportAllFunction);
            // 
            // dXT5ToolStripMenuItem
            // 
            this.dXT5ToolStripMenuItem.Name = "dXT5ToolStripMenuItem";
            this.dXT5ToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.dXT5ToolStripMenuItem.Tag = "1";
            this.dXT5ToolStripMenuItem.Text = "DXT5";
            this.dXT5ToolStripMenuItem.Click += new System.EventHandler(this.exportAllFunction);
            // 
            // dDSToolStripMenuItem
            // 
            this.dDSToolStripMenuItem.Name = "dDSToolStripMenuItem";
            this.dDSToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.dDSToolStripMenuItem.Tag = "2";
            this.dDSToolStripMenuItem.Text = "DDS";
            this.dDSToolStripMenuItem.Click += new System.EventHandler(this.exportAllFunction);
            // 
            // addTextureDialog
            // 
            this.addTextureDialog.Filter = "All Picture Files|*.dds;*.dxt5;*.bmp;*.png;*.jpg|Bitmap Image|*.bmp| PNG Image|*." +
    "png| Jpeg Image|*.jpg| DXT5 Texture|*.dxt5| DDS Texture|*.dds";
            // 
            // TextureViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1230, 450);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Name = "TextureViewer";
            this.Text = "Texture Viewer";
            this.Load += new System.EventHandler(this.TextureViewer_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.textureContext.ResumeLayout(false);
            this.texImgGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.textureImage)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label texAmountLabel;
        public System.Windows.Forms.ListBox textureList;
        private System.Windows.Forms.GroupBox texImgGroupBox;
        private System.Windows.Forms.PictureBox textureImage;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ListView texListView;
        private System.Windows.Forms.ImageList texImages;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pngToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dXT5ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dDSToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog exportAllFolderSelect;
        private System.Windows.Forms.OpenFileDialog addTextureDialog;
        private System.Windows.Forms.ContextMenuStrip textureContext;
        private System.Windows.Forms.ToolStripMenuItem textureReplaceCM;
    }
}