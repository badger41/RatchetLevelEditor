using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RatchetLevelEditor
{
    public partial class ConsoleWindow : Form
    {
        TextBox _output = null;

        public ConsoleWindow(Main main)
        {
            InitializeComponent();
            TextBoxStreamWriter _writer = new TextBoxStreamWriter(txtConsole);
            Console.SetOut(_writer);
            this.Location = new Point(0, (main.Height - this.Height) - 70);
        }
    }
}
