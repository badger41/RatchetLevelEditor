namespace RatchetLevelEditor
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openMapMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMapMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.terrainToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.objEditTool = new System.Windows.Forms.ToolStripButton();
            this.levelObjToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.modelToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.texToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.texConfigToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.spritesToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.stringsToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.spawnsToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.interfaceStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.consoleStripButton = new System.Windows.Forms.ToolStripButton();
            this.mainMenu.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.exportDataToolStripMenuItem});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(1076, 24);
            this.mainMenu.TabIndex = 0;
            this.mainMenu.Text = "Menu Strip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openMapMenuItem,
            this.saveMapMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openMapMenuItem
            // 
            this.openMapMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openMapMenuItem.Image")));
            this.openMapMenuItem.Name = "openMapMenuItem";
            this.openMapMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openMapMenuItem.Text = "Open Map";
            this.openMapMenuItem.Click += new System.EventHandler(this.openMapMenuItem_Click);
            // 
            // saveMapMenuItem
            // 
            this.saveMapMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveMapMenuItem.Image")));
            this.saveMapMenuItem.Name = "saveMapMenuItem";
            this.saveMapMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveMapMenuItem.Text = "Save Map";
            this.saveMapMenuItem.Click += new System.EventHandler(this.saveMapMenuItem_Click);
            // 
            // exportDataToolStripMenuItem
            // 
            this.exportDataToolStripMenuItem.Enabled = false;
            this.exportDataToolStripMenuItem.Name = "exportDataToolStripMenuItem";
            this.exportDataToolStripMenuItem.Size = new System.Drawing.Size(79, 20);
            this.exportDataToolStripMenuItem.Text = "Export Data";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.terrainToolStripButton,
            this.objEditTool,
            this.levelObjToolStripButton,
            this.toolStripSeparator1,
            this.modelToolStripButton,
            this.texToolStripButton,
            this.texConfigToolStripButton,
            this.spritesToolStripButton,
            this.stringsToolStripButton,
            this.spawnsToolStripButton,
            this.interfaceStripButton,
            this.toolStripSeparator2,
            this.consoleStripButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1076, 25);
            this.toolStrip1.TabIndex = 6;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // terrainToolStripButton
            // 
            this.terrainToolStripButton.CheckOnClick = true;
            this.terrainToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.terrainToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.terrainToolStripButton.Name = "terrainToolStripButton";
            this.terrainToolStripButton.Size = new System.Drawing.Size(48, 22);
            this.terrainToolStripButton.Text = "Terrain";
            this.terrainToolStripButton.CheckedChanged += new System.EventHandler(this.terrainToolStripButton_CheckedChanged);
            // 
            // objEditTool
            // 
            this.objEditTool.CheckOnClick = true;
            this.objEditTool.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.objEditTool.Enabled = false;
            this.objEditTool.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.objEditTool.Name = "objEditTool";
            this.objEditTool.Size = new System.Drawing.Size(47, 22);
            this.objEditTool.Text = "&Mobys";
            this.objEditTool.CheckedChanged += new System.EventHandler(this.objEditTool_CheckedChanged);
            // 
            // levelObjToolStripButton
            // 
            this.levelObjToolStripButton.CheckOnClick = true;
            this.levelObjToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.levelObjToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.levelObjToolStripButton.Name = "levelObjToolStripButton";
            this.levelObjToolStripButton.Size = new System.Drawing.Size(81, 22);
            this.levelObjToolStripButton.Text = "Level Objects";
            this.levelObjToolStripButton.CheckedChanged += new System.EventHandler(this.levelObjToolStripButton_CheckedChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // modelToolStripButton
            // 
            this.modelToolStripButton.CheckOnClick = true;
            this.modelToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.modelToolStripButton.Enabled = false;
            this.modelToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.modelToolStripButton.Name = "modelToolStripButton";
            this.modelToolStripButton.Size = new System.Drawing.Size(50, 22);
            this.modelToolStripButton.Text = "&Models";
            this.modelToolStripButton.CheckedChanged += new System.EventHandler(this.modelToolStripButton_CheckedChanged);
            // 
            // texToolStripButton
            // 
            this.texToolStripButton.CheckOnClick = true;
            this.texToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.texToolStripButton.Enabled = false;
            this.texToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.texToolStripButton.Name = "texToolStripButton";
            this.texToolStripButton.Size = new System.Drawing.Size(55, 22);
            this.texToolStripButton.Text = "&Textures";
            this.texToolStripButton.CheckedChanged += new System.EventHandler(this.texToolStripButton_CheckedChanged);
            // 
            // texConfigToolStripButton
            // 
            this.texConfigToolStripButton.CheckOnClick = true;
            this.texConfigToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.texConfigToolStripButton.Enabled = false;
            this.texConfigToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.texConfigToolStripButton.Name = "texConfigToolStripButton";
            this.texConfigToolStripButton.Size = new System.Drawing.Size(94, 22);
            this.texConfigToolStripButton.Text = "Texture Configs";
            // 
            // spritesToolStripButton
            // 
            this.spritesToolStripButton.CheckOnClick = true;
            this.spritesToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.spritesToolStripButton.Enabled = false;
            this.spritesToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.spritesToolStripButton.Name = "spritesToolStripButton";
            this.spritesToolStripButton.Size = new System.Drawing.Size(46, 22);
            this.spritesToolStripButton.Text = "&Sprites";
            // 
            // stringsToolStripButton
            // 
            this.stringsToolStripButton.CheckOnClick = true;
            this.stringsToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.stringsToolStripButton.Enabled = false;
            this.stringsToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.stringsToolStripButton.Name = "stringsToolStripButton";
            this.stringsToolStripButton.Size = new System.Drawing.Size(47, 22);
            this.stringsToolStripButton.Text = "&Strings";
            // 
            // spawnsToolStripButton
            // 
            this.spawnsToolStripButton.CheckOnClick = true;
            this.spawnsToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.spawnsToolStripButton.Enabled = false;
            this.spawnsToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.spawnsToolStripButton.Name = "spawnsToolStripButton";
            this.spawnsToolStripButton.Size = new System.Drawing.Size(82, 22);
            this.spawnsToolStripButton.Text = "&Spawn Points";
            // 
            // interfaceStripButton
            // 
            this.interfaceStripButton.CheckOnClick = true;
            this.interfaceStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.interfaceStripButton.Enabled = false;
            this.interfaceStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.interfaceStripButton.Name = "interfaceStripButton";
            this.interfaceStripButton.Size = new System.Drawing.Size(62, 22);
            this.interfaceStripButton.Text = "&Interfaces";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // consoleStripButton
            // 
            this.consoleStripButton.CheckOnClick = true;
            this.consoleStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.consoleStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.consoleStripButton.Name = "consoleStripButton";
            this.consoleStripButton.Size = new System.Drawing.Size(54, 22);
            this.consoleStripButton.Text = "&Console";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1076, 487);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.mainMenu);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.mainMenu;
            this.Name = "Main";
            this.Text = "Ratchet Level Editor";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.RatchetLevelEditor_Load);
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openMapMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveMapMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportDataToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton objEditTool;
        public System.Windows.Forms.ToolStripButton modelToolStripButton;
        public System.Windows.Forms.ToolStripButton texToolStripButton;
        private System.Windows.Forms.ToolStripButton texConfigToolStripButton;
        public System.Windows.Forms.ToolStripButton spritesToolStripButton;
        public System.Windows.Forms.ToolStripButton stringsToolStripButton;
        public System.Windows.Forms.ToolStripButton spawnsToolStripButton;
        private System.Windows.Forms.ToolStripButton interfaceStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        public System.Windows.Forms.ToolStripButton consoleStripButton;
        private System.Windows.Forms.ToolStripButton terrainToolStripButton;
        private System.Windows.Forms.ToolStripButton levelObjToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}

