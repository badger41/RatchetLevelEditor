using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatchetLevelEditor.Gameplay
{
    class DataStoreGameplay
    {
        public static GameplayHeader gameplayHeader;
        public static uint mobyUnknownVal;
        public static List<RatchetMoby> mobs = new List<RatchetMoby>();
        public static List<byte[]> pVarList;
        public static List<Spline> splines;
    }
}
