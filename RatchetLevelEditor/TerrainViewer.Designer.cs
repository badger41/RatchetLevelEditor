namespace RatchetLevelEditor
{
    partial class TerrainViewer
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
            this.glControl1 = new OpenTK.GLControl();
            this.button2 = new System.Windows.Forms.Button();
            this.tickTimer = new System.Windows.Forms.Timer(this.components);
            this.mobyCheck = new System.Windows.Forms.CheckBox();
            this.levelButton = new System.Windows.Forms.Button();
            this.levelCheck = new System.Windows.Forms.CheckBox();
            this.sceneryCheck = new System.Windows.Forms.CheckBox();
            this.sceneryButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.camX = new System.Windows.Forms.Label();
            this.camY = new System.Windows.Forms.Label();
            this.camZ = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.terrainCheck = new System.Windows.Forms.CheckBox();
            this.collCheck = new System.Windows.Forms.CheckBox();
            this.button3 = new System.Windows.Forms.Button();
            this.chunkCheck = new System.Windows.Forms.CheckBox();
            this.splineCheck = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // glControl1
            // 
            this.glControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.glControl1.BackColor = System.Drawing.Color.Black;
            this.glControl1.Location = new System.Drawing.Point(150, 12);
            this.glControl1.Name = "glControl1";
            this.glControl1.Size = new System.Drawing.Size(1348, 850);
            this.glControl1.TabIndex = 13;
            this.glControl1.VSync = false;
            this.glControl1.Paint += new System.Windows.Forms.PaintEventHandler(this.glControl1_Paint);
            this.glControl1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.glControl1_KeyDown);
            this.glControl1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.glControl1_KeyUp);
            this.glControl1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseClick);
            this.glControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseDown);
            this.glControl1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseUp);
            this.glControl1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.glConstrol1_ScrollWheel);
            this.glControl1.Resize += new System.EventHandler(this.glControl1_Resize);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(122, 23);
            this.button2.TabIndex = 17;
            this.button2.Text = "Load mobies";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // tickTimer
            // 
            this.tickTimer.Enabled = true;
            this.tickTimer.Interval = 16;
            this.tickTimer.Tick += new System.EventHandler(this.tickTimer_Tick);
            // 
            // mobyCheck
            // 
            this.mobyCheck.AutoSize = true;
            this.mobyCheck.Location = new System.Drawing.Point(12, 128);
            this.mobyCheck.Name = "mobyCheck";
            this.mobyCheck.Size = new System.Drawing.Size(90, 17);
            this.mobyCheck.TabIndex = 19;
            this.mobyCheck.Text = "Show Mobies";
            this.mobyCheck.UseVisualStyleBackColor = true;
            this.mobyCheck.CheckedChanged += new System.EventHandler(this.mobyCheck_CheckedChanged);
            // 
            // levelButton
            // 
            this.levelButton.Location = new System.Drawing.Point(12, 41);
            this.levelButton.Name = "levelButton";
            this.levelButton.Size = new System.Drawing.Size(122, 23);
            this.levelButton.TabIndex = 20;
            this.levelButton.Text = "Load Level Models";
            this.levelButton.UseVisualStyleBackColor = true;
            this.levelButton.Click += new System.EventHandler(this.levelButton_Click);
            // 
            // levelCheck
            // 
            this.levelCheck.AutoSize = true;
            this.levelCheck.Location = new System.Drawing.Point(12, 151);
            this.levelCheck.Name = "levelCheck";
            this.levelCheck.Size = new System.Drawing.Size(119, 17);
            this.levelCheck.TabIndex = 19;
            this.levelCheck.Text = "Show Level Models";
            this.levelCheck.UseVisualStyleBackColor = true;
            this.levelCheck.CheckedChanged += new System.EventHandler(this.mobyCheck_CheckedChanged);
            // 
            // sceneryCheck
            // 
            this.sceneryCheck.AutoSize = true;
            this.sceneryCheck.Location = new System.Drawing.Point(12, 174);
            this.sceneryCheck.Name = "sceneryCheck";
            this.sceneryCheck.Size = new System.Drawing.Size(132, 17);
            this.sceneryCheck.TabIndex = 19;
            this.sceneryCheck.Text = "Show Scenery Models";
            this.sceneryCheck.UseVisualStyleBackColor = true;
            this.sceneryCheck.CheckedChanged += new System.EventHandler(this.mobyCheck_CheckedChanged);
            // 
            // sceneryButton
            // 
            this.sceneryButton.Location = new System.Drawing.Point(12, 70);
            this.sceneryButton.Name = "sceneryButton";
            this.sceneryButton.Size = new System.Drawing.Size(122, 23);
            this.sceneryButton.TabIndex = 20;
            this.sceneryButton.Text = "Load Scenery Models";
            this.sceneryButton.UseVisualStyleBackColor = true;
            this.sceneryButton.Click += new System.EventHandler(this.sceneryButton_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 810);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Camera position:";
            // 
            // camX
            // 
            this.camX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.camX.AutoSize = true;
            this.camX.Location = new System.Drawing.Point(9, 823);
            this.camX.Name = "camX";
            this.camX.Size = new System.Drawing.Size(35, 13);
            this.camX.TabIndex = 22;
            this.camX.Text = "label2";
            // 
            // camY
            // 
            this.camY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.camY.AutoSize = true;
            this.camY.Location = new System.Drawing.Point(9, 836);
            this.camY.Name = "camY";
            this.camY.Size = new System.Drawing.Size(35, 13);
            this.camY.TabIndex = 22;
            this.camY.Text = "label2";
            // 
            // camZ
            // 
            this.camZ.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.camZ.AutoSize = true;
            this.camZ.Location = new System.Drawing.Point(9, 849);
            this.camZ.Name = "camZ";
            this.camZ.Size = new System.Drawing.Size(35, 13);
            this.camZ.TabIndex = 22;
            this.camZ.Text = "label2";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 99);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(122, 23);
            this.button1.TabIndex = 23;
            this.button1.Text = "Load Collision";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // terrainCheck
            // 
            this.terrainCheck.AutoSize = true;
            this.terrainCheck.Checked = true;
            this.terrainCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.terrainCheck.Location = new System.Drawing.Point(12, 220);
            this.terrainCheck.Name = "terrainCheck";
            this.terrainCheck.Size = new System.Drawing.Size(89, 17);
            this.terrainCheck.TabIndex = 24;
            this.terrainCheck.Text = "Show Terrain";
            this.terrainCheck.UseVisualStyleBackColor = true;
            this.terrainCheck.CheckedChanged += new System.EventHandler(this.terrainCheck_CheckedChanged);
            // 
            // collCheck
            // 
            this.collCheck.AutoSize = true;
            this.collCheck.Location = new System.Drawing.Point(12, 197);
            this.collCheck.Name = "collCheck";
            this.collCheck.Size = new System.Drawing.Size(94, 17);
            this.collCheck.TabIndex = 25;
            this.collCheck.Text = "Show Collision";
            this.collCheck.UseVisualStyleBackColor = true;
            this.collCheck.CheckedChanged += new System.EventHandler(this.collCheck_CheckedChanged);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(12, 331);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(132, 54);
            this.button3.TabIndex = 26;
            this.button3.Text = "Chunk";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // chunkCheck
            // 
            this.chunkCheck.AutoSize = true;
            this.chunkCheck.Location = new System.Drawing.Point(12, 391);
            this.chunkCheck.Name = "chunkCheck";
            this.chunkCheck.Size = new System.Drawing.Size(91, 17);
            this.chunkCheck.TabIndex = 27;
            this.chunkCheck.Text = "Show chunks";
            this.chunkCheck.UseVisualStyleBackColor = true;
            this.chunkCheck.CheckedChanged += new System.EventHandler(this.chunkCheck_CheckedChanged);
            // 
            // splineCheck
            // 
            this.splineCheck.AutoSize = true;
            this.splineCheck.Location = new System.Drawing.Point(12, 243);
            this.splineCheck.Name = "splineCheck";
            this.splineCheck.Size = new System.Drawing.Size(90, 17);
            this.splineCheck.TabIndex = 28;
            this.splineCheck.Text = "Show Splines";
            this.splineCheck.UseVisualStyleBackColor = true;
            this.splineCheck.CheckedChanged += new System.EventHandler(this.splineCheck_CheckedChanged);
            // 
            // TerrainViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1510, 874);
            this.Controls.Add(this.splineCheck);
            this.Controls.Add(this.chunkCheck);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.collCheck);
            this.Controls.Add(this.terrainCheck);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.camZ);
            this.Controls.Add(this.camY);
            this.Controls.Add(this.camX);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.sceneryButton);
            this.Controls.Add(this.sceneryCheck);
            this.Controls.Add(this.levelButton);
            this.Controls.Add(this.levelCheck);
            this.Controls.Add(this.mobyCheck);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.glControl1);
            this.Name = "TerrainViewer";
            this.Text = "TerrainViewer";
            this.Load += new System.EventHandler(this.TerrainViewer_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private OpenTK.GLControl glControl1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Timer tickTimer;
        private System.Windows.Forms.CheckBox mobyCheck;
        private System.Windows.Forms.Button levelButton;
        private System.Windows.Forms.CheckBox levelCheck;
        private System.Windows.Forms.CheckBox sceneryCheck;
        private System.Windows.Forms.Button sceneryButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label camX;
        private System.Windows.Forms.Label camY;
        private System.Windows.Forms.Label camZ;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox terrainCheck;
        private System.Windows.Forms.CheckBox collCheck;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.CheckBox chunkCheck;
        private System.Windows.Forms.CheckBox splineCheck;
    }
}