using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using static RatchetModel;
using static ModelParser;
using static TextureParser;
using RatchetLevelEditor.Engine;

namespace RatchetLevelEditor
{
    public partial class ModelViewer : Form
    {
        int vertexArrayID;
        int[] texIDarr;

        int MatrixID;

        int pgmID;
        int vsID;
        int fsID;
        int VBO;
        int indexBufferID;

        float modelSize;

        bool ready = false;

        public RatchetModel_General selectedModel;

        Matrix4 mvp;

        public ModelViewer(Main main)
        {
            InitializeComponent();
        }
        public void inval()
        {
            if (selectedModel.vertBuff != null) //Check that there's actually vertex data to be rendered
            {
                modelDataList.Items.Clear();
                modelDataList.Items.Add("Faces: " + selectedModel.faceCount.ToString("X"));
                modelDataList.Items.Add("Vertices: " + selectedModel.vertexCount.ToString("X"));
                string texIDs = "";
                for (int i = 0; i < selectedModel.textureConfig.Count; i++)
                {
                    texIDs += " " + selectedModel.textureConfig[i].ID.ToString("X");
                }
                modelDataList.Items.Add("Textures: " + texIDs);
                modelDataList.Items.Add("Size: " + selectedModel.size.ToString());

                animListBox.Items.Clear();
                if (selectedModel.animPointer != null)
                {
                    for (int i = 0; i < selectedModel.animPointer.Count; i++)
                    {
                        animListBox.Items.Add(selectedModel.animPointer[i].ToString("X"));
                    }
                }
                modelSize = selectedModel.size;

                int vertCnt = selectedModel.vertBuff.Count;
                float[] vertArr = selectedModel.vertBuff.ToArray();
                GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
                GL.BufferData(BufferTarget.ArrayBuffer, vertCnt * sizeof(float), vertArr, BufferUsageHint.StaticDraw);

                int indCnt = selectedModel.indiceBuff.Count;
                UInt16[] indArr = selectedModel.indiceBuff.ToArray();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBufferID);
                GL.BufferData(BufferTarget.ElementArrayBuffer, indCnt * sizeof(UInt16), indArr, BufferUsageHint.StaticDraw);

                //Delete old textures, so we don't fill up the GPU memory
                if (texIDarr != null)
                {
                    GL.DeleteTextures(texIDarr.Count(), texIDarr);
                }

                //TODO make a method to only load textures once
                int texCount = selectedModel.textureConfig.Count;
                texIDarr = new int[texCount];
                GL.GenTextures(texCount, texIDarr);
                for (int i = 0; i < texCount; i++)
                {
                    Bitmap file = getTextureImage((int)selectedModel.textureConfig[i].ID);

                    if (file != null)
                    {
                        GL.BindTexture(TextureTarget.Texture2D, texIDarr[i]);

                        BitmapData data = file.LockBits(new System.Drawing.Rectangle(0, 0, file.Width, file.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                        file.UnlockBits(data);

                        //The DDS files have built in mipmaps, should probably use those instead
                        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                    }
                    else
                    {
                        Console.WriteLine("Error loading texture: File was null");
                    }
                }

                ready = true;
                glControl1.Invalidate();
            }
        }
        private void ModelViewer_Load(object sender, EventArgs e)
        {
            updateModelList();

            GL.ClearColor(Color.SkyBlue);

            pgmID = GL.CreateProgram();
            loadShader("shaders/vs.glsl", ShaderType.VertexShader, pgmID, out vsID);
            loadShader("shaders/fs.glsl", ShaderType.FragmentShader, pgmID, out fsID);
            GL.LinkProgram(pgmID);
            Console.WriteLine(GL.GetProgramInfoLog(pgmID));


            GL.GenVertexArrays(1, out vertexArrayID);
            GL.BindVertexArray(vertexArrayID);

            MatrixID = GL.GetUniformLocation(pgmID, "MVP");


            GL.Enable(EnableCap.DepthTest);
            GL.GenBuffers(1, out VBO);
            GL.GenBuffers(1, out indexBufferID);
        }

        void loadShader(String filename, ShaderType type, int program, out int address)
        {
            address = GL.CreateShader(type);
            using (StreamReader sr = new StreamReader(filename))
            {
                GL.ShaderSource(address, sr.ReadToEnd());
            }
            GL.CompileShader(address);
            GL.AttachShader(program, address);
            Console.WriteLine(GL.GetShaderInfoLog(address));
        }

        public void updateModelList()
        {
            TreeNode spawnableModels;
            TreeNode levelModels;
            TreeNode sceneryModels;
            TreeNode terrainModels;

            #region Spawnables
            if (DataStoreEngine.spawnableModels.Count > 0)
            {
                List<TreeNode> spawnableModelNodes = new List<TreeNode>();
                foreach (RatchetModel_General model in DataStoreEngine.spawnableModels)
                {

                    string modelName = Main.modelNames != null ? Main.modelNames.Find(x => x.Substring(0, 4).ToUpper() == model.modelID.ToString("X4")) : null;

                    spawnableModelNodes.Add(new TreeNode()
                    {
                        Text = modelName != null ? modelName.Split('=')[1].Substring(1) : model.modelID.ToString("X"),
                        Tag = model.modelType,
                        Name = "Spawnable",
                        ForeColor = model.vertBuff != null ? Color.Black : Color.Red
                    }
                    );
                };

                spawnableModels = new TreeNode("Spawnable", spawnableModelNodes.ToArray());
                modelsListTree.Nodes.Add(spawnableModels);
            }
            #endregion

            #region Level Models
            if(DataStoreEngine.levelModels.Count > 0)
            {
                List<TreeNode> levelModelNodes = new List<TreeNode>();
                foreach (RatchetModel_General model in DataStoreEngine.levelModels)
                {
                    levelModelNodes.Add(new TreeNode()
                    {
                        Text = model.modelID.ToString("X"),
                        Tag = ModelType.Level,

                    }
                    );
                };

                levelModels = new TreeNode("Level", levelModelNodes.ToArray());
                modelsListTree.Nodes.Add(levelModels);
            }
            #endregion

            #region Scenery Models
            if(DataStoreEngine.sceneryModels.Count > 0)
            {
                List<TreeNode> sceneryModelNodes = new List<TreeNode>();
                foreach (RatchetModel_General model in DataStoreEngine.sceneryModels)
                {
                    sceneryModelNodes.Add(new TreeNode()
                    {
                        Text = model.modelID.ToString("X"),
                        Tag = model.modelType
                    }
                    );
                };

                sceneryModels = new TreeNode("Scenery", sceneryModelNodes.ToArray());
                modelsListTree.Nodes.Add(sceneryModels);
            }
            #endregion

            #region Terrain and Terrain Collision
            if(!DataStoreEngine.terrainModel.Equals(null) || !DataStoreEngine.terrainCollisionModel.Equals(null))
            {
                terrainModels = new TreeNode("Terrain");
                
                if(!DataStoreEngine.terrainModel.Equals(null))
                {
                    TreeNode terrainMesh = new TreeNode()
                    {
                        Text = "Terrain Mesh",
                        Tag = DataStoreEngine.terrainModel.modelType,
                        Name = "TerrainMesh",
                    };
                    terrainModels.Nodes.Add(terrainMesh);
                };

                if (!DataStoreEngine.terrainCollisionModel.Equals(null))
                {
                    TreeNode terrainCollision = new TreeNode()
                    {
                        Text = "Terrain Collision"
                    };
                    terrainModels.Nodes.Add(terrainCollision);
                };

                if (DataStoreEngine.chunks.Count > 0)
                {
                    foreach (RatchetModel_Terrain chunk in DataStoreEngine.chunks)
                    {
                        TreeNode terrainMesh = new TreeNode()
                        {
                            Text = "Chunk " + DataStoreEngine.chunks.IndexOf(chunk)
                        };
                        terrainModels.Nodes.Add(terrainMesh);
                    }
                };

                modelsListTree.Nodes.Add(terrainModels);
            }
            #endregion

            #region Missions
            if (DataStoreGlobal.missions.Count > 0)
            {
                int missionIndex = 0;
                foreach (RatchetMission mission in DataStoreGlobal.missions)
                {
                    List<TreeNode> spawnableModelNodes = new List<TreeNode>();
                    foreach (RatchetModel_General model in mission.spawnableModels)
                    {

                        string modelName = Main.modelNames != null ? Main.modelNames.Find(x => x.Substring(0, 4).ToUpper() == model.modelID.ToString("X4")) : null;

                        spawnableModelNodes.Add(new TreeNode()
                        {
                            Text = modelName != null ? modelName.Split('=')[1].Substring(1) : model.modelID.ToString("X"),
                            Tag = model.modelType,
                            Name = "Mission_" + missionIndex,
                            ForeColor = model.vertBuff != null ? Color.Black : Color.Red
                        }
                        );
                    };

                    spawnableModels = new TreeNode("Mission_" + missionIndex, spawnableModelNodes.ToArray());
                    modelsListTree.Nodes.Add(spawnableModels);
                    missionIndex++;
                }
            }
            #endregion
        }

        private void modelsListTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Level == 1 && e.Node.Tag != null)
            { //Check that we're actually on a model
                switch ((ModelType)e.Node.Tag)
                {
                    case ModelType.Spawnable:
                        if(e.Node.Name.IndexOf("Mission_") != -1)
                            selectedModel = DataStoreGlobal.missions[int.Parse(e.Node.Name.Substring(8))].spawnableModels[e.Node.Index];
                        else
                            selectedModel = DataStoreEngine.spawnableModels[e.Node.Index];
                        break;
                    case ModelType.Level:
                        selectedModel = DataStoreEngine.levelModels[e.Node.Index];
                        break;
                    case ModelType.Scenery:
                        selectedModel = DataStoreEngine.sceneryModels[e.Node.Index];
                        break;
                    case ModelType.Terrain:
                        RatchetModel_Terrain terrModel = DataStoreEngine.terrainModel;
                        modelDataList.Items.Clear();
                        modelDataList.Items.Add("RENDERING DISABLED");
                        modelDataList.Items.Add("Faces: " + terrModel.indiceBuff.Count.ToString("X"));
                        modelDataList.Items.Add("Vertices: " + terrModel.vertBuff.Count.ToString("X"));
                        break;
                }
                inval();
            }
            else
            {
                modelDataList.Items.Clear();
            }
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (ready)
            {
                 glControl1.MakeCurrent();
                Matrix4 scale = Matrix4.CreateScale(modelSize);
                Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, glControl1.Width / (float)glControl1.Height, 0.1f, 40.0f);
                Matrix4 rot = Matrix4.CreateRotationZ((float)rotBar.Value / 100);
                Matrix4 trans = Matrix4.CreateTranslation(0.0f, 0.0f, -5.0f);
                Matrix4 View = Matrix4.LookAt(new Vector3(10 - ((float)zoomBar.Value / 10), 10-((float)zoomBar.Value/10), 10 - ((float)zoomBar.Value / 10)), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f));
                mvp =  trans * scale * rot * View * projection;  //Has to be done in this order to work correctly

                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                GL.UseProgram(pgmID);
                GL.UniformMatrix4(MatrixID, false, ref mvp);

                GL.EnableVertexAttribArray(0);
                GL.EnableVertexAttribArray(1);

                GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBufferID);

                //Verts
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 8, 0);
                //UV's
                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, sizeof(float) * 8, sizeof(float) * 6);


                //Bind textures one by one, applying it to the relevant vertices based on the index array
                for(int i = 0; i < selectedModel.textureConfig.Count(); i++)
                {
                    GL.BindTexture(TextureTarget.Texture2D, texIDarr[i]);
                    GL.DrawElements(PrimitiveType.Triangles, (int)selectedModel.textureConfig[i].size, DrawElementsType.UnsignedShort, (int)selectedModel.textureConfig[i].start * sizeof(ushort));
                }

                //GL.DrawElements(PrimitiveType.Triangles, indCnt, DrawElementsType.UnsignedShort, 0);

                GL.DisableVertexAttribArray(1);
                GL.DisableVertexAttribArray(0);

                GL.Flush();
                glControl1.SwapBuffers();
            }
        }

        private void zoomBar_Scroll(object sender, EventArgs e)
        {
            //Force the control to repaint
            glControl1.Invalidate();
        }

        private void rotBar_Scroll(object sender, EventArgs e)
        {
            //Force the control to repaint
            glControl1.Invalidate();
        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, glControl1.Width, glControl1.Height);
        }

        private void exportBtn_Click(object sender, EventArgs e)
        {
            if (objSaveDialog.ShowDialog() == DialogResult.OK)
            {
                string OBJfilename = objSaveDialog.FileName;
                string MTLfilename = objSaveDialog.FileName.Split('.')[0] + ".mtl";

                if (modelsListTree.SelectedNode.Level == 1)
                {
                    if(modelsListTree.SelectedNode.Parent.Index == 0)       //Level model
                    {
                        RatchetModel_General selectedModel = DataStoreEngine.spawnableModels[modelsListTree.SelectedNode.Index];
                        SpawnableToObj(ref selectedModel, OBJfilename, MTLfilename);
                        Console.WriteLine("OBJ file created: " + OBJfilename);
                        Console.WriteLine("MTL file created: " + MTLfilename);
                    }
                    else if (modelsListTree.SelectedNode.Parent.Index == 3)    //Terrain
                    {
                        RatchetModel_Terrain model = DataStoreEngine.terrainModel;
                        terrainMeshToObj(ref model, OBJfilename, MTLfilename);
                        Console.WriteLine("OBJ file created: " + OBJfilename);
                        Console.WriteLine("MTL file created: " + MTLfilename);
                    }
                    else
                    {
                        MessageBox.Show("Not currently supported");
                    }
                }

            }
        }
    }
}
