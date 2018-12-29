using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RatchetModel;
using static DataFunctions;
using RatchetLevelEditor;

class ModelParser
    {

    //Passing it by reference to avoid making a copy
    public static void SpawnableToObj(ref RatchetModel_General model, string OBJfilename, string MTLfilename)
    {

        StreamWriter OBJfs = new StreamWriter(OBJfilename);
        StreamWriter MTLfs = new StreamWriter(MTLfilename);


        OBJfs.WriteLine("o Object_" + model.modelID.ToString("X4"));
        if (model.textureConfig != null)
            OBJfs.WriteLine("mtllib " + MTLfilename.Split('\\').Last());


        List<uint> usedMtls = new List<uint>(); //To prevent it from making double mtl entries
        for (int i = 0; i < model.textureConfig.Count; i++)
        {
            uint modelTextureID = model.textureConfig[i].ID;
            if (!usedMtls.Contains(modelTextureID))
            {
                MTLfs.WriteLine("newmtl mtl_" + modelTextureID);
                MTLfs.WriteLine("Ns 1000");
                MTLfs.WriteLine("Ka 1.000000 1.000000 1.000000");
                MTLfs.WriteLine("Kd 1.000000 1.000000 1.000000");
                MTLfs.WriteLine("Ni 1.000000");
                MTLfs.WriteLine("d 0.000000");
                MTLfs.WriteLine("illum 1");
                MTLfs.WriteLine("map_Kd tex_" + model.textureConfig[i].ID + ".png");
                usedMtls.Add(modelTextureID);
            }
        }
        MTLfs.Close();


        //Vertices, normals, UV's
        float[] vertData = model.vertBuff.ToArray();
        for (int x = 0; x < model.vertexCount; x++)
        {
            float px = vertData[(x * 0x08) + 0x0];
            float py = vertData[(x * 0x08) + 0x1];
            float pz = vertData[(x * 0x08) + 0x2];
            float nx = vertData[(x * 0x08) + 0x3];
            float ny = vertData[(x * 0x08) + 0x4];
            float nz = vertData[(x * 0x08) + 0x5];
            float tu = vertData[(x * 0x08) + 0x6];
            float tv = 1f - vertData[(x * 0x08) + 0x7];
            OBJfs.WriteLine("v " + px.ToString("G") + " " + py.ToString("G") + " " + pz.ToString("G"));
            OBJfs.WriteLine("vn " + nx.ToString("G") + " " + ny.ToString("G") + " " + nz.ToString("G"));
            OBJfs.WriteLine("vt " + tu.ToString("G") + " " + tv.ToString("G"));
        }


        //Faces
        int tCnt = 0;
        ushort[] inds = model.indiceBuff.ToArray();
        for (int i = 0; i < model.indiceBuff.Count / 3; i++)
        {
            if (model.textureConfig != null && tCnt < model.textureConfig.Count)
            {
                if (i * 3 >= model.textureConfig[tCnt].start)
                {
                    OBJfs.WriteLine("usemtl mtl_" + model.textureConfig[tCnt].ID.ToString(""));
                    OBJfs.WriteLine("g Texture_" + model.textureConfig[tCnt].ID.ToString(""));
                    tCnt++;
                }
            }
            int f1 = inds[i * 3 + 0] + 1;
            int f2 = inds[i * 3 + 1] + 1;
            int f3 = inds[i * 3 + 2] + 1;
            OBJfs.WriteLine("f " + (f1 + "/" + f1 + "/" + f1) + " " + (f2 + "/" + f2 + "/" + f2) + " " + (f3 + "/" + f3 + "/" + f3));
            //OBJfs.WriteLine("f " + (f3 + "/" + f3 + "/" + f3) + " " + (f2 + "/" + f2 + "/" + f2) + " " + (f1 + "/" + f1 + "/" + f1));
        }
        OBJfs.Close();

    }

    public static void terrainMeshToObj(ref RatchetModel_Terrain model, string OBJfilename, string MTLfilename)
    {
        StreamWriter OBJfs = new StreamWriter(OBJfilename);
        StreamWriter MTLfs = new StreamWriter(MTLfilename);

        OBJfs.WriteLine("o Object_" + model.modelID.ToString("X4"));
        if (model.textureConfig != null)
            OBJfs.WriteLine("mtllib " + MTLfilename.Split('\\').Last());


        List<uint> usedMtls = new List<uint>(); //To prevent it from making double mtl entries
        for (int i = 0; i < model.textureConfig.Count; i++)
        {
            uint modelTextureID = model.textureConfig[i].ID;
            if (!usedMtls.Contains(modelTextureID))
            {
                MTLfs.WriteLine("newmtl mtl_" + modelTextureID);
                MTLfs.WriteLine("Ns 1000");
                MTLfs.WriteLine("Ka 1.000000 1.000000 1.000000");
                MTLfs.WriteLine("Kd 1.000000 1.000000 1.000000");
                MTLfs.WriteLine("Ni 1.000000");
                MTLfs.WriteLine("d 0.000000");
                MTLfs.WriteLine("illum 1");
                MTLfs.WriteLine("map_Kd tex_" + model.textureConfig[i].ID + ".png");
                usedMtls.Add(modelTextureID);
            }
        }
        MTLfs.Close();

        int vertElemSize = 0x0C;

        //Vertices, normals, UV's
        float[] vertData = model.vertBuff.ToArray();
        for (int x = 0; x < model.vertexCount; x++)
        {
            
            float px = vertData[x * vertElemSize + 0x0];
            float py = vertData[x * vertElemSize + 0x1];
            float pz = vertData[x * vertElemSize + 0x2];
            float nx = vertData[x * vertElemSize + 0x3];
            float ny = vertData[x * vertElemSize + 0x4];
            float nz = vertData[x * vertElemSize + 0x5];
            float tu = vertData[x * vertElemSize + 0x6];
            float tv = 1f - vertData[x * vertElemSize + 0x7];
            OBJfs.WriteLine("v " + px.ToString("G") + " " + py.ToString("G") + " " + pz.ToString("G"));
            OBJfs.WriteLine("vn " + nx.ToString("G") + " " + ny.ToString("G") + " " + nz.ToString("G"));
            OBJfs.WriteLine("vt " + tu.ToString("G") + " " + tv.ToString("G"));
        }

        //Faces
        int tCnt = 0;
        uint[] inds = model.indiceBuff.ToArray();
        for (int i = 0; i < model.indiceBuff.Count / 3; i++)
        {
            if (model.textureConfig != null && tCnt < model.textureConfig.Count)
            {
                if (i * 3 >= model.textureConfig[tCnt].start)
                {
                    OBJfs.WriteLine("usemtl mtl_" + model.textureConfig[tCnt].ID.ToString(""));
                    OBJfs.WriteLine("g Texture_" + model.textureConfig[tCnt].ID.ToString(""));
                    tCnt++;
                }
            }
            uint f1 = inds[i * 3 + 0] + 1;
            uint f2 = inds[i * 3 + 1] + 1;
            uint f3 = inds[i * 3 + 2] + 1;
            OBJfs.WriteLine("f " + (f1 + "/" + f1 + "/" + f1) + " " + (f2 + "/" + f2 + "/" + f2) + " " + (f3 + "/" + f3 + "/" + f3));
            //OBJfs.WriteLine("f " + (f3 + "/" + f3 + "/" + f3) + " " + (f2 + "/" + f2 + "/" + f2) + " " + (f1 + "/" + f1 + "/" + f1));
        }
        OBJfs.Close();

    }
}
