using RatchetLevelEditor;
using RatchetLevelEditor.Engine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static RatchetTexture;

class TextureParser
{
    public static Bitmap getTextureImage(int textureId)
    {
        RatchetTexture_General tex = DataStoreEngine.textures.Find(x => x.ID == textureId);
        int textureIndex = DataStoreEngine.textures.IndexOf(tex);
        if (tex.renderedImage == null)
        {
            Bitmap texImageBMap;
            byte[] imgRaw;
            byte[] newbytes;
            int width = tex.width;
            int height = tex.height;
            imgRaw = DecompressDxt5(tex.texData, width, height);
            if(imgRaw != null)
            {
                texImageBMap = new Bitmap(width, height, 4 * width, PixelFormat.Format32bppArgb, Marshal.UnsafeAddrOfPinnedArrayElement(imgRaw, 0));
                tex.renderedImage = texImageBMap;
            }
            
            //Update our data store with the updated texture
            //DataStore.textures[textureIndex] = tex;
            //return tex.renderedImage;// !tex.reverseRGB ? imgReverseRGB(texImageBMap) : texImageBMap;
        }

        return tex.renderedImage;
    }
    internal static byte[] DecompressDxt5(byte[] imageData, int width, int height)
    {
        if(imageData != null)
        {
            using (MemoryStream imageStream = new MemoryStream(imageData))
                return DecompressDxt5(imageStream, width, height);
        }
        return null;
    }

    internal static byte[] DecompressDxt5(Stream imageStream, int width, int height)
    {
        byte[] imageData = new byte[width * height * 4];

        using (BinaryReader imageReader = new BinaryReader(imageStream))
        {
            int blockCountX = (width + 3) / 4;
            int blockCountY = (height + 3) / 4;

            for (int y = 0; y < blockCountY; y++)
            {
                for (int x = 0; x < blockCountX; x++)
                {
                    DecompressDxt5Block(imageReader, x, y, blockCountX, width, height, imageData);
                }
            }
        }

        return imageData;
    }

    private static void DecompressDxt5Block(BinaryReader imageReader, int x, int y, int blockCountX, int width, int height, byte[] imageData)
    {
        byte alpha0 = imageReader.ReadByte();
        byte alpha1 = imageReader.ReadByte();

        ulong alphaMask = (ulong)imageReader.ReadByte();
        alphaMask += (ulong)imageReader.ReadByte() << 8;
        alphaMask += (ulong)imageReader.ReadByte() << 16;
        alphaMask += (ulong)imageReader.ReadByte() << 24;
        alphaMask += (ulong)imageReader.ReadByte() << 32;
        alphaMask += (ulong)imageReader.ReadByte() << 40;

        ushort c0 = imageReader.ReadUInt16();
        ushort c1 = imageReader.ReadUInt16();

        byte r0, g0, b0;
        byte r1, g1, b1;
        //ConvertRgb565ToRgb888(c0, out r0, out g0, out b0);
        //ConvertRgb565ToRgb888(c1, out r1, out g1, out b1);
        ConvertRgb565ToRgb888(c0, out b0, out g0, out r0);
        ConvertRgb565ToRgb888(c1, out b1, out g1, out r1);

        uint lookupTable = imageReader.ReadUInt32();

        for (int blockY = 0; blockY < 4; blockY++)
        {
            for (int blockX = 0; blockX < 4; blockX++)
            {
                byte r = 0, g = 0, b = 0, a = 255;
                uint index = (lookupTable >> 2 * (4 * blockY + blockX)) & 0x03;

                uint alphaIndex = (uint)((alphaMask >> 3 * (4 * blockY + blockX)) & 0x07);
                if (alphaIndex == 0)
                {
                    a = alpha0;
                }
                else if (alphaIndex == 1)
                {
                    a = alpha1;
                }
                else if (alpha0 > alpha1)
                {
                    a = (byte)(((8 - alphaIndex) * alpha0 + (alphaIndex - 1) * alpha1) / 7);
                }
                else if (alphaIndex == 6)
                {
                    a = 0;
                }
                else if (alphaIndex == 7)
                {
                    a = 0xff;
                }
                else
                {
                    a = (byte)(((6 - alphaIndex) * alpha0 + (alphaIndex - 1) * alpha1) / 5);
                }

                switch (index)
                {
                    case 0:
                        r = r0;
                        g = g0;
                        b = b0;
                        break;
                    case 1:
                        r = r1;
                        g = g1;
                        b = b1;
                        break;
                    case 2:
                        r = (byte)((2 * r0 + r1) / 3);
                        g = (byte)((2 * g0 + g1) / 3);
                        b = (byte)((2 * b0 + b1) / 3);
                        break;
                    case 3:
                        r = (byte)((r0 + 2 * r1) / 3);
                        g = (byte)((g0 + 2 * g1) / 3);
                        b = (byte)((b0 + 2 * b1) / 3);
                        break;
                }

                int px = (x << 2) + blockX;
                int py = (y << 2) + blockY;
                if ((px < width) && (py < height))
                {
                    int offset = ((py * width) + px) << 2;
                    imageData[offset] = r;
                    imageData[offset + 1] = g;
                    imageData[offset + 2] = b;
                    imageData[offset + 3] = a;
                }
            }
        }
    }
    private static void ConvertRgb565ToRgb888(ushort color, out byte r, out byte g, out byte b)
    {
        int temp;

        temp = (color >> 11) * 255 + 16;
        r = (byte)((temp / 32 + temp) / 32);
        temp = ((color & 0x07E0) >> 5) * 255 + 32;
        g = (byte)((temp / 64 + temp) / 64);
        temp = (color & 0x001F) * 255 + 16;
        b = (byte)((temp / 32 + temp) / 32);
    }
}

