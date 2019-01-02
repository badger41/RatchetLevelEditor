﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataFunctions;
using RatchetLevelEditor;
using static RatchetTexture;
using RatchetLevelEditor.Engine;
using RatchetLevelEditor.Engine.Models;

class GadgetsParser
    {
    public static void parseGadgets(string path, string fileName)
    {
        FileStream fs = File.OpenRead(path + "/" + fileName);

        FileStream vfs = null;
        if (File.Exists(path + "/gadgets.vram"))
            vfs = File.OpenRead(path + "/gadgets.vram");

        #region Gadget Textures
        uint texturePointer = BAToUInt32(ReadBlock(fs, 0x3C4, 4), 0);
        int textureCount = 0;
        for (uint i = texturePointer; i < fs.Length; i += 0x24)
        {
            textureCount += 1;
        }
        for (int x = 0; x < textureCount; x++)
        {
            RatchetTexture_General texture = new RatchetTexture_General();
            texture.texHeader = ReadBlock(fs, texturePointer + (uint)(0x24 * x), 0x24);
            uint nextTexturePointer = x < textureCount - 1 ? ReadUInt32(ReadBlock(fs, texturePointer + (uint)(0x24 * (x + 1)), 0x4), 0) : vfs != null ? (uint)vfs.Length : 0;
            uint currentTexturePointer = ReadUInt32(texture.texHeader, 0);
            //Console.WriteLine("Parsing texture " + x + " " + currentTexturePointer.ToString("X") + " " + nextTexturePointer.ToString("X"));
            texture.ID = x;
            texture.width = BAToShort(texture.texHeader, 0x18);
            texture.height = BAToShort(texture.texHeader, 0x1A);
            texture.texData = vfs != null ? ReadBlock(vfs, currentTexturePointer, nextTexturePointer - currentTexturePointer) : null;
            texture.reverseRGB = false;
            DataStoreEngine.textures.Add(texture);
        }
        #endregion

        #region Gadget Models
        uint gadgetCount = 47;

        byte[] pointerBlock = ReadBlock(fs, 0x00, gadgetCount * 4);

        //Used for mapping textures to models
        int modelIndex = 0;

        for (int x = 0; x < gadgetCount; x++)
        {
            RatchetModel_General model = new RatchetModel_General();
            model.modelID = (short)(x + 0xA000); //they dont have IDs
            model.offset = BAToUInt32(pointerBlock, (uint)(x * 4));
           // Console.WriteLine("Model: " + model.modelID.ToString("X4") + " Offset: " + model.offset.ToString("X"));

            if (model.offset != 0)
            {

                uint modelHeadSize = BAToUInt32(ReadBlock(fs, model.offset, 4), 0);
                if (modelHeadSize > 0)
                {
                    byte[] headBlock = ReadBlock(fs, model.offset, modelHeadSize + 0x20); //Head + objectDetails
                    uint objectPtr = BAToUInt32(headBlock, 0);
                    model.animationsCount = headBlock[0x0C];

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

                    model.modelType = ModelType.Spawnable;
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
                        modTex.textureId = (uint)(modelIndex);
                        modTex.faceOffset = BAToUInt32(texBlock, (t * texElemSize) + 0x04);
                        modTex.faceCount = BAToUInt32(texBlock, (t * texElemSize) + 0x08);
                        model.textureConfig.Add(modTex);
                        model.faceCount += modTex.faceCount;
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
                    if (model.vertBuff != null)
                        modelIndex++;
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


    }

}
