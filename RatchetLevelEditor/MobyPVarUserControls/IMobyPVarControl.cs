using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatchetLevelEditor.MobyPVarUserControls
{


    public interface IMobyPVarControl
    {
        event EventHandler<byte[]> OnValueChanged;
        MobyPVar pvar { get; set; }


    }
}
