using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataFunctions;
using OpenTK;

class LevelObject
{
    public LevelObject()
    {
        mat = new Matrix4(v1x, v1y, v1z, v1w, v2x, v2y, v2z, v2w, v3x, v3y, v3z, v3w, x, y, z, w);
    }

    public float v1x;
    public float v1y;
    public float v1z;
    public float v1w;

    public float v2x;
    public float v2y;
    public float v2z;
    public float v2w;

    public float v3x;
    public float v3y;
    public float v3z;
    public float v3w;

    public float x;
    public float y;
    public float z;
    public float w;

    public uint off_40;
    public uint off_44;
    public uint off_48;
    public uint off_4C;

    public ushort off_50;
    public ushort modelID;
    public uint off_54;
    public uint off_58;
    public uint off_5C;

    public uint vertexColorsPtr;
    public uint off_64;
    public uint off_68;
    public uint off_6C;

    public Matrix4 mat;

    public byte[] vertexColors;

}
