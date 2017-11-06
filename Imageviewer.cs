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
    public partial class Imageviewer : Form
    {
        public Imageviewer(Image image)
        {
            InitializeComponent();
            pictureBox1.Image = image;
        }
        ~Imageviewer()
        {
            pictureBox1.Dispose();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
