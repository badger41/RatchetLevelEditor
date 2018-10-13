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
        public int width;
        public int height;
        public byte[] texData;
        public byte[] texHeader;
        public bool reverseRGB;
        public Bitmap renderedImage;
    }

    //Used specifically in model loading
    public struct RatchetTexture_Model
    {
        public uint ID;
        public uint start;//
        public uint size;
    };
}
