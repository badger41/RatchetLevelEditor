using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataFunctions;
using static RatchetModel;
using static RatchetTexture;
using static RatchetLevelEditor.GameplayParser;

namespace RatchetLevelEditor
{
    class MapParser_UYA
    {
        public static void parseMap(string path, string fileName)
        {
            FileStream fs = File.OpenRead(path + "/" + fileName);

            FileStream vfs = null;
            if (File.Exists(path + "/vram.ps3"))
                vfs = File.OpenRead(path + "/vram.ps3");

            FileStream gpf = null;
            if (File.Exists(path + "/gameplay_ntsc"))
                gpf = File.OpenRead(path + "/gameplay_ntsc");

            uint racNum = 0;

            ushort magic = BAToUInt16(ReadBlock(fs, 0xB0, 2), 0);
            switch (magic)
            {
                case 0x7533:
                    racNum = 1;
                    break;
                case 0xEA9C:
                    racNum = 2;
                    break;
                case 0x0000:
                case 0x755D:
                    racNum = 3;
                    break;
            }
            Console.WriteLine("Game determined to be ratchet" + racNum.ToString());

            List<MobyPropertyVariables> pVarConfigs = new List<MobyPropertyVariables>();
            string jsonString = File.ReadAllText(String.Format("./Config/Rac{0}/MobyPropertyVariables.json", racNum));
            pVarConfigs = JsonConvert.DeserializeObject<List<MobyPropertyVariables>>(jsonString);
            DataStore.pVarMap = pVarConfigs;

            #region GameplayHeader
            GameplayHeader gameplayHeader = new GameplayHeader(gpf, racNum);

            DataStore.gameplayHeader = gameplayHeader;

            #endregion

            #region pVars
            //List<byte[]> pVars;
            //uint numpVars;
            //byte[] pVarHeadBlock;
            //uint pVarSectionLength;
            //byte[] pVarBlock;
            //switch (racNum)
            //{
            //    case 1:
            //    case 2:
            //    case 3:
            //        pVars = new List<byte[]>();
            //        numpVars = (gameplayHeader.pVarPointer - gameplayHeader.pVarListPointer) / 8;
            //        pVarHeadBlock = ReadBlock(gpf, gameplayHeader.pVarListPointer, numpVars * 8);
            //        pVarSectionLength = 0;
            //        for (uint i = 0; i < numpVars; i++)
            //        {
            //            pVarSectionLength += BAToUInt32(pVarHeadBlock, (i * 8) + 0x04);
            //        }
            //        pVarBlock = ReadBlock(gpf, gameplayHeader.pVarPointer, pVarSectionLength);
            //        for (uint i = 0; i < numpVars; i++)
            //        {
            //            uint mobpVarsStart = BAToUInt32(pVarHeadBlock, (i * 8));
            //            uint mobpVarsCount = BAToUInt32(pVarHeadBlock, (i * 8) + 0x04);
            //            byte[] mobpVars = new byte[mobpVarsCount];
            //            mobpVars = getBytes(pVarBlock, (int)mobpVarsStart, (int)mobpVarsCount);
            //            pVars.Add(mobpVars);
            //        }
            //        DataStore.pVarList = pVars;
            //        break;
            //}
            #endregion pVars

            #region Mobies

            /*uint mobyCount = BAToUInt32(ReadBlock(gpf, gameplayHeader.mobyPointer, 4), 0);
            byte[] mobyBlock = ReadBlock(gpf, gameplayHeader.mobyPointer + 0x10, mobyCount * gameplayHeader.mobyElemSize);

            for(uint i = 0; i < mobyCount; i++)
            {
                RatchetMoby mob = new RatchetMoby(racNum, mobyBlock, i);
                DataStore.mobs.Add(mob);
            }*/
            #endregion

            //DataStore.splines = new List<Spline>();

            //uint splineCount = BAToUInt32(ReadBlock(gpf, gameplayHeader.splinePointer, 4), 0);
            //uint splineOffset = BAToUInt32(ReadBlock(gpf, gameplayHeader.splinePointer + 4, 4), 0);
            //uint splineSectionSize = BAToUInt32(ReadBlock(gpf, gameplayHeader.splinePointer + 8, 4), 0);
            //byte[] splineHeadBlock = ReadBlock(gpf, gameplayHeader.splinePointer + 0x10, splineCount * 4);
            //byte[] splineBlock = ReadBlock(gpf, gameplayHeader.splinePointer + splineOffset, splineSectionSize);
            //for (uint i = 0; i < splineCount; i++)
            //{
            //    uint offset = BAToUInt32(splineHeadBlock, (i * 4));
            //    DataStore.splines.Add(new Spline(splineBlock, offset));
            //}


            gpf.Close();
            parseGameplay(path, (int) racNum);

            EngineHeader engineHeader = new EngineHeader(fs, 1);
            DataStore.engineHeader = engineHeader;

            //Load UYA Spawnable Models
            #region Spawnables
            uint spawnablesCount = BAToUInt32(ReadBlock(fs, engineHeader.spawnablesPointer, 4), 0);
            byte[] idBlock = ReadBlock(fs, engineHeader.spawnablesPointer + 4, spawnablesCount * 8);

            for (int x = 0; x < spawnablesCount; x++)
            {
                RatchetModel_General model = new RatchetModel_General();
                model.modelType = ModelType.Spawnable;

                model.modelID = BAToShort(idBlock, (uint)(x * 8) + 2);
                model.offset = BAToUInt32(idBlock, (uint)(x * 8) + 4);

                if (model.offset != 0)
                {
                    uint modelHeadSize = BAToUInt32(ReadBlock(fs, model.offset, 4), 0);

                    if(modelHeadSize > 0)
                    {
                        byte[] headBlock = ReadBlock(fs, model.offset, modelHeadSize + 0x20); //Head + objectDetails
                        uint objectPtr =            BAToUInt32( headBlock, 0);
                        model.animationsCount = headBlock[0x0C];
                        //(0x04)null
                        //(0x08)[count for 0x14 and 0x18][unknown][unknown][unknown]
                        //(0x0c)[animation count][unknown][unknown][unknown]
                        //(0x10)Pointer to list
                        //(0x14)Pointer to skeleton joints (0x40 structure)
                        //(0x18)Pointer to skeleton somethings (0x16 structure)
                        //(0x1C)Pointer to left arm animations (Ratchet)
                        //(0x20)null
                        model.size =                BAToFloat(  headBlock, 0x24);
                        uint texCount =             BAToUInt32( headBlock, objectPtr + 0x00);
                        uint otherCount =           BAToUInt32( headBlock, objectPtr + 0x04);
                        uint texBlockPointer =      BAToUInt32( headBlock, objectPtr + 0x08);
                        uint otherBlockPointer =    BAToUInt32( headBlock, objectPtr + 0x0C);
                        uint vertPointer =          BAToUInt32( headBlock, objectPtr + 0x10);
                        uint indexPointer =         BAToUInt32( headBlock, objectPtr + 0x14);
                        model.vertexCount =         BAToUInt16( headBlock, objectPtr + 0x18);
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
                            modTex.ID =     BAToUInt32(texBlock, (t * texElemSize) + 0x0);
                            modTex.start =  BAToUInt32(texBlock, (t * texElemSize) + 0x04);
                            modTex.size =   BAToUInt32(texBlock, (t * texElemSize) + 0x08);
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
                DataStore.spawnableModels.Add(model);
            }
            #endregion

            //Load UYA Level Models
            #region Level Models
            uint levelElemSize = 0x40;
            byte[] levelBlock = ReadBlock(fs, engineHeader.levelModelsPointer, engineHeader.levelModelsCount * levelElemSize);

            for (uint x = 0; x < engineHeader.levelModelsCount; x++)
            {

                RatchetModel_General model = new RatchetModel_General();
                model.modelType = ModelType.Level;
                model.size = 0.5f;

                uint vertPointer =      BAToUInt32( levelBlock, (x * levelElemSize) + 0x10);
                uint UVPointer =        BAToUInt32( levelBlock, (x * levelElemSize) + 0x14);
                uint indicePointer =    BAToUInt32( levelBlock, (x * levelElemSize) + 0x18);
                uint texPointer =       BAToUInt32( levelBlock, (x * levelElemSize) + 0x1C);
                model.vertexCount =     (ushort)BAToUInt32( levelBlock, (x * levelElemSize) + 0x24);
                ushort texCount =       BAToUInt16( levelBlock, (x * levelElemSize) + 0x28);
                model.modelID =         BAToShort(  levelBlock, (x * levelElemSize) + 0x30);

                model.offset = (uint)(x * 0x40) + engineHeader.levelModelsPointer;
                model.modelType = ModelType.Level;

                uint texElemSize = 0x18;
                byte[] texBlock = ReadBlock(fs, texPointer, texCount * texElemSize);
                model.textureConfig = new List<RatchetTexture_Model>();
                model.faceCount = 0;
                for (uint t = 0; t < texCount; t++)
                {
                    RatchetTexture_Model dlt = new RatchetTexture_Model();
                    dlt.ID =    BAToUInt32(texBlock, (t * texElemSize) + 0x00);
                    dlt.start = BAToUInt32(texBlock, (t * texElemSize) + 0x08);
                    dlt.size =  BAToUInt32(texBlock, (t * texElemSize) + 0x0C);
                    model.textureConfig.Add(dlt);
                    model.faceCount += dlt.size;
                }
                
                //Flip endianness of vertex array float[vert_x, vert_y, vert_z, norm_x, norm_y, norm_z, uv_u, uv_v, reserved reserved]
                uint vertElemSize = 0x18;
                uint vertBuffSize = model.vertexCount * vertElemSize;
                byte[] vertBlock = ReadBlock(fs, vertPointer, vertBuffSize);

                uint UVElemsize = 0x08;
                uint UVBuffSize = model.vertexCount * UVElemsize;
                byte[] UVBlock = ReadBlock(fs, UVPointer, UVBuffSize);


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
                byte[] indiceBuff = ReadBlock(fs, indicePointer, model.faceCount * sizeof(ushort));
                model.indiceBuff = new List<ushort>();
                for (uint i = 0; i < model.faceCount; i++)
                {
                    model.indiceBuff.Add(BAToUInt16(indiceBuff, i * sizeof(ushort)));
                }

                DataStore.levelModels.Add(model);
            }
            #endregion

            //Load UYA Scenery Models
            #region Scenery Models
            uint sceneElemSize = 0x40;
            byte[] sceneBlock = ReadBlock(fs, engineHeader.sceneryModelsPointer, engineHeader.sceneryModelsCount * sceneElemSize);

            for (uint x = 0; x < engineHeader.sceneryModelsCount; x++)
            {
                RatchetModel_General model = new RatchetModel_General();
                model.modelType = ModelType.Scenery;
                model.size = 0.5f;

                uint vertPointer =      BAToUInt32(sceneBlock, (x * sceneElemSize) + 0x10);
                uint UVPointer =        BAToUInt32(sceneBlock, (x * sceneElemSize) + 0x14);
                uint indicePointer =    BAToUInt32(sceneBlock, (x * sceneElemSize) + 0x18);
                uint texPointer =       BAToUInt32(sceneBlock, (x * sceneElemSize) + 0x1C);
                model.vertexCount =     (ushort)BAToUInt32(sceneBlock, (x * sceneElemSize) + 0x24);
                ushort texCount =       BAToUInt16(sceneBlock, (x * sceneElemSize) + 0x28);
                model.modelID =         BAToShort(sceneBlock, (x * sceneElemSize) + 0x30);

                uint texElemSize = 0x10;
                byte[] texBlock = ReadBlock(fs, texPointer, texCount * texElemSize);
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
                byte[] vertBlock = ReadBlock(fs, vertPointer, vertBuffSize);

                uint UVElemsize = 0x08;
                uint UVBuffSize = model.vertexCount * UVElemsize;
                byte[] UVBlock = ReadBlock(fs, UVPointer, UVBuffSize);


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
                byte[] indiceBuff = ReadBlock(fs, indicePointer, model.faceCount * sizeof(ushort));
                model.indiceBuff = new List<ushort>();
                for (uint i = 0; i < model.faceCount; i++)
                {
                    model.indiceBuff.Add(BAToUInt16(indiceBuff, i * sizeof(ushort)));
                }

                DataStore.sceneryModels.Add(model);
                //Console.WriteLine("Scenery Model Added: 0x" + model.modelID.ToString("X4"));
            }
            #endregion

            //Load Terrain
            #region Terrain
            {
                uint terrainElemSize = 0x30;
                uint texElemSize = 0x10;
                uint vertElemSize = 0x1c;
                uint UVElemSize = 0x08;
                uint ColorElemSize = 0x04;

                RatchetModel_Terrain model = new RatchetModel_Terrain();
                model.offset = engineHeader.terrainPointer;
                model.modelID = 1111;
                model.modelType = ModelType.Terrain;
                byte[] terrainHeadBlock = ReadBlock(fs, engineHeader.terrainPointer, 0x60);

                uint headPointer =  BAToUInt32(terrainHeadBlock, 0x0);
                ushort headCount =  BAToUInt16(terrainHeadBlock, 0x06);

                byte[] terrainBlock = ReadBlock(fs, headPointer, headCount * terrainElemSize);
                model.textureConfig = new List<RatchetTexture_Model>();
                model.vertBuff = new List<float>();
                model.indiceBuff = new List<uint>();
                uint vertCount = 0;
                uint prevVertCount = 0;
                uint prevFaceCount = 0;
                uint prevHeadCount = 0;
                uint prevPrevFaceCount = 0;

                for (uint i = 0; i < 4 ; i++)   //I have yet to see any level have more than two of these, but the header theoretically has space for 4
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
                            uint texPointer =       BAToUInt32(terrainBlock, (ii * terrainElemSize) + 0x10);
                            uint texCount =         BAToUInt32(terrainBlock, (ii * terrainElemSize) + 0x14);
                            ushort vertStart =      BAToUInt16(terrainBlock, (ii * terrainElemSize) + 0x18);
                            ushort headVertCount =  BAToUInt16(terrainBlock, (ii * terrainElemSize) + 0x1A);
                            vertCount += headVertCount;

                            byte[] texBlock = ReadBlock(fs, texPointer, texElemSize * texCount);
                            for (uint iii = 0; iii < texCount; iii++)
                            {
                                RatchetTexture_Model tex = new RatchetTexture_Model();
                                tex.ID =        BAToUInt32(texBlock, (iii * texElemSize) + 0x00);
                                tex.start =     BAToUInt32(texBlock, (iii * texElemSize) + 0x04) + prevPrevFaceCount;
                                tex.size =      BAToUInt32(texBlock, (iii * texElemSize) + 0x08) + prevPrevFaceCount;
                                localFaceCount +=    BAToUInt32(texBlock, (iii * texElemSize) + 0x08);
                                model.textureConfig.Add(tex);
                            }

                            byte[] vertBlock = ReadBlock(fs, vertPointer + vertStart * vertElemSize, headVertCount * vertElemSize);
                            byte[] colBlock = ReadBlock(fs, colPointer + vertStart * ColorElemSize, headVertCount * ColorElemSize);
                            byte[] UVBlock = ReadBlock(fs, UVPointer + vertStart * UVElemSize, headVertCount * UVElemSize);

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

                            byte[] indiceBuff = ReadBlock(fs, indicePointer + prevFaceCount * sizeof(ushort), localFaceCount * sizeof(ushort));
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
                DataStore.terrainModel = model;
            }
            #endregion

            //Load Level Objects
            #region Level objects
            byte[] levelObjectBlock = ReadBlock(fs, engineHeader.levelObjectPointer, engineHeader.levelObjectCount * 0x70);
            DataStore.levelObjects = new List<LevelObject>();
            for(uint i = 0; i < engineHeader.levelObjectCount; i++)
            {
                LevelObject levObj = new LevelObject(levelObjectBlock, i);
                DataStore.levelObjects.Add(levObj);
            }
            #endregion

            //Load Scenery Objects
            #region Scenery objects
            byte[] sceneryObjectBlock = ReadBlock(fs, engineHeader.sceneryObjectsPointer, engineHeader.sceneryObjectsCount * 0x70);
            DataStore.sceneryObjects = new List<LevelObject>();
            for (uint i = 0; i < engineHeader.sceneryObjectsCount; i++)
            {
                LevelObject levObj = new LevelObject(sceneryObjectBlock, i);
                DataStore.sceneryObjects.Add(levObj);
            }
            #endregion

            //Load Textures
            #region Textures
            RatchetTexture_General texture = new RatchetTexture_General();
            for (int x = 0; x < engineHeader.UYATextureCount; x++)
            {

                texture.texHeader = ReadBlock(fs, engineHeader.texturesPointer + (uint)(0x24 * x), 0x24);
                uint nextTexturePointer = x < engineHeader.UYATextureCount -1 ? ReadUInt32(ReadBlock(fs, engineHeader.texturesPointer + (uint)(0x24 * (x + 1)), 0x4), 0) : vfs != null ? (uint)vfs.Length : 0;
                uint currentTexturePointer =  ReadUInt32(texture.texHeader, 0);
                //Console.WriteLine("Parsing texture " + x + " " + currentTexturePointer.ToString("X") + " " + nextTexturePointer.ToString("X"));
                texture.ID = x;
                texture.width = BAToShort(texture.texHeader, 0x18);
                texture.height = BAToShort(texture.texHeader, 0x1A);
                texture.texData = vfs != null ? ReadBlock(vfs, currentTexturePointer, nextTexturePointer - currentTexturePointer) : null;
                texture.reverseRGB = false;
                DataStore.textures.Add(texture);
                //Console.WriteLine("Texture : " + x + " Width: " + texture.width + " Height: " + texture.height);
            }
            #endregion



            //Terrain Collision
            #region Terrain Collision
            float div = 1024f;

            uint collisionVertCount = 0;

            uint collisionStart = engineHeader.collisionPointer + BAToUInt32(ReadBlock(fs, engineHeader.collisionPointer + 0x00, 4), 0);
            uint collisionLength = engineHeader.collisionPointer + collisionStart + BAToUInt32(ReadBlock(fs, engineHeader.collisionPointer + 0x04, 4), 0);
            byte[] collision = ReadBlock(fs, collisionStart, collisionLength);

            List<float> collisionVertexBuff = new List<float>();
            List<uint> collisionIndiceBuff = new List<uint>();

            ushort zShift =    BAToUInt16(collision, 0);
            ushort zCount =     BAToUInt16(collision, 2);
            
            for (uint z = 0; z < zCount; z++)
            {
                uint yOffset = BAToUInt32(collision, (z * 4) + 0x04);
                if(yOffset != 0)
                {
                    ushort yShift = BAToUInt16(collision, yOffset + 0);
                    ushort yCount = BAToUInt16(collision, yOffset + 2);
                    for (uint y = 0; y < yCount; y++)
                    {
                        uint xOffset = BAToUInt32(collision, yOffset + (y * 4) + 0x04);
                        if(xOffset != 0)
                        {
                            ushort xShift = BAToUInt16(collision, xOffset + 0);
                            ushort xCount = BAToUInt16(collision, xOffset + 2);
                            for(uint x = 0; x < xCount; x++)
                            {
                                uint vOffset = BAToUInt32(collision, xOffset + (x * 4) + 4);
                                ushort faceCount = BAToUInt16(collision, vOffset);
                                byte vertexCount = collision[vOffset + 2];
                                byte rCount = collision[vOffset + 3];

                                if(vOffset != 0)
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
                                        if(f < rCount)
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
            DataStore.collVertBuff = collisionVertexBuff;
            DataStore.collIndBuff = collisionIndiceBuff;
            #endregion

            #region Chunks
            List<int> chunkIds = new List<int>();
            for (int chunkIndex = 0; chunkIndex < 5; chunkIndex++)
            {
                if (File.Exists(path + "/chunk" + chunkIndex + ".ps3"))
                {
                    chunkIds.Add(chunkIndex);
                }
            }

            foreach (int chunkId in chunkIds)
            {
                FileStream cfs = File.OpenRead(path + "/chunk" + chunkId + ".ps3");
                Console.WriteLine("Loading chunk " + chunkId);
                uint terrainElemSize = 0x30;
                uint texElemSize = 0x10;
                uint vertElemSize = 0x1c;
                uint UVElemSize = 0x08;
                uint ColorElemSize = 0x04;

                RatchetModel_Terrain model = new RatchetModel_Terrain();
                model.offset = 0x10;
                model.modelID = 1111;
                model.modelType = ModelType.Terrain;
                byte[] terrainHeadBlock = ReadBlock(cfs, 0x10, 0x60);

                uint headPointer = BAToUInt32(terrainHeadBlock, 0x0);
                ushort headCount = BAToUInt16(terrainHeadBlock, 0x06);

                byte[] terrainBlock = ReadBlock(cfs, headPointer, headCount * terrainElemSize);

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

                            byte[] texBlock = ReadBlock(cfs, texPointer, texElemSize * texCount);
                            for (uint iii = 0; iii < texCount; iii++)
                            {
                                RatchetTexture_Model tex = new RatchetTexture_Model();
                                tex.ID = BAToUInt32(texBlock, (iii * texElemSize) + 0x00);
                                tex.start = BAToUInt32(texBlock, (iii * texElemSize) + 0x04) + prevPrevFaceCount;
                                tex.size = BAToUInt32(texBlock, (iii * texElemSize) + 0x08) + prevPrevFaceCount;

                                localFaceCount += BAToUInt32(texBlock, (iii * texElemSize) + 0x08);

                                model.textureConfig.Add(tex);
                            }


                            byte[] vertBlock = ReadBlock(cfs, vertPointer + vertStart * vertElemSize, headVertCount * vertElemSize);
                            byte[] colBlock = ReadBlock(cfs, colPointer + vertStart * ColorElemSize, headVertCount * ColorElemSize);
                            byte[] UVBlock = ReadBlock(cfs, UVPointer + vertStart * UVElemSize, headVertCount * UVElemSize);

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



                            byte[] indiceBuff = ReadBlock(cfs, indicePointer + prevFaceCount * sizeof(ushort), localFaceCount * sizeof(ushort));
                            //Console.WriteLine(localFaceCount.ToString("X8"));
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
                DataStore.chunks.Add(model);
            }
            #endregion

            //TODO: textureConfig - Menus

            //TODO: textureConfig - 2D GFX

            //TODO: Sprites


            //Load the remaining unknown or unhandled data
            #region Remaining Data
            /*
            DataStore.mapRenderDefintions = getSection(fs, 0x04);
            DataStore.Ptr_08 = getSection(fs, 0x08);
            DataStore.Ptr_0C = getSection(fs, 0x0C);
            DataStore.skyBox = getSection(fs, 0x10);
            DataStore.collisionMap = getSection(fs, 0x14);
            DataStore.campaignPlayerAnimations = getSection(fs, 0x18);
            DataStore.Ptr_40 = getSection(fs, 0x40);
            DataStore.Ptr_44 = getSection(fs, 0x44);
            DataStore.soundsConfig = getSection(fs, 0x48);
            DataStore.Ptr_4C = getSection(fs, 0x4C);
            DataStore.Ptr_50 = getSection(fs, 0x50);
            DataStore.lighting = getSection(fs, 0x5C);
            DataStore.lightingConfig = getSection(fs, 0x64);
            DataStore.textureConfigMenu = getSection(fs, 0x68);
            DataStore.textureConfig2DGFX = getSection(fs, 0x70);
            DataStore.spriteDef = getSection(fs, 0x74);
            */
            #endregion

            #region raw Data //need to properly handle these eventually
            //DataStore.terrainModel.rawData = getSection(fs, 0x3C);
            //rawLevelModelBlock = getSection(fs, 0x1C);
            //rawSceneryModelBlock = getSection(fs, 0x2C);
            #endregion

            fs.Close();

            Console.WriteLine("Engine data loaded with new engine parser for UYA.");

        }
        //Returns the raw bytes for a model in the engine file
        public static byte[] getModelDataRaw(ModelType modelType, int modelID, string enginePath)
        {
            byte[] data = null;

            FileStream fs = File.OpenRead(enginePath);
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
        static byte[] getSection(FileStream fs, int off)
        {
            uint ptr = BAToUInt32(ReadBlock(fs, (uint)off, 0x04), 0);

            if (ptr == 0x00000000)
                return new byte[0];

            //Find next largest pointer to get size of section
            uint size = FindNextLargest(ReadBlock(fs, 0x00, 0x78), off) - ptr;

            byte[] ret = ReadBlock(fs, ptr, size);
            return ret;
        }
    }
}
