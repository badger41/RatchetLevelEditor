using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RatchetTexture;


namespace RatchetLevelEditor.Engine.Models
{
        public enum ModelType
        {
            Spawnable,
            Level,
            Scenery,
            Terrain,
            TerrainCollision,
            SkyBox
        };

        public class RatchetModel_General
        {
            public uint offset;
            public ushort vertexCount;
            public uint faceCount;
            public short modelID;
            public ModelType modelType;
            public byte[] rawData;
            public float size;
            public List<RatchetTexture_Model> textureConfig;
            public List<float> vertBuff;
            public List<ushort> indiceBuff;
            public byte animationsCount;
            public List<uint> animPointer;
        };

        class RatchetModel_Terrain
        {
            public uint offset;
            public uint headCount;
            public uint vertexCount;
            public uint faceCount;
            public uint totalSize;
            public uint textureIndex;
            public uint animationsCount;
            public List<RatchetTexture_Model> textureConfig;
            public ModelType modelType;
            public short modelID;
            public byte[] rawData;
            public string wavefront_Obj;
            public string mtls;
            public List<float> vertBuff;
            public List<uint> indiceBuff;
            public byte[] terrainBlock;
            public byte[] unknownBlock1;
            public byte[] unknownBlock2;
            public byte[] unknownBlock3;
            public byte[] vertexIndices;
            public byte[] vertexColors; //DL
        }

        class RatchetModel_TerrainCollision
        {
            public uint offset;
            public uint vertexCount;
            public uint faceCount;
            public uint totalSize;
            public uint textureIndex;
            public uint animationsCount;
            public List<int> textureIds;
            public short modelID;
            public ModelType modelType;
            public byte[] rawData;
        }

        class RatchetModel_SkyBox
        {
            public ModelType modelType;
            public int layerCount;
            public List<SkyBoxLayer> layers;
            public uint verticesPointer;
            public uint facesPointer;
            public int faceCount;
            public int vertexCount;
            public List<ModelVertex> vertices;
            public List<ModelFace> faces;
        }

        class SkyBoxLayer
        {
            public uint pointer;

            public short off_00;
            public short textureCount;
            public List<RatchetTexture_Model> textures;
        }

        class ModelVertex
        {
            public float x;
            public float y;
            public float z;

            public float uvu;
            public float uvv;

            public int rgba;
        }

        class ModelFace
        {
            public int index;
            public short v1;
            public short v2;
            public short v3;
        }

}
