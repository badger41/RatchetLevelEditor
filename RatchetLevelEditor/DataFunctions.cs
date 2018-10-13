using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    class DataFunctions
    {
    public static void writeBytes(byte[] array, int line, byte[] data, int size)
    {
        for (int i = 0; i < size; i++)
        {
            array[line + i] = data[i];
        }
    }
    //writes the x bytes to a line of the specified (ie mapConfig) array
    public static void writeBytes(byte[] array, int line, uint data, int size)
    {
        // byte[] newData = UIntToByteArray(data);
        byte[] newData = BitConverter.GetBytes(data);
        Array.Reverse(newData);
        for (int i = 0; i < size; i++)
        {
            array[line + i] = newData[i];
        }
    }
    public double Matrix3DDeterminant(
        double m00, double m01, double m02,
        double m10, double m11, double m12,
        double m20, double m21, double m22)
    {
        return m00 * m11 * m22 + m01 * m12 * m20 + m02 * m10 * m21 - m00 * m12 * m21 - m01 * m10 * m22 - m02 * m11 * m20;
    }

    public Single[] MatrixToVector(byte[] block)
    {
        Single[] row0 = new Single[3], row1 = new Single[3], row2 = new Single[3];

        row0[0] = BAToFloat(block, 0x00);
        row0[1] = BAToFloat(block, 0x04);
        row0[2] = BAToFloat(block, 0x08);

        row1[0] = BAToFloat(block, 0x10);
        row1[1] = BAToFloat(block, 0x14);
        row1[2] = BAToFloat(block, 0x18);

        row2[0] = BAToFloat(block, 0x20);
        row2[1] = BAToFloat(block, 0x24);
        row2[2] = BAToFloat(block, 0x28);

        return MatrixToVector(row0, row1, row2);
    }

    public Single[] MatrixToVector(Single[] row0, Single[] row1, Single[] row2)
    {
        Single[] ret = new Single[3];
        Single[,] m = new Single[3, 3];
        m[0, 0] = row0[0]; m[0, 1] = row0[1]; m[0, 2] = row0[2];
        m[1, 0] = row1[0]; m[1, 1] = row1[1]; m[1, 2] = row1[2];
        m[2, 0] = row2[0]; m[2, 1] = row2[1]; m[2, 2] = row2[2];


        /*
            qw = √(1 + m00 + m11 + m22) /2
            qx = (m21 - m12)/( 4 *qw)
            qy = (m02 - m20)/( 4 *qw)
            qz = (m10 - m01)/( 4 *qw)
         */

        Single tr = row0[0] + row1[1] + row2[2];
        Single qw, qx, qy, qz;
        Single absQ2 = DoubleCheckFloat((float)Math.Pow(
            Matrix3DDeterminant(
            row0[0], row0[1], row0[2],
            row1[0], row1[1], row1[2],
            row2[0], row2[1], row2[2]), 1.0 / 3.0));
        if (absQ2 == 0f)
            absQ2 = 1f;

        qw = (float)Math.Sqrt(Math.Max(0, absQ2 + m[0, 0] + m[1, 1] + m[2, 2])) / 2;
        qx = (float)Math.Sqrt(Math.Max(0, absQ2 + m[0, 0] - m[1, 1] - m[2, 2])) / 2;
        qy = (float)Math.Sqrt(Math.Max(0, absQ2 - m[0, 0] + m[1, 1] - m[2, 2])) / 2;
        qz = (float)Math.Sqrt(Math.Max(0, absQ2 - m[0, 0] - m[1, 1] + m[2, 2])) / 2;
        qx *= Math.Sign(qx * (m[2, 1] - m[1, 2]));
        qy *= Math.Sign(qy * (m[0, 2] - m[2, 0]));
        qz *= Math.Sign(qz * (m[1, 0] - m[0, 1]));

        ret[0] = DoubleCheckFloat(qx) + 1f; // (4 * qw));
        ret[1] = DoubleCheckFloat(qz) + 1f; // (4 * qw));
        ret[2] = DoubleCheckFloat(qy) + 1f; // (4 * qw));

        return ret;
    }

    public static float DoubleCheckFloat(float ret)
    {
        if (float.IsInfinity(ret) || float.IsNaN(ret) || float.IsNegativeInfinity(ret) || float.IsPositiveInfinity(ret))
            return 0f;
        return ret;
    }

    public Single[] BAToQuaternion(byte[] a, uint off)
    {
        Single[] ret = new Single[4];

        for (int x = 0; x < 4; x++)
        {
            ret[x] = BAToFloat(a, off + (uint)(x * 4));
        }

        return ret;
    }

    public static float BAToFloat(byte[] a, uint off)
    {
        float ret = 0;
        if (BitConverter.IsLittleEndian)
        {
            byte[] temp = new byte[4];
            Array.Copy(a, off, temp, 0, 4);
            Array.Reverse(temp);
            ret = BitConverter.ToSingle(temp, 0);
        }
        else
        {
            ret = BitConverter.ToSingle(a, (int)off);
        }

        return DoubleCheckFloat(ret);
    }

    public static short BAToShort(byte[] a, uint off)
    {
        if (BitConverter.IsLittleEndian)
        {
            byte[] temp = new byte[2];
            Array.Copy(a, off, temp, 0, 2);
            Array.Reverse(temp);
            return BitConverter.ToInt16(temp, 0);
        }
        else
        {
            return BitConverter.ToInt16(a, (int)off);
        }
    }

    public static uint BAToUInt32(byte[] a, uint off)
    {
        if (BitConverter.IsLittleEndian)
        {
            byte[] temp = new byte[4];
            Array.Copy(a, off, temp, 0, 4);
            Array.Reverse(temp);
            return BitConverter.ToUInt32(temp, 0);
        }
        else
        {
            return BitConverter.ToUInt32(a, (int)off);
        }
    }

    public static int BAToInt32(byte[] a, uint off)
    {
        if (BitConverter.IsLittleEndian)
        {
            byte[] temp = new byte[4];
            Array.Copy(a, off, temp, 0, 4);
            Array.Reverse(temp);
            return BitConverter.ToInt32(temp, 0);
        }
        else
        {
            return BitConverter.ToInt32(a, (int)off);
        }
    }
    public static ushort BAToUInt16(byte[] a, uint off)
    {
        if (BitConverter.IsLittleEndian)
        {
            byte[] temp = new byte[2];
            Array.Copy(a, off, temp, 0, 2);
            Array.Reverse(temp);
            return BitConverter.ToUInt16(temp, 0);
        }
        else
        {
            return BitConverter.ToUInt16(a, (int)off);
        }
    }


    public static bool isBAEqual(byte[] a, byte[] b)
    {
        for (int x = 0; x < a.Length; x++)
        {
            if (a[x] != b[x])
                return false;
        }
        return true;
    }

    //Returns a byte array from inside another byte array
    public static byte[] getBytes(byte[] array, int ind, int length)
    {
        byte[] data = new byte[length];
        for (int i = 0; i < length; i++)
        {
            data[i] = array[ind + i];
        }
        return data;
    }

    public static byte[] ReadBlock(FileStream fs, uint offset, uint len)
    {
        byte[] ret = new byte[len];

        try
        {
            if (fs.Position != offset)
                fs.Seek(offset, SeekOrigin.Begin);
            fs.Read(ret, 0, (int)len);
        }
        catch { return null; }

        return ret;
    }
    public static byte[] ReadBlock(Stream s, uint offset, uint len)
    {
        byte[] ret = new byte[len];

        try
        {
            if (s.Position != (long)offset)
                s.Seek((long)offset, SeekOrigin.Begin);
            int x = 0;
            while (x < len)
            {
                ret[x] = (byte)s.ReadByte();
                x++;
            }
        }
        catch { return null; }

        return ret;
    }

    public static void WriteUint32(ref byte[] byteArr, int offset, UInt32 input)
    {
        byte[] byt = BitConverter.GetBytes(input);
        byteArr[offset + 0] = byt[3];
        byteArr[offset + 1] = byt[2];
        byteArr[offset + 2] = byt[1];
        byteArr[offset + 3] = byt[0];
    }

    public static void WriteFloat(ref byte[] byteArr, int offset, float input)
    {
        byte[] byt = BitConverter.GetBytes(input);
        byteArr[offset + 0] = byt[3];
        byteArr[offset + 1] = byt[2];
        byteArr[offset + 2] = byt[1];
        byteArr[offset + 3] = byt[0];
    }

    public static void WriteUint16(ref byte[] byteArr, int offset, UInt16 input)
    {
        byte[] byt = BitConverter.GetBytes(input);
        byteArr[offset + 0] = byt[1];
        byteArr[offset + 1] = byt[0];
    }



    public static void WriteInt32(ref byte[] byteArr, int offset, Int32 input)
    {
        byte[] byt = BitConverter.GetBytes(input);
        byteArr[offset + 0] = byt[3];
        byteArr[offset + 1] = byt[2];
        byteArr[offset + 2] = byt[1];
        byteArr[offset + 3] = byt[0];
    }

    public static uint ReadUInt32(byte[] a, int offset)
    {
        return (uint)((a[offset + 0] << 24) | (a[offset + 1] << 16) | (a[offset + 2] << 8) | (a[offset + 3] << 0));
    }
    public static int ReadInt32(byte[] a, int offset)
    {
        return (int)((a[offset + 0] << 24) | (a[offset + 1] << 16) | (a[offset + 2] << 8) | (a[offset + 3] << 0));
    }
    public static ushort ReadUInt16(byte[] a, int offset)
    {
        return (ushort)((a[offset + 0] << 8) | (a[offset + 1] << 0));
    }

    public static uint FindNextLargest(byte[] header, int index)
    {
        uint ret = 0;
        List<uint> pointers = new List<uint>();
        if (ReadUInt32(header, index) == 0x00000000)
            return 0;
        for (int i = 0; i < 0x78; i += 4)
        {
            if (i != 0x20 && i != 0x28 && i != 0x30 && i != 0x38 && i != 0x58 && i != 0x60 && i != 0x6C && ReadUInt32(header, i) != 0x00000000)
                pointers.Add(ReadUInt32(header, i));
        }
        pointers = pointers.OrderByDescending(z => z).Reverse().ToList();
        int slot = pointers.IndexOf(ReadUInt32(header, index));
        if (slot + 1 < pointers.Count)
            ret = pointers[slot + 1];
        else
            ret = pointers[slot];
        return ret;
    }
    public static uint FindNextLargestDL(byte[] header, int index)
    {
        uint ret = 0;
        uint pointer = ReadUInt32(header, index);
        List<uint> pointers = new List<uint>();
        if (ReadUInt32(header, index) == 0x00000000)
            return 0;
        for (int i = 0; i < 0x90; i += 4)
        {
            if (i != 0x24 && i != 0x2C && i != 0x38 && i != 0x40 && i != 0x64 && i != 0x6C && i != 0x78 && ReadUInt32(header, i) != 0x00000000)
                pointers.Add(ReadUInt32(header, i));
        }
        pointers = pointers.OrderByDescending(z => z).Reverse().ToList();
        int slot = pointers.IndexOf(pointer);
        if (slot + 1 < pointers.Count)
            ret = pointers[slot + 1];
        else
            ret = pointers[slot];
        return ret;
    }
}

