using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using FeatureMatchingExample;
using ImageSimilarFinder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
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
           // findMatches(@"C:\Users\Ashraf\AppData\Roaming\Skype\My Skype Received Files\Issues\Cropped\9614468.jpg");
            InitializeComponent();
        }

        void findMatches(string imagePath)
        {
            var img = ResizeImage(Image.FromFile(imagePath), 500, 500);
           // img.Save("test.png", System.Drawing.Imaging.ImageFormat.Jpeg);
            var dd = DrawMatches.GetImageDescriptors(img);

            var img1 = loadImage(imagePath );
           // img1.Save("test1.png", System.Drawing.Imaging.ImageFormat.Jpeg);
            var dd1 = DrawMatches.GetImageDescriptors((Bitmap)img1);

            Mat similarity;
            var result = CheckSimilarty("test.png", "test1.png", dd, dd1, out similarity);


        }

        public Bitmap ResizeImage_(Bitmap img, int width, int height)
        {
            using (Bitmap b = new Bitmap(width, height))
            {
                using (Graphics g = Graphics.FromImage(b))
                {
                    g.DrawImage(img, 0, 0, width, height);
                }

                return (Bitmap)b.Clone();
            }
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
#if DEBUG == false
            textBox1.Text = "";
#endif
            textBox1.Focus();
            try
            {
                Text += " v" + Application.ProductVersion;
                Icon = Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetEntryAssembly().Location);
            }
            catch (Exception er)
            {

            }

          //  testt();

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


        Bitmap loadImage(string imagepath )
        {
            if (string.IsNullOrWhiteSpace(imagepath)) return null;
            if (!File.Exists(imagepath)) return null;
            var btm= new Bitmap((Bitmap)new Mat(imagepath).Bitmap.Clone());
            if (_resizeHugeImage && btm.Width > 700)
            {
              //  
                //calculate the ratio
                double dbl = (double)btm.Width / (double)btm.Height;
                int boxHeight = 600;
                //set height of image to boxHeight and check if resulting width is less than boxWidth, 
                //else set width of image to boxWidth and calculate new height
                if ((int)((double)boxHeight * dbl) <= boxHeight)
                {
                    btm = ResizeImage_(btm, (int)((double)boxHeight * dbl), boxHeight);
                   
                }
                else
                {
                    btm = ResizeImage_(btm, boxHeight, (int)((double)boxHeight / dbl));
                   
                }

            }
            return btm;


            //if (imgExts.Contains((Path.GetExtension(imagepath) + "").ToLower()))
            //{
            //    if (File.Exists(imagepath))
            //        return (Bitmap)Bitmap.FromStream(new MemoryStream(File.ReadAllBytes(imagepath)));
            //    else
            //        return null;
            //}
            //return null;

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
                Stopwatch sw = Stopwatch.StartNew();
                task = Task.Run(() =>
                  {

                      //ElencySolutions.CsvHelper.CsvReader r = new ElencySolutions.CsvHelper.CsvReader(textBox1.Text);
                      //var dt = r.ReadIntoDictionary(_csv_has_headers);
                     
                      startSearch();
                  }).ContinueWith((t) =>
                  {
                      sw.Stop();
                      _Isrunning = false;
                      SetButtonText( "Search",true );
                      SetStatusInfo("Done in: " + sw.Elapsed.TotalSeconds + " Seconds. | Images processed: " + _images_processed_cnt + " , Images with errors: " + _images_errors_cnt);

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
        int _images_processed_cnt = 0, _images_errors_cnt = 0;
        void startSearch()
        {
            StreamWriter _wtire_csv = null;
            StreamWriter _wtire_csv_errors = null;
            try
            {

                _images_processed_cnt = 0; _images_errors_cnt = 0;
                Dictionary<string, Tuple<VectorOfKeyPoint, Mat>> items = new Dictionary<string, Tuple<VectorOfKeyPoint, Mat>>();
                string strLine1 = "path1,path2,Similar,Error";
                string _csvFile_File_Name = _csvFile.File_Name + ".log";
                if (File.Exists(_csvFile_File_Name))
                    File.Delete(_csvFile_File_Name);
                 _wtire_csv = File.AppendText(_csvFile_File_Name);
                _wtire_csv.WriteLine(strLine1);

                var _csvFile_errors = _csvFile.File_Name + ".errors";
                if (File.Exists(_csvFile_errors))
                    File.Delete(_csvFile_errors);
                _wtire_csv_errors = File.AppendText(_csvFile_errors );
                _wtire_csv_errors.WriteLine("path1,path2,Error");

                Parallel.ForEach(_csvFile.Records, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, (item, loopState) =>
                {
                    
                    if (!_Isrunning)
                    {
                        loopState.Stop();
                        return;
                    }


                    var imageBase = item.Fields[0];
                    var image = item.Fields[1];
                  var  strLine = $"{imageBase},{image},";
                    if (string.IsNullOrWhiteSpace(imageBase) || string.IsNullOrWhiteSpace(image)) return;
                    ListViewItem i = new ListViewItem(imageBase);
                    try
                    {

                        if (!File.Exists(imageBase)) throw new FileNotFoundException("File does not exist!", imageBase);
                        if (!File.Exists(image)) throw new FileNotFoundException("File does not exist!", image);
                        lock (this)
                        {
                           
                            _wtire_csv.WriteLine(strLine);
                            _wtire_csv.Flush();
                        }

                        lock (items)
                        {
                            if (!items.ContainsKey(imageBase))
                            {
                                
                               var image_descriptors = DrawMatches.GetImageDescriptors(loadImage(imageBase ));
                                if (image_descriptors == null)
                                {
                                    
                                    _wtire_csv_errors.WriteLine(strLine);
                                    _wtire_csv_errors.Flush();
                                    return;
                                }
                                items.Add(imageBase, image_descriptors);

                            }

                            if (!items.ContainsKey(image))
                            {

                                var image_descriptors = DrawMatches.GetImageDescriptors(loadImage(image ));
                                if (image_descriptors == null)
                                {
                                    _images_errors_cnt++;
                                    _wtire_csv_errors.WriteLine(strLine);
                                    _wtire_csv_errors.Flush();
                                    return;
                                }
                                items.Add(image, image_descriptors);
                            }

                        }

                        i.Tag = image;

                        Mat similarity;
                        var result = CheckSimilarty(imageBase, image, items[imageBase], items[image], out similarity);
                        i.SubItems.Add(result + "");
                        i.SubItems.Add(image);
                        i.ImageIndex = result ? 0 : 1;

                        if (similarity != null)
                            i.Tag = new Bitmap((Bitmap)similarity.Bitmap.Clone());

                      
                        _images_processed_cnt++;
                       // GC.Collect();
                    }

                    catch (Exception er)
                    {
                        if (strLine.Count(a => a == ',') <= 2)
                            strLine += "N/A,";
                        strLine += er.Message;
                        _wtire_csv.WriteLine(strLine);
                        _wtire_csv.Flush();

                        i.ImageIndex = 3;
                        i.ForeColor = Color.Red;

                        if (i.SubItems.Count <= 1)
                            i.SubItems.Add("Eror");
                        if (i.SubItems.Count <= 2)
                            i.SubItems.Add(image);
                        if (i.SubItems.Count <= 3)
                            i.SubItems.Add(er.Message);
                    }
                    finally
                    {
                        SetStatusInfo("Working... | Images processed: " + _images_processed_cnt + " Images with errors: " + _images_errors_cnt);
                        AddItem(i);

                    }

                });

                
            }
            finally
            {
                _wtire_csv.Close();
                _wtire_csv_errors.Close();
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
                pictureBox1.Image = loadImage(listView1.SelectedItems[0].Text  + "" );

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
        bool _resizeHugeImage = true;
        private void toolStripStatusLabel2_Click(object sender, EventArgs e)
        {
            ReviewImageSimilarity f = new ImageSimilarFinder.ReviewImageSimilarity();
            f.Show(this);
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            _resizeHugeImage = checkBox3.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            _csv_has_headers = checkBox2.Checked;

        }

        Dictionary<string, List<string>> ProcessText(string strText)
        {
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            foreach (var item in strText.Split('\n'))
            {

                if (string.IsNullOrWhiteSpace(item)) continue;
                if (item=="\r") continue;
                var number = System.Text.RegularExpressions.Regex.Match(item, @"\d+").Value;
                var text = ExtractTextWithoutNumber(item);
                if (result.ContainsKey(number))
                    result[number].Add(text);
                else
                    result.Add(number, new List<string>() { text });

            }
            return result;
        }

        public string ExtractTextWithoutNumber(string original)
        {
            return new string(original.Where(c => !Char.IsDigit(c)).ToArray());
        }
    }
}
