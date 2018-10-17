using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RatchetLevelEditor.GameplayParser;
using static DataFunctions;
using System.IO;

namespace RatchetLevelEditor
{
    /***
     * This class handles serializing the gameplay elements into the format that is used in the games.
     * 
     * 
     ***/

    class GameplaySerializer
    {
        //Dictionary to define the mapping of each index of the gameplay file and its respective function
        //Same deal as gameplayParser, except its the reverse, we will be converting each element into a byte array
        public static Dictionary<int, dynamic> headerMap;

        static List<unknownDataIndex> offsets = new List<unknownDataIndex>();
        static byte[] gameplay_ntsc;

        public static void getHeaderMap(int racNum)
        {
            switch (racNum)
            {
                case 1:
                    break;
                case 2:
                case 3:
                    headerMap = new Dictionary<int, dynamic>()
                    {
                        {0x00, new Action<dynamic>(i => { serializeFogAndDeathBarrier(0x00); })},
                        {0x04, new Action<dynamic>(i => { serializeCameraDef(0x04); })},
                        {0x08, new Action<dynamic>(i => { serializeUnknownHeaderData(0x08); }) },
                        {0x0C, new Action<dynamic>(i => { serializeUnknownHeaderData(0x0C); }) },
                        {0x10, new Action<dynamic>(i => { serializeGameStrings(0x10); })},
                        {0x14, new Action<dynamic>(i => { serializeGameStrings(0x14); })},
                        {0x18, new Action<dynamic>(i => { serializeGameStrings(0x18); })},
                        {0x1C, new Action<dynamic>(i => { serializeGameStrings(0x1C); })},
                        {0x20, new Action<dynamic>(i => { serializeGameStrings(0x20); })},
                        {0x24, new Action<dynamic>(i => { serializeGameStrings(0x24); })},
                        {0x28, new Action<dynamic>(i => { serializeGameStrings(0x28); })},
                        {0x2C, new Action<dynamic>(i => { serializeGameStrings(0x2C); })},
                        {0x30, new Action<dynamic>(i => { serializeUnknownHeaderData(0x30); }) },
                        {0x34, new Action<dynamic>(i => { serializeUnknownHeaderData(0x34); }) },
                        {0x38, new Action<dynamic>(i => { serializeUnknownHeaderData(0x38); }) },
                        {0x3C, new Action<dynamic>(i => { serializeUnknownHeaderData(0x3C); }) },
                        {0x40, new Action<dynamic>(i => { serializeUnknownHeaderData(0x40); }) },
                        {0x44, new Action<dynamic>(i => { serializeUnknownHeaderData(0x44); }) },
                        {0x48, new Action<dynamic>(i => { serializeUnknownHeaderData(0x48); }) },
                        {0x4C, new Action<dynamic>(i => { RatchetMoby.serialize(ref gameplay_ntsc, 0x4C, racNum); })},
                        {0x50, new Action<dynamic>(i => { serializeUnknownHeaderData(0x50); }) },
                        {0x54, new Action<dynamic>(i => { serializeUnknownHeaderData(0x54); }) },
                        {0x58, new Action<dynamic>(i => { serializeUnknownHeaderData(0x58); }) },
                        {0x5C, new Action<dynamic>(i => { serializeMobyPvarSizes(0x5C); })},
                        {0x60, new Action<dynamic>(i => { serializeMobyPvars(0x60); })},
                        {0x64, new Action<dynamic>(i => { serializeMobyPvarHeaderPointers(0x64); })},
                        {0x68, new Action<dynamic>(i => { serializeGameplayCoordinates(0x68); })},
                        {0x6C, new Action<dynamic>(i => { serializeUnknownHeaderData(0x6C); }) },
                        {0x70, new Action<dynamic>(i => { serializeUnknownHeaderData(0x70); }) },
                        {0x74, new Action<dynamic>(i => { serializeUnknownHeaderData(0x74); }) },
                        {0x78, new Action<dynamic>(i => { serializeSplines(0x78); })},
                        {0x7C, new Action<dynamic>(i => { serializeUnknownHeaderData(0x7C); }) },
                        {0x80, new Action<dynamic>(i => { serializeUnknownHeaderData(0x80); }) },
                        {0x84, new Action<dynamic>(i => { serializeUnknownHeaderData(0x84); }) },
                        {0x88, new Action<dynamic>(i => { serializeUnknownHeaderData(0x88); }) },
                        {0x8C, new Action<dynamic>(i => { serializeUnknownHeaderData(0x8C); }) },
                        {0x90, new Action<dynamic>(i => { serializeRenderDef(0x90); })},
                        {0x94, new Action<dynamic>(i => { serializeUnknownHeaderData(0x94); }) },
                        {0x98, new Action<dynamic>(i => { serializeUnknownHeaderData(0x98); }) },
                        {0x9C, new Action<dynamic>(i => { serializeUnknownHeaderData(0x9C); }) },

                    };
                    break;
                case 4:
                default:
                    break;
            }
        }

        public async static void serialize(string path, int racNum)
        {
            getHeaderMap(racNum);

            offsets = GameplayParser.offsets;

            gameplay_ntsc = new byte[headerMap.Count * 4];

            foreach(unknownDataIndex offset in offsets)
            {
                KeyValuePair<int, dynamic> header = headerMap.Where(x => x.Key == offset.offset).FirstOrDefault();
                if (!header.Equals(default(KeyValuePair<int, dynamic>)))
                {
                    if (offset.pointer != 0)
                    {
                        WriteUint32(ref gameplay_ntsc, (int)offset.offset, (uint) gameplay_ntsc.Length);
                        headerMap[header.Key].DynamicInvoke(header);

                    }
                    else
                    {
                        WriteUint32(ref gameplay_ntsc, (int)offset.offset, 0);
                    }
                }
            }
            /*if(!File.Exists(path + "/test_ntsc"))
            {
                File.Create(path + "/test_ntsc");
            }*/

            File.WriteAllBytes(path + "/gameplay_ntsc", gameplay_ntsc);

        }

        public static byte[] serializeUnknownHeaderData(int index)
        {
            int currentOffset = gameplay_ntsc.Length;
            uint blankData = 0;
            unknownDataModel model = unknownGameplayData.Find(x => x.index == index);
            //while ((model.data.Length + blankData) % 10 != 0)
               // blankData += 0x04;

            Array.Resize(ref gameplay_ntsc, (int) (gameplay_ntsc.Length + model.data.Length + blankData));

            writeBytes(gameplay_ntsc, currentOffset, model.data, model.data.Length);

            byte[] test = new byte[4] { 0x3F, 0x80, 0x40, 0x40 };
            return test;
        }

        public static byte[] serializeFogAndDeathBarrier(int index)
        {
            return serializeUnknownHeaderData(index);
        }

        public static byte[] serializeCameraDef(int index)
        {
            return serializeUnknownHeaderData(index);
        }

        public static byte[] serializeGameStrings(int index)
        {
            return serializeUnknownHeaderData(index);
        }



        public static void serializeMobyPvarSizes(int index)
        {
            int currentOffset = gameplay_ntsc.Length;

            //We have to ensure that pointers end on a 0, or else the game will not parse it
            //byte[] space = new byte[0x08];
            //Array.Resize(ref gameplay_ntsc, (int)(gameplay_ntsc.Length + space.Length));
            //writeBytes(gameplay_ntsc, currentOffset, space, space.Length);

            //pVar sizes have to be stored as well
            int expectedSize = DataStore.pVarList.Count * 0x08;

            //We have to make sure that offsets end in a 0, or else the game will crash
            //while ((gameplay_ntsc.Length + expectedSize) % 10 != 0)
                //expectedSize += 0x04;

            byte[] pVarSizes = new byte[expectedSize];
            uint pVarSizeOffset = 0;

            foreach (byte[] pvar in DataStore.pVarList)
            {

                byte[] pVarSize = new byte[0x08];
                //Write the current offset of the size config
                WriteUint32(ref pVarSize, 0x00, pVarSizeOffset);

                //Write the size of the current pvar
                WriteUint32(ref pVarSize, 0x04, (uint)pvar.Length);

                //Write the new pvar size data to appropriate index
                int pVarIndex = DataStore.pVarList.IndexOf(pvar);
                writeBytes(pVarSizes, pVarIndex * 0x08, pVarSize, 8);

                //Increment the size of the offset
                pVarSizeOffset += (uint)pvar.Length;
            }

            currentOffset = gameplay_ntsc.Length;
            Array.Resize(ref gameplay_ntsc, (int)(gameplay_ntsc.Length + pVarSizes.Length));
            writeBytes(gameplay_ntsc, currentOffset, pVarSizes, pVarSizes.Length);
        }

        public static void serializeMobyPvars(int index)
        {
            int currentOffset = gameplay_ntsc.Length;
            byte[] pvarBlock = new byte[0];

            foreach (byte[] pvar in DataStore.pVarList)
            {
                //Add 0x08 to the size of the pvar size array
                Array.Resize(ref pvarBlock, (int)(pvarBlock.Length + pvar.Length));
                writeBytes(pvarBlock, pvarBlock.Length - pvar.Length, pvar, pvar.Length);

            }
            Array.Resize(ref gameplay_ntsc, (int)(gameplay_ntsc.Length + pvarBlock.Length));
            writeBytes(gameplay_ntsc, currentOffset, pvarBlock, pvarBlock.Length);
        }

        public static byte[] serializeMobyPvarHeaderPointers(int index)
        {
            return serializeUnknownHeaderData(index);
        }

        public static byte[] serializeGameplayCoordinates(int index)
        {
            return serializeUnknownHeaderData(index);
        }

        public static void serializeSplines(int index)
        {
            int currentOffset = gameplay_ntsc.Length;

            //This pointer comes right after the spline count, it tells the game where to start reading the actual spline data
            uint pointerToSplineData = 0;
            int expectedMasterHeaderSize = 0x10 + (DataStore.splines.Count * 0x04);

            //We have to do this to ensure our pointers end in a 0, if not, the game will crash
            while (expectedMasterHeaderSize % 10 != 0)
                expectedMasterHeaderSize += 0x04;

            byte[] splineMasterHeader = new byte[expectedMasterHeaderSize];
            WriteUint32(ref splineMasterHeader, 0x00, (uint) DataStore.splines.Count);

            byte[] splineDataBlock = new byte[0];

            foreach (Spline spline in DataStore.splines)
            {
                int currentSplineOffset = splineDataBlock.Length;
                //Write the pointer to the current spline data in the header
                WriteUint32(ref splineMasterHeader, 0x10 + (DataStore.splines.IndexOf(spline) * 4), (uint)currentSplineOffset);

                //How many vertices are in this spline
                int vertexCount = spline.vertexBuffer.Length / 3;

                //Create a local byte array thats the size we need for this spline
                uint expectedSize = (uint)(0x10 + (vertexCount * 0x10));
                byte[] splineHeader = new byte[expectedSize];
                WriteUint32(ref splineHeader, 0x00, (uint)vertexCount);
                WriteUint32(ref splineHeader, 0x04, 0x00000000);
                WriteUint32(ref splineHeader, 0x08, 0x00000000);
                WriteUint32(ref splineHeader, 0x0C, 0x00000000);
                for(int i = 0; i < vertexCount; i++)
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

            currentOffset = gameplay_ntsc.Length;
            Array.Resize(ref gameplay_ntsc, (int)(gameplay_ntsc.Length + splineDataBlock.Length));
            writeBytes(gameplay_ntsc, currentOffset, splineDataBlock, splineDataBlock.Length - splineMasterHeader.Length);
            //return serializeUnknownHeaderData(index);
        }

        public static byte[] serializeRenderDef(int index)
        {
            return serializeUnknownHeaderData(index);
        }
    }
}
