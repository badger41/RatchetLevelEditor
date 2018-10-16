using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RatchetLevelEditor
{
    public partial class Main : Form
    {

        /* Define Variables */
        public static string workingDirectory;
        public static RCGame currentGame;
        private static bool errorShown = false;
        public static List<string> modelNames;
        /* End Variable Definitions */

        /*Define our compoents*/
        public MapOpen openMap;
        public ConsoleWindow consoleWindow;
        public TextureViewer texWindow;
        public ModelViewer modelViewer;
        public ObjectViewer objViewer;
        public TerrainViewer terrainViewer;
        public LevelObjectViewer levelObjectViewer;
        /* End Component Definitions */


        public Main()
        {
            InitializeComponent();
        }

        private void RatchetLevelEditor_Load(object sender, EventArgs e)
        {

            consoleWindow = new ConsoleWindow(this);
            consoleWindow.MdiParent = this;
            consoleWindow.Visible = true;
            openMap = new MapOpen(this);
            openMap.ShowDialog();
            texWindow = new TextureViewer(this);
            modelViewer = new ModelViewer(this);
            objViewer = new ObjectViewer(this);
            terrainViewer = new TerrainViewer(this);
            levelObjectViewer = new LevelObjectViewer(this);

        }

        private void openMapMenuItem_Click(object sender, EventArgs e)
        {
            openMap.ShowDialog(this);
        }


        public void loadLevelFiles(string directory, string fileName, RCGame game)
        {
            //Determine what format we will be working with
            currentGame = game;

            getModelNames(currentGame);

            //Set our working directory, this will be referenced around the entire editor
            workingDirectory = directory;
            DataStore.workingDirectory = workingDirectory;

            //Identify file with a trailing number, such as skin20.ps3
            string fileNumber = Regex.Match(fileName, @"\d+").Value;
            if (fileNumber == "")
                fileNumber = "-1";
            var regex = new Regex(Regex.Escape(fileNumber));
            var newText = regex.Replace(fileName, "%d", 1);

            //First we interate through all of the known files in constants
            for (int i = 0; i < Constants.fileNames.GetLength(0); i++)
            {
                if (fileName == Constants.fileNames[i, 0] || regex.Replace(fileName, "%d", 1) == Constants.fileNames[i, 0])
                {
                    for (int subFiles = 0; subFiles < 3; subFiles++)
                    {
                        //catch null files in subfiles object
                        try
                        {
                            if (Constants.fileNames[i, subFiles] != null && File.Exists(directory + "/" + Constants.fileNames[i, subFiles].Replace("%d", fileNumber)))
                            {
                                switch (Constants.fileNames[i, subFiles])
                                {

                                    case "engine.ps3":
                                        switch (currentGame)
										{
											case RCGame.RatchetAndClank:
											case RCGame.GoingCommando:
											case RCGame.UpYourArsenal:
												MapParser_UYA.parseMap(directory, Constants.fileNames[i, subFiles]);
												break;
											case RCGame.Deadlocked:
												MapParser_DL.parseMap(directory, Constants.fileNames[i, subFiles]);
												break;
										}
                                        break;
                                    case "gadgets.ps3":
                                        GadgetsParser.parseGadgets(directory, Constants.fileNames[i, subFiles]);
                                        break;
                                    case "mobyload%d.ps3":
                                        MobyParser.parseMoby(directory, Constants.fileNames[i, subFiles].Replace("%d", fileNumber));
                                        break;
                                }
                            }
                            else if (!errorShown && Constants.fileNames[i, subFiles] != null)
                            {
                                reportError(1, Constants.fileNames[i, subFiles]);
                            }
                        }
                        catch (Exception nullFiles)
                        {
                            Console.WriteLine(nullFiles);
                        }
                    }
                    break;
                }
                else
                {
                    if (!errorShown && i >= Constants.fileNames.GetLength(0) - 1)
                    {
                        reportError(0, fileName);
                        break;
                    }
                }
            }

            //Once we are done loading everything, check DataSource and enable our buttons as needed
            texToolStripButton.Enabled = DataStore.textures.Count > 0;
            bool hasModels = (
                DataStore.spawnableModels.Count > 0
                ||
                DataStore.levelModels.Count > 0
                ||
                DataStore.sceneryModels.Count > 0
                );
            modelToolStripButton.Enabled = hasModels;

            objEditTool.Enabled = DataStore.mobs.Count > 0;
        }
        // Read the file and display it line by line.
        public void getModelNames(RCGame game)
        {
            modelNames = new List<string>();
            string stringCounter;
            StreamReader stream = null;
            try
            {
				switch (game)
				{
					case RCGame.RatchetAndClank:
						stream = new StreamReader(Application.StartupPath + "/ModelLists/RC1.txt");
						Console.WriteLine("Loaded model names for Ratchet & Clank.");
						break;
					case RCGame.GoingCommando:
						stream = new StreamReader(Application.StartupPath + "/ModelLists/GC.txt");
						Console.WriteLine("Loaded model names for Ratchet & Clank: Going Commando.");
						break;
					case RCGame.UpYourArsenal:
						stream = new StreamReader(Application.StartupPath + "/ModelLists/UYA.txt");
						Console.WriteLine("Loaded model names for Ratchet & Clank: Up Your Arsenal.");
						break;
					case RCGame.Deadlocked:
						stream = new StreamReader(Application.StartupPath + "/ModelLists/DL.txt");
						Console.WriteLine("Loaded model names for Ratchet: Deadlocked.");
						break;
				}
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("Model list file not found! No names for you!");
                modelNames = null;
                return;
            }
            while ((stringCounter = stream.ReadLine()) != null)
            {
                modelNames.Add(stringCounter);
            }
            stream.Close();
        }
        //Used to display an appropriate error message
        public static void reportError(int errorCode)
        {
            errorShown = true;
            DialogResult result;
            if (errorCode < Constants.errorCodes.GetLength(0))
                result = MessageBox.Show(Constants.errorCodes[errorCode, 0], Constants.errorCodes[errorCode, 1], MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                result = MessageBox.Show("Unknown error code: (" + errorCode + ")\nPlease Report.", "Unknown Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            if (result == DialogResult.OK)
                errorShown = false;
        }
        //Used to display an appropriate error message with dynamic relay string
        //Example: used when loading level files, displays filename that it cannot find
        public static void reportError(int errorCode, string relayString)
        {
            errorShown = true;
            DialogResult result;
            if (errorCode < Constants.errorCodes.GetLength(0))
                result = MessageBox.Show(Constants.errorCodes[errorCode, 0].Replace("%s", relayString), Constants.errorCodes[errorCode, 1], MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                result = MessageBox.Show("Unknown error code: (" + errorCode + ")\nPlease Report.", "Unknown Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            if (result == DialogResult.OK)
                errorShown = false;
        }

        private void texToolStripButton_CheckedChanged(object sender, EventArgs e)
        {
            texWindow.MdiParent = this;
            if (texToolStripButton.Checked)
                texWindow.Visible = true;
            else
                texWindow.Visible = false;
        }

        private void modelToolStripButton_CheckedChanged(object sender, EventArgs e)
        {
            modelViewer = new ModelViewer(this);
            modelViewer.MdiParent = this;
            if (modelToolStripButton.Checked)
                modelViewer.Visible = true;
            else
                modelViewer.Visible = false;
        }

        private void objEditTool_CheckedChanged(object sender, EventArgs e)
        {
            objViewer = new ObjectViewer(this);
            objViewer.MdiParent = this;
            if (objEditTool.Checked)
                objViewer.Visible = true;
            else
                objViewer.Visible = false;
        }

        private void terrainToolStripButton_CheckedChanged(object sender, EventArgs e)
        {
            terrainViewer = new TerrainViewer(this);
            terrainViewer.MdiParent = this;
            if (terrainToolStripButton.Checked)
                terrainViewer.Visible = true;
            else
                terrainViewer.Visible = false;
        }

        private void levelObjToolStripButton_CheckedChanged(object sender, EventArgs e)
        {
            levelObjectViewer = new LevelObjectViewer(this);
            levelObjectViewer.MdiParent = this;
            if (levelObjToolStripButton.Checked)
                levelObjectViewer.Visible = true;
            else
                levelObjectViewer.Visible = false;
        }
    }
}
