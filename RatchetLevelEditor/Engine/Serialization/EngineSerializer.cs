using RatchetLevelEditor.Engine.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataFunctions;
using static RatchetTexture;

namespace RatchetLevelEditor.Engine.Serialization
{
    /***
     * This class handles serializing the engine elements into the format that is used in the games.
     * 
     * 
     ***/


    class EngineSerializer
    {
        //Dictionary to define the mapping of each index of the file and its respective variable
        public static Dictionary<int, dynamic> headerMap;
        static byte[] engine;
        static byte[] vram = new byte[0];

        public static void getHeaderMap(int racNum)
        {
            switch (racNum)
            {
                case 1:
                case 2:
                case 3:
                    headerMap = new Dictionary<int, dynamic>()
                    {
                        {0x00, new Action<dynamic>(i => { serializeSpawnables(ref engine, 0x00, racNum); }) },
                        {0x04, new Action<dynamic>(i => { serializeUnknownHeaderData(0x04); }) },
                        {0x08, new Action<dynamic>(i => { serializeUnknownHeaderData(0x08); }) },
                        {0x0C, new Action<dynamic>(i => { serializeUnknownHeaderData(0x0C); }) },
                        {0x10, new Action<dynamic>(i => { serializeSkyBox(ref engine, 0x10, racNum); }) },
                        {0x14, new Action<dynamic>(i => { serializeTerrainCollision(ref engine, 0x14, racNum); }) },
                        {0x18, new Action<dynamic>(i => { serializePlayerAnims(ref engine, 0x18, racNum); }) },
                        {0x1C, new Action<dynamic>(i => { serializeTieModels(ref engine, 0x1C, racNum); }) },
                        {0x24, new Action<dynamic>(i => { serializeTies(ref engine, 0x24, racNum); }) },
                        {0x2C, new Action<dynamic>(i => { serializeShrubModels(ref engine, 0x2C, racNum); }) },
                        {0x34, new Action<dynamic>(i => { serializeShrubs(ref engine, 0x34, racNum); }) },
                        {0x3C, new Action<dynamic>(i => { serializeTerrain(ref engine, 0x3C, racNum); }) },
                        {0x40, new Action<dynamic>(i => { serializeUnknownHeaderData(0x40); }) },
                        {0x44, new Action<dynamic>(i => { serializeUnknownHeaderData(0x44); }) },
                        {0x48, new Action<dynamic>(i => { serializeUnknownHeaderData(0x48); }) },
                        {0x4C, new Action<dynamic>(i => { serializeUnknownHeaderData(0x4C); }) },
                        {0x50, new Action<dynamic>(i => { serializeUnknownHeaderData(0x50); }) },
                        {0x54, new Action<dynamic>(i => { serializeTextures(ref engine, ref vram, 0x54, racNum); }) },
                        {0x5C, new Action<dynamic>(i => { serializeLevelLighting(0x5C); }) },
                        {0x64, new Action<dynamic>(i => { serializeUnknownHeaderData(0x64); }) },
                        {0x68, new Action<dynamic>(i => { serializeMenuTextureConfig(0x68); }) },
                        {0x70, new Action<dynamic>(i => { serializeUnknownHeaderData(0x70); }) },
                        {0x74, new Action<dynamic>(i => { serializeUnknownHeaderData(0x74); }) },
                        {0x78, new Action<dynamic>(i => { serializeUnknownHeaderData(0x78); }) },
                    };
                    break;
                case 4:
                default:
                    break;
            }
        }

        public async static void serialize(string path, int racNum)
        {
            getHeaderMap(racNum);

            uint size = 0 ;
            uint oneIndex = 0;
            uint twoIndex = 0;

            //Hack but whatever
            switch(racNum)
            {
                case 1:
                case 2:
                case 3:
                    size = 0x90;
                    oneIndex = 0x7C;
                    twoIndex = 0x80;
                    break;
                case 4:
                    size = 0xA0;
                    oneIndex = 0x90;
                    twoIndex = 0x94;
                    break;
            }
            engine = new byte[(int) size];

            WriteUint32(ref engine, (int)oneIndex, (uint) 1);
            WriteUint32(ref engine, (int)twoIndex, (uint) 2);

            foreach (UnknownDataIndex offset in DataStoreEngine.unhandledOffsets)
            {
                KeyValuePair<int, dynamic> header = headerMap.Where(x => x.Key == offset.offset).FirstOrDefault();
                if (!header.Equals(default(KeyValuePair<int, dynamic>)))
                {
                    if (offset.pointer != 0)
                    {
                        WriteUint32(ref engine, (int)offset.offset, (uint)engine.Length);
                        if(headerMap.Where(h => h.Key == header.Key) != null)
                            headerMap[header.Key].DynamicInvoke(header);

                    }
                    else
                    {
                        WriteUint32(ref engine, (int)offset.offset, 0);
                    }
                }
            }


            File.WriteAllBytes(path + "/engineTEST.ps3", engine);
            File.WriteAllBytes(path + "/vramTEST.ps3", vram);

            Console.WriteLine("Serializing engine and vram complete.");

        }

        public static void serializeUnknownHeaderData(int index)
        {
            Console.WriteLine("Serializing Unhandled Data - 0x" + index.ToString("X"));

            int currentOffset = engine.Length;
            uint padding = 0;
            UnknownDataModel model = DataStoreEngine.unknownEngineData.Where(x => x.index == index).Select(x => x).FirstOrDefault();

            if (model != null)
            {
                while ((model.data.Length + padding) % 0x10 != 0)
                padding++;

                Array.Resize(ref engine, (int)(engine.Length + model.data.Length + padding));

                writeBytes(engine, currentOffset, model.data, model.data.Length);
            }

            return;

        }

        #region Spawnable Models
        public static void serializeSpawnables(ref byte[] engineData, int index, int racNum)
        {
            Console.WriteLine("Serializing Moby Models");

            int currentOffset = engine.Length;
            int modelsCount = DataStoreEngine.spawnableModels.Count;

            //Write the models count
            byte[] spawnablesInitHeader = new byte[0x04];
            WriteUint32(ref spawnablesInitHeader, 0x00, (uint) modelsCount);
            Array.Resize(ref engine, (int)(engine.Length + spawnablesInitHeader.Length));
            writeBytes(engine, currentOffset, spawnablesInitHeader, spawnablesInitHeader.Length);

            //Update the currentOffset to the new size
            currentOffset = engine.Length;

            //Add the space for the Model ID + Pointer block
            Array.Resize(ref engine, (int)(engine.Length + (modelsCount * 0x08)));

            //Save the currentOffset at the point of creating the header (we will need this)
            int modelOffset = currentOffset;
            int offset = 0x00;
            foreach (RatchetModel_General model in DataStoreEngine.spawnableModels )
            {
                writeBytes(engine, modelOffset + offset, (uint) model.modelID, 4);
                offset += 0x04;
                if (model.rawData != null)
                {
                    //Write the current offset
                    writeBytes(engine, modelOffset + offset, (uint) engine.Length, 4);

                    currentOffset = engine.Length;

                    //Add the space for the Model Data
                    Array.Resize(ref engine, (int)(engine.Length + model.rawData.Length));
              
                    //Write the model data
                    writeBytes(engine, currentOffset, model.rawData, model.rawData.Length);
                }
                else
                {
                    //writeBytes(engine, currentOffset + offset, 0, 4);
                }
                offset += 0x04;
            }
        }
        #endregion

        #region Terrain Collision
        public static void serializeTerrainCollision(ref byte[] engineData, int index, int racNum)
        {
            Console.WriteLine("Serializing Terrain Collision");

            int currentOffset = engine.Length;
            Array.Resize(ref engine, (int)(engine.Length + DataStoreEngine.collisionDataRaw.Length));
            writeBytes(engine, currentOffset, DataStoreEngine.collisionDataRaw, DataStoreEngine.collisionDataRaw.Length);
        }
        #endregion

        #region Tie Models
        public static void serializeTieModels(ref byte[] engineData, int index, int racNum)
        {
            Console.WriteLine("Serializing Tie Models");

            int currentOffset = engine.Length;
            int modelsCount = DataStoreEngine.levelModels.Count;

            //Write the models count to the header
            WriteUint32(ref engine, index + 0x04, (uint) modelsCount);

            byte[] tieModelBlock = new byte[DataStoreEngine.tieModels.Count * 0x40];
            Array.Resize(ref engine, (int)(engine.Length + tieModelBlock.Length));

            //??
            //writeBytes(engine, currentOffset, tieModelBlock, tieModelBlock.Length);

            List<byte[]> dataBytes = new List<byte[]>();



            int dOffset = engine.Length;
            foreach (TieModel tModel in DataStoreEngine.tieModels)
            {
                byte[] tieHeader = new byte[0x40];

                WriteFloat(ref tieHeader, 0x00, tModel.ind_00);
                WriteFloat(ref tieHeader, 0x04, tModel.ind_04);
                WriteFloat(ref tieHeader, 0x08, tModel.ind_08);
                WriteFloat(ref tieHeader, 0x0C, tModel.ind_0C);

                //Texture Data
                WriteUint32(ref tieHeader, 0x1C, (uint)dOffset);
                dOffset += tModel.textureData.Length;
                dataBytes.Add(tModel.textureData);

                //Vertex Data
                WriteUint32(ref tieHeader, 0x10, (uint)dOffset);
                dOffset += tModel.vertexData.Length;
                dataBytes.Add(tModel.vertexData);

                //UV data
                WriteUint32(ref tieHeader, 0x14, (uint)dOffset);
                dOffset += tModel.uVData.Length;
                dataBytes.Add(tModel.uVData);


                //Face Data
                WriteUint32(ref tieHeader, 0x18, (uint)dOffset);

                int faceSizeNeeded = (int) (tModel.modelData.faceCount) * 2;
                while (faceSizeNeeded % 0x10 != 0)
                {
                    faceSizeNeeded++;
                };
                byte[] faceData = new byte[faceSizeNeeded];

                uint fPos = 0;
                foreach (TieModelFace face in tModel.faces)
                {
                    WriteUint16(ref faceData, (int)fPos + 0x00, (ushort)face.v1);
                    WriteUint16(ref faceData, (int)fPos + 0x02, (ushort)face.v2);
                    WriteUint16(ref faceData, (int)fPos + 0x04, (ushort)face.v3);
                    fPos += 0x06;
                }

                dataBytes.Add(faceData);
                dOffset += faceData.Length;

                int padding = 0;
                while ((engine.Length + padding) % 0x10 != 0)
                {
                    padding++;
                }

                dOffset += padding;

                //Rest of the header
                WriteUint32(ref tieHeader, 0x20, tModel.ind_20);
                WriteUint32(ref tieHeader, 0x24, (uint) tModel.vertexCount);
                WriteUint16(ref tieHeader, 0x28, (ushort) tModel.texCount);
                WriteUint16(ref tieHeader, 0x2A, (ushort)tModel.ind_2A);
                WriteFloat(ref tieHeader, 0x2C, tModel.ind_2C);

                WriteUint16(ref tieHeader, 0x30, (ushort)tModel.modelId);
                WriteUint16(ref tieHeader, 0x32, (ushort)tModel.ind_32);
                WriteUint32(ref tieHeader, 0x34, tModel.ind_34);
                WriteUint32(ref tieHeader, 0x38, tModel.ind_38);
                WriteUint32(ref tieHeader, 0x3C, tModel.rgba);

                //Write header data to engine array
                writeBytes(engine, currentOffset, tieHeader, tieHeader.Length);
                currentOffset += 0x40;
            }

            int offs = engine.Length;
            Array.Resize(ref engine, dOffset);
            

            //Write data array to engine array
            foreach(byte[] data in dataBytes)
            {
                writeBytes(engine, offs, data, data.Length);
                offs += data.Length;

                while ((offs) % 0x10 != 0)
                {
                    offs++;
                }
            }
        }
        #endregion

        #region Ties
        public static void serializeTies(ref byte[] engine, int index, int racNum)
        {
            Console.WriteLine("Serializing Ties");

            int currentOffset = engine.Length;
            int tieOffset = engine.Length;

            byte[] tieBlock = new byte[DataStoreEngine.levelObjects.Count * 0x70];
            Array.Resize(ref engine, (int)(engine.Length + tieBlock.Length));
            writeBytes(engine, currentOffset, tieBlock, tieBlock.Length);


            int vertexColorOffset = engine.Length;
            List<byte[]> vertexColorBlock = new List<byte[]>();

            //Write the ties count to the header
            WriteUint32(ref engine, index + 0x04, (uint)DataStoreEngine.levelObjects.Count);

            foreach (LevelObject tie in DataStoreEngine.levelObjects)
            {
                byte[] tieData = new byte[0x70];

                WriteFloat(ref tieData, 0x00, tie.v1x);
                WriteFloat(ref tieData, 0x04, tie.v1y);
                WriteFloat(ref tieData, 0x08, tie.v1z);
                WriteFloat(ref tieData, 0x0C, tie.v1w);

                WriteFloat(ref tieData, 0x10, tie.v2x);
                WriteFloat(ref tieData, 0x14, tie.v2y);
                WriteFloat(ref tieData, 0x18, tie.v2z);
                WriteFloat(ref tieData, 0x1C, tie.v2w);

                WriteFloat(ref tieData, 0x20, tie.v3x);
                WriteFloat(ref tieData, 0x24, tie.v3y);
                WriteFloat(ref tieData, 0x28, tie.v3z);
                WriteFloat(ref tieData, 0x2C, tie.v3w);

                WriteFloat(ref tieData, 0x30, tie.x);
                WriteFloat(ref tieData, 0x34, tie.y);
                WriteFloat(ref tieData, 0x38, tie.z);
                WriteFloat(ref tieData, 0x3C, tie.w);

                WriteUint32(ref tieData, 0x40, tie.off_40);
                WriteUint32(ref tieData, 0x44, tie.off_44);
                WriteUint32(ref tieData, 0x48, tie.off_48);
                WriteUint32(ref tieData, 0x4C, tie.off_4C);

                WriteUint16(ref tieData, 0x50, tie.off_50);
                WriteUint16(ref tieData, 0x52, tie.modelID);
                WriteUint32(ref tieData, 0x54, tie.off_54);
                WriteUint32(ref tieData, 0x58, tie.off_58);
                WriteUint32(ref tieData, 0x5C, tie.off_5C);

                WriteUint32(ref tieData, 0x60, (uint) vertexColorOffset);
                WriteUint32(ref tieData, 0x64, tie.off_64);
                WriteUint32(ref tieData, 0x68, tie.off_68);
                WriteUint32(ref tieData, 0x6C, tie.off_6C);

                vertexColorBlock.Add(tie.vertexColors);
                vertexColorOffset += tie.vertexColors.Length;

                while((vertexColorOffset) % 0x10 != 0)
                {
                    vertexColorOffset++;
                }

                /*currentOffset = engine.Length;
                Array.Resize(ref engine, (int)(engine.Length + tie.vertexColors.Length + padding));
                writeBytes(engine, currentOffset, tie.vertexColors, tie.vertexColors.Length);*/

                writeBytes(engine, tieOffset, tieData, tieData.Length);
                tieOffset += 0x70;

            }
            int engineLength = engine.Length;
            Array.Resize(ref engine, vertexColorOffset);
            foreach(var vertexColor in vertexColorBlock)
            {
                vertexColor.CopyTo(engine, engineLength);
                engineLength += vertexColor.Length;
                while ((engineLength) % 0x10 != 0)
                {
                    engineLength++;
                }
            }
        }
        #endregion

        #region Shrub Models
        public static void serializeShrubModels(ref byte[] engineData, int index, int racNum)
        {
            Console.WriteLine("Serializing Shrub Models");

            int currentOffset = engine.Length;
            int modelsCount = DataStoreEngine.sceneryModels.Count;

            //Write the count to the header
            WriteUint32(ref engine, index + 0x04, (uint)modelsCount);

            byte[] shrubModelBlock = new byte[DataStoreEngine.shrubModels.Count * 0x40];
            Array.Resize(ref engine, (int)(engine.Length + shrubModelBlock.Length));

            //writeBytes(engine, currentOffset, shrubModelBlock, shrubModelBlock.Length);

            int dOffset = engine.Length;
            List<byte[]> dataBytes = new List<byte[]>();

            foreach (ShrubModel sModel in DataStoreEngine.shrubModels)
            {
                byte[] shrubHeader = new byte[0x40];

                WriteFloat(ref shrubHeader, 0x00, sModel.ind_00);
                WriteFloat(ref shrubHeader, 0x04, sModel.ind_04);
                WriteFloat(ref shrubHeader, 0x08, sModel.ind_08);
                WriteFloat(ref shrubHeader, 0x0C, sModel.ind_0C);


                WriteUint32(ref shrubHeader, 0x1C, (uint)dOffset);
                dOffset += sModel.textureData.Length;
                dataBytes.Add(sModel.textureData);

                WriteUint32(ref shrubHeader, 0x10, (uint)dOffset);
                dOffset += sModel.vertexData.Length;
                dataBytes.Add(sModel.vertexData);

                WriteUint32(ref shrubHeader, 0x14, (uint)dOffset);
                dOffset += sModel.uVData.Length;
                dataBytes.Add(sModel.uVData);


                //Index Data
                WriteUint32(ref shrubHeader, 0x18, (uint)dOffset);

                int faceSizeNeeded = (int)(sModel.modelData.faceCount) * 2;
                while (faceSizeNeeded % 0x10 != 0)
                {
                    faceSizeNeeded++;
                };
                byte[] faceData = new byte[faceSizeNeeded];

                uint fPos = 0;
                foreach (ShrubModelFace face in sModel.faces)
                {
                    WriteUint16(ref faceData, (int)fPos + 0x00, (ushort)face.v1);
                    WriteUint16(ref faceData, (int)fPos + 0x02, (ushort)face.v2);
                    WriteUint16(ref faceData, (int)fPos + 0x04, (ushort)face.v3);
                    fPos += 0x06;
                }
                dOffset += faceData.Length;
                dataBytes.Add(faceData);


                while ((dOffset) % 0x10 != 0)
                {
                    dOffset++;
                }

                WriteUint32(ref shrubHeader, 0x20, sModel.ind_20);
                WriteUint32(ref shrubHeader, 0x24, (uint)sModel.vertexCount);
                WriteUint16(ref shrubHeader, 0x28, (ushort)sModel.texCount);
                WriteUint16(ref shrubHeader, 0x2A, (ushort)sModel.ind_2A);
                WriteFloat(ref shrubHeader, 0x2C, sModel.ind_2C);

                WriteUint16(ref shrubHeader, 0x30, (ushort)sModel.modelId);
                WriteUint16(ref shrubHeader, 0x32, (ushort)sModel.ind_32);
                WriteUint32(ref shrubHeader, 0x34, sModel.ind_34);
                WriteUint32(ref shrubHeader, 0x38, sModel.ind_38);
                WriteUint32(ref shrubHeader, 0x3C, sModel.rgba);

                writeBytes(engine, currentOffset, shrubHeader, shrubHeader.Length);
                currentOffset += 0x40;

            }

            int offs = engine.Length;
            Array.Resize(ref engine, dOffset);

            //Write data array to engine array
            foreach (byte[] data in dataBytes)
            {
                writeBytes(engine, offs, data, data.Length);
                offs += data.Length;

                while ((offs) % 0x10 != 0) { offs++; }
            }
        }
        #endregion

        #region Shrubs
        public static void serializeShrubs(ref byte[] engine, int index, int racNum)
        {
            Console.WriteLine("Serializing Shrubs");

            int currentOffset = engine.Length;
            int shrubOffset = engine.Length;

            //Write the count to the header
            WriteUint32(ref engine, index + 0x04, (uint)DataStoreEngine.sceneryObjects.Count);

            byte[] shrubBlock = new byte[DataStoreEngine.sceneryObjects.Count * 0x70];
            Array.Resize(ref engine, (int)(engine.Length + shrubBlock.Length));
            writeBytes(engine, currentOffset, shrubBlock, shrubBlock.Length);

            foreach (LevelObject shrub in DataStoreEngine.sceneryObjects)
            {
                byte[] tieData = new byte[0x70];

                WriteFloat(ref tieData, 0x00, shrub.v1x);
                WriteFloat(ref tieData, 0x04, shrub.v1y);
                WriteFloat(ref tieData, 0x08, shrub.v1z);
                WriteFloat(ref tieData, 0x0C, shrub.v1w);

                WriteFloat(ref tieData, 0x10, shrub.v2x);
                WriteFloat(ref tieData, 0x14, shrub.v2y);
                WriteFloat(ref tieData, 0x18, shrub.v2z);
                WriteFloat(ref tieData, 0x1C, shrub.v2w);

                WriteFloat(ref tieData, 0x20, shrub.v3x);
                WriteFloat(ref tieData, 0x24, shrub.v3y);
                WriteFloat(ref tieData, 0x28, shrub.v3z);
                WriteFloat(ref tieData, 0x2C, shrub.v3w);

                WriteFloat(ref tieData, 0x30, shrub.x);
                WriteFloat(ref tieData, 0x34, shrub.y);
                WriteFloat(ref tieData, 0x38, shrub.z);
                WriteFloat(ref tieData, 0x3C, shrub.w);

                WriteUint32(ref tieData, 0x40, shrub.off_40);
                WriteUint32(ref tieData, 0x44, shrub.off_44);
                WriteUint32(ref tieData, 0x48, shrub.off_48);
                WriteUint32(ref tieData, 0x4C, shrub.off_4C);

                WriteUint16(ref tieData, 0x50, shrub.off_50);
                WriteUint16(ref tieData, 0x52, shrub.modelID);
                WriteUint32(ref tieData, 0x54, shrub.off_54);
                WriteUint32(ref tieData, 0x58, shrub.off_58);
                WriteUint32(ref tieData, 0x5C, shrub.off_5C);

                WriteUint32(ref tieData, 0x60, shrub.vertexColorsPtr);
                WriteUint32(ref tieData, 0x64, shrub.off_64);
                WriteUint32(ref tieData, 0x68, shrub.off_68);
                WriteUint32(ref tieData, 0x6C, shrub.off_6C);

                writeBytes(engine, shrubOffset, tieData, tieData.Length);
                shrubOffset += 0x70;

            }
        }
        #endregion

        #region Terrain Mesh
        public static void serializeTerrain(ref byte[] engine, int index, int racNum)
        {
            Console.WriteLine("Serializing Terrain");

            int currentOffset = engine.Length;
            int terrainOffset = engine.Length;

            RatchetModel_Terrain terrainModel = DataStoreEngine.terrainModel;

            byte[] terrainHeader = new byte[0x60];

            Array.Resize(ref engine, engine.Length + terrainHeader.Length);
            WriteUint32(ref terrainHeader, 0x00, (uint) engine.Length);
            WriteUint16(ref terrainHeader, 0x04, (ushort)terrainModel.headCount);
            WriteUint16(ref terrainHeader, 0x06, (ushort)terrainModel.headCount);

            currentOffset = engine.Length;
            Array.Resize(ref engine, engine.Length + terrainModel.terrainBlock.Length);
            writeBytes(engine, currentOffset, terrainModel.terrainBlock, terrainModel.terrainBlock.Length);

            currentOffset = engine.Length;
            WriteUint32(ref terrainHeader, 0x08, (uint)currentOffset);
            Array.Resize(ref engine, engine.Length + terrainModel.unknownBlock1.Length);
            writeBytes(engine, currentOffset, terrainModel.unknownBlock1, terrainModel.unknownBlock1.Length);

            currentOffset = engine.Length;
            WriteUint32(ref terrainHeader, 0x18, (uint)currentOffset);
            Array.Resize(ref engine, engine.Length + terrainModel.unknownBlock2.Length);
            writeBytes(engine, currentOffset, terrainModel.unknownBlock2, terrainModel.unknownBlock2.Length);

            currentOffset = engine.Length;
            WriteUint32(ref terrainHeader, 0x28, (uint)currentOffset);
            Array.Resize(ref engine, engine.Length + terrainModel.unknownBlock3.Length);
            writeBytes(engine, currentOffset, terrainModel.unknownBlock3, terrainModel.unknownBlock3.Length);

            currentOffset = engine.Length;
            WriteUint32(ref terrainHeader, 0x38, (uint)currentOffset);
            Array.Resize(ref engine, engine.Length + terrainModel.vertexIndices.Length);
            writeBytes(engine, currentOffset, terrainModel.vertexIndices, terrainModel.vertexIndices.Length);

            writeBytes(engine, terrainOffset, terrainHeader, terrainHeader.Length);

            uint padding = 0;
            while ((engine.Length + padding) % 0x10 != 0)
            {
                padding++;
            }

            Array.Resize(ref engine, (int)(engine.Length + padding));
        }
        #endregion

        #region Textures
        public static void serializeTextures(ref byte[] engine, ref byte[] vram, int index, int racNum)
        {
            Console.WriteLine("Serializing Textures");

            int currentOffset = engine.Length;

            //Write the count to the header
            WriteUint32(ref engine, index + 0x04, (uint)DataStoreEngine.textures.Count);

            //Resize the engine array to hold the texture headers
            Array.Resize(ref engine, (int)(engine.Length + DataStoreEngine.textures.Count * 0x24));
            
            int vramOffset = 0;
            List<byte[]> vramBytes = new List<byte[]>();

            foreach (RatchetTexture.RatchetTexture_General texture in DataStoreEngine.textures)
            {
                byte[] textureHeader = new byte[0x24];

                //Write the vram offset to the texture header
                WriteUint32(ref textureHeader, 0x00, (uint)vramOffset);
                WriteUint32(ref textureHeader, 0x04, texture.off_04);
                WriteUint32(ref textureHeader, 0x08, texture.off_08);
                WriteUint32(ref textureHeader, 0x0C, texture.off_0C);

                WriteUint32(ref textureHeader, 0x10, texture.off_10);
                WriteUint32(ref textureHeader, 0x14, texture.off_14);
                WriteUint16(ref textureHeader, 0x18, (ushort) texture.width);
                WriteUint16(ref textureHeader, 0x1A, (ushort) texture.height);
                WriteUint32(ref textureHeader, 0x1C, texture.off_1C);

                WriteUint32(ref textureHeader, 0x20, texture.off_20);

                //Write the header to the engine file array
                writeBytes(engine, currentOffset, textureHeader, textureHeader.Length);
                currentOffset += 0x24;


                //Get the correct padding for the vram file
                uint padding = 0;
                while ((texture.texData.Length + padding) % 0x10 != 0){ padding++; }
                vramOffset += (int)(texture.texData.Length + padding);

                //Add the vram data to the vram data list
                vramBytes.Add(texture.texData);
            }

            //Resize the vram array, and write all the data to it
            Array.Resize(ref vram, (int)(vram.Length + vramOffset));
            int ndx = 0;
            foreach(var vrambyte in vramBytes)
            {
                writeBytes(vram, ndx, vrambyte, vrambyte.Length);
                ndx += vrambyte.Length;
            }
            
        }
        #endregion

        #region Level Lighting
        public static void serializeLevelLighting(int index)
        {
            Console.WriteLine("Serializing Lighting");

            //Write the lighting level
            WriteUint32(ref engine, index + 0x04, DataStoreEngine.engineHeader.lightingLevel);

            int currentOffset = engine.Length;
            uint padding = 0;
            UnknownDataModel model = DataStoreEngine.unknownEngineData.Where(x => x.index == index).Select(x => x).FirstOrDefault();

            if (model != null)
            {
                while ((model.data.Length + padding) % 0x10 != 0)
                    padding++;

                Array.Resize(ref engine, (int)(engine.Length + model.data.Length + padding));

                writeBytes(engine, currentOffset, model.data, model.data.Length);
            }

            return;

        }
        #endregion

        #region Menu Texture Config
        public static void serializeMenuTextureConfig(int index)
        {
            Console.WriteLine("Serializing Menu Texture Config");

            //Write the lighting level
            WriteUint32(ref engine, index + 0x04, DataStoreEngine.engineHeader.textureConfigMenuCount);

            int currentOffset = engine.Length;
            uint padding = 0;
            UnknownDataModel model = DataStoreEngine.unknownEngineData.Where(x => x.index == index).Select(x => x).FirstOrDefault();

            if (model != null)
            {
                Console.WriteLine(model == null);
                while ((model.data.Length + padding) % 0x10 != 0)
                    padding++;

                Array.Resize(ref engine, (int)(engine.Length + model.data.Length + padding));

                writeBytes(engine, currentOffset, model.data, model.data.Length);
            }

            return;

        }
        #endregion

        #region Campaign Player Animations
        public static void serializePlayerAnims(ref byte[] engine, int index, int racNum)
        {
            Console.WriteLine("Serializing Campaign Player Animations");

            int currentOffset = engine.Length;
            int playerAnimPointerBlockOffset = engine.Length;

            int animBlockSize = DataStoreEngine.spawnableModels[0].animationsCount * 0x04;
            while((animBlockSize % 0x10) != 0) { animBlockSize++; }

            byte[] playerAnimPointerBlock = new byte[animBlockSize];
            Array.Resize(ref engine, engine.Length + playerAnimPointerBlock.Length);

            int animOffset = 0;
            foreach (RatchetPlayerAnimation anim in DataStoreEngine.playerAnims)
            {
                currentOffset = engine.Length;

                if (anim.rawData != null)
                {
                    writeBytes(playerAnimPointerBlock, animOffset, (uint)currentOffset, 4);
                    Array.Resize(ref engine, engine.Length + anim.rawData.Length);
                    writeBytes(engine, currentOffset, anim.rawData, anim.rawData.Length);

                } else
                {
                    writeBytes(playerAnimPointerBlock, animOffset, 0, 4);
                }
                animOffset += 0x04;
            };
            writeBytes(engine, playerAnimPointerBlockOffset, playerAnimPointerBlock, playerAnimPointerBlock.Length);
        }
        #endregion

        #region Sky Box
        public static void serializeSkyBox(ref byte[] engine, int index, int racNum)
        {
            Console.WriteLine("Serializing Skybox");

            int currentOffset = engine.Length;

            RatchetModel_SkyBox skyBox = DataStoreEngine.skyBox;
            int headerSizeNeeded = 0x1C + (0x04 * skyBox.layerCount);
            while(headerSizeNeeded % 0x10 != 0)
            {
                headerSizeNeeded++;
            };

            byte[] header = new byte[headerSizeNeeded];

            WriteUint32(ref header, 0x04, (uint) skyBox.layerCount);

            int layerIndex = 0;
            foreach(SkyBoxLayer layer in skyBox.layers)
            {
                byte[] layerData = new byte[0x10 + (0x10 * layer.textureCount)];
                WriteUint16(ref layerData, 0x00, (ushort) layer.off_00);
                WriteUint16(ref layerData, 0x02, (ushort) layer.textureCount);

                uint tPos = 0;
                foreach(RatchetTexture_Model texture in layer.textures)
                {
                    WriteUint32(ref layerData, (int) (0x10 + tPos + 0x00), (uint) texture.textureId);
                    WriteUint32(ref layerData, (int) (0x10 + tPos + 0x04), (uint) texture.faceOffset);
                    WriteUint32(ref layerData, (int) (0x10 + tPos + 0x08), (uint) texture.faceCount);
                    tPos += 0x10;
                }

                int layerOffset = header.Length;
                Array.Resize(ref header, header.Length + layerData.Length);
                writeBytes(header, layerOffset, layerData, layerData.Length);
                WriteUint32(ref header, 0x1C + (0x04 * layerIndex), (uint)(engine.Length + layerOffset));
                layerIndex++;
            }

            int vertexSizeNeeded = skyBox.vertexCount * 0x18;
            while (vertexSizeNeeded % 0x10 != 0)
            {
                vertexSizeNeeded++;
            };
            byte[] vertexData = new byte[vertexSizeNeeded];

            uint vPos = 0;
            foreach(ModelVertex vertex in skyBox.vertices)
            {
                WriteFloat(ref vertexData, (int) (vPos + 0x00), vertex.x);
                WriteFloat(ref vertexData, (int) (vPos + 0x04), vertex.y);
                WriteFloat(ref vertexData, (int) (vPos + 0x08), vertex.z);
                WriteFloat(ref vertexData, (int) (vPos + 0x0C), vertex.uvu);
                WriteFloat(ref vertexData, (int) (vPos + 0x10), vertex.uvv);
                WriteUint32(ref vertexData, (int)(vPos + 0x14), (uint) vertex.rgba);
                vPos += 0x18;
            }

            int vertexDataOffset = header.Length;
            WriteUint32(ref header, 0x14, (uint)(engine.Length + vertexDataOffset));
            Array.Resize(ref header, header.Length + vertexData.Length);
            writeBytes(header, vertexDataOffset, vertexData, vertexData.Length);

            int faceSizeNeeded = skyBox.faceCount * 2;
            while (faceSizeNeeded % 0x10 != 0)
            {
                faceSizeNeeded++;
            };
            byte[] faceData = new byte[faceSizeNeeded];

            uint fPos = 0;
            foreach(ModelFace face in skyBox.faces)
            {
                WriteUint16(ref faceData, (int) fPos + 0x00, (ushort) face.v1);
                WriteUint16(ref faceData, (int) fPos + 0x02, (ushort) face.v2);
                WriteUint16(ref faceData, (int) fPos + 0x04, (ushort) face.v3);
                fPos += 0x06;
            }

            int faceDataOffset = header.Length;
            WriteUint32(ref header, 0x18, (uint)(engine.Length + faceDataOffset));
            Array.Resize(ref header, header.Length + faceData.Length);
            writeBytes(header, faceDataOffset, faceData, faceData.Length);


            Array.Resize(ref engine, engine.Length + header.Length);
            writeBytes(engine, currentOffset, header, header.Length);
        }
        #endregion;
    }
}
