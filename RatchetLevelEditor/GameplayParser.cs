using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataFunctions;

namespace RatchetLevelEditor
{
    /***
     * This class handles parsing of the different gameplay files from the ratchet games
     * Files: gameplay_ntsc, gameplay_mission_classes
     * 
     ***/
    public class GameplayParser
    {
        static FileStream gpf = null;
        static GameplayHeader gameplayHeader;
        static int racNum;



        //All pointers of the gameplay file that are unknown will be thrown in here.
        //This will make it easy to map header pointers to a known index later (with more documentation being done etc)
        public static List<byte[]> unknownGameplayData;

        //List of all of the pointers in the gameplay header. We will use these to calculate the data needed to load into the unknown byte arrays
        static List<uint> offsets = new List<uint>();

        public static byte[] fogAndDeathBarrier = new byte[10];
        public static byte[] cameraDef;
        public static List<byte[]> gameStrings;
        public static List<RatchetMoby> mobyDef = new List<RatchetMoby>();
        public static Dictionary<int, int> mobyPvarSizes;
        public static List<MobyPVar> mobyPvars;
        public static Dictionary<int, int> mobyPvarHeaderPointers;
        public static List<RatchetCoordinate> gameplayCoordinates;
        public static List<Spline> splines;
        public static byte[] renderDef;

        //Dictionary to define the mapping of each index of the gameplay file and its respective variable
        //Usage: {index, variable} => (UYA) {0x4c, MobyDef}
        public static Dictionary<int, dynamic> headerMap;

        public static void createHeaderMap(int racNum)
        {
            switch (racNum)
            {
                case 1:
                    break;
                case 2:
                case 3:
                    headerMap = new Dictionary<int, dynamic>()
                    {
                        {0x00, new Action<dynamic>(i => { parseFogAndDeathBarrier(0x00); })},
                        {0x04, new Action<dynamic>(i => { parseCameraDef(0x04); })},
                        {0x08, new Action<dynamic>(i => { getUnknownHeaderData(0x08); }) },
                        {0x0C, new Action<dynamic>(i => { getUnknownHeaderData(0x0C); }) },
                        {0x10, new Action<dynamic>(i => { parseGameStrings(0x10); })},
                        {0x14, new Action<dynamic>(i => { parseGameStrings(0x14); })},
                        {0x18, new Action<dynamic>(i => { parseGameStrings(0x18); })},
                        {0x1C, new Action<dynamic>(i => { parseGameStrings(0x1C); })},
                        {0x20, new Action<dynamic>(i => { parseGameStrings(0x20); })},
                        {0x24, new Action<dynamic>(i => { parseGameStrings(0x24); })},
                        {0x28, new Action<dynamic>(i => { parseGameStrings(0x28); })},
                        {0x2C, new Action<dynamic>(i => { parseGameStrings(0x2C); })},
                        {0x30, new Action<dynamic>(i => { getUnknownHeaderData(0x30); }) },
                        {0x34, new Action<dynamic>(i => { getUnknownHeaderData(0x34); }) },
                        {0x38, new Action<dynamic>(i => { getUnknownHeaderData(0x38); }) },
                        {0x3C, new Action<dynamic>(i => { getUnknownHeaderData(0x3C); }) },
                        {0x40, new Action<dynamic>(i => { getUnknownHeaderData(0x40); }) },
                        {0x44, new Action<dynamic>(i => { getUnknownHeaderData(0x44); }) },
                        {0x48, new Action<dynamic>(i => { getUnknownHeaderData(0x48); }) },
                        {0x4C, new Action<dynamic>(i => { parseMobyDef(0x4C); })},
                        {0x50, new Action<dynamic>(i => { getUnknownHeaderData(0x50); }) },
                        {0x54, new Action<dynamic>(i => { getUnknownHeaderData(0x54); }) },
                        {0x58, new Action<dynamic>(i => { getUnknownHeaderData(0x58); }) },
                        {0x5C, new Action<dynamic>(i => { parseMobyPvarSizes(0x5C); })},
                        {0x60, new Action<dynamic>(i => { parseMobyPvars(0x60); })},
                        {0x64, new Action<dynamic>(i => { parseMobyPvarHeaderPointers(0x64); })},
                        {0x68, new Action<dynamic>(i => { parseGameplayCoordinates(0x68); })},
                        {0x6C, new Action<dynamic>(i => { getUnknownHeaderData(0x6C); }) },
                        {0x70, new Action<dynamic>(i => { getUnknownHeaderData(0x70); }) },
                        {0x74, new Action<dynamic>(i => { getUnknownHeaderData(0x74); }) },
                        {0x78, new Action<dynamic>(i => { parseSplines(0x78); })},
                        {0x7C, new Action<dynamic>(i => { getUnknownHeaderData(0x7C); }) },
                        {0x80, new Action<dynamic>(i => { getUnknownHeaderData(0x80); }) },
                        {0x84, new Action<dynamic>(i => { getUnknownHeaderData(0x84); }) },
                        {0x88, new Action<dynamic>(i => { getUnknownHeaderData(0x88); }) },
                        {0x8C, new Action<dynamic>(i => { getUnknownHeaderData(0x8C); }) },
                        {0x90, new Action<dynamic>(i => { parseRenderDef(0x90); })},
                        {0x94, new Action<dynamic>(i => { getUnknownHeaderData(0x94); }) },
                        {0x98, new Action<dynamic>(i => { getUnknownHeaderData(0x98); }) },
                        {0x9C, new Action<dynamic>(i => { getUnknownHeaderData(0x9C); }) },

                    };
                    break;
                case 4:
                default:
                    break;
            }
        }

        public static void parseGameplay(string path, int racNum)
        {
            GameplayParser.racNum = racNum;

            Console.WriteLine("Parsing gameplay_ntsc");

            //Initialize our filestream
            if (File.Exists(path + "/gameplay_ntsc"))
                gpf = File.OpenRead(path + "/gameplay_ntsc");

            gameplayHeader = new GameplayHeader(gpf, (uint)racNum);

            //Initialize the unknown gameplay data list;
            unknownGameplayData = new List<byte[]>();

            //Create our header map complete with functions each index is responsible for
            createHeaderMap(racNum);

            //Determine which header length we have depending on which ratchet game
            int offsetCount = headerMap.Count;

            //Load our header block, we need this to pass offsets and also build the offset list
            byte[] headerBlock = ReadBlock(gpf, 0, (uint) offsetCount * 4);

            //Loop through the header and load all of our offsets
            for(int i = 0; i < offsetCount; i++)
            {
                offsets.Add(ReadUInt32(headerBlock,(i * 4)));
            }

            //Sort the offsets so we can easily get the next offset to determine size of our unknown data
            offsets.Sort();

            //The important part
            //This loops through the headerMap and invokes the action that is required for that index.
            //This is what makes it easy to pivot and change how a certain bit of data is processed without much code change
            foreach(KeyValuePair<int, dynamic> header in headerMap)
            {
                headerMap[header.Key].DynamicInvoke(header); // invoke appropriate delegate
            }

            //Close the stream
            gpf.Close();
        }

        //Deadlocked specific
        public void parseGameplayMission(string path, int index)
        {
            //TODO
        }

        //Any unhandled gameplay element is funneled through this method.
        //It takes the chunk of data and loads it into a byte array
        //We will still need it when serializing the gameplay file later
        public static void getUnknownHeaderData(int index)
        {
            uint offset = ReadUInt32(ReadBlock(gpf, (uint) index, 4), 0);
            int offsetIndex = offsets.IndexOf(offset);
            unknownGameplayData.Add(ReadBlock(gpf, offset, (offsets[offsetIndex + 1] - offset)));
        }


        public static void parseFogAndDeathBarrier(int index)
        {

        }

        public static void parseCameraDef(int index)
        {

        }

        public static void parseGameStrings(int index)
        {

        }

        public static void parseMobyDef(int index)
        {
            uint pointer = BAToUInt32(ReadBlock(gpf, (uint) index, 4), 0);
            uint mobyCount = BAToUInt32(ReadBlock(gpf, pointer, 4), 0);
            byte[] mobyBlock = ReadBlock(gpf, pointer + 0x10, mobyCount * gameplayHeader.mobyElemSize);

            for (uint i = 0; i < mobyCount; i++)
            {
                RatchetMoby mob = new RatchetMoby((uint) racNum, mobyBlock, i);
                DataStore.mobs.Add(mob);
            }
        }

        public static void parseMobyPvarSizes(int index)
        {

        }

        public static void parseMobyPvars(int index)
        {

        }

        public static void parseMobyPvarHeaderPointers(int index)
        {

        }

        public static void parseGameplayCoordinates(int index)
        {

        }

        public static void parseSplines(int index)
        {

        }

        public static void parseRenderDef(int index)
        {

        }
    }
}
