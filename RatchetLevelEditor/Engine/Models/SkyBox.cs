using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatchetLevelEditor.Engine.Models
{
    class SkyBox
    {
        public int layerCount;
        public List<SkyBoxLayer> layers;
        public uint verticesPointer;
        public uint facesPointer;
        public int faceCount;
        public int vertexCount;
        public List<SkyBoxVertex> vertices;
        public List<SkyBoxFace> faces;

    }

    class SkyBoxLayer
    {
        public uint pointer;

        public short off_00;
        public short textureCount;
        public List<SkyBoxTexture> textures;
    }

    class SkyBoxTexture
    {
        public int textureId;
        public uint faceOffset;
        public int faceCount;

    }

    class SkyBoxVertex
    {
        public float x;
        public float y;
        public float z;

        public float uvu;
        public float uvv;

        public int rgba;
    }

    class SkyBoxFace
    {
        public int index;
        public short v1;
        public short v2;
        public short v3;
    }
}
