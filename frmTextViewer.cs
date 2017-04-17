using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogoDetector
{
    public partial class frmTextViewer : Form
    {
        public frmTextViewer(string strstring)
        {
            InitializeComponent();
            textBox1.Text = strstring;
        }
    }
}
