using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogoDetector
{
    public partial class Form1 : Form
    {
        List<ImageLogoInfo> processedImages = new List<ImageLogoInfo>();
        int maxItemsinListview = 100 * 1000;

        CancellationTokenSource TokenCanceller = null;
        bool OperationStarted = false;

        public Form1()
        {
            InitializeComponent();
            TokenCanceller = new CancellationTokenSource();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (OperationStarted)
            {
                button1.Text = "Process";
                TokenCanceller.Cancel(false);
                TokenCanceller.Dispose();
                return;
            }
            if(string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("You have to set the images folder!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1.Focus();
                textBox1.SelectAll();
                return;
            }
            if (!Directory.Exists (textBox1.Text))
            {
                MessageBox.Show("This directory is not exist!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1.Focus();
                textBox1.SelectAll();
                return;
            }
            TokenCanceller = new CancellationTokenSource();
            button1.Text = "Stop";
            OperationStarted = true;
            listView1.Items.Clear();
            processedImages.Clear();
            listView1.SuspendLayout();
            string folderPath = textBox1.Text;
            double total_process_time = 0;
            long _cnt = 0, _cnt_true = 0, _cnt_false = 0;
            var s = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                var imgExts = new string[] { "*.jpeg", "*.jpg", "*.png", "*.BMP", "*.GIF", "*.TIFF", "*.Exif", "*.WMF", "*.EMF" };
                // foreach (var item in MyDirectory.GetFiles(textBox1.Text, imgExts, SearchOption.AllDirectories))

                Task task = Task.Factory.StartNew(delegate
                {
                    try
                    {


                        Parallel.ForEach(MyDirectory.GetFiles(folderPath, imgExts, SearchOption.AllDirectories), new ParallelOptions { MaxDegreeOfParallelism = System.Environment.ProcessorCount, CancellationToken = TokenCanceller.Token }, (item) =>
                        {

                            if (TokenCanceller.IsCancellationRequested)
                            {

                                return;
                            }
                            {
                                ListViewItem lvi = null;
                                ImageLogoInfo info = null;
                                if (File.Exists(item))
                                {

                                    info = ImageLogoInfo.ProccessImage(item);

                                    lvi = new ListViewItem(info.ImageName, info.HasLogo ? 0 : 1);
                                    if (info.ConfusedImage )
                                    {
                                        lvi.ImageIndex = 2;
                                        lvi.ForeColor = Color.Orange;
                                    }

                                    lvi.SubItems.Add(info .ConfusedImage == true ? "Maybe" :  info.HasLogo ? "Yes" : "No");
                                    lvi.SubItems.Add(info.ProcessingTime + " ms");
                                    lvi.SubItems.Add(info.Confidence + " %");
                                    total_process_time += info.ProcessingTime;
                                    lvi.Tag = info;
                                    processedImages.Add(info);
                                    _cnt++;
                                    if (info.HasLogo)
                                        _cnt_true++;
                                    else
                                        _cnt_false++;


                                }
                                else
                                {

                                    lvi = new ListViewItem(Path.GetFileName(item), 1);
                                    lvi.SubItems.Add("File does not exist!");
                                    lvi.BackColor = Color.Red;
                                    lvi.ForeColor = Color.White;

                                }

                                BeginInvoke((Action)(() =>
                                {
                                    if (_cnt < maxItemsinListview)
                                        if ((checkBox1.Checked && info.HasLogo) || (checkBox2.Checked && !info.HasLogo && !info.ConfusedImage) || (checkBox3.Checked && info.ConfusedImage))
                                            listView1.Items.Add(lvi);
                                    stat_time.Text  = s.Elapsed.TotalSeconds + " Seconds" + " [Total Process Time: " + total_process_time / 1000 + " Seconds]" + " (" + _cnt + " Items, True=" + _cnt_true + " False=" + _cnt_false + ")";
                                    status_info.Text = listView1.Items.Count + " Items";
                                }));

                            }
                        }
                     );
                    }
                    catch (OperationCanceledException er)
                    {


                    }
                    catch (Exception er)
                    {
                        MessageBox.Show(er.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                });
                task.ContinueWith((t) =>
        BeginInvoke((Action)(() =>
        {
            s.Stop();
            stat_time.Text = s.Elapsed.TotalSeconds + " Seconds" + " [Total Process Time: " + total_process_time / 1000 + " Seconds]" + " (" + _cnt + " Items, True=" + _cnt_true + " False=" + _cnt_false + ")";
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listView1.ResumeLayout();
            OperationStarted = false;
            button1.Text = "Process";
        }))
);


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {

            }

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {

                var info = (ImageLogoInfo)listView1.SelectedItems[0].Tag;
                if (info == null)
                {
                    pictureBox1.Image =
                    pictureBox2.Image = null;
                }
                else
                {
                    var source = (Bitmap)Bitmap.FromStream(new MemoryStream(File.ReadAllBytes(info.ImagePath)));
                    pictureBox1.Image = source;
                  //  ImageLogoInfo info1 = ImageLogoInfo.ProccessImage(info.ImagePath);
                    pictureBox2.Image = info.ProcessedImage;
                }

            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                status_info.Text = "Working..";
                listView1.SuspendLayout();
                listView1.Items.Clear();
                listView1.Sorting = SortOrder.None;sortColumn = -1;
                Cursor = Cursors.WaitCursor;
                long _cnt = 0;
                for (int i = 0; i < processedImages.Count; i++)
                {
                    if (_cnt >= maxItemsinListview) return;
                    var info = processedImages[i];
                    if ((checkBox1.Checked && info.HasLogo) || (checkBox2.Checked && !info.HasLogo && !info.ConfusedImage) || (checkBox3.Checked && info.ConfusedImage))
                    {
                     
                        var lvi = new ListViewItem(info.ImageName, info.HasLogo ? 0 : 1);
                        
                        if (info.ConfusedImage  )
                        {
                            lvi.ForeColor = Color.Orange;
                            lvi.ImageIndex = 2;
                        }
                        lvi.SubItems.Add(info.ConfusedImage == true ? "Maybe" : info.HasLogo ? "Yes" : "No");
                        lvi.SubItems.Add(info.ProcessingTime + " ms");
                        lvi.SubItems.Add(info.Confidence + " %");
                        lvi.Tag = info;
                        listView1.Items.Add(lvi);
                        _cnt++;
                    }
                }
            }
            catch { }
            finally
            {
                listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

                listView1.ResumeLayout();
                status_info.Text = listView1.Items.Count + " Items";
                Cursor = Cursors.Default;
            }
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            try
            {

                if (DialogResult.OK != saveFileDialog1.ShowDialog(this))
                    return;
                StringBuilder txt = new StringBuilder();

                txt.AppendLine("Image Path,Has Logo,Processing Time,Confidence");
                for (int i = 0; i < processedImages.Count; i++)

                {
                    var item = processedImages[i];
                    if ((checkBox1.Checked && item.HasLogo) || (checkBox2.Checked && !item.HasLogo && !item.ConfusedImage) || (checkBox3.Checked && item.ConfusedImage))
                        txt.AppendLine(item.ImagePath + "," + (item.ConfusedImage ==true ?"Maybe": item.HasLogo+"" )+ "," + item.ProcessingTime + "ms,"+ item.Confidence + "%");
                }
                var fpath = saveFileDialog1.FileName;

                File.WriteAllText(saveFileDialog1.FileName, txt.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
#if DEBUG
#else
            textBox1.Text = "";
#endif
        }
        private int sortColumn = -1;

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
           var sw= System.Diagnostics.Stopwatch.StartNew();
            if (e.Column != sortColumn)
            {
                // Set the sort column to the new column.
                sortColumn = e.Column;
                // Set the sort order to ascending by default.
                listView1.Sorting = SortOrder.Ascending;
            }
            else
            {
                // Determine what the last sort order was and change it.
                if (listView1.Sorting == SortOrder.Ascending)
                    listView1.Sorting = SortOrder.Descending;
                else
                    listView1.Sorting = SortOrder.Ascending;
            }

            // Call the sort method to manually sort.
            listView1.Sort();
            // Set the ListViewItemSorter property to a new ListViewItemComparer
            // object.
            this.listView1.ListViewItemSorter = new ListViewItemComparer(e.Column,
                                                              listView1.Sorting);
            sw.Stop();
            status_info.Text = sw.ElapsedMilliseconds + " Milliseconds.";

        }

        private void button2_Click(object sender, EventArgs e)
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
            var imgExts = new string[] { "*.jpeg", "*.jpg", "*.png", "*.BMP", "*.GIF", "*.TIFF", "*.Exif", "*.WMF", "*.EMF" };
            var cnt=MyDirectory.GetFiles(textBox1.Text, imgExts, SearchOption.AllDirectories).LongCount();

            MessageBox.Show("Number of images= " + cnt +" images"+ Environment.NewLine + "Images supported are:" + Environment.NewLine + string.Join(" , ", imgExts)     +"");

        }
    }




    public static class BitmapProcess
    {
        public static KeyValuePair<Bitmap, int> HasLogo(Bitmap source, int minShapes, int MinPixels = 30, int MaxPixels = 150)
        {
            var sourceData = new LockBitmap(source);
            var target = new Bitmap(source.Width, source.Height, PixelFormat.Format24bppRgb);
            var targetData = new LockBitmap(target);
            sourceData.LockBits();
            targetData.LockBits();
            try
            {
                //Filter the shapes similar to logo
                var closedPaths = sourceData.FindClosedAreas(MinPixels, MaxPixels);
                var shapesFopund = closedPaths.Count;
                if (closedPaths.Count >= minShapes)
                {
                    closedPaths = closedPaths.FindShapesInCirclesBorder(minShapes);
                    foreach (var item in closedPaths)
                    {
                        // var cmykColors= item.ConvertAll(c => ColorSpaceHelper.RGBtoCMYK(sourceData[c.X, c.Y]));
                        targetData.ChangeColor(item, Color.Red);
                    }
                }
                var conf = closedPaths.Count < minShapes ? 0 : 100 - Math.Abs(closedPaths.Count - 5) * 20;
                return new KeyValuePair<Bitmap, int>(target, conf);
            }
            finally
            {
                sourceData.UnlockBits();
                targetData.UnlockBits();
                //bitmap.Save("c:\\d\\logo.png");
            }
        }




    }

    public static class MyDirectory
    {   // Regex version
        public static IEnumerable<string> GetFiles(string path,
                            string searchPatternExpression = "",
                            SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            Regex reSearchPattern = new Regex(searchPatternExpression, RegexOptions.IgnoreCase);
            return Directory.EnumerateFiles(path, "*", searchOption)
                            .Where(file =>
                                     reSearchPattern.IsMatch(Path.GetExtension(file)));
        }

        // Takes same patterns, and executes in parallel
        public static IEnumerable<string> GetFiles(string path,
                            string[] searchPatterns,
                            SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return searchPatterns.AsParallel()
                   .SelectMany(searchPattern =>
                          Directory.EnumerateFiles(path, searchPattern, searchOption));
        }
    }

    /// <summary>
    /// This class just holds the image info after we process it.
    /// </summary>
    class ImageLogoInfo
    {
        public long ProcessingTime { get; set; }
        public string ImagePath { get; set; }
        public string ImageName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ImagePath))
                    return null;
                else
                    return Path.GetFileName(ImagePath);
            }
        }
        public bool HasLogo { get; set; }
        public int Confidence { get; set; }
        public bool ConfusedImage { get { return Confidence < 50 && Confidence > 45; } }
        public Bitmap ProcessedImage { get; set; }

        //public static ImageLogoInfo ProccessImage_old2(string imgPath)
        //{
        //    ImageLogoInfo info = new ImageLogoInfo();
        //    info.ImagePath = imgPath;
        //    Bitmap source = (Bitmap)Bitmap.FromStream(new MemoryStream(File.ReadAllBytes(imgPath)));
        //    var sw = System.Diagnostics.Stopwatch.StartNew();
        //    var min = Math.Min(source.Width, source.Height);
        //    var scales = min > 500 ? new float[] { 1 } : (min > 400 ? new float[] { 1, 1.5f } : new float[] { 1, 1.5f, 2f });
        //    var grayScales = new byte[] { 90, 100, 130, 150, 160, 180, 200, 220 };
        //    foreach (var scale in scales)
        //    {
        //        foreach (var gray in grayScales)
        //        {
        //            var firstCheck = MyLogoDetector.Process(source.Crop(65, 65, scale), gray, 4);
        //            info.HasLogo = firstCheck.Value > 50;
        //            info.Confidence = firstCheck.Value;
        //            info.ProcessedImage = firstCheck.Key;
        //            if (info.HasLogo) break;
        //        }

        //        if (info.HasLogo) break;
        //    }
        //    sw.Stop();
        //    info.ProcessingTime = sw.ElapsedMilliseconds;

        //    return info;
        //}
        public static ImageLogoInfo ProccessImage(string imgPath)
        {
            ImageLogoInfo info = new ImageLogoInfo();
            info.ImagePath = imgPath;
          
            Bitmap source = (Bitmap)Bitmap.FromStream(new MemoryStream(File.ReadAllBytes(imgPath)));
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var min = Math.Min(source.Width, source.Height);
            var scales = min > 500 ? new float[] { 1 } : (min > 400 ? new float[] { 1, 1.5f } : new float[] { 1, 1.5f, 2f });
            foreach (var scale in scales)
            {
                var image = source.Crop(65, 65, scale);
                var firstCheck = MyTemplateMatching.DetectLogo(image);
                info.HasLogo = firstCheck > 50;
                info.Confidence = (int)firstCheck;
                info.ProcessedImage = image;

                if (info.HasLogo) break;
            }
            sw.Stop();
            info.ProcessingTime = sw.ElapsedMilliseconds;

            return info;
        }
    }
}
