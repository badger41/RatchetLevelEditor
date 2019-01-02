using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class RatchetTexture
{
    public struct RatchetTexture_General
    {
        public int ID;
        public byte[] texData;
        public byte[] texHeader;
        public bool reverseRGB;
        public Bitmap renderedImage;

        public uint texDataPointer;
        public uint off_04;
        public uint off_08;
        public uint off_0C;
        public uint off_10;
        public uint off_14;
        public int width;
        public int height;
        public uint off_1C;
        public uint off_20;
    }

    //Used specifically in model loading
    public struct RatchetTexture_Model
    {
        public uint textureId;
        public uint faceOffset;//
        public uint faceCount;
    };
}
