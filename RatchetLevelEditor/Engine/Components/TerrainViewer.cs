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
using OpenTK.Graphics.OpenGL;
using static RatchetModel;
using static TextureParser;
using System.Drawing.Imaging;
using System.IO;
using static RatchetTexture;

namespace RatchetLevelEditor
{
    public partial class TerrainViewer : Form
    {
        Main mainForm;

        Matrix4 projection;
        int vertexArrayID;
        int[] texIDarr;

        int MatrixID;

        int pgmID;
        int pgm2ID;
        int colorOnlyShaderID;
        int vsID;
        int fsID;
        int pgmID_rcID;
        int VBO;
        int indexBufferID;

        int indCnt;

        bool wKey = false;
        bool sKey = false;
        bool dKey = false;
        bool aKey = false;
        bool qKey = false;
        bool eKey = false;
        bool rMouse = false;
        bool lMouse = false;

        float cSpeed = 0.2f;
        float pitch = 1.59f;
        float yaw = 0f;
        Vector3 cPosition;
        Vector3 cTarget;

        int lastMouseX = 0;
        int lastMouseY = 0;

        List<uint> texList;

        bool ready = false;

        public RatchetModel_Terrain selectedModel;
        public RatchetModel_General objModel;

        Matrix4 mvp;

        List<uint> modTexList;
        List<uint> mobList;
        int[] mobVerts;
        int[] mobInds;
        int[] mobTexIDArr;
        bool modReady = false;

        List<uint> levelTexList;
        List<uint> levelList;
        int[] levelVerts;
        int[] levelInds;
        int[] levelbTexIDArr;
        bool levelReady = false;

        List<uint> sceneryTexList;
        List<uint> sceneryList;
        int[] sceneryVerts;
        int[] sceneryInds;
        int[] sceneryTexIDArr;
        bool sceneryReady = false;

        List<uint> chunkTexList;
        List<uint> chunkList;
        int[] chunkVerts;
        int[] chunkInds;
        int[] chunkTexIDArr;
        bool chunksReady = false;


        int collVBO;
        int collInd;
        bool collReady = false;



        public TerrainViewer(Main main)
        {
            InitializeComponent();
            mainForm = main;
        }

        struct Byte4
        {
            public byte R, G, B, A;

            public Byte4(byte[] input)
            {
                R = input[0];
                G = input[1];
                B = input[2];
                A = input[3];
            }

            public uint ToUInt32()
            {
                byte[] temp = new byte[] { this.R, this.G, this.B, this.A };
                return BitConverter.ToUInt32(temp, 0);
            }

            public override string ToString()
            {
                return this.R + ", " + this.G + ", " + this.B + ", " + this.A;
            }
        }

        private void TerrainViewer_Load(object sender, EventArgs e)
        {
            projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, glControl1.Width / (float)glControl1.Height, 0.1f, 800.0f);
            GL.ClearColor(Color.SkyBlue);

            pgmID = GL.CreateProgram();
            loadShader("shaders/vs_colored.glsl", ShaderType.VertexShader, pgmID, out vsID);
            loadShader("shaders/fs_colored.glsl", ShaderType.FragmentShader, pgmID, out fsID);
            GL.LinkProgram(pgmID);
            //Console.WriteLine(GL.GetProgramInfoLog(pgmID));

            pgm2ID = GL.CreateProgram();
            loadShader("shaders/vs.glsl", ShaderType.VertexShader, pgm2ID, out vsID);
            loadShader("shaders/fs.glsl", ShaderType.FragmentShader, pgm2ID, out fsID);
            GL.LinkProgram(pgm2ID);
            //Console.WriteLine(GL.GetProgramInfoLog(pgm2ID));

            colorOnlyShaderID = GL.CreateProgram();
            loadShader("shaders/vs_color_only.glsl", ShaderType.VertexShader, colorOnlyShaderID, out vsID);
            loadShader("shaders/fs_color_only.glsl", ShaderType.FragmentShader, colorOnlyShaderID, out fsID);
            GL.LinkProgram(colorOnlyShaderID);
            //Console.WriteLine(GL.GetProgramInfoLog(colorOnlyShaderID));


            GL.GenVertexArrays(1, out vertexArrayID);
            GL.BindVertexArray(vertexArrayID);

            MatrixID = GL.GetUniformLocation(pgmID, "MVP");
            pgmID_rcID = GL.GetUniformLocation(pgmID, "replacementColor");

            GL.Enable(EnableCap.DepthTest);
            GL.LineWidth(1.0f);

            GL.GenBuffers(1, out VBO);
            GL.GenBuffers(1, out indexBufferID);


            selectedModel = DataStore.terrainModel;
            if (selectedModel.vertBuff != null) //Check that there's actually vertex data to be rendered
            {
                int vertCnt = selectedModel.vertBuff.Count;
                float[] vertArr = selectedModel.vertBuff.ToArray();
                GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
                GL.BufferData(BufferTarget.ArrayBuffer, vertCnt * sizeof(float), vertArr, BufferUsageHint.StaticDraw);

                indCnt = selectedModel.indiceBuff.Count;
                uint[] indArr = selectedModel.indiceBuff.ToArray();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBufferID);
                GL.BufferData(BufferTarget.ElementArrayBuffer, indCnt * sizeof(uint), indArr, BufferUsageHint.StaticDraw);


                //Delete old textures, so we don't fill up the GPU memory
                if (texIDarr != null)
                {
                    GL.DeleteTextures(texIDarr.Count(), texIDarr);
                }

                texList = new List<uint>();
                foreach (RatchetTexture_Model tex in selectedModel.textureConfig)
                {
                    if (!texList.Contains(tex.ID)) texList.Add(tex.ID);

                }

                int texCount = texList.Count;
                texIDarr = new int[texCount];
                GL.GenTextures(texCount, texIDarr);
                for (int i = 0; i < texCount; i++)
                {
                    Bitmap file = getTextureImage((int)texList[i]);

                    if (file != null)
                    {
                        GL.BindTexture(TextureTarget.Texture2D, texIDarr[i]);

                        BitmapData data = file.LockBits(new Rectangle(0, 0, file.Width, file.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
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
        void loadShader(String filename, ShaderType type, int program, out int address)
        {
            address = GL.CreateShader(type);
            using (StreamReader sr = new StreamReader(filename))
            {
                GL.ShaderSource(address, sr.ReadToEnd());
            }
            GL.CompileShader(address);
            GL.AttachShader(program, address);
            //Console.WriteLine(GL.GetShaderInfoLog(address));
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(Color.SkyBlue);
            glControl1.MakeCurrent();
            GL.UseProgram(pgm2ID);

            GL.EnableVertexAttribArray(0);
            

            camX.Text = cPosition.X.ToString();
            camY.Text = cPosition.Y.ToString();
            camZ.Text = cPosition.Z.ToString();

            Matrix4 View = Matrix4.LookAt(cPosition, cTarget, Vector3.UnitZ);
            Matrix4 worldView = View * projection;

            if (splineCheck.Checked)
            {
                GL.UniformMatrix4(MatrixID, false, ref worldView);
                foreach (Spline spline in DataStore.splines)
                {
                    spline.getVBO();
                    GL.DrawArrays(PrimitiveType.LineStrip, 0, spline.vertexBuffer.Length / 3);
                }
            }


            GL.EnableVertexAttribArray(1);

            if (modReady && mobyCheck.Checked == true)
            {
                foreach (RatchetMoby mob in DataStore.mobs)
                {
                    objModel = DataStore.spawnableModels.Find(x => x.modelID == mob.modelID);
                    int insx = mobList.IndexOf((uint)objModel.modelID);
                    if (insx != -1)
                    {
                        Matrix4 modTrans = Matrix4.CreateTranslation(mob.x, mob.y, mob.z);
                        Matrix4 modScale = Matrix4.CreateScale(objModel.size * mob.size);
                        Matrix4 xRot = Matrix4.CreateRotationX(mob.rot1);
                        Matrix4 yRot = Matrix4.CreateRotationY(mob.rot2);
                        Matrix4 zRot = Matrix4.CreateRotationZ(mob.rot3);
                        Matrix4 modRot = xRot * yRot * zRot;
                        mvp = modScale * modRot * modTrans * worldView;  //Has to be done in this order to work correctly
                        GL.UniformMatrix4(MatrixID, false, ref mvp);

                        GL.BindBuffer(BufferTarget.ArrayBuffer, mobVerts[insx]);
                        GL.BindBuffer(BufferTarget.ElementArrayBuffer, mobInds[insx]);

                        //Verts
                        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 8, 0);
                        //UV's
                        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, sizeof(float) * 8, sizeof(float) * 6);


                        //Bind textures one by one, applying it to the relevant vertices based on the index array
                        foreach (RatchetTexture_Model tex in objModel.textureConfig)
                        {
                            int indx = modTexList.IndexOf(tex.ID);
                            GL.BindTexture(TextureTarget.Texture2D, mobTexIDArr[indx]);

                            if (DataStore.selectedMoby == mob)
                            {
                                GL.Uniform4(pgmID_rcID, new Vector4(1, 1, 1, 1));
                                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                                GL.DrawElements(PrimitiveType.Triangles, (int)tex.size, DrawElementsType.UnsignedShort, (int)tex.start * sizeof(ushort));

                                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                            }
                            GL.Uniform4(pgmID_rcID, new Vector4(mob.r / 255f, mob.g / 255f, mob.b / 255f, 0.5f));
                            GL.DrawElements(PrimitiveType.Triangles, (int)tex.size, DrawElementsType.UnsignedShort, (int)tex.start * sizeof(ushort));
                        }
                    }
                }
            }
            if (levelReady && levelCheck.Checked == true)
            {
                foreach (LevelObject levObj in DataStore.levelObjects)
                {
                    objModel = DataStore.levelModels.Find(x => x.modelID == levObj.modelID);
                    int insx = levelList.IndexOf((uint)objModel.modelID);
                    if (insx != -1)
                    {
                        mvp = levObj.mat * worldView;  //Has to be done in this order to work correctly
                        GL.UniformMatrix4(MatrixID, false, ref mvp);

                        GL.BindBuffer(BufferTarget.ArrayBuffer, levelVerts[insx]);
                        GL.BindBuffer(BufferTarget.ElementArrayBuffer, levelInds[insx]);

                        //Verts
                        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 8, 0);
                        //UV's
                        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, sizeof(float) * 8, sizeof(float) * 6);


                        //Bind textures one by one, applying it to the relevant vertices based on the index array
                        foreach (RatchetTexture_Model tex in objModel.textureConfig)
                        {
                            int indx = levelTexList.IndexOf(tex.ID);
                            GL.BindTexture(TextureTarget.Texture2D, levelbTexIDArr[indx]);

                            if (DataStore.selectedLevelObject == levObj)
                            {

                                GL.Uniform4(pgmID_rcID, new Vector4(1, 1, 1, 1));
                                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                                GL.DrawElements(PrimitiveType.Triangles, (int)tex.size, DrawElementsType.UnsignedShort, (int)tex.start * sizeof(ushort));
                                GL.Uniform4(pgmID_rcID, new Vector4(0, 0, 0, 0));
                                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                            }


                            GL.DrawElements(PrimitiveType.Triangles, (int)tex.size, DrawElementsType.UnsignedShort, (int)tex.start * sizeof(ushort));
                        }
                    }
                }
            }

            if (sceneryReady && sceneryCheck.Checked == true)
            {
                foreach (LevelObject sceneryObj in DataStore.sceneryObjects)
                {
                    objModel = DataStore.sceneryModels.Find(x => x.modelID == sceneryObj.modelID);

                    int insx = sceneryList.IndexOf((uint)objModel.modelID);
                    if (insx != -1)
                    {
                        mvp = sceneryObj.mat * worldView;  //Has to be done in this order to work correctly
                        GL.UniformMatrix4(MatrixID, false, ref mvp);


                        GL.BindBuffer(BufferTarget.ArrayBuffer, sceneryVerts[insx]);
                        GL.BindBuffer(BufferTarget.ElementArrayBuffer, sceneryInds[insx]);

                        //Verts
                        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 8, 0);
                        //UV's
                        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, sizeof(float) * 8, sizeof(float) * 6);


                        //Bind textures one by one, applying it to the relevant vertices based on the index array
                        foreach (RatchetTexture_Model tex in objModel.textureConfig)
                        {
                            int indx = sceneryTexList.IndexOf(tex.ID);
                            GL.BindTexture(TextureTarget.Texture2D, sceneryTexIDArr[indx]);
                            GL.DrawElements(PrimitiveType.Triangles, (int)tex.size, DrawElementsType.UnsignedShort, (int)tex.start * sizeof(ushort));
                        }
                    }
                }
            }

            if (collReady && collCheck.Checked)
            {
                GL.UseProgram(colorOnlyShaderID);
                mvp = worldView;

                GL.UniformMatrix4(MatrixID, false, ref mvp);

                GL.BindBuffer(BufferTarget.ArrayBuffer, collVBO);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, collInd);

                //Verts
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 6, 0);
                //Colors
                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, sizeof(float) * 6, sizeof(float) * 3);

                GL.UseProgram(pgmID);
                GL.UniformMatrix4(MatrixID, false, ref mvp);
                GL.Uniform4(pgmID_rcID, new Vector4(1, 1, 1, 1));
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                GL.DrawElements(PrimitiveType.Triangles, DataStore.collIndBuff.Count, DrawElementsType.UnsignedInt, 0);

                GL.UseProgram(colorOnlyShaderID);
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                GL.DrawElements(PrimitiveType.Triangles, DataStore.collIndBuff.Count, DrawElementsType.UnsignedInt, 0);
            }

            if (ready && terrainCheck.Checked)
            {
                GL.UseProgram(pgmID);
                mvp = worldView;

                GL.UniformMatrix4(MatrixID, false, ref mvp);

                GL.EnableVertexAttribArray(2);

                GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBufferID);

                //Verts
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 12, 0);
                //UV's
                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, sizeof(float) * 12, sizeof(float) * 6);
                //Colors
                GL.VertexAttribPointer(2, 4, VertexAttribPointerType.Float, false, sizeof(float) * 12, sizeof(float) * 8);


                //Bind textures one by one, applying it to the relevant vertices based on the index array
                foreach (RatchetTexture_Model tex in selectedModel.textureConfig)
                {
                    int indx = texList.IndexOf(tex.ID);
                    GL.BindTexture(TextureTarget.Texture2D, texIDarr[indx]);

                    if (false)
                    {
                        GL.Uniform4(pgmID_rcID, new Vector4(1, 1, 1, 1));
                        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                        GL.DrawElements(PrimitiveType.Triangles, (int)tex.size, DrawElementsType.UnsignedInt, (int)tex.start * sizeof(uint));
                    }

                    GL.Uniform4(pgmID_rcID, new Vector4(0, 0, 0, 0));
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                    GL.DrawElements(PrimitiveType.Triangles, (int)tex.size, DrawElementsType.UnsignedInt, (int)tex.start * sizeof(uint));
                }

                GL.DisableVertexAttribArray(2);
            }


            if (chunksReady && chunkCheck.Checked)
            {
                for (int i = 0; i < DataStore.chunks.Count; i++)
                {
                    RatchetModel_Terrain chunk = DataStore.chunks[i];

                    GL.UseProgram(pgmID);
                    mvp = worldView;

                    GL.UniformMatrix4(MatrixID, false, ref mvp);

                    GL.EnableVertexAttribArray(2);

                    GL.BindBuffer(BufferTarget.ArrayBuffer, chunkVerts[i]);
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, chunkInds[i]);

                    //Verts
                    GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 12, 0);
                    //UV's
                    GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, sizeof(float) * 12, sizeof(float) * 6);
                    //Colors
                    GL.VertexAttribPointer(2, 4, VertexAttribPointerType.Float, false, sizeof(float) * 12, sizeof(float) * 8);


                    //Bind textures one by one, applying it to the relevant vertices based on the index array
                    foreach (RatchetTexture_Model tex in chunk.textureConfig)
                    {
                        int indx = chunkTexList.IndexOf(tex.ID);
                        GL.BindTexture(TextureTarget.Texture2D, chunkTexIDArr[indx]);

                        GL.Uniform4(pgmID_rcID, new Vector4(0, 0, 0, 0));
                        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                        GL.DrawElements(PrimitiveType.Triangles, (int)tex.size, DrawElementsType.UnsignedInt, (int)tex.start * sizeof(uint));
                    }

                    GL.DisableVertexAttribArray(2);
                }
            }


            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(0);
            glControl1.SwapBuffers();
        }




        private void glControl1_Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, glControl1.Width, glControl1.Height);
            projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, glControl1.Width / (float)glControl1.Height, 0.1f, 800.0f);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            mobList = new List<uint>();
            modTexList = new List<uint>();

            foreach (RatchetMoby mob in DataStore.mobs)
            {

                if (!mobList.Contains(mob.modelID))
                {
                    objModel = DataStore.spawnableModels.Find(x => x.modelID == mob.modelID);
                    if (objModel.vertBuff != null)
                    {
                        mobList.Add(mob.modelID);
                    }
                }
            }
            int mobCount = mobList.Count();
            mobVerts = new int[mobCount];
            mobInds = new int[mobCount];

            GL.GenBuffers(mobCount, mobVerts);
            GL.GenBuffers(mobCount, mobInds);

            for (int i = 0; i < mobList.Count(); i++)
            {
                objModel = DataStore.spawnableModels.Find(x => x.modelID == mobList[i]);
                if (objModel.vertBuff != null)
                {
                    int vertCnt = objModel.vertBuff.Count;
                    float[] vertArr = objModel.vertBuff.ToArray();
                    GL.BindBuffer(BufferTarget.ArrayBuffer, mobVerts[i]);
                    GL.BufferData(BufferTarget.ArrayBuffer, vertCnt * sizeof(float), vertArr, BufferUsageHint.StaticDraw);

                    int indCnt = objModel.indiceBuff.Count;
                    UInt16[] indArr = objModel.indiceBuff.ToArray();
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, mobInds[i]);
                    GL.BufferData(BufferTarget.ElementArrayBuffer, indCnt * sizeof(UInt16), indArr, BufferUsageHint.StaticDraw);

                    foreach (RatchetTexture_Model tex in objModel.textureConfig)
                    {
                        if (!modTexList.Contains(tex.ID)) modTexList.Add(tex.ID);
                    }
                }
            }

            int texCount = modTexList.Count;
            mobTexIDArr = new int[texCount];
            GL.GenTextures(texCount, mobTexIDArr);
            for (int ii = 0; ii < texCount; ii++)
            {
                Bitmap file = getTextureImage((int)modTexList[ii]);

                if (file != null)
                {
                    GL.BindTexture(TextureTarget.Texture2D, mobTexIDArr[ii]);

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
            mobyCheck.Checked = true;
            modReady = true;
            glControl1.Invalidate();
        }

        private void mobyCheck_CheckedChanged(object sender, EventArgs e)
        {
            glControl1.Invalidate();
        }
        public void updateView()
        {
            glControl1.Invalidate();
        }

        private void levelButton_Click(object sender, EventArgs e)
        {
            levelList = new List<uint>();
            levelTexList = new List<uint>();

            foreach (LevelObject levObj in DataStore.levelObjects)
            {
                if (!levelList.Contains(levObj.modelID))
                {
                    objModel = DataStore.levelModels.Find(x => x.modelID == levObj.modelID);
                    if (objModel.vertBuff != null)
                    {
                        levelList.Add(levObj.modelID);
                    }
                }
            }
            int levCount = levelList.Count();
            levelVerts = new int[levCount];
            levelInds = new int[levCount];

            GL.GenBuffers(levCount, levelVerts);
            GL.GenBuffers(levCount, levelInds);

            for (int i = 0; i < levelList.Count(); i++)
            {
                objModel = DataStore.levelModels.Find(x => x.modelID == levelList[i]);
                if (objModel.vertBuff != null)
                {
                    int vertCnt = objModel.vertBuff.Count;
                    float[] vertArr = objModel.vertBuff.ToArray();
                    GL.BindBuffer(BufferTarget.ArrayBuffer, levelVerts[i]);
                    GL.BufferData(BufferTarget.ArrayBuffer, vertCnt * sizeof(float), vertArr, BufferUsageHint.StaticDraw);

                    int indCnt = objModel.indiceBuff.Count;
                    UInt16[] indArr = objModel.indiceBuff.ToArray();
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, levelInds[i]);
                    GL.BufferData(BufferTarget.ElementArrayBuffer, indCnt * sizeof(UInt16), indArr, BufferUsageHint.StaticDraw);

                    foreach (RatchetTexture_Model tex in objModel.textureConfig)
                    {
                        if (!levelTexList.Contains(tex.ID)) levelTexList.Add(tex.ID);
                    }
                }
            }

            int texCount = levelTexList.Count;
            levelbTexIDArr = new int[texCount];
            GL.GenTextures(texCount, levelbTexIDArr);
            for (int ii = 0; ii < texCount; ii++)
            {
                Bitmap file = getTextureImage((int)levelTexList[ii]);

                if (file != null)
                {
                    GL.BindTexture(TextureTarget.Texture2D, levelbTexIDArr[ii]);

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
            levelCheck.Checked = true;
            levelReady = true;
            glControl1.Invalidate();
        }
        private void sceneryButton_Click(object sender, EventArgs e)
        {
            sceneryList = new List<uint>();
            sceneryTexList = new List<uint>();
            foreach (LevelObject levObj in DataStore.sceneryObjects)
            {
                if (!sceneryList.Contains(levObj.modelID))
                {

                    objModel = DataStore.sceneryModels.Find(x => x.modelID == levObj.modelID);

                    if (objModel.vertBuff != null)
                    {
                        sceneryList.Add(levObj.modelID);
                    }
                }
            }
            int levCount = sceneryList.Count();
            sceneryVerts = new int[levCount];
            sceneryInds = new int[levCount];

            GL.GenBuffers(levCount, sceneryVerts);
            GL.GenBuffers(levCount, sceneryInds);

            for (int i = 0; i < sceneryList.Count(); i++)
            {
                objModel = DataStore.sceneryModels.Find(x => x.modelID == sceneryList[i]);
                if (objModel.vertBuff != null)
                {

                    int vertCnt = objModel.vertBuff.Count;
                    float[] vertArr = objModel.vertBuff.ToArray();
                    GL.BindBuffer(BufferTarget.ArrayBuffer, sceneryVerts[i]);
                    GL.BufferData(BufferTarget.ArrayBuffer, vertCnt * sizeof(float), vertArr, BufferUsageHint.StaticDraw);

                    int indCnt = objModel.indiceBuff.Count;
                    UInt16[] indArr = objModel.indiceBuff.ToArray();
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, sceneryInds[i]);
                    GL.BufferData(BufferTarget.ElementArrayBuffer, indCnt * sizeof(UInt16), indArr, BufferUsageHint.StaticDraw);

                    foreach (RatchetTexture_Model tex in objModel.textureConfig)
                    {
                        if (!sceneryTexList.Contains(tex.ID)) sceneryTexList.Add(tex.ID);
                    }
                }
            }

            int texCount = sceneryTexList.Count;
            sceneryTexIDArr = new int[texCount];
            GL.GenTextures(texCount, sceneryTexIDArr);
            for (int ii = 0; ii < texCount; ii++)
            {
                Bitmap file = getTextureImage((int)sceneryTexList[ii]);

                if (file != null)
                {
                    GL.BindTexture(TextureTarget.Texture2D, sceneryTexIDArr[ii]);

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
            sceneryCheck.Checked = true;
            sceneryReady = true;
            glControl1.Invalidate();
        }
        public void setCamPos(float x, float y, float z)
        {
            cPosition = new Vector3(x, y, z);
        }


        #region Input

        private void glConstrol1_ScrollWheel(object sender, MouseEventArgs e)
        {

        }

        private void tickTimer_Tick(object sender, EventArgs e)
        {
            Vector3 moveDir = Vector3.Zero;
            float deltaTime = 0.016f;
            bool invalidate = false;
            float moveSpeed = 25;
            float shiftMultiplier = 3;
            float nonShiftMultiplier = 0.5f;

            if (wKey)
            {
                moveDir.Y += (ModifierKeys == Keys.Shift ? shiftMultiplier : nonShiftMultiplier) * deltaTime * moveSpeed;
                invalidate = true;
            }
            if (sKey)
            {
                moveDir.Y -= (ModifierKeys == Keys.Shift ? shiftMultiplier : nonShiftMultiplier) * deltaTime * moveSpeed;
                invalidate = true;
            }
            if (aKey)
            {
                moveDir.X -= (ModifierKeys == Keys.Shift ? shiftMultiplier : nonShiftMultiplier) * deltaTime * moveSpeed;
                invalidate = true;
            }
            if (dKey)
            {
                moveDir.X += (ModifierKeys == Keys.Shift ? shiftMultiplier : nonShiftMultiplier) * deltaTime * moveSpeed;
                invalidate = true;
            }
            if (eKey)
            {
                moveDir.Z += (ModifierKeys == Keys.Shift ? shiftMultiplier : nonShiftMultiplier) * deltaTime * moveSpeed;
                invalidate = true;
            }
            if (qKey)
            {
                moveDir.Z -= (ModifierKeys == Keys.Shift ? shiftMultiplier : nonShiftMultiplier) * deltaTime * moveSpeed;
                invalidate = true;
            }
            if (rMouse)
            {
                yaw -= (Cursor.Position.X - lastMouseX) * cSpeed * 0.016f;
                pitch -= (Cursor.Position.Y - lastMouseY) * cSpeed * 0.016f;
                pitch = MathHelper.Clamp(pitch, MathHelper.DegreesToRadians(-89.9f), MathHelper.DegreesToRadians(89.9f));

                invalidate = true;
            }

            Matrix3 rot = Matrix3.CreateRotationX(pitch) * Matrix3.CreateRotationZ(yaw);
            Vector3 forward = Vector3.Transform(Vector3.UnitY, rot);

            cPosition += Vector3.Transform(moveDir, rot);
            cTarget = cPosition + forward;


            lastMouseX = Cursor.Position.X;
            lastMouseY = Cursor.Position.Y;

            if (invalidate)
                glControl1.Invalidate();
        }

        private void glControl1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    wKey = true;
                    break;
                case Keys.S:
                    sKey = true;
                    break;
                case Keys.A:
                    aKey = true;
                    break;
                case Keys.D:
                    dKey = true;
                    break;
                case Keys.Q:
                    qKey = true;
                    break;
                case Keys.E:
                    eKey = true;
                    break;
            }
        }

        private void glControl1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    wKey = false;
                    break;
                case Keys.S:
                    sKey = false;
                    break;
                case Keys.A:
                    aKey = false;
                    break;
                case Keys.D:
                    dKey = false;
                    break;
                case Keys.Q:
                    qKey = false;
                    break;
                case Keys.E:
                    eKey = false;
                    break;
            }
        }


        private void glControl1_MouseDown(object sender, MouseEventArgs e)
        {
            rMouse = e.Button == MouseButtons.Right;
            lMouse = e.Button == MouseButtons.Left;
        }

        private void glControl1_MouseUp(object sender, MouseEventArgs e)
        {
            rMouse = false;
            lMouse = false;
        }

        private void glControl1_MouseMove(object sender, MouseEventArgs e)
        {

        }

        #endregion

        private void glControl1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                glControl1.MakeCurrent();
                GL.UseProgram(pgm2ID);

                GL.EnableVertexAttribArray(0);

                camX.Text = cPosition.X.ToString();
                camY.Text = cPosition.Y.ToString();
                camZ.Text = cPosition.Z.ToString();

                Matrix4 View = Matrix4.LookAt(cPosition, cTarget, Vector3.UnitZ);
                Matrix4 worldView = View * projection;


                if (modReady && mobyCheck.Checked)
                {
                    GL.ClearColor(0, 0, 0, 0);
                    foreach (RatchetMoby mob in DataStore.mobs)
                    {
                        objModel = DataStore.spawnableModels.Find(x => x.modelID == mob.modelID);
                        int insx = mobList.IndexOf((uint)objModel.modelID);
                        if (insx != -1)
                        {
                            int mobNum = DataStore.mobs.IndexOf(mob);
                            byte[] cols = BitConverter.GetBytes(mobNum);
                            Matrix4 modTrans = Matrix4.CreateTranslation(mob.x, mob.y, mob.z);
                            Matrix4 modScale = Matrix4.CreateScale(objModel.size * mob.size);
                            Matrix4 xRot = Matrix4.CreateRotationX(mob.rot1);
                            Matrix4 yRot = Matrix4.CreateRotationY(mob.rot2);
                            Matrix4 zRot = Matrix4.CreateRotationZ(mob.rot3);
                            Matrix4 modRot = xRot * yRot * zRot;
                            mvp = modScale * modRot * modTrans * worldView;  //Has to be done in this order to work correctly
                            GL.UniformMatrix4(MatrixID, false, ref mvp);

                            GL.BindBuffer(BufferTarget.ArrayBuffer, mobVerts[insx]);
                            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mobInds[insx]);

                            //Verts
                            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 8, 0);

                            GL.Uniform4(pgmID_rcID, new Vector4(cols[0] / 255f, cols[1] / 255f, cols[2] / 255f, 1));
                            GL.DrawElements(PrimitiveType.Triangles, objModel.indiceBuff.Count(), DrawElementsType.UnsignedShort, 0);
                        }
                    }
                    Byte4 pixel = new Byte4();
                    GL.ReadPixels(glControl1.PointToClient(Cursor.Position).X, glControl1.Height - glControl1.PointToClient(Cursor.Position).Y, 1, 1, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, ref pixel);
                    if (pixel.A == 0)
                    {
                        mainForm.objViewer.selectList(pixel.ToUInt32());
                    }
                    GL.ClearColor(Color.SkyBlue);
                }
                glControl1.Invalidate();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (DataStore.collVertBuff != null) //Check that there's actually vertex data to be rendered
            {

                GL.GenBuffers(1, out collVBO);
                GL.GenBuffers(1, out collInd);

                int vertCnt = DataStore.collVertBuff.Count;
                float[] vertArr = DataStore.collVertBuff.ToArray();
                GL.BindBuffer(BufferTarget.ArrayBuffer, collVBO);
                GL.BufferData(BufferTarget.ArrayBuffer, vertCnt * sizeof(float), vertArr, BufferUsageHint.StaticDraw);

                indCnt = DataStore.collIndBuff.Count;
                uint[] indArr = DataStore.collIndBuff.ToArray();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, collInd);
                GL.BufferData(BufferTarget.ElementArrayBuffer, indCnt * sizeof(uint), indArr, BufferUsageHint.StaticDraw);

                collReady = true;
                collCheck.Checked = true;
                glControl1.Invalidate();
            }
        }

        private void terrainCheck_CheckedChanged(object sender, EventArgs e)
        {
            glControl1.Invalidate();
        }

        private void collCheck_CheckedChanged(object sender, EventArgs e)
        {
            glControl1.Invalidate();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int chunkCount = DataStore.chunks.Count();
            chunkVerts = new int[chunkCount];
            chunkInds = new int[chunkCount];

            GL.GenBuffers(chunkCount, chunkVerts);
            GL.GenBuffers(chunkCount, chunkInds);

            chunkTexList = new List<uint>();
            Console.WriteLine("Checking chunk");
            for (int i = 0; i < chunkCount; i++)
            {
                RatchetModel_Terrain chunk = DataStore.chunks[i];
                if (chunk.vertBuff != null) //Check that there's actually vertex data to be rendered
                {
                    int vertCnt = chunk.vertBuff.Count;
                    float[] vertArr = chunk.vertBuff.ToArray();
                    GL.BindBuffer(BufferTarget.ArrayBuffer, chunkVerts[i]);
                    GL.BufferData(BufferTarget.ArrayBuffer, vertCnt * sizeof(float), vertArr, BufferUsageHint.StaticDraw);

                    indCnt = chunk.indiceBuff.Count;
                    uint[] indArr = chunk.indiceBuff.ToArray();
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, chunkInds[i]);
                    GL.BufferData(BufferTarget.ElementArrayBuffer, indCnt * sizeof(uint), indArr, BufferUsageHint.StaticDraw);


                    //Delete old textures, so we don't fill up the GPU memory
                    if (chunkTexIDArr != null)
                    {
                        GL.DeleteTextures(chunkTexIDArr.Count(), chunkTexIDArr);
                    }


                    foreach (RatchetTexture_Model tex in chunk.textureConfig)
                    {
                        if (!chunkTexList.Contains(tex.ID)) chunkTexList.Add(tex.ID);
                    }
                }
            }

            //Console.WriteLine(chunkTexList.Count.ToString("X8"));
            int texCount = chunkTexList.Count;
            chunkTexIDArr = new int[texCount];
            GL.GenTextures(texCount, chunkTexIDArr);
            for (int t = 0; t < texCount; t++)
            {
                Bitmap file = getTextureImage((int)chunkTexList[t]);

                if (file != null)
                {
                    GL.BindTexture(TextureTarget.Texture2D, chunkTexIDArr[t]);

                    BitmapData data = file.LockBits(new Rectangle(0, 0, file.Width, file.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
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

            chunksReady = true;
            chunkCheck.Checked = true;
            glControl1.Invalidate();
        }

        private void chunkCheck_CheckedChanged(object sender, EventArgs e)
        {
            glControl1.Invalidate();
        }

        private void splineCheck_CheckedChanged(object sender, EventArgs e)
        {
            glControl1.Invalidate();
        }
    }
}
