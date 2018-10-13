using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataFunctions;
using static RatchetModel;
using static RatchetTexture;

namespace RatchetLevelEditor
{
    /***
     * This class is specific to Deadlocked, campaign maps are separated into different missions
     * This file parses those mission files. See RatchetMission.cs
     * **/
    class MissionParser
    {
        public static void parseMissions(string path)
        {
            List<int> missionIds = new List<int>();
            //Because its unlikely there will be more than 50 missions in a map
            //We use a list because some missions might be missing/cut so we cant simply break the loop
            for (int fileIndex = 0; fileIndex < 50; fileIndex++)
            {
                if (File.Exists(path + "/gameplay_mission_classes[" + fileIndex + "].ps3"))
                {
                    missionIds.Add(fileIndex);
                }
            }


            foreach(int missionIndex in missionIds)
            {
                RatchetMission mission = new RatchetMission();
                FileStream fs = File.OpenRead(path + "/gameplay_mission_classes[" + missionIndex + "].ps3");
                FileStream vfs = File.OpenRead(path + "/gameplay_mission_classes[" + missionIndex + "].vram");
                Console.WriteLine("Loading mission " + missionIndex);

                byte[] header = ReadBlock(fs, 0, 0x0c);
                uint spawnablesCount = BAToUInt32(ReadBlock(fs, 0x00, 4), 0);

                byte[] idBlock = ReadBlock(fs, 0x10, spawnablesCount * 8);

                #region Spawnables
                mission.spawnableModels = new List<RatchetModel_General>();
                for (int x = 0; x < spawnablesCount; x++)
                {
                    RatchetModel_General model = new RatchetModel_General();
                    model.modelType = ModelType.Spawnable;

                    model.modelID = BAToShort(idBlock, (uint)(x * 8) + 2);
                    model.offset = BAToUInt32(idBlock, (uint)(x * 8) + 4);

                    if (model.offset != 0)
                    {
                        uint modelHeadSize = BAToUInt32(ReadBlock(fs, model.offset, 4), 0);

                        if (modelHeadSize > 0)
                        {
                            byte[] headBlock = ReadBlock(fs, model.offset, modelHeadSize + 0x20); //Head + objectDetails
                            uint objectPtr = BAToUInt32(headBlock, 0);
                            model.animationsCount = headBlock[0x0C];
                            //(0x04)null
                            //(0x08)[count for 0x14 and 0x18][unknown][unknown][unknown]
                            //(0x0c)[animation count][unknown][unknown][unknown]
                            //(0x10)Pointer to list
                            //(0x14)Pointer to skeleton joints (0x40 structure)
                            //(0x18)Pointer to skeleton somethings (0x16 structure)
                            //(0x1C)Pointer to left arm animations (Ratchet)
                            //(0x20)null
                            model.size = BAToFloat(headBlock, 0x24);
                            uint texCount = BAToUInt32(headBlock, objectPtr + 0x00);
                            uint otherCount = BAToUInt32(headBlock, objectPtr + 0x04);
                            uint texBlockPointer = BAToUInt32(headBlock, objectPtr + 0x08);
                            uint otherBlockPointer = BAToUInt32(headBlock, objectPtr + 0x0C);
                            uint vertPointer = BAToUInt32(headBlock, objectPtr + 0x10);
                            uint indexPointer = BAToUInt32(headBlock, objectPtr + 0x14);
                            model.vertexCount = BAToUInt16(headBlock, objectPtr + 0x18);
                            //(0x1A)count
                            //(0x1C)count
                            //(0x1E)null


                            //model.rawData = getModelDataRaw(model.modelType, model.modelID, path + "/" + fileName);

                            model.animPointer = new List<uint>();
                            for (uint i = 0; i < model.animationsCount; i++)
                            {
                                model.animPointer.Add(BAToUInt32(headBlock, (i * sizeof(uint)) + 0x48));
                            }

                            uint texElemSize = 0x10;
                            byte[] texBlock = ReadBlock(fs, model.offset + texBlockPointer, texCount * texElemSize);
                            model.textureConfig = new List<RatchetTexture_Model>();
                            model.faceCount = 0;
                            for (uint t = 0; t < texCount; t++)
                            {
                                RatchetTexture_Model modTex = new RatchetTexture_Model();
                                modTex.ID = BAToUInt32(texBlock, (t * texElemSize) + 0x0);
                                modTex.start = BAToUInt32(texBlock, (t * texElemSize) + 0x04);
                                modTex.size = BAToUInt32(texBlock, (t * texElemSize) + 0x08);
                                model.textureConfig.Add(modTex);
                                model.faceCount += modTex.size;
                            }


                            //Flip endianness of vertex array float[vert_x, vert_y, vert_z, norm_x, norm_y, norm_z, uv_u, uv_v, reserved reserved]
                            uint vertElemSize = 0x28;
                            uint vertBuffSize = model.vertexCount * vertElemSize;
                            byte[] vertBlock = ReadBlock(fs, model.offset + vertPointer, vertBuffSize);
                            model.vertBuff = new List<float>();
                            for (uint i = 0; i < model.vertexCount; i++)
                            {
                                model.vertBuff.Add(BAToFloat(vertBlock, (i * vertElemSize) + 0x00));    //Vertx
                                model.vertBuff.Add(BAToFloat(vertBlock, (i * vertElemSize) + 0x04));    //Verty
                                model.vertBuff.Add(BAToFloat(vertBlock, (i * vertElemSize) + 0x08));    //Vertz
                                model.vertBuff.Add(BAToFloat(vertBlock, (i * vertElemSize) + 0x0C));    //Nomrx
                                model.vertBuff.Add(BAToFloat(vertBlock, (i * vertElemSize) + 0x10));    //Normy
                                model.vertBuff.Add(BAToFloat(vertBlock, (i * vertElemSize) + 0x14));    //Normz
                                model.vertBuff.Add(BAToFloat(vertBlock, (i * vertElemSize) + 0x18));    //UVu
                                model.vertBuff.Add(BAToFloat(vertBlock, (i * vertElemSize) + 0x1C));    //UVv
                            }

                            //Flip endianness of index array
                            byte[] indiceBuff = ReadBlock(fs, model.offset + indexPointer, model.faceCount * sizeof(ushort));
                            model.indiceBuff = new List<ushort>();
                            for (uint i = 0; i < model.faceCount; i++)
                            {
                                model.indiceBuff.Add(BAToUInt16(indiceBuff, i * sizeof(ushort)));
                            }
                        }

                    }
                    mission.spawnableModels.Add(model);
                }
                #endregion

                #region Textures
                int textureCount = (int) BAToUInt32(ReadBlock(fs, 0x04, 4), 0);
                uint texturesPointer = BAToUInt32(ReadBlock(fs, 0x08, 4), 0);
                mission.textures = new List<RatchetTexture_General>();
                RatchetTexture_General texture = new RatchetTexture_General();
                for (int x = 0; x < textureCount; x++)
                {

                    texture.texHeader = ReadBlock(fs, texturesPointer + (uint)(0x24 * x), 0x24);
                    uint nextTexturePointer = x < textureCount - 1 ? ReadUInt32(ReadBlock(fs, texturesPointer + (uint)(0x24 * (x + 1)), 0x4), 0) : vfs != null ? (uint)vfs.Length : 0;
                    uint currentTexturePointer = ReadUInt32(texture.texHeader, 0);
                    texture.ID = x;
                    texture.width = BAToShort(texture.texHeader, 0x18);
                    texture.height = BAToShort(texture.texHeader, 0x1A);
                    texture.texData = vfs != null ? ReadBlock(vfs, currentTexturePointer, nextTexturePointer - currentTexturePointer) : null;
                    texture.reverseRGB = false;
                    mission.textures.Add(texture);
                }
                #endregion
                DataStore.missions.Add(mission);
                fs.Close();
            }
        }
    }
}
