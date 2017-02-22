using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
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
                TokenCanceller.Cancel(false );
                TokenCanceller.Dispose();
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

                                   lvi.SubItems.Add(info.HasLogo ? "Yes" : "No");
                                   lvi.SubItems.Add(info.ProcessingTime + " ms");
                                   lvi.SubItems.Add(info.HasLogo ? info.Confidence + " %" : "");
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
                                       if ((checkBox1.Checked && info.HasLogo) || (checkBox2.Checked && !info.HasLogo))
                                           listView1.Items.Add(lvi);
                                   Text = s.Elapsed.TotalSeconds + " Seconds" + " [Total Process Time: " + total_process_time / 1000 + " Seconds]" + " (" + _cnt + " Items, True=" + _cnt_true + " False=" + _cnt_false + ")";
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
            Text = s.Elapsed.TotalSeconds + " Seconds" + " [Total Process Time: " + total_process_time / 1000 + " Seconds]" + " (" + _cnt + " Items, True=" + _cnt_true + " False=" + _cnt_false + ")";
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
                    // var target = source.Crop(60, 60);
                    pictureBox1.Image = source;
                    //   pictureBox2.Image = target;
                    ImageLogoInfo info1 = ImageLogoInfo.ProccessImage(info.ImagePath );
                    pictureBox2.Image = info1.ProcessedImage;
                }

            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                listView1.SuspendLayout();
                listView1.Items.Clear();
                Cursor = Cursors.WaitCursor;
                long _cnt = 0;
                for (int i = 0; i < processedImages.Count; i++)
                {
                    if (_cnt >= maxItemsinListview) return;
                    var info = processedImages[i];
                    if ((checkBox1.Checked && info.HasLogo) || (checkBox2.Checked && !info.HasLogo))
                    {
                        var lvi = new ListViewItem(info.ImageName, info.HasLogo ? 0 : 1);

                        lvi.SubItems.Add(info.HasLogo ? "Yes" : "No");
                        lvi.SubItems.Add(info.ProcessingTime + " ms");
                        lvi.SubItems.Add(info.HasLogo ? info.Confidence + " %" : "");
                        lvi.Tag = info;
                        listView1.Items.Add(lvi);
                        _cnt++;
                    }
                }
            }
            catch { }
            finally
            {
                listView1.ResumeLayout();
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

                txt.AppendLine("Image Path,Has Logo,Confidence");
                for (int i = 0; i < processedImages.Count; i++)

                {
                    var item = processedImages[i];
                    if ((checkBox1.Checked && item.HasLogo) || (checkBox2.Checked && !item.HasLogo))
                        txt.AppendLine(item.ImagePath + "," + item.HasLogo + "," + (item.Confidence>0?"%"+ item.Confidence:""));
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
    }




    public static class BitmapProcess
    {
        public static int HasLogo(Bitmap bitmap, int minShapes, int MinPixels=30, int MaxPixels=150)
        {
            var pixels = new LockBitmap(bitmap);
            pixels.LockBits();
            try
            {
                //Filter the shapes similar to logo
                var closedPaths = pixels.FindClosedAreas(MinPixels, MaxPixels);
                 var shapesFopund = closedPaths.Count;
                if (closedPaths.Count >= minShapes)
                {
                    closedPaths=closedPaths.FindShapesInCirclesBorder(minShapes);
                    //var farShapesDic = new Dictionary<List<Point>, List<Point>>();
                    //var closestShapesDic = new Dictionary<List<Point>, List<Point>>();
                    //var clone = closedPaths.ToList();
                    //foreach (var item in closedPaths)
                    //{
                    //    var alreadyAdded = farShapesDic.Where(c => c.Value == item).Select(c => c.Key).FirstOrDefault();
                    //    if (alreadyAdded != null) clone.Remove(alreadyAdded);
                    //    var farShape = clone.FindShape(item, false);
                    //    if (alreadyAdded != null) clone.Add(alreadyAdded);
                    //    if (farShape == null) continue;
                    //    farShapesDic[item] = farShape;

                    //    alreadyAdded = closestShapesDic.Where(c => c.Value == item).Select(c => c.Key).FirstOrDefault();
                    //    if (alreadyAdded != null) clone.Remove(alreadyAdded);
                    //    var closestShape = clone.FindShape(item, true);
                    //    if (alreadyAdded != null) clone.Add(alreadyAdded);
                    //    if (closestShape == null) continue;
                    //    closestShapesDic[item] = closestShape;
                    //}

                    //var farDistances = farShapesDic.Select(c => c.Key.GetDistanceBetween(c.Value, false)).ToList();
                    //var averageFarDistance = farDistances.Average();
                    //var minFarDistance = farDistances.Min();
                    //var maxFarDistance = farDistances.Max();

                    //var closestDistances = closestShapesDic.Select(c => c.Key.GetDistanceBetween(c.Value, true)).ToList();
                    //var averageClosestDistance = closestDistances.Average();
                    //var minClosestDistance = closestDistances.Min();
                    //var maxClosestDistance = closestDistances.Max();

                    //var validShapes = farShapesDic.Where(c =>
                    //{
                    //    var d1 = (int)c.Key.GetDistanceBetween(c.Value, false);
                    //    if (Math.Abs(averageFarDistance - d1) > 7 || d1 > 50 || d1 < 15)
                    //        return false;
                    //    if (d1 - minFarDistance > 10 || maxFarDistance - d1 > 10)
                    //        return false;

                    //    d1 = (int)c.Key.GetDistanceBetween(closestShapesDic[c.Key], true);
                    //    if (Math.Abs(averageClosestDistance - d1) > 7 || d1 > 15 || d1 <= 0)
                    //        return false;
                    //    if (d1 - minClosestDistance > 10 || maxClosestDistance - d1 > 10)
                    //        return false;
                    //    return true;
                    //}).ToList();
                    //closedPaths = validShapes.Select(c => c.Key).ToList();

                    foreach (var item in closedPaths)
                        pixels.ChangeColor(item, Color.Red);
                }
                if (closedPaths.Count < minShapes)
                    return 0;
                return 100 - Math.Abs(closedPaths.Count - 5) * 10;
            }
            finally
            {
                pixels.UnlockBits();
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
        public Bitmap ProcessedImage { get; set; }

        public static ImageLogoInfo ProccessImage(string imgPath)
        {
            ImageLogoInfo info = new ImageLogoInfo();
            info.ImagePath = imgPath;
            Bitmap source = (Bitmap)Bitmap.FromStream(new MemoryStream(File.ReadAllBytes(imgPath)));
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var min = Math.Min(source.Width,source.Height);
            var scales = min > 500 ? new float[] { 1 }:(min > 400 ? new float[] { 1,1.5f } : new float[] { 1, 1.5f, 2f });
            foreach (var item in scales)
            {
                // crop the right button image
                var croppedImage = source.Crop(65, 65, item);
                var firstCheck = BitmapProcess.HasLogo(croppedImage, 4);
                if (firstCheck < 50)
                {
                    croppedImage = EdgeDetector.ProposedEdgeDetection(source.Crop(65, 65, item));
                    firstCheck = BitmapProcess.HasLogo(croppedImage, 4);
                }
                if (firstCheck < 50)
                {
                    croppedImage = EdgeDetector.Sobel(source.Crop(65, 65, item));
                    firstCheck = BitmapProcess.HasLogo(croppedImage, 4);
                }
                info.HasLogo = firstCheck > 50;
                if (info.HasLogo)
                {
                    var image = source.Crop(65, 65, item);
                    var secondCheck = BitmapProcess.HasLogo(image, 5);
                    if (secondCheck < firstCheck)
                        info.Confidence = firstCheck - 20;
                    else
                    {
                        croppedImage = image;
                        info.Confidence = secondCheck;
                    }
                }
                info.ProcessedImage = croppedImage;
                if (info.HasLogo) break;
            }
            

           
            sw.Stop();
            info.ProcessingTime = sw.ElapsedMilliseconds;

            return info;
        }

    }
}
