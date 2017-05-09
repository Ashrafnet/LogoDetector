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
    public partial class ClarifiConfig_UI : Form
    {
        public ClarifiConfig Clarificonfig { get;private  set; }
        public ClarifiConfig_UI(ClarifiConfig config)
        {
            InitializeComponent();

            Clarificonfig = config;
            propertyGrid1.SelectedObject = Clarificonfig;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Clarificonfig.Batch_Size < 1)
            {
                MessageBox.Show("Batch Size can't be less than 1", "invalid value", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DialogResult = DialogResult.OK;
        }
    }
}
