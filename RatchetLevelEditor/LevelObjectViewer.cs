using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using static RatchetModel;
using static DataFunctions;
using System.IO;

namespace RatchetLevelEditor
{
    public partial class LevelObjectViewer : Form
    {
        LevelObject selectedLevelObj;
        Main mainForm;
        List<int> mods;


        public LevelObjectViewer(Main main)
        {
            InitializeComponent();
            mainForm = main;
        }


        private void LevelObjectViewer_Load(object sender, EventArgs e)
        {
            foreach(LevelObject levelObj in DataStore.levelObjects)
            {
                levelObjList.Items.Add(levelObj.modelID.ToString("X"));
            }
            mods = new List<int>();
            for (int i = 0; i < DataStore.levelModels.Count(); i++)
            {
                comboBox1.Items.Add(DataStore.levelModels[i].modelID.ToString("X"));
                mods.Add(DataStore.levelModels[i].modelID);
            }
        }

        private void levelObjList_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedLevelObj = DataStore.levelObjects[levelObjList.SelectedIndex];
            DataStore.selectedLevelObject = selectedLevelObj;
            DataStore.selectedMoby = null;
            updateValues();

        }
        private void updateValues()
        {
            xNum.Value = (decimal)selectedLevelObj.mat.M41;
            yNum.Value = (decimal)selectedLevelObj.mat.M42;
            zNum.Value = (decimal)selectedLevelObj.mat.M43;
            objProperties.Items.Clear();

            objProperties.Items.Add(selectedLevelObj.off_50.ToString("X8"));
            objProperties.Items.Add(selectedLevelObj.off_54.ToString("X8"));
            objProperties.Items.Add(selectedLevelObj.off_58.ToString("X8"));
            objProperties.Items.Add(selectedLevelObj.off_5C.ToString("X8"));

            objProperties.Items.Add(selectedLevelObj.off_60.ToString("X8"));
            objProperties.Items.Add(selectedLevelObj.off_64.ToString("X8"));
            objProperties.Items.Add(selectedLevelObj.off_68.ToString("X8"));
            objProperties.Items.Add(selectedLevelObj.off_6C.ToString("X8"));
            Vector3 scaleVec = selectedLevelObj.mat.ExtractScale();
            scaleXNum.Value = (decimal)scaleVec.X;
            scaleYNum.Value = (decimal)scaleVec.Y;
            scaleZNum.Value = (decimal)scaleVec.Z;
            comboBox1.SelectedIndex = mods.IndexOf(selectedLevelObj.modelID);
        }

        private void xNum_ValueChanged(object sender, EventArgs e)
        {
            selectedLevelObj.mat.M41 = (float)xNum.Value;
            mainForm.terrainViewer.updateView();
        }

        private void yNum_ValueChanged(object sender, EventArgs e)
        {
            selectedLevelObj.mat.M42 = (float)yNum.Value;
            mainForm.terrainViewer.updateView();
        }

        private void zNum_ValueChanged(object sender, EventArgs e)
        {
            selectedLevelObj.mat.M43 = (float)zNum.Value;
            mainForm.terrainViewer.updateView();
        }

        private void teleportButton_Click(object sender, EventArgs e)
        {
            mainForm.terrainViewer.setCamPos(selectedLevelObj.mat.M41 + 2, selectedLevelObj.mat.M42, selectedLevelObj.mat.M43 + 10);
            mainForm.terrainViewer.updateView();
        }
        

        private void scaleSetButton_Click(object sender, EventArgs e)
        {
            Matrix4 scale = Matrix4.CreateScale((float)scaleXNum.Value, (float)scaleYNum.Value, (float)scaleZNum.Value);
            selectedLevelObj.mat = scale * selectedLevelObj.mat.ClearScale();
            mainForm.terrainViewer.updateView();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedLevelObj.modelID = (ushort)mods[comboBox1.SelectedIndex];
            mainForm.terrainViewer.updateView();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            int objCount = DataStore.levelObjects.Count();
            byte[] test = new byte[objCount * 0x70];

            for(int i = 0; i < objCount; i++)
            {
                LevelObject levObj = DataStore.levelObjects[i];
                WriteFloat(ref test, (i * 0x70) + 0x00, levObj.mat.M11);
                WriteFloat(ref test, (i * 0x70) + 0x04, levObj.mat.M12);
                WriteFloat(ref test, (i * 0x70) + 0x08, levObj.mat.M13);
                WriteFloat(ref test, (i * 0x70) + 0x0C, levObj.mat.M14);

                WriteFloat(ref test, (i * 0x70) + 0x10, levObj.mat.M21);
                WriteFloat(ref test, (i * 0x70) + 0x14, levObj.mat.M22);
                WriteFloat(ref test, (i * 0x70) + 0x18, levObj.mat.M23);
                WriteFloat(ref test, (i * 0x70) + 0x1C, levObj.mat.M24);

                WriteFloat(ref test, (i * 0x70) + 0x20, levObj.mat.M31);
                WriteFloat(ref test, (i * 0x70) + 0x24, levObj.mat.M32);
                WriteFloat(ref test, (i * 0x70) + 0x28, levObj.mat.M33);
                WriteFloat(ref test, (i * 0x70) + 0x2C, levObj.mat.M34);

                WriteFloat(ref test, (i * 0x70) + 0x30, levObj.mat.M41);
                WriteFloat(ref test, (i * 0x70) + 0x34, levObj.mat.M42);
                WriteFloat(ref test, (i * 0x70) + 0x38, levObj.mat.M43);
                WriteFloat(ref test, (i * 0x70) + 0x3C, levObj.mat.M44);

                WriteUint16(ref test, (i * 0x70) + 0x50, levObj.off_50);
                WriteUint16(ref test, (i * 0x70) + 0x52, levObj.modelID);
                WriteUint32(ref test, (i * 0x70) + 0x54, levObj.off_54);
                WriteUint32(ref test, (i * 0x70) + 0x58, levObj.off_58);
                WriteUint32(ref test, (i * 0x70) + 0x5C, levObj.off_5C);

                WriteUint32(ref test, (i * 0x70) + 0x60, levObj.off_60);
                WriteUint32(ref test, (i * 0x70) + 0x64, levObj.off_64);
                WriteUint32(ref test, (i * 0x70) + 0x68, levObj.off_68);
                WriteUint32(ref test, (i * 0x70) + 0x6C, levObj.off_6C);
            }


            FileStream vfs = null;
            vfs = File.OpenWrite(DataStore.workingDirectory + "/engine.ps3");
            vfs.Seek(DataStore.engineHeader.levelObjectPointer, SeekOrigin.Begin);
            vfs.Write(test, 0, objCount * 0x70);
            vfs.Close();
            Console.WriteLine("File written successfully, enjoy :)");
        }
    }
}
