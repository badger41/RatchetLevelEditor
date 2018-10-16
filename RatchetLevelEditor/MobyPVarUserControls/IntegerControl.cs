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
    public partial class IntegerControl : UserControl, IMobyPVarControl
    {
        MobyPVar pVarConfig;
        public IntegerControl(byte[] pVarData, MobyPVar pVarConfig)
        {
            InitializeComponent();
            this.pVarConfig = pVarConfig;
            intControlGroup.Text = pVarConfig.label;
            decimal min = pVarConfig.min;
            decimal max = pVarConfig.max;
            intValueControl.Minimum = min != -1 && min != 0 ? min : decimal.MinValue;
            intValueControl.Maximum = max != -1 && max != 0 ? max : decimal.MaxValue;
            int returnValue;
            switch(pVarConfig.length)
            {
                case 1:
                    returnValue = pVarData[pVarConfig.index];
                    break;
                case 2:
                    returnValue = BAToShort(pVarData, pVarConfig.index);
                    break;
                case 4:
                default:
                    returnValue = BAToInt32(pVarData, pVarConfig.index);
                    break;
            }
            intValueControl.Value = returnValue;
        }

        public MobyPVar pvar { get; set; }

        public event EventHandler<byte[]> OnValueChanged;

        private void floatValueControl_ValueChanged(object sender, EventArgs e)
        {
            //Console.WriteLine(floatValueControl.Value);
            if (OnValueChanged != null)
            {
                byte[] data;
                switch (pVarConfig.length)
                {
                    case 1:
                        data = new byte[1] { (byte)intValueControl.Value };
                        break;
                    case 2:
                        data = BitConverter.GetBytes((short)intValueControl.Value);
                        break;
                    case 4:
                    default:
                        data = BitConverter.GetBytes((int) intValueControl.Value);
                        break;
                };
                Array.Reverse(data);
                OnValueChanged.Invoke(this, data);
            }
        }
    }
}
