using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataFunctions;
using OpenTK;

class LevelObject
{
    const int LEVELOBJECTELEMSIZE = 0x70;

    public ushort off_50;
    public ushort modelID;
    public uint off_54;
    public uint off_58;
    public uint off_5C;

    public uint off_60;
    public uint off_64;
    public uint off_68;
    public uint off_6C;

    public Matrix4 mat;

    public LevelObject(byte[] levelBlock, uint num)
    {
        float v1x =       BAToFloat(levelBlock, (LEVELOBJECTELEMSIZE * num) + 0x00);
        float v1y =       BAToFloat(levelBlock, (LEVELOBJECTELEMSIZE * num) + 0x04);
        float v1z =       BAToFloat(levelBlock, (LEVELOBJECTELEMSIZE * num) + 0x08);
        float v1w =       BAToFloat(levelBlock, (LEVELOBJECTELEMSIZE * num) + 0x0C);

        float v2x =       BAToFloat(levelBlock, (LEVELOBJECTELEMSIZE * num) + 0x10);
        float v2y =       BAToFloat(levelBlock, (LEVELOBJECTELEMSIZE * num) + 0x14);
        float v2z =       BAToFloat(levelBlock, (LEVELOBJECTELEMSIZE * num) + 0x18);
        float v2w =       BAToFloat(levelBlock, (LEVELOBJECTELEMSIZE * num) + 0x1C);

        float v3x =       BAToFloat(levelBlock, (LEVELOBJECTELEMSIZE * num) + 0x20);
        float v3y =       BAToFloat(levelBlock, (LEVELOBJECTELEMSIZE * num) + 0x24);
        float v3z =       BAToFloat(levelBlock, (LEVELOBJECTELEMSIZE * num) + 0x28);
        float v3w =       BAToFloat(levelBlock, (LEVELOBJECTELEMSIZE * num) + 0x2C);

        float x =         BAToFloat(levelBlock, (LEVELOBJECTELEMSIZE * num) + 0x30);
        float y =         BAToFloat(levelBlock, (LEVELOBJECTELEMSIZE * num) + 0x34);
        float z =         BAToFloat(levelBlock, (LEVELOBJECTELEMSIZE * num) + 0x38);
        float w =         BAToFloat(levelBlock, (LEVELOBJECTELEMSIZE * num) + 0x3C);

        mat = new Matrix4(v1x, v1y, v1z, v1w,    v2x, v2y, v2z,    v2w,v3x, v3y, v3z, v3w,    x, y, z, w);

        /* These offsets are just placeholders for the render distance quaternion which is set in-game
        off_40 =    BAToUInt32(levelBlock, (LEVELOBJECTELEMSIZE * num) + 0x40);
        off_44 =    BAToUInt32(levelBlock, (LEVELOBJECTELEMSIZE * num) + 0x44);
        off_48 =    BAToUInt32(levelBlock, (LEVELOBJECTELEMSIZE * num) + 0x48);
        off_4C =    BAToUInt32(levelBlock, (LEVELOBJECTELEMSIZE * num) + 0x4C);
        */

        off_50 =    BAToUInt16(levelBlock, (LEVELOBJECTELEMSIZE * num) + 0x50);
        modelID =   BAToUInt16(levelBlock, (LEVELOBJECTELEMSIZE * num) + 0x52);
        off_54 =    BAToUInt32(levelBlock, (LEVELOBJECTELEMSIZE * num) + 0x54);
        off_58 =    BAToUInt32(levelBlock, (LEVELOBJECTELEMSIZE * num) + 0x58);
        off_5C =    BAToUInt32(levelBlock, (LEVELOBJECTELEMSIZE * num) + 0x5C);

        off_60 =    BAToUInt32(levelBlock, (LEVELOBJECTELEMSIZE * num) + 0x60);
        off_64 =    BAToUInt32(levelBlock, (LEVELOBJECTELEMSIZE * num) + 0x64);
        off_68 =    BAToUInt32(levelBlock, (LEVELOBJECTELEMSIZE * num) + 0x68);
        off_6C =    BAToUInt32(levelBlock, (LEVELOBJECTELEMSIZE * num) + 0x6C);
    }
}
