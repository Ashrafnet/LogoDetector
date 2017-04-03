using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
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
        List<ImageLogoInfo> listviewItems = new List<ImageLogoInfo>();
        string[] imgExts = new string[] { "*.jpeg", "*.jpg", "*.png", "*.BMP", "*.GIF", "*.TIFF", "*.Exif", "*.WMF", "*.EMF", "*.ppm", "*.pgm", "*.pbm" };
      

        CancellationTokenSource cancellationTokenSource;
        public Form1()
        {
            InitializeComponent();
        }
        double total_process_time = 0;
        long withLogoCount = 0;
        Stopwatch processStopwatch;
        private void button1_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy)
            {
                if (MessageBox.Show(this,"Do you want to cancel the process?", "Cancel process", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    return;
                button1.Text = "Process";
                backgroundWorker1.CancelAsync();
                cancellationTokenSource.Cancel();
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
            button1.Text = "Stop";
            buttonPause.Text = "Pause";
          buttonPause.Enabled = true;
            backgroundWorker1.RunWorkerAsync( textBox1.Text);
           

        }
        bool processPaused;
        private void buttonPause_Click(object sender, EventArgs e)
        {
            processPaused = !processPaused;
            buttonPause.Text = processPaused ? "Resume" : "Pause";
            if (processPaused) processStopwatch.Stop();
            else processStopwatch.Start();
        }
        ProcessedItems items;
        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            processedImages.Clear();
            string folderPath = e.Argument+"";
            total_process_time = 0;
            withLogoCount = 0;
            processPaused = false;
            processStopwatch = Stopwatch.StartNew();
            cancellationTokenSource = new CancellationTokenSource();
            if (items != null)
                items.Dispose();
            items = new ProcessedItems(true);
           
            Parallel.ForEach(MyDirectory.GetFiles(folderPath, imgExts, SearchOption.AllDirectories), new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount, CancellationToken = cancellationTokenSource.Token }, (item) =>
            {
                if (backgroundWorker1.CancellationPending)
                    return;
                while (processPaused && !backgroundWorker1.CancellationPending)
                    Thread.Sleep(1000);
                var info = ImageLogoInfo.ProccessImage(item);
                total_process_time += info.ProcessingTime;
                

                for (int i = 0; i < 1000 * 1000; i++)
                {

                     lock (processedImages)  processedImages.Add(info);
                    // items.InsertItemToSqlite(info);


                    if (info.HasLogo)
                        withLogoCount++;
                }
            });

        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            buttonPause.Enabled = false;
            processPaused = false;
            processStopwatch.Stop();
            timerRefreshlistview_Tick(null, null);
            button1.Text = "Process";
            if (e.Error != null&&!(e.Error is OperationCanceledException))
            {
                MessageBox.Show(e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (e.Error == null)
            {
                MessageBox.Show("Process completed", "Ready", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedIndexes = listView1.SelectedIndices;
            if (selectedIndexes.Count == 1)
            {

                var info = listviewItems[selectedIndexes[0]];
                labelFailImage.Visible = false;
                pictureBox1.Image =pictureBox2.Image = null;
                if (info == null) { }
                else if (info.Error != null)
                {
                    labelFailImage.Text = info.Error.Message;
                    labelFailImage.Visible = true;
                }
                else
                {
                   // var source = (Bitmap)Bitmap.FromStream(new MemoryStream(File.ReadAllBytes(info.ImagePath)));
                    Bitmap source =ImageLogoInfo. GetBitmap(info.ImagePath);
                    pictureBox1.Image = source;
                    //  ImageLogoInfo info1 = ImageLogoInfo.ProccessImage(info.ImagePath);
                    pictureBox2.Image = info.ProcessedImage ?? source.Crop(65, 65);
                }

            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            listView1.SelectedIndices.Clear();
            timerRefreshlistview_Tick(null, null) ;
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (listviewItems == null || listviewItems.Count < 1)
                {
                    MessageBox.Show("No items in list to export!", "No Items", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (DialogResult.OK != saveFileDialog1.ShowDialog(this))
                    return;
                var fpath = saveFileDialog1.FileName;
                StringBuilder txt = new StringBuilder();

                txt.AppendLine("Image Path,Has Logo,Processing Time,Confidence");
                using (StreamWriter outfile = new StreamWriter(fpath))
                {

                    foreach (var item in listviewItems)
                    {

                        txt.AppendLine(item.ImagePath + "," + (item.ConfusedImage == true ? "Maybe" : item.HasLogo + "") + "," + item.ProcessingTime + "ms," + item.Confidence + "%");
                        outfile.Write(txt.ToString());


                        txt.Clear();
                    }

                }
                

               // File.WriteAllText(fpath, txt.ToString());
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
            timerRefreshlistview_Tick(null, null);
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
            var cnt=MyDirectory.GetFiles(textBox1.Text, imgExts, SearchOption.AllDirectories).LongCount();

            MessageBox.Show("Number of images= " + cnt +" images"+ Environment.NewLine + "Images supported are:" + Environment.NewLine + string.Join(" , ", imgExts)     +"");

        }

        private void listView1_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            var info = listviewItems[e.ItemIndex];
            var lvi = new ListViewItem(info.ImageName, info.HasLogo ? 0 : 1);

            if (info.ConfusedImage)
            {
                lvi.ForeColor = Color.Orange;
                lvi.ImageIndex = 2;
            }
            else if (info.Error!=null)
            {
                lvi.ForeColor = Color.Red;
                lvi.ImageIndex = 3;
            }
            lvi.SubItems.Add(info.ConfusedImage == true ? "Maybe" : info.HasLogo ? "Yes" : "No");
            lvi.SubItems.Add(info.ProcessingTime + " ms");
            lvi.SubItems.Add(info.Confidence + " %");
            lvi.Tag = info;
            e.Item = lvi;
        }

        private void timerRefreshlistview_Tick(object sender, EventArgs e)
        {
            if (this.items == null) return;
            if (!backgroundWorker1.IsBusy&& sender==timerRefreshlistview) return;

            status_info.Text = processedImages.Count + " Items";
            if(processStopwatch!=null )
                stat_time.Text = processStopwatch.Elapsed.TotalSeconds + " Seconds" + " [Total Process Time: " + total_process_time / 1000 + " Seconds]" + " (" + processedImages.Count + " Items, True=" + withLogoCount + " False=" + (processedImages.Count - withLogoCount) + ")";
            if (cancellationTokenSource != null && cancellationTokenSource.IsCancellationRequested)
                status_info.Text += " (User canceled the process)";
            else if (!backgroundWorker1.IsBusy)
                status_info.Text += " (Process completed)";

            var items=   this.items.FindAll(checkBox1.Checked, checkBox2.Checked, checkBox3.Checked, checkBoxShowErrors.Checked);
          // var items = processedImages.FindAll(info => ((checkBox1.Checked && info.HasLogo&&info.Error==null) || (checkBox2.Checked && !info.HasLogo && !info.ConfusedImage && info.Error == null) || (checkBox3.Checked && info.ConfusedImage && info.Error == null) || (checkBoxShowErrors.Checked && info.Error!=null)));
            
            if (sortColumn != -1 && listView1.Sorting != SortOrder.None)
                items.Sort(new ListViewItemComparer(sortColumn, listView1.Sorting));

            listviewItems = items;
            listView1.VirtualListSize = listviewItems.Count;
            buttonCopyImages.Enabled = buttonExportMatches.Enabled = listviewItems!=null && listviewItems.Count > 0;
        }

        private void buttonCopyImages_Click(object sender, EventArgs e)
        {
            if(listviewItems==null || listviewItems.Count <1)
            {
                MessageBox.Show("No items in list to copy!", "No Items", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (DialogResult.OK != folderBrowserDialog1.ShowDialog(this))
                return;
            var files = listviewItems.ConvertAll(c => c.ImagePath);
            new CopyFiles.CopyFiles(files, folderBrowserDialog1.SelectedPath).CopyAsync(new CopyFiles.DIA_CopyFiles() {  SynchronizationObject=this});
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
   public  class ImageLogoInfo
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

        public Exception Error { get;  set; }
        public static ImageLogoInfo ProccessImage(string imgPath)
        {
            ImageLogoInfo info = new ImageLogoInfo();
            info.ImagePath = imgPath;
            var sw = Stopwatch.StartNew();
            try
            {
                Bitmap source = GetBitmap(imgPath);
                var min = Math.Min(source.Width, source.Height);
                var scales = min > 500 ? new float[] { 1 } : (min > 400 ? new float[] { 1, 1.5f } : new float[] { 1, 1.5f, 2f });
                foreach (var scale in scales)
                {
                    var image = source.Crop(65, 65, scale);
                    var firstCheck = MyTemplateMatching.DetectLogo(image);
                    info.HasLogo = firstCheck > 50;
                    info.Confidence = (int)firstCheck;
                    if (info.HasLogo)
                        info.ProcessedImage = image;

                    if (info.HasLogo) break;
                }
            }
            catch (Exception ex) { info.Error = ex; }
            sw.Stop();
            info.ProcessingTime = sw.ElapsedMilliseconds;

            return info;
        }

        internal  static Bitmap GetBitmap(string imgPath)
        {
            string[] imgExts_ppm = new string[] { ".ppm", ".pgm", ".pbm" };
            Bitmap source = null;

            if (imgExts_ppm.Contains((Path.GetExtension(imgPath)+"").ToLower()))
                source = new PixelMap(imgPath).BitMap;
            else
                source = (Bitmap)Bitmap.FromStream(new MemoryStream(File.ReadAllBytes(imgPath)));
            return source;
        }
    }
}
