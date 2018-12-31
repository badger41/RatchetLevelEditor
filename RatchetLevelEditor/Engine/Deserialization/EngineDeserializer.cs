using OpenTK;
using RatchetLevelEditor.Engine.Models;
using RatchetLevelEditor.Engine.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataFunctions;
using static RatchetModel;
using static RatchetTexture;
using static ModelParser;

namespace RatchetLevelEditor.Engine.Deserialization
{
    class EngineDeserializer
    {
        static FileStream efs = null;
        static FileStream vfs = null;
        static int racNum;


        //Dictionary to define the mapping of each index of the file and its respective variable
        public static Dictionary<int, dynamic> headerMap;


        public static void createHeaderMap(int racNum)
        {
            switch (racNum)
            {
                case 1:
                    break;
                case 2:
                case 3:
                    EngineHeader engineHeader = new EngineHeader(efs, (uint) racNum);
                    DataStoreEngine.engineHeader = engineHeader;
                    headerMap = new Dictionary<int, dynamic>()
                    {
                        //Get the counts of things first, other stuff below relies on these being populated
                        //{0x20, new Action<dynamic>(i => { getDataCount(ref DataStoreEngine.engineHeader.levelModelsCount, 0x20); }) },
                        //{0x28, new Action<dynamic>(i => { getDataCount(ref DataStoreEngine.engineHeader.levelObjectCount, 0x28); }) },
                        //{0x30, new Action<dynamic>(i => { getDataCount(ref DataStoreEngine.engineHeader.sceneryModelsCount, 0x30); }) },
                        //{0x38, new Action<dynamic>(i => { getDataCount(ref DataStoreEngine.engineHeader.sceneryObjectsCount, 0x38); }) },
                        //{0x58, new Action<dynamic>(i => { getDataCount(ref DataStoreEngine.engineHeader.textureCount, 0x58); }) },
                        //{0x60, new Action<dynamic>(i => { getDataCount(ref DataStoreEngine.engineHeader.lightingLevel, 0x60); }) },
                        //{0x6C, new Action<dynamic>(i => { getDataCount(ref DataStoreEngine.engineHeader.textureConfigMenuCount, 0x6C); }) },

                        {0x00, new Action<dynamic>(i => { deSerializeSpawnables(ref DataStoreEngine.spawnableModels, 0x00, racNum); }) },
                        {0x04, new Action<dynamic>(i => { getUnknownHeaderData(0x04); }) },
                        {0x08, new Action<dynamic>(i => { getUnknownHeaderData(0x08); }) },
                        {0x0C, new Action<dynamic>(i => { getUnknownHeaderData(0x0C); }) },
                        {0x10, new Action<dynamic>(i => { getUnknownHeaderData(0x10); }) },
                        {0x14, new Action<dynamic>(i => { deSerializeTerrainCollision(ref DataStoreEngine.collVertBuff, ref DataStoreEngine.collIndBuff, 0x14, racNum); }) },
                        {0x18, new Action<dynamic>(i => { getUnknownHeaderData(0x18); }) },
                        {0x1C, new Action<dynamic>(i => { deSerializeTieModels(ref DataStoreEngine.levelModels, ref DataStoreEngine.tieModels, 0x1C, racNum); }) },

                        {0x2C, new Action<dynamic>(i => { deSerializeShrubModelss(ref DataStoreEngine.sceneryModels, ref DataStoreEngine.shrubModels, 0x2C, racNum); }) },
                        {0x34, new Action<dynamic>(i => { deSerializeShrubs(ref DataStoreEngine.sceneryObjects, 0x34, racNum); }) },
                        {0x3C, new Action<dynamic>(i => { deSerializeTerrain(ref DataStoreEngine.terrainModel, 0x3C, racNum); }) },
                        {0x40, new Action<dynamic>(i => { getUnknownHeaderData(0x40); }) },
                        {0x44, new Action<dynamic>(i => { getUnknownHeaderData(0x44); }) },
                        {0x48, new Action<dynamic>(i => { getUnknownHeaderData(0x48); }) },
                        {0x4C, new Action<dynamic>(i => { getUnknownHeaderData(0x4C); }) },
                        {0x50, new Action<dynamic>(i => { getUnknownHeaderData(0x50); }) },
                        {0x54, new Action<dynamic>(i => { deSerializeTextures(ref DataStoreEngine.textures, 0x54, racNum); }) },
                        {0x5C, new Action<dynamic>(i => { getUnknownHeaderData(0x5C); }) },
                        {0x64, new Action<dynamic>(i => { getUnknownHeaderData(0x64); }) },
                        {0x68, new Action<dynamic>(i => { getUnknownHeaderData(0x68); }) },
                        {0x70, new Action<dynamic>(i => { getUnknownHeaderData(0x70); }) },
                        {0x74, new Action<dynamic>(i => { getUnknownHeaderData(0x74); }) },
                        {0x78, new Action<dynamic>(i => { getUnknownHeaderData(0x78); }) },

                        {0x24, new Action<dynamic>(i => { deSerializeTies(ref DataStoreEngine.levelObjects, 0x24, racNum); }) },
                    };
                    break;
                case 4:
                    break;


            }
        }

        public static void parseEngine(string path, int racNum)
        {
            EngineDeserializer.racNum = racNum;

            Console.WriteLine("Parsing engine.ps3");

            //Initialize our filestream
            if (File.Exists(path + "/engine.ps3"))
                efs = File.OpenRead(path + "/engine.ps3");

            if (File.Exists(path + "/vram.ps3"))
                vfs = File.OpenRead(path + "/vram.ps3");

            DataStoreEngine.engineHeader = new EngineHeader(efs, (uint)racNum);

            //Initialize the unknown gameplay data list;
            DataStoreEngine.unknownEngineData = new List<UnknownDataModel>();

            uint[] ignoreIndices;

            //Create our header map complete with functions each index is responsible for
            createHeaderMap(racNum);

            //Determine which header length we have depending on which ratchet game
            int offsetCount = headerMap.Count;

            //Load our header block, we need this to pass offsets and also build the offset list
            byte[] headerBlock = ReadBlock(efs, 0, 0xA0);

            foreach(KeyValuePair<int, dynamic> header in headerMap)
            {
                UnknownDataIndex data = new UnknownDataIndex { offset = (uint) header.Key, pointer = ReadUInt32(headerBlock, (int)header.Key) };
                DataStoreEngine.unhandledOffsets.Add(data);
            }

            ////Loop through the header and load all of our offsets
            //for (int i = 0; i < offsetCount; i++)
            //{
            //    uint offset = (uint)i * 4;
            //    UnknownDataIndex data = new UnknownDataIndex { offset = offset, pointer = ReadUInt32(headerBlock, (int)offset) };
            //    DataStoreEngine.unhandledOffsets.Add(data);
            //}

            //Sort the offsets so we can easily get the next offset to determine size of our unknown data
            DataStoreEngine.unhandledOffsets = DataStoreEngine.unhandledOffsets.OrderBy(x => x.pointer).ToList();

            //The important part
            //This loops through the headerMap and invokes the action that is required for that index.
            //This is what makes it easy to pivot and change how a certain bit of data is processed without much code change
            foreach (KeyValuePair<int, dynamic> header in headerMap)
            {
                headerMap[header.Key].DynamicInvoke(header); // invoke appropriate delegate
            }

            //Close the stream
            efs.Close();

            //EngineSerializer.serialize(path, racNum);
        }

        //Any unhandled engine element is funneled through this method.
        //It takes the chunk of data and loads it into a byte array
        //We will still need it when serializing the gameplay file later
        public static void getUnknownHeaderData(int index)
        {
            uint pointer = ReadUInt32(ReadBlock(efs, (uint)index, 4), 0);
            UnknownDataIndex currentOffset = DataStoreEngine.unhandledOffsets.Where(x => x.pointer == pointer).FirstOrDefault();
            int offsetIndex = DataStoreEngine.unhandledOffsets.IndexOf(currentOffset);

            if (DataStoreEngine.unhandledOffsets.Last().Equals(currentOffset))
            {
                UnknownDataModel data = new UnknownDataModel() { index = (uint)index, data = ReadBlock(efs, pointer, ((uint)efs.Length - pointer)) };
                DataStoreEngine.unknownEngineData.Add(data);
            }
            else
            {
                UnknownDataModel data = new UnknownDataModel() { index = (uint)index, data = ReadBlock(efs, pointer, (DataStoreEngine.unhandledOffsets[offsetIndex + 1].pointer - pointer)) };
                DataStoreEngine.unknownEngineData.Add(data);
            }
        }

        public static void getDataCount(ref uint dataCount, int index)
        {
            dataCount = ReadUInt32(efs, (uint) index);
        }

        #region Spawnable Models
        public static void deSerializeSpawnables(ref List<RatchetModel_General> spawnableModels, int index, int racNum)
        {
            uint pointer = BAToUInt32(ReadBlock(efs, (uint)index, 4), 0);
            uint spawnablesCount = BAToUInt32(ReadBlock(efs, pointer, 4), 0);
            byte[] idBlock = ReadBlock(efs, pointer + 4, spawnablesCount * 8);

            for (int x = 0; x < spawnablesCount; x++)
            {
                RatchetModel_General model = new RatchetModel_General();
                model.modelType = ModelType.Spawnable;

                model.modelID = BAToShort(idBlock, (uint)(x * 8) + 2);
                model.offset = BAToUInt32(idBlock, (uint)(x * 8) + 4);

                if (model.offset != 0)
                {
                    model.rawData = getModelDataRaw(model.modelType, efs, model.modelID);
                    uint modelHeadSize = BAToUInt32(ReadBlock(efs, model.offset, 4), 0);

                    if (modelHeadSize > 0)
                    {
                        byte[] headBlock = ReadBlock(efs, model.offset, modelHeadSize + 0x20); //Head + objectDetails
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


                        

                        model.animPointer = new List<uint>();
                        for (uint i = 0; i < model.animationsCount; i++)
                        {
                            model.animPointer.Add(BAToUInt32(headBlock, (i * sizeof(uint)) + 0x48));
                        }

                        uint texElemSize = 0x10;
                        byte[] texBlock = ReadBlock(efs, model.offset + texBlockPointer, texCount * texElemSize);
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
                        byte[] vertBlock = ReadBlock(efs, model.offset + vertPointer, vertBuffSize);
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
                        byte[] indiceBuff = ReadBlock(efs, model.offset + indexPointer, model.faceCount * sizeof(ushort));
                        model.indiceBuff = new List<ushort>();
                        for (uint i = 0; i < model.faceCount; i++)
                        {
                            model.indiceBuff.Add(BAToUInt16(indiceBuff, i * sizeof(ushort)));
                        }
                    }

                }
                spawnableModels.Add(model);
            }

        }

        #endregion

        #region Tie Models
        public static void deSerializeTieModels(ref List<RatchetModel_General> levelModels, ref List<TieModel> tieModels, int index, int racNum)
        {
            uint pointer = ReadUInt32(ReadBlock(efs, (uint)index, 4), 0);
            uint levelElemSize = 0x40;
            byte[] levelBlock = ReadBlock(efs, pointer, DataStoreEngine.engineHeader.levelModelsCount * levelElemSize);

            for (uint x = 0; x < DataStoreEngine.engineHeader.levelModelsCount; x++)
            {
                uint offset = (x * levelElemSize);

                TieModel tModel = new TieModel()
                {
                    ind_00 = BAToFloat(levelBlock, offset + 0x00),
                    ind_04 = BAToFloat(levelBlock, offset + 0x04),
                    ind_08 = BAToFloat(levelBlock, offset + 0x08),
                    ind_0C = BAToFloat(levelBlock, offset + 0x0C),

                    ptr_vert = BAToUInt32(levelBlock, offset + 0x10),
                    ptr_uv = BAToUInt32(levelBlock, offset + 0x14),
                    ptr_ind = BAToUInt32(levelBlock, offset + 0x18),
                    ptr_tex = BAToUInt32(levelBlock, offset + 0x1C),

                    ind_20 = BAToUInt32(levelBlock, offset + 0x20),
                    vertexCount = BAToInt32(levelBlock, offset + 0x24),
                    texCount = BAToShort(levelBlock, offset + 0x28),
                    ind_2A = BAToShort(levelBlock, offset + 0x2A),
                    ind_2C = BAToFloat(levelBlock, offset + 0x2C),

                    modelId = BAToShort(levelBlock, offset + 0x30),
                    ind_32 = BAToShort(levelBlock, offset + 0x32),
                    ind_34 = BAToUInt32(levelBlock, offset + 0x34),
                    ind_38 = BAToUInt32(levelBlock, offset + 0x38),
                    rgba = BAToUInt32(levelBlock, offset + 0x3C),

                };

                tModel.vertexData = ReadBlock(efs, tModel.ptr_vert, tModel.ptr_uv - tModel.ptr_vert);
                tModel.uVData = ReadBlock(efs, tModel.ptr_uv, tModel.ptr_ind - tModel.ptr_uv);
                tModel.textureData = ReadBlock(efs, tModel.ptr_tex, tModel.ptr_vert - tModel.ptr_tex);
                byte[] indexData = new byte[0];
                ushort compareVertId = 0;
                int vertOffset = 0;
                while(compareVertId <= tModel.vertexCount)
                {
                    //We have to read an entire line because the comparison is made on 0x02 to determine if we have to stop
                    byte[] vertexLine = ReadBlock(efs, (uint)(tModel.ptr_ind + (vertOffset * 0x04)), 4);

                    ushort vertexId1 = ReadUInt16(vertexLine, 0);
                    ushort vertexId2 = ReadUInt16(vertexLine, 2);

                    int currentOffset = indexData.Length;
                    Array.Resize(ref indexData, indexData.Length + 0x04);
                    WriteUint16(ref indexData, currentOffset + 0x00, vertexId1);
                    WriteUint16(ref indexData, currentOffset + 0x02, vertexId2);

                    compareVertId = vertexId2;
                    //If the id we found matches the count, we peek at the next value to see if its also matching
                    //If not, we assume that this is the end of the list and we break the loop
                    if(compareVertId == tModel.vertexCount - 1)
                    {
                        byte[] nextVertexLine = ReadBlock(efs, (uint)(tModel.ptr_ind + (vertOffset + 1* 0x04)), 4);
                        ushort nextVertexId1 = ReadUInt16(nextVertexLine, 0);
                        ushort nextVertexId2 = ReadUInt16(nextVertexLine, 2);

                        if (nextVertexId2 != compareVertId)
                            break;
                    }

                    vertOffset++;
                }

                tModel.indexData = indexData;

                RatchetModel_General model = new RatchetModel_General();

                model.modelType = ModelType.Level;
                model.size = 0.5f;

                uint vertPointer = BAToUInt32(levelBlock, (x * levelElemSize) + 0x10);
                uint UVPointer = BAToUInt32(levelBlock, (x * levelElemSize) + 0x14);
                uint indicePointer = BAToUInt32(levelBlock, (x * levelElemSize) + 0x18);
                uint texPointer = BAToUInt32(levelBlock, (x * levelElemSize) + 0x1C);
                model.vertexCount = (ushort)BAToUInt32(levelBlock, (x * levelElemSize) + 0x24);
                ushort texCount = BAToUInt16(levelBlock, (x * levelElemSize) + 0x28);
                model.modelID = BAToShort(levelBlock, (x * levelElemSize) + 0x30);

                model.offset = (uint)(x * 0x40) + pointer;
                model.modelType = ModelType.Level;

                uint texElemSize = 0x18;
                byte[] texBlock = ReadBlock(efs, texPointer, texCount * texElemSize);
                model.textureConfig = new List<RatchetTexture_Model>();
                model.faceCount = 0;
                for (uint t = 0; t < texCount; t++)
                {
                    RatchetTexture_Model dlt = new RatchetTexture_Model();
                    dlt.ID = BAToUInt32(texBlock, (t * texElemSize) + 0x00);
                    dlt.start = BAToUInt32(texBlock, (t * texElemSize) + 0x08);
                    dlt.size = BAToUInt32(texBlock, (t * texElemSize) + 0x0C);
                    model.textureConfig.Add(dlt);
                    model.faceCount += dlt.size;
                }

                //Flip endianness of vertex array float[vert_x, vert_y, vert_z, norm_x, norm_y, norm_z, uv_u, uv_v, reserved reserved]
                uint vertElemSize = 0x18;
                uint vertBuffSize = model.vertexCount * vertElemSize;
                byte[] vertBlock = ReadBlock(efs, vertPointer, vertBuffSize);

                uint UVElemsize = 0x08;
                uint UVBuffSize = model.vertexCount * UVElemsize;
                byte[] UVBlock = ReadBlock(efs, UVPointer, UVBuffSize);


                model.vertBuff = new List<float>();
                for (uint i = 0; i < model.vertexCount; i++)
                {
                    model.vertBuff.Add(BAToFloat(vertBlock, (i * vertElemSize) + 0x00));    //vertx
                    model.vertBuff.Add(BAToFloat(vertBlock, (i * vertElemSize) + 0x04));    //verty
                    model.vertBuff.Add(BAToFloat(vertBlock, (i * vertElemSize) + 0x08));    //vertz
                    model.vertBuff.Add(BAToFloat(vertBlock, (i * vertElemSize) + 0x0C));    //normx
                    model.vertBuff.Add(BAToFloat(vertBlock, (i * vertElemSize) + 0x10));    //normy
                    model.vertBuff.Add(BAToFloat(vertBlock, (i * vertElemSize) + 0x14));    //normz

                    model.vertBuff.Add(BAToFloat(UVBlock, (i * UVElemsize) + 0x00));    //UVu
                    model.vertBuff.Add(BAToFloat(UVBlock, (i * UVElemsize) + 0x04));    //UVv
                }

                //Flip endianness of index array
                byte[] indiceBuff = ReadBlock(efs, indicePointer, model.faceCount * sizeof(ushort));
                model.indiceBuff = new List<ushort>();
                for (uint i = 0; i < model.faceCount; i++)
                {
                    model.indiceBuff.Add(BAToUInt16(indiceBuff, i * sizeof(ushort)));
                }

                levelModels.Add(model);
                tModel.modelData = model;
                tieModels.Add(tModel);
            }
        }
        #endregion

        #region Shrub Models
        public static void deSerializeShrubModels(ref List<RatchetModel_General> sceneryModels, ref List<ShrubModel> shrubModels, int index, int racNum)
        {
            uint pointer = ReadUInt32(ReadBlock(efs, (uint)index, 4), 0);
            uint sceneElemSize = 0x40;
            byte[] sceneBlock = ReadBlock(efs, pointer, DataStoreEngine.engineHeader.sceneryModelsCount * sceneElemSize);

            for (uint x = 0; x < DataStoreEngine.engineHeader.sceneryModelsCount; x++)
            {
                uint offset = (x * sceneElemSize);

                ShrubModel shrubModel = new ShrubModel()
                {
                    ind_00 = BAToFloat(sceneBlock, offset + 0x00),
                    ind_04 = BAToFloat(sceneBlock, offset + 0x04),
                    ind_08 = BAToFloat(sceneBlock, offset + 0x08),
                    ind_0C = BAToFloat(sceneBlock, offset + 0x0C),

                    ptr_vert = BAToUInt32(sceneBlock, offset + 0x10),
                    ptr_uv = BAToUInt32(sceneBlock, offset + 0x14),
                    ptr_ind = BAToUInt32(sceneBlock, offset + 0x18),
                    ptr_tex = BAToUInt32(sceneBlock, offset + 0x1C),

                    ind_20 = BAToUInt32(sceneBlock, offset + 0x20),
                    vertexCount = BAToInt32(sceneBlock, offset + 0x24),
                    texCount = BAToShort(sceneBlock, offset + 0x28),
                    ind_2A = BAToShort(sceneBlock, offset + 0x2A),
                    ind_2C = BAToFloat(sceneBlock, offset + 0x2C),

                    modelId = BAToShort(sceneBlock, offset + 0x30),
                    ind_32 = BAToShort(sceneBlock, offset + 0x32),
                    ind_34 = BAToUInt32(sceneBlock, offset + 0x34),
                    ind_38 = BAToUInt32(sceneBlock, offset + 0x38),
                    rgba = BAToUInt32(sceneBlock, offset + 0x3C),

                };

                shrubModel.vertexData = ReadBlock(efs, shrubModel.ptr_vert, shrubModel.ptr_uv - shrubModel.ptr_vert);
                shrubModel.uVData = ReadBlock(efs, shrubModel.ptr_uv, shrubModel.ptr_ind - shrubModel.ptr_uv);
                shrubModel.textureData = ReadBlock(efs, shrubModel.ptr_tex, shrubModel.ptr_vert - shrubModel.ptr_tex);
                byte[] indexData = new byte[0];
                ushort compareVertId = 0;
                int vertOffset = 0;
                while (compareVertId <= shrubModel.vertexCount)
                {
                    //We have to read an entire line because the comparison is made on 0x02 to determine if we have to stop
                    byte[] vertexLine = ReadBlock(efs, (uint)(shrubModel.ptr_ind + (vertOffset * 0x04)), 4);

                    ushort vertexId1 = ReadUInt16(vertexLine, 0);
                    ushort vertexId2 = ReadUInt16(vertexLine, 2);

                    int currentOffset = indexData.Length;
                    Array.Resize(ref indexData, indexData.Length + 0x04);
                    WriteUint16(ref indexData, currentOffset + 0x00, vertexId1);
                    WriteUint16(ref indexData, currentOffset + 0x02, vertexId2);

                    compareVertId = vertexId2;
                    //If the id we found matches the count, we peek at the next value to see if its also matching
                    //If not, we assume that this is the end of the list and we break the loop
                    if (compareVertId == shrubModel.vertexCount - 1)
                    {
                        byte[] nextVertexLine = ReadBlock(efs, (uint)(shrubModel.ptr_ind + (vertOffset + 1 * 0x04)), 4);
                        ushort nextVertexId1 = ReadUInt16(nextVertexLine, 0);
                        ushort nextVertexId2 = ReadUInt16(nextVertexLine, 2);

                        if (nextVertexId2 != compareVertId)
                            break;
                    }

                    vertOffset++;
                }

                shrubModel.indexData = indexData;

                RatchetModel_General model = new RatchetModel_General();

                model.modelType = ModelType.Scenery;
                model.size = 0.5f;

                uint vertPointer = BAToUInt32(sceneBlock, (x * sceneElemSize) + 0x10);
                uint UVPointer = BAToUInt32(sceneBlock, (x * sceneElemSize) + 0x14);
                uint indicePointer = BAToUInt32(sceneBlock, (x * sceneElemSize) + 0x18);
                uint texPointer = BAToUInt32(sceneBlock, (x * sceneElemSize) + 0x1C);
                model.vertexCount = (ushort)BAToUInt32(sceneBlock, (x * sceneElemSize) + 0x24);
                ushort texCount = BAToUInt16(sceneBlock, (x * sceneElemSize) + 0x28);
                model.modelID = BAToShort(sceneBlock, (x * sceneElemSize) + 0x30);

                uint texElemSize = 0x10;
                byte[] texBlock = ReadBlock(efs, texPointer, texCount * texElemSize);
                model.textureConfig = new List<RatchetTexture_Model>();
                model.faceCount = 0;
                for (uint t = 0; t < texCount; t++)
                {
                    RatchetTexture_Model dlt = new RatchetTexture_Model();
                    dlt.ID = BAToUInt32(texBlock, (t * texElemSize) + 0x00);
                    dlt.start = BAToUInt32(texBlock, (t * texElemSize) + 0x04);
                    dlt.size = BAToUInt32(texBlock, (t * texElemSize) + 0x08);
                    model.textureConfig.Add(dlt);
                    model.faceCount += dlt.size;
                }

                //Flip endianness of vertex array float[vert_x, vert_y, vert_z, norm_x, norm_y, norm_z, uv_u, uv_v, reserved reserved]
                uint vertElemSize = 0x18;
                uint vertBuffSize = model.vertexCount * vertElemSize;
                byte[] vertBlock = ReadBlock(efs, vertPointer, vertBuffSize);

                uint UVElemsize = 0x08;
                uint UVBuffSize = model.vertexCount * UVElemsize;
                byte[] UVBlock = ReadBlock(efs, UVPointer, UVBuffSize);


                model.vertBuff = new List<float>();
                for (uint i = 0; i < model.vertexCount; i++)
                {
                    model.vertBuff.Add(BAToFloat(vertBlock, (i * vertElemSize) + 0x00));    //vertx
                    model.vertBuff.Add(BAToFloat(vertBlock, (i * vertElemSize) + 0x04));    //verty
                    model.vertBuff.Add(BAToFloat(vertBlock, (i * vertElemSize) + 0x08));    //vertz
                    model.vertBuff.Add(BAToFloat(vertBlock, (i * vertElemSize) + 0x0C));    //normx
                    model.vertBuff.Add(BAToFloat(vertBlock, (i * vertElemSize) + 0x10));    //normy
                    model.vertBuff.Add(BAToFloat(vertBlock, (i * vertElemSize) + 0x14));    //normz

                    model.vertBuff.Add(BAToFloat(UVBlock, (i * UVElemsize) + 0x00));    //UVu
                    model.vertBuff.Add(BAToFloat(UVBlock, (i * UVElemsize) + 0x04));    //UVv
                }

                sceneryModels.Add(model);
                //shrubModel.modelData = model;
                //shrubModels.Add(shrubModel);
            }
        }

        public static void deSerializeShrubModelss(ref List<RatchetModel_General> sceneryModels, ref List<ShrubModel> shrubModels, int index, int racNum)
        {
            uint pointer = ReadUInt32(ReadBlock(efs, (uint)index, 4), 0);
            uint sceneElemSize = 0x40;
            byte[] sceneBlock = ReadBlock(efs, pointer, DataStoreEngine.engineHeader.sceneryModelsCount * sceneElemSize);

            for (uint x = 0; x < DataStoreEngine.engineHeader.sceneryModelsCount; x++)
            {
                uint offset = (x * sceneElemSize);

                ShrubModel shrubModel = new ShrubModel()
                {
                    ind_00 = BAToFloat(sceneBlock, offset + 0x00),
                    ind_04 = BAToFloat(sceneBlock, offset + 0x04),
                    ind_08 = BAToFloat(sceneBlock, offset + 0x08),
                    ind_0C = BAToFloat(sceneBlock, offset + 0x0C),

                    ptr_vert = BAToUInt32(sceneBlock, offset + 0x10),
                    ptr_uv = BAToUInt32(sceneBlock, offset + 0x14),
                    ptr_ind = BAToUInt32(sceneBlock, offset + 0x18),
                    ptr_tex = BAToUInt32(sceneBlock, offset + 0x1C),

                    ind_20 = BAToUInt32(sceneBlock, offset + 0x20),
                    vertexCount = BAToInt32(sceneBlock, offset + 0x24),
                    texCount = BAToShort(sceneBlock, offset + 0x28),
                    ind_2A = BAToShort(sceneBlock, offset + 0x2A),
                    ind_2C = BAToFloat(sceneBlock, offset + 0x2C),

                    modelId = BAToShort(sceneBlock, offset + 0x30),
                    ind_32 = BAToShort(sceneBlock, offset + 0x32),
                    ind_34 = BAToUInt32(sceneBlock, offset + 0x34),
                    ind_38 = BAToUInt32(sceneBlock, offset + 0x38),
                    rgba = BAToUInt32(sceneBlock, offset + 0x3C),

                };

                shrubModel.vertexData = ReadBlock(efs, shrubModel.ptr_vert, shrubModel.ptr_uv - shrubModel.ptr_vert);
                shrubModel.uVData = ReadBlock(efs, shrubModel.ptr_uv, shrubModel.ptr_ind - shrubModel.ptr_uv);
                shrubModel.textureData = ReadBlock(efs, shrubModel.ptr_tex, shrubModel.ptr_vert - shrubModel.ptr_tex);
                byte[] indexData = new byte[0];
                ushort compareVertId = 0;
                int vertOffset = 0;
                while (compareVertId <= shrubModel.vertexCount)
                {
                    //We have to read an entire line because the comparison is made on 0x02 to determine if we have to stop
                    byte[] vertexLine = ReadBlock(efs, (uint)(shrubModel.ptr_ind + (vertOffset * 0x04)), 4);

                    ushort vertexId1 = ReadUInt16(vertexLine, 0);
                    ushort vertexId2 = ReadUInt16(vertexLine, 2);

                    int currentOffset = indexData.Length;
                    Array.Resize(ref indexData, indexData.Length + 0x04);
                    WriteUint16(ref indexData, currentOffset + 0x00, vertexId1);
                    WriteUint16(ref indexData, currentOffset + 0x02, vertexId2);

                    compareVertId = vertexId2;
                    //If the id we found matches the count, we peek at the next value to see if its also matching
                    //If not, we assume that this is the end of the list and we break the loop
                    if (compareVertId == shrubModel.vertexCount - 1)
                    {
                        byte[] nextVertexLine = ReadBlock(efs, (uint)(shrubModel.ptr_ind + (vertOffset + 1 * 0x04)), 4);
                        ushort nextVertexId1 = ReadUInt16(nextVertexLine, 0);
                        ushort nextVertexId2 = ReadUInt16(nextVertexLine, 2);

                        if (nextVertexId2 != compareVertId)
                            break;
                    }

                    vertOffset++;
                }

                shrubModel.indexData = indexData;

                RatchetModel_General model = new RatchetModel_General();
                model.modelType = ModelType.Scenery;
                model.size = 0.5f;

                uint vertPointer = BAToUInt32(sceneBlock, (x * sceneElemSize) + 0x10);
                uint UVPointer = BAToUInt32(sceneBlock, (x * sceneElemSize) + 0x14);
                uint indicePointer = BAToUInt32(sceneBlock, (x * sceneElemSize) + 0x18);
                uint texPointer = BAToUInt32(sceneBlock, (x * sceneElemSize) + 0x1C);
                model.vertexCount = (ushort)BAToUInt32(sceneBlock, (x * sceneElemSize) + 0x24);
                ushort texCount = BAToUInt16(sceneBlock, (x * sceneElemSize) + 0x28);
                model.modelID = BAToShort(sceneBlock, (x * sceneElemSize) + 0x30);

                uint texElemSize = 0x10;
                byte[] texBlock = ReadBlock(efs, texPointer, texCount * texElemSize);
                model.textureConfig = new List<RatchetTexture_Model>();
                model.faceCount = 0;
                for (uint t = 0; t < texCount; t++)
                {
                    RatchetTexture_Model dlt = new RatchetTexture_Model();
                    dlt.ID = BAToUInt32(texBlock, (t * texElemSize) + 0x00);
                    dlt.start = BAToUInt32(texBlock, (t * texElemSize) + 0x04);
                    dlt.size = BAToUInt32(texBlock, (t * texElemSize) + 0x08);
                    model.textureConfig.Add(dlt);
                    model.faceCount += dlt.size;
                }

                //Flip endianness of vertex array float[vert_x, vert_y, vert_z, norm_x, norm_y, norm_z, uv_u, uv_v, reserved reserved]
                uint vertElemSize = 0x18;
                uint vertBuffSize = model.vertexCount * vertElemSize;
                byte[] vertBlock = ReadBlock(efs, vertPointer, vertBuffSize);

                uint UVElemsize = 0x08;
                uint UVBuffSize = model.vertexCount * UVElemsize;
                byte[] UVBlock = ReadBlock(efs, UVPointer, UVBuffSize);


                model.vertBuff = new List<float>();
                for (uint i = 0; i < model.vertexCount; i++)
                {
                    model.vertBuff.Add(BAToFloat(vertBlock, (i * vertElemSize) + 0x00));    //vertx
                    model.vertBuff.Add(BAToFloat(vertBlock, (i * vertElemSize) + 0x04));    //verty
                    model.vertBuff.Add(BAToFloat(vertBlock, (i * vertElemSize) + 0x08));    //vertz
                    model.vertBuff.Add(BAToFloat(vertBlock, (i * vertElemSize) + 0x0C));    //normx
                    model.vertBuff.Add(BAToFloat(vertBlock, (i * vertElemSize) + 0x10));    //normy
                    model.vertBuff.Add(BAToFloat(vertBlock, (i * vertElemSize) + 0x14));    //normz

                    model.vertBuff.Add(BAToFloat(UVBlock, (i * UVElemsize) + 0x00));    //UVu
                    model.vertBuff.Add(BAToFloat(UVBlock, (i * UVElemsize) + 0x04));    //UVv
                }

                //Flip endianness of index array
                byte[] indiceBuff = ReadBlock(efs, indicePointer, model.faceCount * sizeof(ushort));
                model.indiceBuff = new List<ushort>();
                for (uint i = 0; i < model.faceCount; i++)
                {
                    model.indiceBuff.Add(BAToUInt16(indiceBuff, i * sizeof(ushort)));
                }

                sceneryModels.Add(model);
                shrubModel.modelData = model;
                shrubModels.Add(shrubModel);
            }
        }
        #endregion

        #region Terrain Mesh
        public static void deSerializeTerrain(ref RatchetModel_Terrain terrainModel, int index, int racNum)
        {
            uint pointer = ReadUInt32(ReadBlock(efs, (uint)index, 4), 0);
            uint terrainElemSize = 0x30;
            uint texElemSize = 0x10;
            uint vertElemSize = 0x1c;
            uint UVElemSize = 0x08;
            uint ColorElemSize = 0x04;

            RatchetModel_Terrain model = new RatchetModel_Terrain();
            model.offset = pointer;
            model.modelID = 1111;
            model.modelType = ModelType.Terrain;
            byte[] terrainHeadBlock = ReadBlock(efs, pointer, 0x60);

            uint headPointer = BAToUInt32(terrainHeadBlock, 0x0);
            ushort headCount = BAToUInt16(terrainHeadBlock, 0x06);

            model.headCount = headCount;

            byte[] terrainBlock = ReadBlock(efs, headPointer, headCount * terrainElemSize);
            model.textureConfig = new List<RatchetTexture_Model>();
            model.vertBuff = new List<float>();
            model.indiceBuff = new List<uint>();
            uint vertCount = 0;
            uint prevVertCount = 0;
            uint prevFaceCount = 0;
            uint prevHeadCount = 0;
            uint prevPrevFaceCount = 0;

            for (uint i = 0; i < 4; i++)   //I have yet to see any level have more than two of these, but the header theoretically has space for 4
            {
                uint vertPointer = BAToUInt32(terrainHeadBlock, (i * sizeof(uint)) + 0x08);
                uint colPointer = BAToUInt32(terrainHeadBlock, (i * sizeof(uint)) + 0x18);
                uint UVPointer = BAToUInt32(terrainHeadBlock, (i * sizeof(uint)) + 0x28);
                uint indicePointer = BAToUInt32(terrainHeadBlock, (i * sizeof(uint)) + 0x38);

                prevFaceCount = 0;
                if (vertPointer != 0 && UVPointer != 0 && indicePointer != 0)    //Check that there's data at the pointers
                {
                    //Loop throught the heads, and stop when the overflow counter increases
                    for (uint ii = prevHeadCount; ii < headCount && i == BAToUInt16(terrainBlock, (ii * terrainElemSize) + 0x22); ii++)
                    {
                        uint localFaceCount = 0;
                        uint texPointer = BAToUInt32(terrainBlock, (ii * terrainElemSize) + 0x10);
                        uint texCount = BAToUInt32(terrainBlock, (ii * terrainElemSize) + 0x14);
                        ushort vertStart = BAToUInt16(terrainBlock, (ii * terrainElemSize) + 0x18);
                        ushort headVertCount = BAToUInt16(terrainBlock, (ii * terrainElemSize) + 0x1A);
                        vertCount += headVertCount;

                        byte[] texBlock = ReadBlock(efs, texPointer, texElemSize * texCount);
                        for (uint iii = 0; iii < texCount; iii++)
                        {
                            RatchetTexture_Model tex = new RatchetTexture_Model();
                            tex.ID = BAToUInt32(texBlock, (iii * texElemSize) + 0x00);
                            tex.start = BAToUInt32(texBlock, (iii * texElemSize) + 0x04) + prevPrevFaceCount;
                            tex.size = BAToUInt32(texBlock, (iii * texElemSize) + 0x08) + prevPrevFaceCount;
                            localFaceCount += BAToUInt32(texBlock, (iii * texElemSize) + 0x08);
                            model.textureConfig.Add(tex);
                        }

                        byte[] vertBlock = ReadBlock(efs, vertPointer + vertStart * vertElemSize, headVertCount * vertElemSize);
                        byte[] colBlock = ReadBlock(efs, colPointer + vertStart * ColorElemSize, headVertCount * ColorElemSize);
                        byte[] UVBlock = ReadBlock(efs, UVPointer + vertStart * UVElemSize, headVertCount * UVElemSize);

                        for (uint iii = 0; iii < headVertCount; iii++)
                        {
                            model.vertBuff.Add(BAToFloat(vertBlock, (iii * vertElemSize) + 0x00));    //vertx
                            model.vertBuff.Add(BAToFloat(vertBlock, (iii * vertElemSize) + 0x04));    //verty
                            model.vertBuff.Add(BAToFloat(vertBlock, (iii * vertElemSize) + 0x08));    //vertz
                            model.vertBuff.Add(BAToFloat(vertBlock, (iii * vertElemSize) + 0x0C));    //normx
                            model.vertBuff.Add(BAToFloat(vertBlock, (iii * vertElemSize) + 0x10));    //normy
                            model.vertBuff.Add(BAToFloat(vertBlock, (iii * vertElemSize) + 0x14));    //normz

                            model.vertBuff.Add(BAToFloat(UVBlock, (iii * UVElemSize) + 0x00));    //UVu
                            model.vertBuff.Add(BAToFloat(UVBlock, (iii * UVElemSize) + 0x04));    //UVv

                            model.vertBuff.Add(colBlock[(iii * ColorElemSize) + 0x00] / 255f);    //color r
                            model.vertBuff.Add(colBlock[(iii * ColorElemSize) + 0x01] / 255f);    //color g
                            model.vertBuff.Add(colBlock[(iii * ColorElemSize) + 0x02] / 255f);    //color b
                            model.vertBuff.Add(colBlock[(iii * ColorElemSize) + 0x03] / 255f);    //color a
                        }

                        byte[] indiceBuff = ReadBlock(efs, indicePointer + prevFaceCount * sizeof(ushort), localFaceCount * sizeof(ushort));
                        for (uint iii = 0; iii < localFaceCount; iii++)
                        {
                            model.indiceBuff.Add(BAToUInt16(indiceBuff, iii * sizeof(ushort)) + prevVertCount);
                        }
                        prevFaceCount += localFaceCount;
                        prevHeadCount = ii + 1;
                    }
                    prevVertCount = vertCount;
                    prevPrevFaceCount = prevFaceCount;
                }
            }
            model.vertexCount = vertCount;
            model.faceCount = prevFaceCount;

            uint headPtr = ReadUInt32(terrainHeadBlock, 0x00);
            uint uptr_1 = ReadUInt32(terrainHeadBlock, 0x08);
            uint uptr_2 = ReadUInt32(terrainHeadBlock, 0x18);
            uint uptr_3 = ReadUInt32(terrainHeadBlock, 0x28);
            uint vertIndices = ReadUInt32(terrainHeadBlock, 0x38);
            //uint vertColorsPtr = ReadUInt32(terrainHeadBlock, 0x48);

            model.terrainBlock = ReadBlock(efs, headPtr, uptr_1 - headPtr);
            model.unknownBlock1 = ReadBlock(efs, uptr_1, uptr_2 - uptr_1);
            model.unknownBlock2 = ReadBlock(efs, uptr_2, uptr_3 - uptr_2);
            model.unknownBlock3 = ReadBlock(efs, uptr_3, vertIndices - uptr_3);


            byte[] indexData = new byte[0];
            ushort compareVertId = 0;
            int vertOffset = 0;
            while (compareVertId <= model.vertexCount)
            {
                //We have to read an entire line because the comparison is made on 0x02 to determine if we have to stop
                byte[] vertexLine = ReadBlock(efs, (uint)(vertIndices + (vertOffset * 0x04)), 4);

                ushort vertexId1 = ReadUInt16(vertexLine, 0);
                ushort vertexId2 = ReadUInt16(vertexLine, 2);

                int currentOffset = indexData.Length;
                Array.Resize(ref indexData, indexData.Length + 0x04);
                WriteUint16(ref indexData, currentOffset + 0x00, vertexId1);
                WriteUint16(ref indexData, currentOffset + 0x02, vertexId2);

                compareVertId = vertexId2;
                //If the id we found matches the count, we peek at the next value to see if its also matching
                //If not, we assume that this is the end of the list and we break the loop
                if (compareVertId == model.vertexCount - 1)
                {
                    byte[] nextVertexLine = ReadBlock(efs, (uint)(vertIndices + (vertOffset + 1 * 0x04)), 4);
                    ushort nextVertexId1 = ReadUInt16(nextVertexLine, 0);
                    ushort nextVertexId2 = ReadUInt16(nextVertexLine, 2);

                    if (nextVertexId2 != compareVertId)
                        break;
                }

                vertOffset++;
            }

            model.vertexIndices = indexData;

            terrainModel = model;
        }
        #endregion

        #region Ties
        public static void deSerializeTies(ref List<LevelObject> ties, int index, int racNum)
        {
            uint pointer = ReadUInt32(ReadBlock(efs, (uint)index, 4), 0);
            uint tieSize = 0x70;
            byte[] levelObjectBlock = ReadBlock(efs, pointer, DataStoreEngine.engineHeader.levelObjectCount * 0x70);
            for (uint i = 0; i < DataStoreEngine.engineHeader.levelObjectCount; i++)
            {
                LevelObject levObj = new LevelObject()
                {
                    v1x = BAToFloat(levelObjectBlock, (tieSize * i) + 0x00),
                    v1y = BAToFloat(levelObjectBlock, (tieSize * i) + 0x04),
                    v1z = BAToFloat(levelObjectBlock, (tieSize * i) + 0x08),
                    v1w = BAToFloat(levelObjectBlock, (tieSize * i) + 0x0C),

                    v2x = BAToFloat(levelObjectBlock, (tieSize * i) + 0x10),
                    v2y = BAToFloat(levelObjectBlock, (tieSize * i) + 0x14),
                    v2z = BAToFloat(levelObjectBlock, (tieSize * i) + 0x18),
                    v2w = BAToFloat(levelObjectBlock, (tieSize * i) + 0x1C),

                    v3x = BAToFloat(levelObjectBlock, (tieSize * i) + 0x20),
                    v3y = BAToFloat(levelObjectBlock, (tieSize * i) + 0x24),
                    v3z = BAToFloat(levelObjectBlock, (tieSize * i) + 0x28),
                    v3w = BAToFloat(levelObjectBlock, (tieSize * i) + 0x2C),

                    x = BAToFloat(levelObjectBlock, (tieSize * i) + 0x30),
                    y = BAToFloat(levelObjectBlock, (tieSize * i) + 0x34),
                    z = BAToFloat(levelObjectBlock, (tieSize * i) + 0x38),
                    w = BAToFloat(levelObjectBlock, (tieSize * i) + 0x3C),                 

                    /* These offsets are just placeholders for the render distance quaternion which is set in-game*/
                    off_40 = BAToUInt32(levelObjectBlock, (tieSize * i) + 0x40),
                    off_44 = BAToUInt32(levelObjectBlock, (tieSize * i) + 0x44),
                    off_48 = BAToUInt32(levelObjectBlock, (tieSize * i) + 0x48),
                    off_4C = BAToUInt32(levelObjectBlock, (tieSize * i) + 0x4C),
                
                    off_50 = BAToUInt16(levelObjectBlock, (tieSize * i) + 0x50),
                    modelID = BAToUInt16(levelObjectBlock, (tieSize * i) + 0x52),
                    off_54 = BAToUInt32(levelObjectBlock, (tieSize * i) + 0x54),
                    off_58 = BAToUInt32(levelObjectBlock, (tieSize * i) + 0x58),
                    off_5C = BAToUInt32(levelObjectBlock, (tieSize * i) + 0x5C),

                    vertexColorsPtr = BAToUInt32(levelObjectBlock, (tieSize * i) + 0x60),
                    off_64 = BAToUInt32(levelObjectBlock, (tieSize * i) + 0x64),
                    off_68 = BAToUInt32(levelObjectBlock, (tieSize * i) + 0x68),
                    off_6C = BAToUInt32(levelObjectBlock, (tieSize * i) + 0x6C),
                };

                TieModel tModel = DataStoreEngine.tieModels.Find(tm => tm.modelId == levObj.modelID);

                levObj.vertexColors = ReadBlock(efs, levObj.vertexColorsPtr, (uint)tModel.vertexCount * 0x04);

                levObj.mat = new Matrix4(levObj.v1x, levObj.v1y, levObj.v1z, levObj.v1w, levObj.v2x, levObj.v2y, levObj.v2z, levObj.v2w, levObj.v3x, levObj.v3y, levObj.v3z, levObj.v3w, levObj.x, levObj.y, levObj.z, levObj.w);

                ties.Add(levObj);
            }
        }
        #endregion

        #region Shrubs
        public static void deSerializeShrubs(ref List<LevelObject> shrubs, int index, int racNum)
        {
            uint pointer = ReadUInt32(ReadBlock(efs, (uint)index, 4), 0);
            uint shrubSize = 0x70;
            byte[] levelObjectBlock = ReadBlock(efs, pointer, DataStoreEngine.engineHeader.sceneryObjectsCount * 0x70);
            for (uint i = 0; i < DataStoreEngine.engineHeader.sceneryObjectsCount; i++)
            {
                LevelObject levObj = new LevelObject()
                {
                    v1x = BAToFloat(levelObjectBlock, (shrubSize * i) + 0x00),
                    v1y = BAToFloat(levelObjectBlock, (shrubSize * i) + 0x04),
                    v1z = BAToFloat(levelObjectBlock, (shrubSize * i) + 0x08),
                    v1w = BAToFloat(levelObjectBlock, (shrubSize * i) + 0x0C),

                    v2x = BAToFloat(levelObjectBlock, (shrubSize * i) + 0x10),
                    v2y = BAToFloat(levelObjectBlock, (shrubSize * i) + 0x14),
                    v2z = BAToFloat(levelObjectBlock, (shrubSize * i) + 0x18),
                    v2w = BAToFloat(levelObjectBlock, (shrubSize * i) + 0x1C),

                    v3x = BAToFloat(levelObjectBlock, (shrubSize * i) + 0x20),
                    v3y = BAToFloat(levelObjectBlock, (shrubSize * i) + 0x24),
                    v3z = BAToFloat(levelObjectBlock, (shrubSize * i) + 0x28),
                    v3w = BAToFloat(levelObjectBlock, (shrubSize * i) + 0x2C),

                    x = BAToFloat(levelObjectBlock, (shrubSize * i) + 0x30),
                    y = BAToFloat(levelObjectBlock, (shrubSize * i) + 0x34),
                    z = BAToFloat(levelObjectBlock, (shrubSize * i) + 0x38),
                    w = BAToFloat(levelObjectBlock, (shrubSize * i) + 0x3C),

                    /* These offsets are just placeholders for the render distance quaternion which is set in-game*/
                    off_40 = BAToUInt32(levelObjectBlock, (shrubSize * i) + 0x40),
                    off_44 = BAToUInt32(levelObjectBlock, (shrubSize * i) + 0x44),
                    off_48 = BAToUInt32(levelObjectBlock, (shrubSize * i) + 0x48),
                    off_4C = BAToUInt32(levelObjectBlock, (shrubSize * i) + 0x4C),

                    off_50 = BAToUInt16(levelObjectBlock, (shrubSize * i) + 0x50),
                    modelID = BAToUInt16(levelObjectBlock, (shrubSize * i) + 0x52),
                    off_54 = BAToUInt32(levelObjectBlock, (shrubSize * i) + 0x54),
                    off_58 = BAToUInt32(levelObjectBlock, (shrubSize * i) + 0x58),
                    off_5C = BAToUInt32(levelObjectBlock, (shrubSize * i) + 0x5C),

                    vertexColorsPtr = BAToUInt32(levelObjectBlock, (shrubSize * i) + 0x60),
                    off_64 = BAToUInt32(levelObjectBlock, (shrubSize * i) + 0x64),
                    off_68 = BAToUInt32(levelObjectBlock, (shrubSize * i) + 0x68),
                    off_6C = BAToUInt32(levelObjectBlock, (shrubSize * i) + 0x6C),
                };

                levObj.mat = new Matrix4(levObj.v1x, levObj.v1y, levObj.v1z, levObj.v1w, levObj.v2x, levObj.v2y, levObj.v2z, levObj.v2w, levObj.v3x, levObj.v3y, levObj.v3z, levObj.v3w, levObj.x, levObj.y, levObj.z, levObj.w);

                shrubs.Add(levObj);
            }
        }
        #endregion

        #region Textures
        public static void deSerializeTextures(ref List<RatchetTexture_General> textures, int index, int racNum)
        {
            uint pointer = ReadUInt32(ReadBlock(efs, (uint)index, 4), 0);
            for (int x = 0; x < DataStoreEngine.engineHeader.textureCount; x++)
            {
                byte[] texHeader = ReadBlock(efs, pointer + (uint)(0x24 * x), 0x24);
                RatchetTexture_General texture = new RatchetTexture_General();
                texture.texHeader = texHeader;
                uint nextTexturePointer = x < DataStoreEngine.engineHeader.textureCount - 1 ? ReadUInt32(ReadBlock(efs, pointer + (uint)(0x24 * (x + 1)), 0x4), 0) : vfs != null ? (uint)vfs.Length : 0;
                uint currentTexturePointer = ReadUInt32(texHeader, 0);
                texture.ID = x;
                texture.width = BAToShort(texHeader, 0x18);
                texture.height = BAToShort(texHeader, 0x1A);
                texture.texData = vfs != null ? ReadBlock(vfs, currentTexturePointer, nextTexturePointer - currentTexturePointer) : null;
                texture.reverseRGB = false;

                texture.texDataPointer = ReadUInt32(texHeader, 0x00);
                texture.off_04 = ReadUInt32(texHeader, 0x04);
                texture.off_08 = ReadUInt32(texHeader, 0x08);
                texture.off_0C = ReadUInt32(texHeader, 0x0C);
                texture.off_10 = ReadUInt32(texHeader, 0x10);
                texture.off_14 = ReadUInt32(texHeader, 0x14);
                texture.off_1C = ReadUInt32(texHeader, 0x1C);
                texture.off_20 = ReadUInt32(texHeader, 0x20);

                textures.Add(texture);
            }
        }
        #endregion

        #region Terrain Collision
        public static void deSerializeTerrainCollision(ref List<float> collisionVertexBuff, ref List<uint> collisionIndiceBuff, int index, int racNum)
        {
            uint pointer = ReadUInt32(ReadBlock(efs, (uint)index, 4), 0);
            byte[] engineHeader = ReadBlock(efs, 0, 0x80);
            uint nextPointer = FindNextLargest(engineHeader, index);
            DataStoreEngine.collisionDataRaw = ReadBlock(efs, pointer, nextPointer - pointer);
            float div = 1024f;

            uint collisionVertCount = 0;

            uint collisionStart = pointer + BAToUInt32(ReadBlock(efs, pointer + 0x00, 4), 0);
            uint collisionLength = pointer + collisionStart + BAToUInt32(ReadBlock(efs, pointer + 0x04, 4), 0);
            byte[] collision = ReadBlock(efs, collisionStart, collisionLength);

            ushort zShift = BAToUInt16(collision, 0);
            ushort zCount = BAToUInt16(collision, 2);

            for (uint z = 0; z < zCount; z++)
            {
                uint yOffset = BAToUInt32(collision, (z * 4) + 0x04);
                if (yOffset != 0)
                {
                    ushort yShift = BAToUInt16(collision, yOffset + 0);
                    ushort yCount = BAToUInt16(collision, yOffset + 2);
                    for (uint y = 0; y < yCount; y++)
                    {
                        uint xOffset = BAToUInt32(collision, yOffset + (y * 4) + 0x04);
                        if (xOffset != 0)
                        {
                            ushort xShift = BAToUInt16(collision, xOffset + 0);
                            ushort xCount = BAToUInt16(collision, xOffset + 2);
                            for (uint x = 0; x < xCount; x++)
                            {
                                uint vOffset = BAToUInt32(collision, xOffset + (x * 4) + 4);
                                ushort faceCount = BAToUInt16(collision, vOffset);
                                byte vertexCount = collision[vOffset + 2];
                                byte rCount = collision[vOffset + 3];

                                if (vOffset != 0)
                                {
                                    byte[] collisionType = new byte[vertexCount];
                                    //Console.WriteLine("Facecount: " + faceCount.ToString("X2") + "VertCount: " + vertexCount.ToString("X2"));
                                    for (uint f = 0; f < faceCount; f++)
                                    {
                                        uint fOffset = vOffset + ((uint)12 * vertexCount) + 4 + (f * 4);
                                        byte[] fArray = new byte[4];
                                        Array.Copy(collision, fOffset, fArray, 0, 4);

                                        collisionType[fArray[0]] = fArray[3];
                                        collisionType[fArray[1]] = fArray[3];
                                        collisionType[fArray[2]] = fArray[3];

                                        uint f1 = collisionVertCount + fArray[0];
                                        uint f2 = collisionVertCount + fArray[1];
                                        uint f3 = collisionVertCount + fArray[2];
                                        collisionIndiceBuff.Add(f1);
                                        collisionIndiceBuff.Add(f2);
                                        collisionIndiceBuff.Add(f3);
                                        if (f < rCount)
                                        {
                                            uint rOffset = vOffset + 4 + (uint)(12 * vertexCount) + (uint)(4 * faceCount) + f;
                                            uint f4 = collisionVertCount + collision[rOffset];
                                            collisionIndiceBuff.Add(f1);
                                            collisionIndiceBuff.Add(f3);
                                            collisionIndiceBuff.Add(f4);
                                            collisionType[collision[rOffset]] = fArray[3];
                                        }
                                    }

                                    for (uint v = 0; v < vertexCount; v++)
                                    {
                                        uint pOffset = vOffset + (12 * v) + 4;
                                        float pX = BAToFloat(collision, pOffset + 0) / div + 4 * (xShift + x + 0.5f);
                                        float pY = BAToFloat(collision, pOffset + 4) / div + 4 * (yShift + y + 0.5f);
                                        float pZ = BAToFloat(collision, pOffset + 8) / div + 4 * (zShift + z + 0.5f);
                                        collisionVertexBuff.Add(pX);
                                        collisionVertexBuff.Add(pY);
                                        collisionVertexBuff.Add(pZ);
                                        switch (collisionType[v])
                                        {
                                            case 0x1F:
                                                collisionVertexBuff.Add(0.0f);
                                                collisionVertexBuff.Add(0.0f);
                                                collisionVertexBuff.Add(0.0f);
                                                break;
                                            case 0x08:
                                                collisionVertexBuff.Add(1.0f);
                                                collisionVertexBuff.Add(0.0f);
                                                collisionVertexBuff.Add(0.0f);
                                                break;
                                            case 0x0C:
                                                collisionVertexBuff.Add(0.0f);
                                                collisionVertexBuff.Add(1.0f);
                                                collisionVertexBuff.Add(0.0f);
                                                break;
                                            case 0x01:
                                                //This
                                                collisionVertexBuff.Add(0.0f);
                                                collisionVertexBuff.Add(0.0f);
                                                collisionVertexBuff.Add(0.0f);
                                                break;
                                            case 0x3F:
                                                collisionVertexBuff.Add(0.5f);
                                                collisionVertexBuff.Add(0.0f);
                                                collisionVertexBuff.Add(0.0f);
                                                break;
                                            case 0x0B:
                                                collisionVertexBuff.Add(0.0f);
                                                collisionVertexBuff.Add(0.5f);
                                                collisionVertexBuff.Add(0.0f);
                                                break;
                                            case 0x0A:
                                                collisionVertexBuff.Add(0.0f);
                                                collisionVertexBuff.Add(0.0f);
                                                collisionVertexBuff.Add(0.5f);
                                                break;
                                            case 0x28:
                                                collisionVertexBuff.Add(0.25f);
                                                collisionVertexBuff.Add(0.0f);
                                                collisionVertexBuff.Add(0.0f);
                                                break;
                                            case 0x5F:
                                                collisionVertexBuff.Add(0.0f);
                                                collisionVertexBuff.Add(0.25f);
                                                collisionVertexBuff.Add(0.0f);
                                                break;
                                            case 0x09:
                                                collisionVertexBuff.Add(0.0f);
                                                collisionVertexBuff.Add(0.0f);
                                                collisionVertexBuff.Add(0.25f);
                                                break;
                                            case 0x00:
                                                collisionVertexBuff.Add(0.0f);
                                                collisionVertexBuff.Add(0.0f);
                                                collisionVertexBuff.Add(1.0f);
                                                break;

                                            case 0x6C:
                                                collisionVertexBuff.Add(0.5f);
                                                collisionVertexBuff.Add(0.5f);
                                                collisionVertexBuff.Add(0.5f);
                                                break;

                                            default:
                                                collisionVertexBuff.Add(1.0f);
                                                collisionVertexBuff.Add(1.0f);
                                                collisionVertexBuff.Add(1.0f);
                                                break;
                                        }
                                        collisionVertCount++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}
