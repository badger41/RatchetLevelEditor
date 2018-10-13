using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using static DataFunctions;
using RatchetLevelEditor.MobyPVarUserControls;

namespace RatchetLevelEditor
{
    public partial class ObjectViewer : Form
    {
        Main mainForm;
        RatchetMoby selectedMoby;

        public ObjectViewer(Main main)
        {
            InitializeComponent();
            mainForm = main;
        }

        public void selectList(uint num)
        {
            if(num < listBox1.Items.Count)
            {
                listBox1.SelectedIndex = (int)num;
            }
            
        }

        private void Object_viewer_Load(object sender, EventArgs e)
        {
            foreach(RatchetMoby mob in DataStore.mobs)
            {
                string modelName = Main.modelNames != null ? Main.modelNames.Find(x => x.Substring(0, 4).ToUpper() == mob.modelID.ToString("X4")) : null;
                listBox1.Items.Add(DataStore.mobs.IndexOf(mob).ToString("X4") + ": " + (modelName != null ? modelName.Split('=')[1].Substring(1) : mob.modelID.ToString("X")));
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedMoby = DataStore.mobs[listBox1.SelectedIndex];
            DataStore.selectedMoby = selectedMoby;
            updateVals();

            missionBox.Text = selectedMoby.missionID.ToString("X8");
            if(mainForm.modelViewer.Visible)
            {
                mainForm.modelViewer.selectedModel = DataStore.spawnableModels.Find(x => x.modelID == selectedMoby.modelID);
                mainForm.modelViewer.inval();
            }
            DataStore.selectedMoby = selectedMoby;
            mainForm.terrainViewer.updateView();
        }

        private void updateVals()
        {
            listBox2.Items.Clear();
            modelIDBox.Text = selectedMoby.modelID.ToString("X");
            xNum.Value = (decimal)selectedMoby.x;
            yNum.Value = (decimal)selectedMoby.y;
            zNum.Value = (decimal)selectedMoby.z;
            rotXNum.Value = (decimal)selectedMoby.rot1;
            rotYNum.Value = (decimal)selectedMoby.rot2;
            rotZNum.Value = (decimal)selectedMoby.rot3;
            sizeNum.Value = (decimal)selectedMoby.size;
            dropNum.Value = selectedMoby.dropamnt;
            colorPanel.BackColor = Color.FromArgb((int)selectedMoby.r, (int)selectedMoby.g, (int)selectedMoby.b);
            numericUpDown1.Value = selectedMoby.test1;
            numericUpDown2.Value = (decimal)selectedMoby.z2;
            renderVal.Value = selectedMoby.rend1;
            cutsceneNum.Value = selectedMoby.cutScene;
            pVarBox.Text = selectedMoby.propIndex.ToString("X8");

            determinePVarControls(selectedMoby);

            for(int i = 0; i < selectedMoby.pVars.Length/4; i++)
            {
                uint index = (uint)i * 4;
                uint data = BAToUInt32(selectedMoby.pVars, index);
                listBox2.Items.Add(index.ToString("X4") + ": " + data.ToString("X8"));
            }
            
        }

        private void determinePVarControls(RatchetMoby moby)
        {
            mobyPVarGroup.Controls.Clear();
            int yOffset = 15;
            int xOffset = 5;
            if (moby.pVarConfig != null)
            {
                foreach (MobyPVar pVar in moby.pVarConfig)
                {
                    switch(pVar.control)
                    {
                        case "float":
                            FloatControl floatControl = new FloatControl(moby.pVars, pVar);
                            floatControl.Location = new Point(xOffset, yOffset);
                            floatControl.OnValueChanged += (s, e) => handleChange(pVar, e);
                            mobyPVarGroup.Controls.Add(floatControl);
                            yOffset += floatControl.Height;
                            break;
                        case "number":
                            IntegerControl intControl = new IntegerControl(moby.pVars, pVar);
                            intControl.Location = new Point(xOffset, yOffset);
                            intControl.OnValueChanged += (s, e) => handleChange(pVar, e);
                            mobyPVarGroup.Controls.Add(intControl);
                            yOffset += intControl.Height;
                            break;
                    }
                };
            }
        }

        private void handleChange(MobyPVar pVar, byte[] value)
        {
            writeBytes(selectedMoby.pVars, (int) pVar.index, value, value.Length);
        }

        private void updatePVar(int pvarIndex, uint value)
        {
            //selectedMoby.pVars[pvarIndex] = value;
        }

        private void xNum_ValueChanged(object sender, EventArgs e)
        {
            selectedMoby.x = (float)xNum.Value;
            mainForm.terrainViewer.updateView();
        }

        private void yNum_ValueChanged(object sender, EventArgs e)
        {
            selectedMoby.y = (float)yNum.Value;
            mainForm.terrainViewer.updateView();
        }

        private void zNum_ValueChanged(object sender, EventArgs e)
        {
            selectedMoby.z = (float)zNum.Value;
            mainForm.terrainViewer.updateView();
        }

        private void rotNum_ValueChanged(object sender, EventArgs e)
        {
            selectedMoby.rot1 = (float)rotXNum.Value;
            mainForm.terrainViewer.updateView();
        }

        private void sizeNum_ValueChanged(object sender, EventArgs e)
        {
            selectedMoby.size = (float)sizeNum.Value;
            mainForm.terrainViewer.updateView();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            mainForm.terrainViewer.setCamPos(selectedMoby.x, selectedMoby.y, selectedMoby.z + 5);
            mainForm.terrainViewer.updateView();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int mobElemSize = (int)DataStore.gameplayHeader.mobyElemSize;
            int mobCount = DataStore.mobs.Count();
            byte[] test = new byte[mobCount * mobElemSize];

            switch (DataStore.gameplayHeader.gameNum)
            {
                case 1:
                    for (int i = 0; i < mobCount; i++)
                    {
                        RatchetMoby mob = DataStore.mobs[i];
                        WriteUint32(ref test, (i * mobElemSize) + 0x00, mob.length);
                        WriteUint32(ref test, (i * mobElemSize) + 0x04, mob.missionID);
                        WriteUint32(ref test, (i * mobElemSize) + 0x08, mob.unk1);
                        WriteUint32(ref test, (i * mobElemSize) + 0x0C, mob.dataval);

                        WriteUint32(ref test, (i * mobElemSize) + 0x10, mob.dropamnt);
                        WriteUint32(ref test, (i * mobElemSize) + 0x14, mob.unk2);
                        WriteUint32(ref test, (i * mobElemSize) + 0x18, mob.modelID);
                        WriteFloat(ref test, (i * mobElemSize) + 0x1C, mob.size);

                        WriteUint32(ref test, (i * mobElemSize) + 0x20, mob.rend1);
                        WriteUint32(ref test, (i * mobElemSize) + 0x24, mob.rend2);
                        WriteUint32(ref test, (i * mobElemSize) + 0x28, mob.unk3);
                        WriteUint32(ref test, (i * mobElemSize) + 0x2C, mob.unk4);

                        WriteFloat(ref test, (i * mobElemSize) + 0x30, mob.x);
                        WriteFloat(ref test, (i * mobElemSize) + 0x34, mob.y);
                        WriteFloat(ref test, (i * mobElemSize) + 0x38, mob.z);
                        WriteFloat(ref test, (i * mobElemSize) + 0x3C, mob.rot1);

                        WriteFloat(ref test, (i * mobElemSize) + 0x40, mob.rot2);
                        WriteFloat(ref test, (i * mobElemSize) + 0x44, mob.rot3);
                        WriteUint32(ref test, (i * mobElemSize) + 0x48, mob.unk5);
                        WriteInt32(ref test, (i * mobElemSize) + 0x4C, mob.test1);

                        WriteFloat(ref test, (i * mobElemSize) + 0x50, mob.z2);
                        WriteUint32(ref test, (i * mobElemSize) + 0x54, mob.unk8);
                        WriteUint32(ref test, (i * mobElemSize) + 0x58, mob.propIndex);
                        WriteUint32(ref test, (i * mobElemSize) + 0x5C, mob.unk9);

                        WriteUint32(ref test, (i * mobElemSize) + 0x60, mob.unk10);
                        WriteUint32(ref test, (i * mobElemSize) + 0x64, mob.r);
                        WriteUint32(ref test, (i * mobElemSize) + 0x68, mob.g);
                        WriteUint32(ref test, (i * mobElemSize) + 0x6C, mob.b);

                        WriteUint32(ref test, (i * mobElemSize) + 0x70, mob.light);
                        WriteInt32(ref test, (i * mobElemSize) + 0x74, mob.cutScene);
                    }
                    break;

                case 2:
                case 3:
                    for (int i = 0; i < mobCount; i++)
                    {
                        RatchetMoby mob = DataStore.mobs[i];
                        WriteUint32(ref test, (i * mobElemSize) + 0x00, mob.length);
                        WriteUint32(ref test, (i * mobElemSize) + 0x04, mob.missionID);
                        WriteUint32(ref test, (i * mobElemSize) + 0x08, mob.unk1);
                        WriteUint32(ref test, (i * mobElemSize) + 0x0C, mob.dataval);

                        WriteUint32(ref test, (i * mobElemSize) + 0x10, mob.unk2);
                        WriteUint32(ref test, (i * mobElemSize) + 0x14, mob.dropamnt);
                        WriteUint32(ref test, (i * mobElemSize) + 0x18, mob.unk3);
                        WriteUint32(ref test, (i * mobElemSize) + 0x1C, mob.unk4);

                        WriteUint32(ref test, (i * mobElemSize) + 0x20, mob.unk5);
                        WriteUint32(ref test, (i * mobElemSize) + 0x24, mob.unk6);
                        WriteUint32(ref test, (i * mobElemSize) + 0x28, mob.modelID);
                        WriteFloat(ref test, (i * mobElemSize) + 0x2C, mob.size);

                        WriteUint32(ref test, (i * mobElemSize) + 0x30, mob.rend1);
                        WriteUint32(ref test, (i * mobElemSize) + 0x34, mob.rend2);
                        WriteUint32(ref test, (i * mobElemSize) + 0x38, mob.unk7);
                        WriteUint32(ref test, (i * mobElemSize) + 0x3C, mob.unk8);

                        WriteFloat(ref test, (i * mobElemSize) + 0x40, mob.x);
                        WriteFloat(ref test, (i * mobElemSize) + 0x44, mob.y);
                        WriteFloat(ref test, (i * mobElemSize) + 0x48, mob.z);
                        WriteFloat(ref test, (i * mobElemSize) + 0x4C, mob.rot1);

                        WriteFloat(ref test, (i * mobElemSize) + 0x50, mob.rot2);
                        WriteFloat(ref test, (i * mobElemSize) + 0x54, mob.rot3);
                        WriteUint32(ref test, (i * mobElemSize) + 0x58, mob.unk9);
                        WriteUint32(ref test, (i * mobElemSize) + 0x5C, mob.unk10);

                        WriteUint32(ref test, (i * mobElemSize) + 0x60, mob.unk11);
                        WriteUint32(ref test, (i * mobElemSize) + 0x64, mob.unk12);
                        WriteUint32(ref test, (i * mobElemSize) + 0x68, mob.propIndex);
                        WriteUint32(ref test, (i * mobElemSize) + 0x6C, mob.unk14);

                        WriteUint32(ref test, (i * mobElemSize) + 0x70, mob.unk15);
                        WriteUint32(ref test, (i * mobElemSize) + 0x74, mob.r);
                        WriteUint32(ref test, (i * mobElemSize) + 0x78, mob.g);
                        WriteUint32(ref test, (i * mobElemSize) + 0x7C, mob.b);

                        WriteUint32(ref test, (i * mobElemSize) + 0x80, mob.light);
                        WriteUint32(ref test, (i * mobElemSize) + 0x84, mob.unk16);
                    }
                    break;

                case 4:
                    for (int i = 0; i < mobCount; i++)
                    {
                        RatchetMoby mob = DataStore.mobs[i];
                        WriteUint32(ref test, (i * mobElemSize) + 0x00, mob.length);
                        WriteUint32(ref test, (i * mobElemSize) + 0x04, mob.missionID);
                        WriteUint32(ref test, (i * mobElemSize) + 0x08, mob.dataval);
                        WriteUint32(ref test, (i * mobElemSize) + 0x0C, mob.unk1);

                        WriteUint32(ref test, (i * mobElemSize) + 0x10, mob.modelID);
                        WriteFloat(ref test, (i * mobElemSize) + 0x14, mob.size);
                        WriteUint32(ref test, (i * mobElemSize) + 0x18, mob.rend1);
                        WriteUint32(ref test, (i * mobElemSize) + 0x1C, mob.rend2);

                        WriteUint32(ref test, (i * mobElemSize) + 0x20, mob.unk2);
                        WriteUint32(ref test, (i * mobElemSize) + 0x24, mob.unk3);
                        WriteFloat(ref test, (i * mobElemSize) + 0x28, mob.x);
                        WriteFloat(ref test, (i * mobElemSize) + 0x2C, mob.y);

                        WriteFloat(ref test, (i * mobElemSize) + 0x30, mob.z);
                        WriteFloat(ref test, (i * mobElemSize) + 0x34, mob.rot1);
                        WriteFloat(ref test, (i * mobElemSize) + 0x38, mob.rot2);
                        WriteFloat(ref test, (i * mobElemSize) + 0x3C, mob.rot3);

                        WriteUint32(ref test, (i * mobElemSize) + 0x40, mob.unk4);
                        WriteUint32(ref test, (i * mobElemSize) + 0x44, mob.unk5);
                        WriteUint32(ref test, (i * mobElemSize) + 0x48, mob.unk6);
                        WriteUint32(ref test, (i * mobElemSize) + 0x4C, mob.unk7);

                        WriteUint32(ref test, (i * mobElemSize) + 0x50, mob.propIndex);
                        WriteUint32(ref test, (i * mobElemSize) + 0x54, mob.unk8);
                        WriteUint32(ref test, (i * mobElemSize) + 0x58, mob.unk9);
                        WriteUint32(ref test, (i * mobElemSize) + 0x5C, mob.r);

                        WriteUint32(ref test, (i * mobElemSize) + 0x60, mob.g);
                        WriteUint32(ref test, (i * mobElemSize) + 0x64, mob.b);
                        WriteUint32(ref test, (i * mobElemSize) + 0x68, mob.light);
                        WriteUint32(ref test, (i * mobElemSize) + 0x6C, mob.unk14);
                    }
                    break;
            }
            

            FileStream gameplayFile = null;
            gameplayFile = File.OpenWrite(DataStore.workingDirectory + "/gameplay_ntsc");
            Console.WriteLine((DataStore.gameplayHeader.mobyPointer + 0x10).ToString("X8"));
            gameplayFile.Seek(DataStore.gameplayHeader.mobyPointer + 0x10, SeekOrigin.Begin);
            gameplayFile.Write(test, 0, mobCount * mobElemSize);
            gameplayFile.Close();
            Console.WriteLine("File written successfully, enjoy :)");
            
        }

        private void colorPanel_Click(object sender, EventArgs e)
        {
            if (colorPicker.ShowDialog() == DialogResult.OK)
            {
                colorPanel.BackColor = colorPicker.Color;
                selectedMoby.r = colorPicker.Color.R;
                selectedMoby.g = colorPicker.Color.G;
                selectedMoby.b = colorPicker.Color.B;
                mainForm.terrainViewer.updateView();
            }
        }

        private void rotYNum_ValueChanged(object sender, EventArgs e)
        {
            selectedMoby.rot2 = (float)rotYNum.Value;
            mainForm.terrainViewer.updateView();
        }

        private void rotZNum_ValueChanged(object sender, EventArgs e)
        {
            selectedMoby.rot3 = (float)rotZNum.Value;
            mainForm.terrainViewer.updateView();
        }

        private void dropNum_ValueChanged(object sender, EventArgs e)
        {
            selectedMoby.dropamnt = (uint)dropNum.Value;
            mainForm.terrainViewer.updateView();
        }
        private void numericUpDown1_ValueChanged_1(object sender, EventArgs e)
        {
            selectedMoby.test1 = (int)numericUpDown1.Value;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            selectedMoby.z2 = (float)numericUpDown2.Value;
        }

        private void renderVal_ValueChanged(object sender, EventArgs e)
        {
            selectedMoby.rend1 = (uint)renderVal.Value;
        }

        private void cutsceneNum_ValueChanged(object sender, EventArgs e)
        {
            selectedMoby.cutScene = (int)cutsceneNum.Value;
        }
    }
}
