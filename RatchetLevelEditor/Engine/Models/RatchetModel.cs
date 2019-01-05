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
        public ModelType modelType;
        public uint offset;
        public short modelID;
        public ushort vertexCount;
        public uint faceCount;
        public byte animationsCount;
        public byte[] rawData;
        public float size;
        public List<RatchetTexture_Model> textureConfig;
        public List<ModelVertex> vertices;
        public List<ModelFace> faces;
        public List<float> vertBuff;
        public List<ushort> indiceBuff;     
        public List<uint> animPointer;

    };

    class RatchetModel_Moby
    {
        public ModelType modelType {
            get
            {
                return ModelType.Spawnable;
            }
            set
            {
                modelType = value;
            }
        }

        #region Header
        //0x00
        public uint meshOffset;
        public uint header_ind_04;

        //0x08
        public byte boneCount;
        public byte header_ind_09;
        public byte header_ind_0A;
        public byte header_ind_0B;

        //0x0C
        public byte animCount;
        public byte sec0Count;
        public byte header_ind_0E;
        public byte header_ind_0F;

        //0x10
        public uint header_offset_10;
        public uint boneMatrixOffset;
        public uint bonesOffset;
        public uint attachmentListOffset;

        //0x20
        public uint header_ind_20;
        public float scale;
        public uint animSoundConfigOffset;
        public uint header_offset_2C;

        //0x30
        public float header_ind_30;
        public float header_ind_34;
        public float header_ind_38;
        public float header_ind_3C;

        //0x40
        public uint rgba;
        public uint offset_44;
        #endregion

        #region Animations
        //We will store this in case certain offset pointers are 0x00
        //So we can properly re-assign them during serialization
        public List<uint> animOffsets;

        public List<byte[]> animData;
        #endregion

        #region Attachments
        public int attachmentBoneCount;
        public List<uint> attachmentBoneIndices;
        public List<byte[]> attachmentBoneData;
        #endregion

        #region Mesh
        //Header 0x00
        public uint textureCount;
        public uint mesh_header_ind_04;
        public uint textureOffset;
        public uint mesh_header_offset_0C;

        //0x10
        public uint vertexOffset;
        public short vertexCount;
        public short mesh_header_ind_1A;
        public short mesh_header_ind_1C;
        public short mesh_header_ind_1E;

        //Data
        public List<ModelFace> faces;
        public List<RatchetTexture_Model> textureConfig;
        public List<RatchetAnimationSoundConfig> soundConfig;
        #endregion


    }

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

    public class ModelVertex
    {
        public float x;
        public float y;
        public float z;

        public float uvu;
        public float uvv;

        public int rgba;
    }

    public class ModelFace
    {
        public int index;
        public short v1;
        public short v2;
        public short v3;
    }

}
