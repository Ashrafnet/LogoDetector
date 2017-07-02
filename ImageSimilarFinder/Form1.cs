using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using FeatureMatchingExample;
using ImageSimilarFinder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox1.Focus();
            try
            {
                Icon = Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetEntryAssembly().Location);
            }
            catch (Exception er)
            {

            }


        }

      
        ////float compare2images_2(string image1, string image2, Func<string, ulong[]> HashFunction)
        ////{

        ////    var img1_hash_avg = HashFunction(image1);
        ////    var img2_hash_avg = HashFunction(image2);

        ////    var result = DupImageLib.ImageHashes.CompareHashes(img1_hash_avg, img2_hash_avg);
        ////    return result;
        ////}

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
          
          
        }
        string[] imgExts = new string[] { ".jpeg", ".jpg", ".png", ".BMP", ".GIF", ".TIFF", ".Exif", ".WMF", ".EMF" };
        string[] imgExts_ = new string[] { "*.jpeg", "*.jpg", "*.png", "*.BMP", "*.GIF", "*.TIFF", "*.Exif", "*.WMF", "*.EMF" };


        Bitmap loadImage(string imagepath)
        {
            if (string.IsNullOrWhiteSpace(imagepath)) return null;
            if (!File.Exists(imagepath)) return null;
            return new Bitmap((Bitmap)new Mat(imagepath).Bitmap.Clone());


            if (imgExts.Contains((Path.GetExtension(imagepath) + "").ToLower()))
            {
                if (File.Exists(imagepath))
                    return (Bitmap)Bitmap.FromStream(new MemoryStream(File.ReadAllBytes(imagepath)));
                else
                    return null;
            }
            return null;

        }
        void SetStatusInfo(string info)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(SetStatusInfo), info);
            }
            else
            {
                status_info.Text = info;
                Refresh();
            }
        }
        bool _Isrunning = false;
        Task task = null;
        ElencySolutions.CsvHelper.CsvFile _csvFile = null;
        private void button1_Click(object sender, EventArgs e)
        {
            if (_Isrunning)
            {
                _Isrunning = button1.Enabled = false;
                SetButtonText("Stopping..", false );
                SetStatusInfo("Stopping..");
                return;
            }
            else
            {
                SetButtonText("Stop", true );
                SetStatusInfo("Working..");
            }
            if (File.Exists(textBox1.Text) )
            {
                listView1.Items.Clear();
                pictureBox2.Image = pictureBox3.Image= null;
                _Isrunning = button1.Enabled= true;

                 _csvFile = new ElencySolutions.CsvHelper.CsvFile();
                _csvFile.Populate(textBox1.Text, _csv_has_headers);
                task = Task.Run(() =>
                  {

                      //ElencySolutions.CsvHelper.CsvReader r = new ElencySolutions.CsvHelper.CsvReader(textBox1.Text);
                      //var dt = r.ReadIntoDictionary(_csv_has_headers);
                     
                      startSearch();
                  }).ContinueWith((t) =>
                  {
                      
                      _Isrunning = false;
                      SetButtonText( "Search",true );
                      SetStatusInfo("Done");

                  });
            }
            else
                MessageBox.Show("please set a valid image path and folder path.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void SetButtonText(string strText,bool IsEnabled)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string,bool  >(SetButtonText), strText,IsEnabled);
            }
            else
            {
                button1.Text = strText;
                button1.Enabled = IsEnabled;
            }
        }
        bool CheckSimilarty(string ModelImagePath, string ObservedImagePath, Tuple<VectorOfKeyPoint, Mat> x, Tuple<VectorOfKeyPoint, Mat> y, out Mat result)
        {
            bool xxx = DrawMatches.CheckSimilarty(x, y);
            result = null;
            if (_preview_similarity)
            {
                using (Mat modelImage = CvInvoke.Imread(ModelImagePath, ImreadModes.Grayscale))
                using (Mat observedImage = CvInvoke.Imread(ObservedImagePath, ImreadModes.Grayscale))
                {
                    result = DrawMatches.Draw(modelImage, observedImage);
                }
            }
            return xxx;
        }

        bool _preview_similarity = false;
        void startSearch()
        {
            Dictionary<string, Tuple<VectorOfKeyPoint, Mat>> items = new Dictionary<string, Tuple<VectorOfKeyPoint, Mat>>();
            Tuple<VectorOfKeyPoint, Mat> image_base_descriptors = null;
            if (_csvFile.Headers.Count > 0)
            {
                _csvFile.Headers.Add("Similar");
                _csvFile.Headers.Add("Error");
            }

            foreach (var item in _csvFile.Records)
            {
                if (!_Isrunning) break;
                item.Fields.Add("N/A"); item.Fields.Add("");
                var imageBase = item.Fields[0];
                var image = item.Fields[1];
                if (string.IsNullOrWhiteSpace(imageBase) || string.IsNullOrWhiteSpace(image)) continue;
                ListViewItem i  = new ListViewItem(imageBase);
                try
                {

                    if (!File.Exists(imageBase)) throw new FileNotFoundException("File does not exist!", imageBase);
                    if (!File.Exists(image)) throw new FileNotFoundException("File does not exist!", image);

                    if (!items.ContainsKey(imageBase))
                    {

                        image_base_descriptors = DrawMatches.GetImageDescriptors(imageBase);
                        items.Add(item.Fields[0], image_base_descriptors);
                    }




                    i.Tag = image;

                    Mat similarity;
                    var result = CheckSimilarty(imageBase, image, items[imageBase], DrawMatches.GetImageDescriptors(image), out similarity);
                    i.SubItems.Add(result + "");
                    i.SubItems.Add(image);
                    i.ImageIndex = result ? 0 : 1;
                    item.Fields[2] = result ? "Yes" : "No";
                    if (similarity != null)
                        i.Tag = new Bitmap((Bitmap)similarity.Bitmap.Clone());

                    using (ElencySolutions.CsvHelper.CsvWriter csv = new ElencySolutions.CsvHelper.CsvWriter())
                    {
                        csv.WriteCsv(_csvFile, _csvFile.File_Name);


                    }
                }
                catch (Exception er)
                {
                    if (item.Fields.Count <= 3)
                        item.Fields.Add("");
                    item.Fields[3] = er.Message;
                    i.ImageIndex = 3;
                    i.ForeColor = Color.Red;

                    i.SubItems.Add("N/A");
                    i.SubItems.Add(image);
                    i.SubItems.Add(er.Message);
                }
                finally
                {
                    AddItem(i);
                }


            }


            
          


            if (InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                }));
            }
            else
                listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

        }

              
            
        

        private delegate void AddItemCallback(ListViewItem o);

        private void AddItem(ListViewItem o)
        {
            if (this.listView1.InvokeRequired)
            {
                AddItemCallback d = new AddItemCallback(AddItem);
                this.Invoke(d, new object[] { o });
            }
            else
            {
                listView1.Items.Add(o);

            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

          
            if (listView1.SelectedItems == null || listView1.SelectedItems.Count < 1) return;
                pictureBox1.Image = loadImage(listView1.SelectedItems[0].Text  + "");

                if (listView1.SelectedItems[0].Tag is Bitmap)
                {
                    pictureBox2.Image = (Bitmap)listView1.SelectedItems[0].Tag;
                    pictureBox3.Image = loadImage(listView1.SelectedItems[0].SubItems[2].Text + "");

                }
                else
                {
                    pictureBox2.Image = loadImage(listView1.SelectedItems[0].Tag + "");
                    pictureBox3.Image = null;
                }
            }
            catch (Exception err)
            {
               // MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            _preview_similarity = checkBox1.Checked;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            try
            {

                var img = (Image)((PictureBox)sender).Image.Clone();
                Imageviewer i = new Imageviewer(new Bitmap(img));
                i.WindowState = FormWindowState.Maximized;
                i.ShowDialog();
            }
            catch (Exception errr)
            {


            }
        }
        bool _csv_has_headers = true;
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            _csv_has_headers = checkBox2.Checked;

        }
    }
}
