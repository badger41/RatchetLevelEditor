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

    class DataStoreGlobal
    {



        /* End Engine File Specific */


        /*Gameplay file*/



        /* Universal declarations - these lists are present in almost all file types loaded*/

        /* End Universal declarations */






        /* Deadlocked Specific Missions ********/
        public static List<RatchetMission> missions = new List<RatchetMission>();
        /* End Deadlocked Specific Missions ****/


        /* Misc data not directly related to ratchet files */
        public static string workingDirectory;
        public static List<MobyPropertyVariableConfig> pVarMap;
        public static RatchetMoby selectedMoby;
        public static LevelObject selectedLevelObject;
        /* End Misc data not directly related to ratchet files */
    }
}
