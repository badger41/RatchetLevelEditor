using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataFunctions;
using OpenTK.Graphics.OpenGL;
using RatchetLevelEditor.Gameplay;

namespace RatchetLevelEditor
{
    public class Spline
    {

        public float[] vertexBuffer;
        int VBO;

        public Spline(byte[] splineBlock, uint offset)
        {
            int count = BAToInt32(splineBlock, offset);
            vertexBuffer = new float[count * 3];
            for (uint i = 0; i < count; i++)
            {
                vertexBuffer[(i * 3) + 0] = BAToFloat(splineBlock, offset + 0x10 + (i * 0x10) + 0x00);
                vertexBuffer[(i * 3) + 1] = BAToFloat(splineBlock, offset + 0x10 + (i * 0x10) + 0x04);
                vertexBuffer[(i * 3) + 2] = BAToFloat(splineBlock, offset + 0x10 + (i * 0x10) + 0x08);
                //vertexBuffer[i] = ReadFloat(splineBlock, offset + (i * 0x10));
            }
        }

        public void getVBO()
        {
            //Get the vertex buffer object, or create one if one doesn't exist
            if (VBO == 0)
            {
                GL.GenBuffers(1, out VBO);
                GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
                GL.BufferData(BufferTarget.ArrayBuffer, vertexBuffer.Length * sizeof(float), vertexBuffer, BufferUsageHint.StaticDraw);
                //Console.WriteLine("Generated VBO with ID: " + VBO.ToString());
            }
            else
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            }
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 3, 0);
        }

        public static void serialize(ref byte[] gameplay_ntsc, int index)
        {
            int currentOffset = gameplay_ntsc.Length;

            //This pointer comes right after the spline count, it tells the game where to start reading the actual spline data
            uint pointerToSplineData = 0;
            int expectedMasterHeaderSize = 0x10 + (DataStoreGameplay.splines.Count * 0x04);

            //We have to do this to ensure our pointers end in a 0, if not, the game will crash
            while (expectedMasterHeaderSize % 10 != 0)
                expectedMasterHeaderSize += 0x04;

            byte[] splineMasterHeader = new byte[expectedMasterHeaderSize];
            WriteUint32(ref splineMasterHeader, 0x00, (uint)DataStoreGameplay.splines.Count);

            byte[] splineDataBlock = new byte[0];

            foreach (Spline spline in DataStoreGameplay.splines)
            {
                int currentSplineOffset = splineDataBlock.Length;
                //Write the pointer to the current spline data in the header
                WriteUint32(ref splineMasterHeader, 0x10 + (DataStoreGameplay.splines.IndexOf(spline) * 4), (uint)currentSplineOffset);

                //How many vertices are in this spline
                int vertexCount = spline.vertexBuffer.Length / 3;

                //Create a local byte array thats the size we need for this spline
                uint expectedSize = (uint)(0x10 + (vertexCount * 0x10));
                byte[] splineHeader = new byte[expectedSize];
                WriteUint32(ref splineHeader, 0x00, (uint)vertexCount);
                WriteUint32(ref splineHeader, 0x04, 0x00000000);
                WriteUint32(ref splineHeader, 0x08, 0x00000000);
                WriteUint32(ref splineHeader, 0x0C, 0x00000000);
                for (int i = 0; i < vertexCount; i++)
                {
                    WriteFloat(ref splineHeader, 0x10 + (i * 0x10) + 0x00, spline.vertexBuffer[(i * 3) + 0]);
                    WriteFloat(ref splineHeader, 0x10 + (i * 0x10) + 0x04, spline.vertexBuffer[(i * 3) + 1]);
                    WriteFloat(ref splineHeader, 0x10 + (i * 0x10) + 0x08, spline.vertexBuffer[(i * 3) + 2]);
                    WriteUint32(ref splineHeader, 0x10 + (i * 0x10) + 0x0C, 0xBF800000);
                }
                Array.Resize(ref splineDataBlock, (int)(splineDataBlock.Length + expectedSize));
                writeBytes(splineDataBlock, currentSplineOffset, splineHeader, splineHeader.Length);
            }
            WriteUint32(ref splineMasterHeader, 0x04, (uint)splineMasterHeader.Length);
            WriteUint32(ref splineMasterHeader, 0x08, (uint)splineDataBlock.Length);

            //Write our serialized data to the main array
            Array.Resize(ref gameplay_ntsc, (int)(gameplay_ntsc.Length + splineMasterHeader.Length));
            writeBytes(gameplay_ntsc, currentOffset, splineMasterHeader, splineMasterHeader.Length);

            //Ensure that the next offset ends in 0
            uint resize = 0x00;
            while ((gameplay_ntsc.Length + splineDataBlock.Length + resize) % 0x10 != 0)
                resize += 0x04;

            currentOffset = gameplay_ntsc.Length;
            Array.Resize(ref gameplay_ntsc, (int)(gameplay_ntsc.Length + splineDataBlock.Length + resize));
            writeBytes(gameplay_ntsc, currentOffset, splineDataBlock, splineDataBlock.Length - splineMasterHeader.Length);
        }
    }
}
