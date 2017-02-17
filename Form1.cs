using LogoDetector.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogoDetector
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            listView1.SuspendLayout();
            string folderPath = textBox1.Text;
            double  total_process_time = 0;
            long _cnt = 0;
            var s = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                var imgExts = new string[] { "*.jpeg", "*.jpg", "*.png", "*.BMP", "*.GIF", "*.TIFF", "*.Exif","*.WMF", "*.EMF" };
                // foreach (var item in MyDirectory.GetFiles(textBox1.Text, imgExts, SearchOption.AllDirectories))

                Task task = Task.Factory.StartNew(delegate
                {
                    Parallel.ForEach(MyDirectory.GetFiles(folderPath, imgExts, SearchOption.AllDirectories), new ParallelOptions { MaxDegreeOfParallelism = 8 }, item =>
              {
                  ListViewItem lvi = null;
                  ImageLogoInfo info = null;
                  if (File.Exists(item))
                  {



                       info = ImageLogoInfo.ProccessImage(item);
                      
                      lvi = new ListViewItem(info.ImageName, info.HasLogo ? 0 : 1);

                      lvi.SubItems.Add(info.HasLogo ? "Yes" : "No");
                      lvi.SubItems.Add(info.ProcessingTime + " ms");
                      total_process_time += info.ProcessingTime;
                      lvi.Tag = info;
                      _cnt++;
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
                      //if (info.HasLogo)
                          listView1.Items.Add(lvi);
                      Text = s.Elapsed.TotalSeconds + " Seconds" + " [Total Process Time: " + total_process_time / 1000 + " Seconds]" + " (" + _cnt + " Items)";
                  }));
                

              }
                 );
                });
                task.ContinueWith((t) =>
        BeginInvoke((Action)(() =>
        {
            s.Stop();
            Text = s.Elapsed.TotalSeconds + " Seconds" + " [Total Process Time: " + total_process_time / 1000 + " Seconds]" + " (" + _cnt + " Items)";
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listView1.ResumeLayout();
        }))
);


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"error", MessageBoxButtons.OK, MessageBoxIcon.Error );
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
                    var target = source.Crop(60, 60);
                    pictureBox1.Image = source;
                    pictureBox2.Image = target;
                    ImageLogoInfo info1 = ImageLogoInfo.ProccessImage(info.ImagePath );
                    pictureBox2.Image = info1.ProcessedImage;
                }

            }
        }
 
    }


    class ControlInvikerHelper
    {
        delegate void UniversalVoidDelegate();

        /// <summary>
        /// Call form control action from different thread
        /// </summary>
        public static void ControlInvike(Control control, Action function)
        {
            if (control.IsDisposed || control.Disposing)
                return;

            if (control.InvokeRequired)
                control.Invoke(new UniversalVoidDelegate(() => ControlInvike(control, function)));
            else
                function();
        }
    }


    public static class BitmapProcess
    {
        public static bool HasLogo(Bitmap bitmap)
        {
            var pixels = new LockBitmap(bitmap);
            pixels.LockBits();
            try
            {
                //var ddd= pixels.GetConnectedPixels(48, 48);
                // pixels.ChangeColor(ddd, Color.Blue);
                // return true;
                //Filter the shapes similar to logo
                var closedPaths = pixels.FindClosedAreas(30);
                closedPaths = closedPaths.FindAll(area =>
                {
                    var l = area.CalcEdgesQatars();
                    if (l[0] < 5 | l[1] < 5 || l[0] > 20 || l[1] > 20)
                        return false;
                    return true;
                });
                //Filter the shapes that has a similar colors
                closedPaths = closedPaths.FindAll(area1 =>
                {
                    var color1 = pixels.AverageColor(area1);
                    var sameColor = closedPaths.FindAll(area2 =>
                     {
                         var color2 = pixels.AverageColor(area2);
                         return color1.IsSimilarTo(color2);
                     });
                    return sameColor.Count >= 3;
                });
                //Filter connected shapes
                closedPaths = closedPaths.FindAll(area1 => {
                   return area1.IsConnectedPath();
                });
                ////Filter the shapes that close to each other
                //closedPaths = closedPaths.FindAll(area1 => {
                //    return area1.IsConnectedPath();
                //});

                foreach (var item in closedPaths)
                    pixels.ChangeColor(item, Color.Red);
                return closedPaths.Count >= 3&& closedPaths.Count<10;
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
        public string ImageName { get
            {
                if (string.IsNullOrWhiteSpace(ImagePath))
                    return null;
                else
                    return Path.GetFileName(ImagePath);
            }
        }
        public bool  HasLogo { get; set; }
        public Bitmap ProcessedImage { get; set; }

        public static ImageLogoInfo ProccessImage(string imgPath)
        {
            ImageLogoInfo info = new ImageLogoInfo();
            info.ImagePath = imgPath;
            Bitmap bitmap = (Bitmap)Bitmap.FromStream(new MemoryStream(File.ReadAllBytes(imgPath)));
            var sw = System.Diagnostics.Stopwatch.StartNew();


            bitmap = bitmap.Crop(70, 70);// crop the right button image
           // var pixles = new BitmapPixels(bitmap);// BitmapProcess.MarkImage(bitmap);
           // var closedPaths = BitmapProcess.FindClosedPaths(pixles , 50);
           // var repeated = BitmapProcess.CalculateTheRepeatedPathsCount(closedPaths.ConvertAll(c => c.Count), 15);
            info.HasLogo= BitmapProcess.HasLogo(bitmap);
            info.ProcessedImage = bitmap;
           // info.HasLogo = repeated.Count > 0 && repeated[0] >= 3;
            sw.Stop();
            info.ProcessingTime = sw.ElapsedMilliseconds;

            return info;
        }

    }
}
