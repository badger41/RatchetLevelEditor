using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RatchetTexture;

public class RatchetModel
{
    public enum ModelType
    {
        Spawnable,
        Level,
        Scenery,
        Terrain,
        TerrainCollision
    };

    public struct RatchetModel_General
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

    public struct RatchetModel_Terrain
    {
        public uint offset;
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
    }

    public struct RatchetModel_TerrainCollision
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

}
