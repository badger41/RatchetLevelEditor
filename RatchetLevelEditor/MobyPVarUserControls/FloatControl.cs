using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DataFunctions;

namespace RatchetLevelEditor.MobyPVarUserControls
{
    public partial class FloatControl : UserControl, IMobyPVarControl
    {
        public FloatControl(byte[] pVarData, MobyPVar pVarConfig)
        {
            InitializeComponent();
            floatControlGroup.Text = pVarConfig.label;
            decimal min = pVarConfig.min;
            decimal max = pVarConfig.max;
            floatValueControl.Minimum = min != -1 && min != 0 ? min : decimal.MinValue;
            floatValueControl.Maximum = max != -1 && max != 0 ? max : decimal.MaxValue;
            float first = BAToFloat(pVarData, pVarConfig.index);
           // float test = BitConverter.ToSingle(pVarData, (int) pVarConfig.index);
            floatValueControl.Value = (decimal)first;
        }

        public MobyPVar pvar { get; set; }

        public event EventHandler<byte[]> OnValueChanged;

        private void floatValueControl_ValueChanged(object sender, EventArgs e)
        {
            //Console.WriteLine(floatValueControl.Value);
            if (OnValueChanged != null)
            {
                byte[] data = BitConverter.GetBytes((float)floatValueControl.Value);
                Array.Reverse(data);
                OnValueChanged.Invoke(this, data);
            }
        }
    }
}
