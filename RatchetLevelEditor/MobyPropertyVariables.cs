using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatchetLevelEditor
{
    public class MobyPropertyVariables
    {
        public List<uint> modelIds { get; set; }
        public List<MobyPVar> pVars { get; set; }

    };

    public struct MobyPVar
    {
        public uint index { get; set; }
        public string label { get; set; }
        public string control { get; set; }
        public string specialInd { get; set; }
        public uint length { get; set; }
        public int min { get; set; }
        public int max { get; set; }
    }
}
