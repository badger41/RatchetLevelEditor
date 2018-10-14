using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RatchetTexture;
using static RatchetModel;

namespace RatchetLevelEditor
{

    /***
     * This class is the storage center that all of the components will reference when displaying data
     * All of the parsing methods funnel the parsed data into here
     * This excludes gameplay file data since it will be housed in its own data store file
     ***/

    class DataStore
    {

        /* Engine File Specific, universal between the different ratchet games */
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
        public static List<RatchetModel_General> levelModels = new List<RatchetModel_General>();
        public static List<LevelObject> levelObjects = new List<LevelObject>();
        public static List<LevelObject> sceneryObjects = new List<LevelObject>();
        public static uint levelModelsCount;
        public static List<RatchetModel_General> sceneryModels = new List<RatchetModel_General>();
        public static uint sceneryModelsCount;
        public static RatchetModel_Terrain terrainModel = new RatchetModel_Terrain();
        public static RatchetModel_TerrainCollision terrainCollisionModel = new RatchetModel_TerrainCollision();

        public static List<float> collVertBuff = new List<float>();
        public static List<uint> collIndBuff = new List<uint>();

        /* End Engine File Specific */

        /* Chunk Files */
        public static List<RatchetModel_Terrain> chunks = new List<RatchetModel_Terrain>();
        /* End Chunk Files */

        /*Gameplay file*/
        public static GameplayHeader gameplayHeader;
        public static List<RatchetMoby> mobs = new List<RatchetMoby>();
        public static List<byte[]> pVarList;
        public static List<Spline> splines;


        /* Universal declarations - these lists are present in almost all file types loaded*/
        public static List<RatchetTexture_General> textures = new List<RatchetTexture_General>();
        public static uint textureCount;
        /* End Universal declarations */


        /* UYA Specific mapped engine data *****/
        public static byte[] campaignPlayerAnimations;
        /* End UYA Specific mapped engine data */

        /* UYA Engine Unmapped data ************/
        public static byte[] enginePtr_08;                                   //0x08 - null
        public static byte[] enginePtr_0C;                                   //0x0C - null
        public static byte[] enginePtr_40;                                   //0x40 - null
        public static byte[] enginePtr_44;                                   //0x44 - null
        public static byte[] enginePtr_4C;                                   //0x4C - null    Weapon pointer(rac1)
        public static byte[] enginePtr_50;                                   //0x50 - null    Weapon count (rac1)
        /* End Engine UYA Unmapped Data ********/

        /* Deadlocked Specific Missions ********/
        public static List<RatchetMission> missions = new List<RatchetMission>();
        /* End Deadlocked Specific Missions ****/


        /* Misc data not directly related to ratchet files */
        public static string workingDirectory;
        public static List<MobyPropertyVariables> pVarMap;
        public static RatchetMoby selectedMoby;
        public static LevelObject selectedLevelObject;
        /* End Misc data not directly related to ratchet files */
    }
}
