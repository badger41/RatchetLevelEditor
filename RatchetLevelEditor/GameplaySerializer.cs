using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static DataFunctions;
using static RatchetLevelEditor.GameplayParser;

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
		public static int headerSize; // some headers have padding

		static List<unknownDataIndex> offsets = new List<unknownDataIndex>();
        static byte[] gameplay_ntsc;

        public static void GetHeaderMap(int racNum)
        {
            switch (racNum)
            {
                case 1:
					headerMap = new Dictionary<int, dynamic>()
					{
						{0x00, new Action<dynamic>(i => SerializeFogAndDeathBarrier(0x00))},
						{0x04, new Action<dynamic>(i => SerializeUnknownHeaderData(0x04))},
						{0x08, new Action<dynamic>(i => SerializeCameraDef(0x08))},
						{0x0C, new Action<dynamic>(i => SerializeUnknownHeaderData(0x0C))},
						{0x10, new Action<dynamic>(i => SerializeGameStrings(0x10))},
						{0x14, new Action<dynamic>(i => SerializeGameStrings(0x14))},
						{0x18, new Action<dynamic>(i => SerializeGameStrings(0x18))},
						{0x1C, new Action<dynamic>(i => SerializeGameStrings(0x1C))},
						{0x20, new Action<dynamic>(i => SerializeGameStrings(0x20))},
						{0x24, new Action<dynamic>(i => SerializeGameStrings(0x24))},
						{0x28, new Action<dynamic>(i => SerializeGameStrings(0x28))},
						{0x2C, new Action<dynamic>(i => SerializeGameStrings(0x2C))},
						{0x30, new Action<dynamic>(i => SerializeUnknownHeaderData(0x30))},
						{0x34, new Action<dynamic>(i => SerializeUnknownHeaderData(0x34))},
						{0x38, new Action<dynamic>(i => SerializeUnknownHeaderData(0x38))},
						{0x3C, new Action<dynamic>(i => SerializeUnknownHeaderData(0x3C))},
						{0x40, new Action<dynamic>(i => SerializeUnknownHeaderData(0x40))},
						{0x44, new Action<dynamic>(i => RatchetMoby.serialize(ref gameplay_ntsc, 0x44, racNum))},
						{0x48, new Action<dynamic>(i => SerializeUnknownHeaderData(0x48))},
						{0x4C, new Action<dynamic>(i => SerializeUnknownHeaderData(0x4C))},
						{0x50, new Action<dynamic>(i => SerializeUnknownHeaderData(0x50))},
						{0x54, new Action<dynamic>(i => RatchetMoby.serializeMobyPvarSizes(ref gameplay_ntsc, 0x54))},
						{0x58, new Action<dynamic>(i => RatchetMoby.serializeMobyPvars(ref gameplay_ntsc, 0x58))},
						{0x5C, new Action<dynamic>(i => SerializeUnknownHeaderData(0x5C))},
						{0x60, new Action<dynamic>(i => SerializeUnknownHeaderData(0x60))},
						{0x64, new Action<dynamic>(i => SerializeUnknownHeaderData(0x64))},
						{0x68, new Action<dynamic>(i => SerializeUnknownHeaderData(0x68))},
						{0x6C, new Action<dynamic>(i => SerializeUnknownHeaderData(0x6C))},
						{0x70, new Action<dynamic>(i => Spline.serialize(ref gameplay_ntsc, 0x70))},
						{0x74, new Action<dynamic>(i => SerializeUnknownHeaderData(0x74))},
						{0x78, new Action<dynamic>(i => SerializeUnknownHeaderData(0x78))},
						{0x7C, new Action<dynamic>(i => SerializeUnknownHeaderData(0x7C))},
						{0x80, new Action<dynamic>(i => SerializeUnknownHeaderData(0x80))},
						{0x84, new Action<dynamic>(i => SerializeUnknownHeaderData(0x84))},
						{0x88, new Action<dynamic>(i => SerializeUnknownHeaderData(0x88))},
						{0x8C, new Action<dynamic>(i => SerializeUnknownHeaderData(0x8C))}
					};
					headerSize = 0xA0;
                    break;
                case 2:
                case 3:
                    headerMap = new Dictionary<int, dynamic>()
                    {
                        {0x00, new Action<dynamic>(i => { SerializeFogAndDeathBarrier(0x00); })},
                        {0x04, new Action<dynamic>(i => { SerializeCameraDef(0x04); })},
                        {0x08, new Action<dynamic>(i => { SerializeUnknownHeaderData(0x08); }) },
                        {0x0C, new Action<dynamic>(i => { SerializeUnknownHeaderData(0x0C); }) },
                        {0x10, new Action<dynamic>(i => { SerializeGameStrings(0x10); })},
                        {0x14, new Action<dynamic>(i => { SerializeGameStrings(0x14); })},
                        {0x18, new Action<dynamic>(i => { SerializeGameStrings(0x18); })},
                        {0x1C, new Action<dynamic>(i => { SerializeGameStrings(0x1C); })},
                        {0x20, new Action<dynamic>(i => { SerializeGameStrings(0x20); })},
                        {0x24, new Action<dynamic>(i => { SerializeGameStrings(0x24); })},
                        {0x28, new Action<dynamic>(i => { SerializeGameStrings(0x28); })},
                        {0x2C, new Action<dynamic>(i => { SerializeGameStrings(0x2C); })},
                        {0x30, new Action<dynamic>(i => { SerializeUnknownHeaderData(0x30); }) },
                        {0x34, new Action<dynamic>(i => { SerializeUnknownHeaderData(0x34); }) },
                        {0x38, new Action<dynamic>(i => { SerializeUnknownHeaderData(0x38); }) },
                        {0x3C, new Action<dynamic>(i => { SerializeUnknownHeaderData(0x3C); }) },
                        {0x40, new Action<dynamic>(i => { SerializeUnknownHeaderData(0x40); }) },
                        {0x44, new Action<dynamic>(i => { SerializeUnknownHeaderData(0x44); }) },
                        {0x48, new Action<dynamic>(i => { SerializeUnknownHeaderData(0x48); }) },
                        {0x4C, new Action<dynamic>(i => { RatchetMoby.serialize(ref gameplay_ntsc, 0x4C, racNum); })},
                        {0x50, new Action<dynamic>(i => { SerializeUnknownHeaderData(0x50); }) },
                        {0x54, new Action<dynamic>(i => { SerializeUnknownHeaderData(0x54); }) },
                        {0x58, new Action<dynamic>(i => { SerializeUnknownHeaderData(0x58); }) },
                        {0x5C, new Action<dynamic>(i => { RatchetMoby.serializeMobyPvarSizes(ref gameplay_ntsc, 0x5C); })},
                        {0x60, new Action<dynamic>(i => { RatchetMoby.serializeMobyPvars(ref gameplay_ntsc, 0x60); })},
                        {0x64, new Action<dynamic>(i => { SerializeMobyPvarHeaderPointers(0x64); })},
                        {0x68, new Action<dynamic>(i => { SerializeGameplayCoordinates(0x68); })},
                        {0x6C, new Action<dynamic>(i => { SerializeUnknownHeaderData(0x6C); }) },
                        {0x70, new Action<dynamic>(i => { SerializeUnknownHeaderData(0x70); }) },
                        {0x74, new Action<dynamic>(i => { SerializeUnknownHeaderData(0x74); }) },
                        {0x78, new Action<dynamic>(i => { Spline.serialize(ref gameplay_ntsc, 0x78); })},
                        {0x7C, new Action<dynamic>(i => { SerializeUnknownHeaderData(0x7C); }) },
                        {0x80, new Action<dynamic>(i => { SerializeUnknownHeaderData(0x80); }) },
                        {0x84, new Action<dynamic>(i => { SerializeUnknownHeaderData(0x84); }) },
                        {0x88, new Action<dynamic>(i => { SerializeUnknownHeaderData(0x88); }) },
                        {0x8C, new Action<dynamic>(i => { SerializeUnknownHeaderData(0x8C); }) },
                        {0x90, new Action<dynamic>(i => { SerializeRenderDef(0x90); })},
                        {0x94, new Action<dynamic>(i => { SerializeUnknownHeaderData(0x94); }) },
                        {0x98, new Action<dynamic>(i => { SerializeUnknownHeaderData(0x98); }) },
                        {0x9C, new Action<dynamic>(i => { SerializeUnknownHeaderData(0x9C); }) },

                    };
					headerSize = 0xA0;
                    break;
                case 4:
                default:
                    break;
            }
        }

        public static void Serialize(string path, int racNum)
        {
            GetHeaderMap(racNum);

            offsets = GameplayParser.offsets;

            gameplay_ntsc = new byte[headerSize];

            foreach(unknownDataIndex offset in offsets)
            {
                KeyValuePair<int, dynamic> header = headerMap.Where(x => x.Key == offset.offset).FirstOrDefault();
                if (!header.Equals(default(KeyValuePair<int, dynamic>)))
                {
                    if (offset.pointer != 0)
                    {
                        WriteUint32(ref gameplay_ntsc, (int)offset.offset, (uint)gameplay_ntsc.Length);
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

        public static byte[] SerializeUnknownHeaderData(int index)
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

        public static byte[] SerializeFogAndDeathBarrier(int index)
        {
            return SerializeUnknownHeaderData(index);
        }

        public static byte[] SerializeCameraDef(int index)
        {
            return SerializeUnknownHeaderData(index);
        }

        public static byte[] SerializeGameStrings(int index)
        {
            return SerializeUnknownHeaderData(index);
        }





        public static byte[] SerializeMobyPvarHeaderPointers(int index)
        {
            return SerializeUnknownHeaderData(index);
        }

        public static byte[] SerializeGameplayCoordinates(int index)
        {
            return SerializeUnknownHeaderData(index);
        }



        public static byte[] SerializeRenderDef(int index)
        {
            return SerializeUnknownHeaderData(index);
        }
    }
}
