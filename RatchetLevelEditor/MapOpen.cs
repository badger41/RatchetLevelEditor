using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RatchetLevelEditor
{
    public partial class MapOpen : Form
    {
        public string fileName;
        public Main main;
        public MapOpen(Main _main)
        {
            main = _main;
            InitializeComponent();
        }

        private void loadMapButton_Click(object sender, EventArgs e)
        {
            if (mapSelectDialog.ShowDialog() == DialogResult.OK)
            {
                FileInfo fi = new FileInfo(mapSelectDialog.FileName);
                string path = fi.Directory.FullName;
                mapDirTextBox.Text = path;
                fileName = mapSelectDialog.FileName;
                main.Text = fi.Directory.Name + " - " + "Ratchet Level Editor";
            }
        }

		private void loadButton_Click(object sender, EventArgs e)
		{
			RCGame game = RCGame.RatchetAndClank;

			// might need to improve this part
			if (radioButton1.Checked) game = RCGame.RatchetAndClank;
			else if (radioButton2.Checked) game = RCGame.GoingCommando;
			else if (radioButton3.Checked) game = RCGame.UpYourArsenal;
			else if (radioButton4.Checked) game = RCGame.Deadlocked;

			if (mapDirTextBox.Text != "")
			{
				main.loadLevelFiles(mapDirTextBox.Text, Path.GetFileName(fileName), game);
				Hide();
			}
			else Main.reportError(5);
		}

        private void MapOpen_Load(object sender, EventArgs e)
        {

        }
    }
}
