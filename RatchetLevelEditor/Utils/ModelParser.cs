using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataFunctions;
using RatchetLevelEditor;
using RatchetLevelEditor.Engine.Models;

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
            uint modelTextureID = model.textureConfig[i].textureId;
            if (!usedMtls.Contains(modelTextureID))
            {
                MTLfs.WriteLine("newmtl mtl_" + modelTextureID);
                MTLfs.WriteLine("Ns 1000");
                MTLfs.WriteLine("Ka 1.000000 1.000000 1.000000");
                MTLfs.WriteLine("Kd 1.000000 1.000000 1.000000");
                MTLfs.WriteLine("Ni 1.000000");
                MTLfs.WriteLine("d 1.000000");
                MTLfs.WriteLine("illum 1");
                MTLfs.WriteLine("map_Kd tex_" + model.textureConfig[i].textureId + ".png");
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
                if (i * 3 >= model.textureConfig[tCnt].faceOffset)
                {
                    OBJfs.WriteLine("usemtl mtl_" + model.textureConfig[tCnt].textureId.ToString(""));
                    OBJfs.WriteLine("g Texture_" + model.textureConfig[tCnt].textureId.ToString(""));
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
            uint modelTextureID = model.textureConfig[i].textureId;
            if (!usedMtls.Contains(modelTextureID))
            {
                MTLfs.WriteLine("newmtl mtl_" + modelTextureID);
                MTLfs.WriteLine("Ns 1000");
                MTLfs.WriteLine("Ka 1.000000 1.000000 1.000000");
                MTLfs.WriteLine("Kd 1.000000 1.000000 1.000000");
                MTLfs.WriteLine("Ni 1.000000");
                MTLfs.WriteLine("d 0.000000");
                MTLfs.WriteLine("illum 1");
                MTLfs.WriteLine("map_Kd tex_" + model.textureConfig[i].textureId + ".png");
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
                if (i * 3 >= model.textureConfig[tCnt].faceOffset)
                {
                    OBJfs.WriteLine("usemtl mtl_" + model.textureConfig[tCnt].textureId.ToString(""));
                    OBJfs.WriteLine("g Texture_" + model.textureConfig[tCnt].textureId.ToString(""));
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

    //Returns the raw bytes for a model in the engine file
    public static byte[] getModelDataRaw(ModelType modelType, FileStream fs, int modelID)
    {
        byte[] data = null;

        byte[] engineHeader = ReadBlock(fs, 0, 0x80);

        uint modelPtr = BAToUInt32(engineHeader, 0);
        uint levelPtr = BAToUInt32(engineHeader, 0x1C);
        uint scenePtr = BAToUInt32(engineHeader, 0x2c);
        uint terraPtr = BAToUInt32(engineHeader, 0x3c);

        switch (modelType)
        {
            case ModelType.Spawnable:
                uint modelPtrSize = BAToUInt32(ReadBlock(fs, modelPtr, 4), 0);
                uint pointerStart = 0;
                uint pointerEnd = 0;
                byte[] idBlock = ReadBlock(fs, modelPtr + 4, modelPtrSize * 8);
                for (int i = 0; i < modelPtrSize; i++)
                {
                    //check if the model in the list is equal to what we called for
                    if (BAToShort(idBlock, (uint)i * 8 + 2) == modelID)
                    {
                        //pointer of our start model
                        pointerStart = BAToUInt32(idBlock, (uint)(i * 8) + 4);
                        //models with 0 as the pointer have no data
                        if (pointerStart == 0x00000000)
                        {
                            return new byte[0];
                        }
                        if (i + 1 < modelPtrSize)
                        {

                            //loops to the end of the list starting from our start pointer to find next model offset thats not 0
                            for (int x = 1; x < modelPtrSize - i; x++)
                            {
                                uint slot = BAToUInt32(idBlock, (uint)((i + x) * 8) + 4);
                                if (slot != 0x00000000)
                                {
                                    pointerEnd = slot;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            pointerEnd = FindNextLargest(engineHeader, 0);
                        }
                        data = ReadBlock(fs, pointerStart, pointerEnd - pointerStart);
                        break;
                    }
                }
                break;
            case ModelType.Level:
                //TODO
                break;
            case ModelType.Scenery:
                //TODO
                break;
        }
        return data;
    }
}
