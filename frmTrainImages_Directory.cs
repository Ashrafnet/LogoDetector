using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogoDetector
{
    public partial class frmTrainImages_Directory : Form
    {
        public frmTrainImages_Directory(string [] imgExts)
        {
            InitializeComponent();
            _imgExts = imgExts;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {


                if (string.IsNullOrWhiteSpace(textBox1.Text))
                {
                    textBox1.Focus();
                    textBox1.SelectAll();
                    throw new Exception("you must choose directory!");
                }
                if (!Directory.Exists(textBox1.Text))
                {
                    textBox1.Focus();
                    textBox1.SelectAll();
                    throw new Exception("you must choose a vaild directory!");
                }

                FolderPath = textBox1.Text;
                HasLogo = radioButton1.Checked == true;
                IncludeAllSub_Folders = checkBox1.Checked? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly ;
                DialogResult = DialogResult.OK;
            }
            catch (Exception er)
            {

                MessageBox.Show(er.FullErrorMessage(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public string FolderPath { get; set; }
        public bool HasLogo { get; set; }
        public SearchOption IncludeAllSub_Folders { get; set; }
        private string[] _imgExts;

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {



                if (string.IsNullOrWhiteSpace(textBox1.Text))
                {
                    MessageBox.Show("You have to set the images folder!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox1.Focus();
                    textBox1.SelectAll();
                    return;
                }
                if (!Directory.Exists(textBox1.Text))
                {
                    MessageBox.Show("This directory is not exist!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox1.Focus();
                    textBox1.SelectAll();
                    return;
                }
                Cursor = Cursors.WaitCursor;
                Stopwatch sw = Stopwatch.StartNew();
         
                var cnt = MyDirectory.GetFiles(textBox1.Text, _imgExts, new Dictionary<string , float>() , checkBox1.Checked ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).LongCount(x => !string.IsNullOrWhiteSpace(x));
                sw.Stop();
                Text = sw.ElapsedMilliseconds + " ms";
             
                MessageBox.Show("Number of images= " + cnt + " images" + Environment.NewLine + "Images supported are:" + Environment.NewLine + string.Join(" , ", _imgExts) + "");

            }
            catch (Exception er)
            {

                MessageBox.Show(er.FullErrorMessage(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default ;
            }

          
        }

        private void button3_Click(object sender, EventArgs e)
        {
           if(Directory.Exists (textBox1.Text ))
                folderBrowserDialog1.SelectedPath = textBox1.Text;
            if (folderBrowserDialog1.ShowDialog()== DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
        }
    }
}
