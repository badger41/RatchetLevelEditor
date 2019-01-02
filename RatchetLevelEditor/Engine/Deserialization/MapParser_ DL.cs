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
using RatchetLevelEditor.Engine;
using RatchetLevelEditor.Gameplay;

namespace RatchetLevelEditor
{
    class MapParser_DL
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

            loadMobyPvarMap();


            uint racNum = 4;

            #region GameplayHeader
            GameplayHeader gameplayHeader = new GameplayHeader(gpf, racNum);
            DataStoreGameplay.gameplayHeader = gameplayHeader;

            #endregion

            #region pVars
            List<byte[]> pVars;
            uint numpVars;
            byte[] pVarHeadBlock;
            uint pVarSectionLength;
            byte[] pVarBlock;
            switch (racNum)
            {
                case 4:
                    pVars = new List<byte[]>();
                    numpVars = (gameplayHeader.pVarPointer - gameplayHeader.pVarListPointer) / 8;
                    pVarHeadBlock = ReadBlock(gpf, gameplayHeader.pVarListPointer, numpVars * 8);
                    pVarSectionLength = 0;
                    for (uint i = 0; i < numpVars; i++)
                    {
                        pVarSectionLength += BAToUInt32(pVarHeadBlock, (i * 8) + 0x04);
                    }
                    pVarBlock = ReadBlock(gpf, gameplayHeader.pVarPointer, pVarSectionLength);
                    for (uint i = 0; i < numpVars; i++)
                    {
                        uint mobpVarsStart = BAToUInt32(pVarHeadBlock, (i * 8));
                        uint mobpVarsCount = BAToUInt32(pVarHeadBlock, (i * 8) + 0x04);
                        byte[] mobpVars = new byte[mobpVarsCount];
                        mobpVars = getBytes(pVarBlock, (int) mobpVarsStart, (int) mobpVarsCount);
                        pVars.Add(mobpVars);
                    }
                    DataStoreGameplay.pVarList = pVars;
                    break;
            }
            #endregion pVars

            #region Mobies

            uint mobyCount = BAToUInt32(ReadBlock(gpf, gameplayHeader.mobyPointer, 4), 0);
            byte[] mobyBlock = ReadBlock(gpf, gameplayHeader.mobyPointer + 0x10, mobyCount * gameplayHeader.mobyElemSize);

            for (uint i = 0; i < mobyCount; i++)
            {
                RatchetMoby mob = new RatchetMoby(racNum, mobyBlock, i);
                DataStoreGameplay.mobs.Add(mob);
            }
            #endregion

            DataStoreGameplay.splines = new List<Spline>();

            uint splineCount = BAToUInt32(ReadBlock(gpf, gameplayHeader.splinePointer, 4), 0);
            uint splineOffset = BAToUInt32(ReadBlock(gpf, gameplayHeader.splinePointer + 4, 4), 0);
            uint splineSectionSize = BAToUInt32(ReadBlock(gpf, gameplayHeader.splinePointer + 8, 4), 0);
            byte[] splineHeadBlock = ReadBlock(gpf, gameplayHeader.splinePointer + 0x10, splineCount * 4);
            byte[] splineBlock = ReadBlock(gpf, gameplayHeader.splinePointer + splineOffset, splineSectionSize);
            for (uint i = 0; i < splineCount; i++)
            {
                uint offset = BAToUInt32(splineHeadBlock, (i * 4));
                DataStoreGameplay.splines.Add(new Spline(splineBlock, offset));
            }


            #region EngineHeader
            byte[] engineHeader = ReadBlock(fs, 0, 0x90);

            //Data from header
                uint spawnablesPointer = BAToUInt32(engineHeader, 0x00);
                //(0x04)Map render definitions
                //(0x08)null
                //(0x0C)null
                //(0x14)Skybox
                //(0x18)Collisionmap
                uint levelModelsPointer = BAToUInt32(engineHeader, 0x20);
                uint levelModelsCount = BAToUInt32(engineHeader, 0x24);
                uint levelModelsPlacementPointer = BAToUInt32(engineHeader, 0x28);
                uint levelObjectsCount = BAToUInt32(engineHeader, 0x2C);
                uint sceneryModelsPointer = BAToUInt32(engineHeader, 0x34);
                uint sceneryModelsCount = BAToUInt32(engineHeader, 0x38);
                uint sceneryModelsPlacementPointer = BAToUInt32(engineHeader, 0x3C);
                uint sceneryObjectsCount = BAToUInt32(engineHeader, 0x40);
                uint terrainPointer = BAToUInt32(engineHeader, 0x48);
                //(0x44) ??
                //(0x54)Sound config (Menus + terrain)
                uint texturesPointer = BAToUInt32(engineHeader, 0x60);
                uint textureCount = BAToUInt32(engineHeader, 0x64);
                //(0x68)Lighting pointer
                uint lightingLevel = BAToUInt32(engineHeader, 0x6C);
                uint textureConfigMenuCount = BAToUInt32(engineHeader, 0x78);
            //(0x7C)textureConfig2DGFX
            //(0x80)Sprite def
            //(0x88) sound config (spawnables + everything else)

            EngineHeader engHead = new EngineHeader(fs, 4);
            DataStoreEngine.engineHeader = engHead;
            #endregion

            //Load DL Spawnable Models
            #region Spawnables
            uint spawnablesCount = BAToUInt32(ReadBlock(fs, spawnablesPointer, 4), 0);
            byte[] idBlock = ReadBlock(fs, spawnablesPointer + 4, spawnablesCount * 8);

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
                DataStoreEngine.spawnableModels.Add(model);
            }
            #endregion

            //Load DL Level Models
            #region Level Models
            uint levelElemSize = 0x40;
            byte[] levelBlock = ReadBlock(fs, levelModelsPointer, levelModelsCount * levelElemSize);

            for (uint x = 0; x < levelModelsCount; x++)
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

                model.offset = (uint)(x * 0x40) + levelModelsPointer;
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

                DataStoreEngine.levelModels.Add(model);
            }
            #endregion

            //Load DL Scenery Models
            #region Scenery Models
            uint sceneElemSize = 0x40;
            byte[] sceneBlock = ReadBlock(fs, sceneryModelsPointer, sceneryModelsCount * sceneElemSize);

            for (uint x = 0; x < sceneryModelsCount; x++)
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

                DataStoreEngine.sceneryModels.Add(model);
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
                model.offset = terrainPointer;
                model.modelID = 1111;
                model.modelType = ModelType.Terrain;
                byte[] terrainHeadBlock = ReadBlock(fs, terrainPointer, 0x60);

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
                DataStoreEngine.terrainModel = model;
            }
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
                DataStoreEngine.chunks.Add(model);
            }
            #endregion


            //Load Level Model Placement
            byte[] levelObjectBlock = ReadBlock(fs, levelModelsPlacementPointer, levelObjectsCount * 0x70);
            DataStoreEngine.levelObjects = new List<LevelObject>();
            for (uint i = 0; i < levelObjectsCount; i++)
            {
                //LevelObject levObj = new LevelObject(levelObjectBlock, i);
                //DataStoreEngine.levelObjects.Add(levObj);
            }

            //Load Scenery Model Placement
            byte[] sceneryObjectBlock = ReadBlock(fs, sceneryModelsPlacementPointer, sceneryObjectsCount * 0x70);
            DataStoreEngine.sceneryObjects = new List<LevelObject>();
            for (uint i = 0; i < sceneryObjectsCount; i++)
            {
                //LevelObject levObj = new LevelObject(sceneryObjectBlock, i);
               // DataStoreEngine.sceneryObjects.Add(levObj);
            }

            /*
            //Load Level Model Placement
            #region Level Model Placement
            for (int x = 0; x < UYALevelObjectsCount; x++)
            {
                UYALevelObjectConfig levelObject = new UYALevelObjectConfig();
                byte[] objectBlock = ReadBlock(fs, levelModelsPlacementPointer + (uint)(0x70 * x), 0x70);

                levelObject.v1 = BAToQuaternion(objectBlock, 0x00);
                levelObject.v2 = BAToQuaternion(objectBlock, 0x10);
                levelObject.v3 = BAToQuaternion(objectBlock, 0x20);
                levelObject.off = BAToQuaternion(objectBlock, 0x30);
                levelObject.size = BAToFloat(objectBlock, 0x3C);
                levelObject.off_40 = BAToUInt32(objectBlock, 0x40);
                levelObject.off_44 = BAToUInt32(objectBlock, 0x44);
                levelObject.off_48 = BAToUInt32(objectBlock, 0x48);
                levelObject.off_4C = BAToUInt32(objectBlock, 0x4C);
                levelObject.ID = (ushort)BAToShort(objectBlock, 0x52);
                levelObject.off_54 = BAToUInt32(objectBlock, 0x54);
                levelObject.off_58 = BAToUInt32(objectBlock, 0x58);
                levelObject.off_5C = BAToUInt32(objectBlock, 0x5C);
                levelObject.ptr_60 = BAToUInt32(objectBlock, 0x60);
                levelObject.off_64 = BAToUInt32(objectBlock, 0x64);
                levelObject.off_68 = BAToUInt32(objectBlock, 0x68);
                levelObject.off_6C = BAToUInt32(objectBlock, 0x6C);
                if (x + 1 < UYALevelObjectsCount)
                    levelObject.unknown_data_block = ReadBlock(fs, levelObject.ptr_60, (BAToUInt32(ReadBlock(fs, levelModelsPlacementPointer + (uint)(0x70 * (x + 1)) + 0x60, 4), 0x00) - levelObject.ptr_60));
                else
                    levelObject.unknown_data_block = ReadBlock(fs, levelObject.ptr_60, FindNextLargest(engineHeader, 0x24) - levelObject.ptr_60);
                UYALevelObjects.Add(levelObject);
            }
            #endregion

            //Load Scenery Models Placement
            #region Scenery Model Placement
            for (int x = 0; x < UYASceneryObjectsCount; x++)
            {
                UYASceneryObjectConfig sceneryObject = new UYASceneryObjectConfig();
                byte[] objectBlock = ReadBlock(fs, sceneryModelsPlacementPointer + (uint)(0x70 * x), 0x70);

                sceneryObject.v1 = BAToQuaternion(objectBlock, 0x00);
                sceneryObject.v2 = BAToQuaternion(objectBlock, 0x10);
                sceneryObject.v3 = BAToQuaternion(objectBlock, 0x20);
                sceneryObject.off = BAToQuaternion(objectBlock, 0x30);
                sceneryObject.size = BAToFloat(objectBlock, 0x3C);
                sceneryObject.off_40 = BAToUInt32(objectBlock, 0x40);
                sceneryObject.off_44 = BAToUInt32(objectBlock, 0x44);
                sceneryObject.off_48 = BAToUInt32(objectBlock, 0x48);
                sceneryObject.off_4C = BAToUInt32(objectBlock, 0x4C);
                sceneryObject.ID = (ushort)BAToShort(objectBlock, 0x52);
                sceneryObject.off_54 = BAToUInt32(objectBlock, 0x54);
                sceneryObject.off_54 = BAToUInt32(objectBlock, 0x58);
                sceneryObject.off_54 = BAToUInt32(objectBlock, 0x5C);
                sceneryObject.off_60 = BAToUInt32(objectBlock, 0x60);
                sceneryObject.off_64 = BAToUInt32(objectBlock, 0x64);
                sceneryObject.off_64 = BAToUInt32(objectBlock, 0x68);
                sceneryObject.off_64 = BAToUInt32(objectBlock, 0x6C);

                UYASceneryObjects.Add(sceneryObject);
            }
            #endregion
    */
            //Load Textures
            #region Textures
            RatchetTexture_General texture = new RatchetTexture_General();
            for (int x = 0; x < textureCount; x++)
            {

                texture.texHeader = ReadBlock(fs, texturesPointer + (uint)(0x24 * x), 0x24);
                uint nextTexturePointer = x < textureCount -1 ? ReadUInt32(ReadBlock(fs, texturesPointer + (uint)(0x24 * (x + 1)), 0x4), 0) : vfs != null ? (uint)vfs.Length : 0;
                uint currentTexturePointer =  ReadUInt32(texture.texHeader, 0);
                //Console.WriteLine("Parsing texture " + x + " " + currentTexturePointer.ToString("X") + " " + nextTexturePointer.ToString("X"));
                texture.ID = x;
                texture.width = BAToShort(texture.texHeader, 0x18);
                texture.height = BAToShort(texture.texHeader, 0x1A);
                texture.texData = vfs != null ? ReadBlock(vfs, currentTexturePointer, nextTexturePointer - currentTexturePointer) : null;
                texture.reverseRGB = false;
                DataStoreEngine.textures.Add(texture);
                //Console.WriteLine("Texture : " + x + " Width: " + texture.width + " Height: " + texture.height);
            }
            #endregion

            //TODO: textureConfig - Menus

            //TODO: textureConfig - 2D GFX

            //TODO: Sprites


            //Load the remaining unknown or unhandled data
            #region Remaining Data

            DataStoreEngine.mapRenderDefintions = getSection(fs, 0x04);
            DataStoreEngine.enginePtr_08 = getSection(fs, 0x08);
            DataStoreEngine.enginePtr_0C = getSection(fs, 0x0C);
            //DataStoreEngine.skyBox = getSection(fs, 0x14);
            DataStoreEngine.collisionMap = getSection(fs, 0x18);
            DataStoreEngine.enginePtr_44 = getSection(fs, 0x44);
            DataStoreEngine.soundsConfig = getSection(fs, 0x54);
            DataStoreEngine.enginePtr_4C = getSection(fs, 0x4C);
            DataStoreEngine.enginePtr_50 = getSection(fs, 0x50);
            DataStoreEngine.lighting = getSection(fs, 0x68);
            DataStoreEngine.lightingConfig = getSection(fs, 0x6C);
            DataStoreEngine.textureConfigMenu = getSection(fs, 0x74);
            DataStoreEngine.textureConfig2DGFX = getSection(fs, 0x7C);
            DataStoreEngine.spriteDef = getSection(fs, 0x80);

            #endregion

            #region raw Data //need to properly handle these eventually
            DataStoreEngine.terrainModel.rawData = getSection(fs, 0x48);
            //rawLevelModelBlock = getSection(fs, 0x1C);
            //rawSceneryModelBlock = getSection(fs, 0x2C);
            #endregion

            #region Missions
            MissionParser.parseMissions(path);
            #endregion

            fs.Close();
            gpf.Close();


            Console.WriteLine("Engine data loaded with new engine parser for DL.");

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
            uint size = FindNextLargestDL(ReadBlock(fs, 0x00, 0x90), off) - ptr;

            byte[] ret = ReadBlock(fs, ptr, size);
            return ret;
        }

        public static void loadMobyPvarMap()
        {
            List<MobyPropertyVariableConfig> pVarConfigs = new List<MobyPropertyVariableConfig>();
            string jsonString = File.ReadAllText("./Config/Rac4/MobyPropertyVariables.json");
            pVarConfigs = JsonConvert.DeserializeObject<List<MobyPropertyVariableConfig>>(jsonString);
            DataStoreGlobal.pVarMap = pVarConfigs;
        }
    }

}
