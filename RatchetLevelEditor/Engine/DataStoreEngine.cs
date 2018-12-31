using RatchetLevelEditor.Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RatchetModel;
using static RatchetTexture;

namespace RatchetLevelEditor.Engine
{
    /***
     * This class is the storage center for all of the data in the engine file
     * All of the parsing methods funnel the parsed data into here
     ***/

    class DataStoreEngine
    {
        /* Universal between the different ratchet games */
        public static EngineHeader engineHeader;
        public static byte[] mapRenderDefintions;
        public static byte[] skyBox;
        public static byte[] collisionMap;
        public static byte[] soundsConfig;
        public static byte[] lighting;
        public static uint lightingLevel;
        public static byte[] lightingConfig;
        public static byte[] textureConfigMenu;
        public static byte[] textureConfig2DGFX;
        public static byte[] spriteDef;
        public static List<RatchetModel_General> spawnableModels = new List<RatchetModel_General>();
        public static uint spawnableModelsCount;

        //Will be deprecated eventually, tieModels is replacing it
        public static List<RatchetModel_General> levelModels = new List<RatchetModel_General>();
        public static List<TieModel> tieModels = new List<TieModel>();

        //Will be deprecated eventually, shrubModels is replacing it
        public static List<RatchetModel_General> sceneryModels = new List<RatchetModel_General>();
        public static List<ShrubModel> shrubModels = new List<ShrubModel>();

        public static List<RatchetPlayerAnimation> playerAnims = new List<RatchetPlayerAnimation>();

        public static List<LevelObject> levelObjects = new List<LevelObject>();
        public static List<LevelObject> sceneryObjects = new List<LevelObject>();
        public static uint levelModelsCount;

        public static uint sceneryModelsCount;
        public static RatchetModel_Terrain terrainModel = new RatchetModel_Terrain();
        public static RatchetModel_TerrainCollision terrainCollisionModel = new RatchetModel_TerrainCollision();
        public static List<RatchetTexture_General> textures = new List<RatchetTexture_General>();
        public static uint textureCount;

        /* UYA Specific mapped engine data *****/
        public static byte[] campaignPlayerAnimations;
        /* End UYA Specific mapped engine data */

        //All pointers of the file that are unknown will be thrown in here.
        //This will make it easy to map header pointers to a known index later (with more documentation being done etc)
        public static List<UnknownDataModel> unknownEngineData;

        //List of all of the pointers in the header. We will use these to calculate the data needed to load into the unknown byte arrays
        public static List<UnknownDataIndex> unhandledOffsets = new List<UnknownDataIndex>();

        /* UYA Engine Unmapped data ************/
        public static byte[] enginePtr_08;                                   //0x08 - null
        public static byte[] enginePtr_0C;                                   //0x0C - null
        public static byte[] enginePtr_40;                                   //0x40 - null
        public static byte[] enginePtr_44;                                   //0x44 - null
        public static byte[] enginePtr_4C;                                   //0x4C - null    Weapon pointer(rac1)
        public static byte[] enginePtr_50;                                   //0x50 - null    Weapon count (rac1)
                                                                             /* End Engine UYA Unmapped Data ********

        /* Chunk Files */
        public static List<RatchetModel_Terrain> chunks = new List<RatchetModel_Terrain>();
        /* End Chunk Files */

        /* Collision data */
        public static List<float> collVertBuff = new List<float>();
        public static List<uint> collIndBuff = new List<uint>();
        public static byte[] collisionDataRaw;


    }
}
