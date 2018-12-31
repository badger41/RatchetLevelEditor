using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatchetLevelEditor.Engine.Models
{

    public class UnknownDataIndex
    {
        public uint offset { get; set; }
        public uint pointer { get; set; }
    }

    public class UnknownDataModel
    {
        public uint index { get; set; }
        public byte[] data { get; set; }
    }
}
