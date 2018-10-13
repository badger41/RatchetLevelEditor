namespace RatchetLevelEditor
{
    partial class LevelObjectViewer
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
            this.levelObjList = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.xNum = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.yNum = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.zNum = new System.Windows.Forms.NumericUpDown();
            this.objProperties = new System.Windows.Forms.ListBox();
            this.teleportButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.scaleXNum = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.scaleYNum = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.scaleZNum = new System.Windows.Forms.NumericUpDown();
            this.scaleSetButton = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.xNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleXNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleYNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleZNum)).BeginInit();
            this.SuspendLayout();
            // 
            // levelObjList
            // 
            this.levelObjList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.levelObjList.FormattingEnabled = true;
            this.levelObjList.IntegralHeight = false;
            this.levelObjList.Location = new System.Drawing.Point(12, 12);
            this.levelObjList.Name = "levelObjList";
            this.levelObjList.Size = new System.Drawing.Size(157, 440);
            this.levelObjList.TabIndex = 0;
            this.levelObjList.SelectedIndexChanged += new System.EventHandler(this.levelObjList_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(175, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Model ID";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(175, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(14, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "X";
            // 
            // xNum
            // 
            this.xNum.DecimalPlaces = 2;
            this.xNum.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.xNum.Location = new System.Drawing.Point(178, 67);
            this.xNum.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.xNum.Minimum = new decimal(new int[] {
            5000,
            0,
            0,
            -2147483648});
            this.xNum.Name = "xNum";
            this.xNum.Size = new System.Drawing.Size(118, 20);
            this.xNum.TabIndex = 4;
            this.xNum.ValueChanged += new System.EventHandler(this.xNum_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(175, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Y";
            // 
            // yNum
            // 
            this.yNum.DecimalPlaces = 2;
            this.yNum.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.yNum.Location = new System.Drawing.Point(178, 106);
            this.yNum.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.yNum.Minimum = new decimal(new int[] {
            5000,
            0,
            0,
            -2147483648});
            this.yNum.Name = "yNum";
            this.yNum.Size = new System.Drawing.Size(118, 20);
            this.yNum.TabIndex = 4;
            this.yNum.ValueChanged += new System.EventHandler(this.yNum_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(175, 129);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(14, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Z";
            // 
            // zNum
            // 
            this.zNum.DecimalPlaces = 2;
            this.zNum.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.zNum.Location = new System.Drawing.Point(178, 145);
            this.zNum.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.zNum.Minimum = new decimal(new int[] {
            5000,
            0,
            0,
            -2147483648});
            this.zNum.Name = "zNum";
            this.zNum.Size = new System.Drawing.Size(118, 20);
            this.zNum.TabIndex = 4;
            this.zNum.ValueChanged += new System.EventHandler(this.zNum_ValueChanged);
            // 
            // objProperties
            // 
            this.objProperties.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.objProperties.FormattingEnabled = true;
            this.objProperties.IntegralHeight = false;
            this.objProperties.Location = new System.Drawing.Point(178, 317);
            this.objProperties.Name = "objProperties";
            this.objProperties.Size = new System.Drawing.Size(118, 106);
            this.objProperties.TabIndex = 5;
            // 
            // teleportButton
            // 
            this.teleportButton.Location = new System.Drawing.Point(178, 429);
            this.teleportButton.Name = "teleportButton";
            this.teleportButton.Size = new System.Drawing.Size(118, 23);
            this.teleportButton.TabIndex = 6;
            this.teleportButton.Text = "Goto position";
            this.teleportButton.UseVisualStyleBackColor = true;
            this.teleportButton.Click += new System.EventHandler(this.teleportButton_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(175, 168);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "X Scale";
            // 
            // scaleXNum
            // 
            this.scaleXNum.DecimalPlaces = 4;
            this.scaleXNum.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.scaleXNum.Location = new System.Drawing.Point(178, 184);
            this.scaleXNum.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.scaleXNum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.scaleXNum.Name = "scaleXNum";
            this.scaleXNum.Size = new System.Drawing.Size(118, 20);
            this.scaleXNum.TabIndex = 4;
            this.scaleXNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(175, 207);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(44, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "Y Scale";
            // 
            // scaleYNum
            // 
            this.scaleYNum.DecimalPlaces = 4;
            this.scaleYNum.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.scaleYNum.Location = new System.Drawing.Point(178, 223);
            this.scaleYNum.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.scaleYNum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.scaleYNum.Name = "scaleYNum";
            this.scaleYNum.Size = new System.Drawing.Size(118, 20);
            this.scaleYNum.TabIndex = 4;
            this.scaleYNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(175, 246);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(44, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "Z Scale";
            // 
            // scaleZNum
            // 
            this.scaleZNum.DecimalPlaces = 4;
            this.scaleZNum.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.scaleZNum.Location = new System.Drawing.Point(178, 262);
            this.scaleZNum.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.scaleZNum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.scaleZNum.Name = "scaleZNum";
            this.scaleZNum.Size = new System.Drawing.Size(118, 20);
            this.scaleZNum.TabIndex = 4;
            this.scaleZNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            // 
            // scaleSetButton
            // 
            this.scaleSetButton.Location = new System.Drawing.Point(178, 288);
            this.scaleSetButton.Name = "scaleSetButton";
            this.scaleSetButton.Size = new System.Drawing.Size(118, 23);
            this.scaleSetButton.TabIndex = 7;
            this.scaleSetButton.Text = "Set Scale";
            this.scaleSetButton.UseVisualStyleBackColor = true;
            this.scaleSetButton.Click += new System.EventHandler(this.scaleSetButton_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(178, 28);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(118, 21);
            this.comboBox1.TabIndex = 8;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 458);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(284, 41);
            this.button1.TabIndex = 10;
            this.button1.Text = "Save All";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // LevelObjectViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(308, 511);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.scaleSetButton);
            this.Controls.Add(this.teleportButton);
            this.Controls.Add(this.objProperties);
            this.Controls.Add(this.scaleZNum);
            this.Controls.Add(this.scaleYNum);
            this.Controls.Add(this.scaleXNum);
            this.Controls.Add(this.zNum);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.yNum);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.xNum);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.levelObjList);
            this.Name = "LevelObjectViewer";
            this.Text = "LevelObjectViewer";
            this.Load += new System.EventHandler(this.LevelObjectViewer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.xNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleXNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleYNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleZNum)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox levelObjList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown xNum;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown yNum;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown zNum;
        private System.Windows.Forms.ListBox objProperties;
        private System.Windows.Forms.Button teleportButton;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown scaleXNum;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown scaleYNum;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown scaleZNum;
        private System.Windows.Forms.Button scaleSetButton;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button button1;
    }
}