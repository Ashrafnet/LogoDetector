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
                    ImageLogoInfo info1 = ImageLogoInfo.ProccessImage(info.ImagePath );
                    pictureBox1.Image = (Bitmap)Bitmap.FromStream(new MemoryStream(File.ReadAllBytes(info.ImagePath)));
                    pictureBox2.Image = info.ProcessedImage ;
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


        public static bool IsCloseColor(Color color1, Color color2)
        {
            return Math.Abs(color1.R - color2.R) < 10 && Math.Abs(color1.G - color2.G) < 10 && Math.Abs(color1.B - color2.B) < 10;
        }
        /// <summary>
        /// Resize the image
        /// </summary>
        public static Bitmap GetLastWidthHeight(Bitmap bitmap, int Width, int Height)
        {
            var x = Math.Max(0, bitmap.Width - Width);
            var y = Math.Max(0, bitmap.Height - Height);
            Width = Math.Min(Width, bitmap.Width);
            Height = Math.Min(Height, bitmap.Height);
            return bitmap.Clone(new Rectangle(x, y, Width, Height), bitmap.PixelFormat);

        }


        /// <summary>
        /// Finds all areas with this color
        /// </summary>
        public static List<Point> FillPath(BitmapPixels bitmap, int X, int Y)
        {
            List<Point> path = new List<Point>();
            Queue q = new Queue();
            path.Add(new Point(X, Y));
            q.Enqueue(new Point(X, Y));
            var pixelColor = bitmap[X, Y];
            while (q.Count > 0)
            {
                var p = (Point)q.Dequeue();
                X = p.X;
                Y = p.Y;
                for (int i = 1; i <= 4; i++)
                {
                    int x2 = X;
                    int y2 = Y;
                    switch (i)
                    {
                        case 1://left
                            x2--;
                            break;
                        case 2://Right
                            x2++;
                            break;
                        case 3://down
                            y2++;
                            break;
                        case 4://up
                            y2--;
                            break;
                    }
                    if (x2 < bitmap.Width && x2 >= 0 && y2 < bitmap.Height && y2 >= 0)
                    {
                        var p2 = new Point(x2, y2);
                        if (!path.Contains(p2) &&bitmap[x2, y2]==pixelColor)
                        {
                            q.Enqueue(p2);
                            path.Add(p2);
                        }
                    }

                }
            }
            return path;
        }
        /// <summary>
        /// Finds all closed paths with the same color
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static List<List<Point>> FindClosedPaths(BitmapPixels bitmap, int PathPixelCount)
        {
            var paths = new List<List<Point>>();
            var pixels = bitmap.NotbackgroundPixels.ToList();

            while (pixels.Count > 0)
            {
                var path = FillPath(bitmap, pixels[0].X, pixels[0].Y);
                if (path.Count >= PathPixelCount)
                    paths.Add(path);
                pixels = pixels.Except(path).ToList();
            }
            return paths;
        }
        /// <summary>
        /// Calc the distance between the two points
        /// </summary>
        /// <returns></returns>
        public static double Distance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }
        /// <summary>
        /// Finds the distance between edges
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static double[] CalcEdgesQatars(List<Point> Path)
        {
            var p1 = (from p in Path orderby p.X ascending select p).First();
            var p2 = (from p in Path orderby p.X descending select p).First();
            var p3 = (from p in Path orderby p.Y ascending select p).First();
            var p4 = (from p in Path orderby p.Y descending select p).First();

            var distances = new List<double>();
            var d1 = Distance(p1, p2);
            var d2 = Distance(p3, p4);
            return new double[] { d1, d2 };
        }

        public static bool HasLogo(Bitmap bitmap)
        {

          
            var pixels = new BitmapPixels(bitmap);
            var closedPaths = BitmapProcess.FindClosedPaths(pixels, 45);
            closedPaths = closedPaths.FindAll(c => c.Count < 70);
            foreach (var item in closedPaths)
                MarkPath(bitmap, item, Color.Black);
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    if (!pixels[i, j]) continue;
                    bitmap.SetPixel(i,j,Color.White);
                }
            }
            
            // bitmap.Save("c:\\d\\logo.png");
            var repeated = BitmapProcess.CalculateTheRepeatedPathsCount(closedPaths.ConvertAll(c => c.Count), 15);
            if (repeated.Count == 0 || repeated[0] < 3)
                return false;

            var distances = closedPaths.ConvertAll(c => CalcEdgesQatars(c));
            var sameDistanceAndColor = new List<int>();
            for (int i = 0; i < distances.Count; i++)
            {
                var item1 = distances[i];
                var p1 = closedPaths[i][0];
                var counter = 0;
                for (int j = 0; j < distances.Count; j++)
                {
                    var item2= distances[j];
                    var p2 = closedPaths[j][0];
                    var c1 = item2[0] < 20 && item2[1] < 20 && Math.Abs(item2[0] - item2[1]) < 10;
                    var c2 = Math.Abs(item1[0] - item2[0]) < 4 || Math.Abs(item1[0] - item2[1]) < 4;
                    var c3 = Math.Abs(item1[1] - item2[0]) < 4 || Math.Abs(item1[1] - item2[1]) < 4;
                    var c4 = true;// Math.Abs((item1[1] + item1[0]) - (item2[1] + item2[0])) <= 3;
                    var c5 = true;// IsCloseColor(pixels.Colors[p1.X, p1.Y], pixels.Colors[p2.X, p2.Y]);
                    if (c1 && c2 && c3 && c4 && c5) counter++;
                }
                sameDistanceAndColor.Add(counter);
            }
            var dic = new Dictionary<int, int>();
            foreach (var item in sameDistanceAndColor)
            {
                if (item < 3) continue;
                if(!dic.ContainsKey(item)) dic[item] = 0;
                dic[item]++;
            }
            var isLogo = dic.Values.Any(c=>c>=3);
            return isLogo;
        }
        /// <summary>
        /// Mark the path
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static void MarkPath(Bitmap bitmap, List<Point> Path, Color Color)
        {
            foreach (var item in Path)
                bitmap.SetPixel(item.X, item.Y, Color);
        }
        /// <summary>
        /// Finds how many each number repeated
        /// </summary>
        /// <param name="Counts"></param>
        /// <returns></returns>
        public static List<int> CalculateTheRepeatedPathsCount(List<int> Counts, int Threshold)
        {
            var list = new List<int>();
            foreach (var item in Counts)
            {
                var n = Counts.Count(c =>
                {
                    var diff = Math.Abs(c - item);
                    return diff <= Threshold;
                });
                if (!list.Contains(n))
                    list.Add(n);
            }
            list.Sort();
            list.Reverse();
            return list;
        }

    }

    public class BitmapPixels
    {
        /// <summary>
        /// If Color is backgroun then True
        /// </summary>
        public bool[,] IsBackground { get; private set; }
        /// <summary>
        /// Store the colors of image
        /// </summary>
        public Color[,] Colors { get; private set; }
        /// <summary>
        /// The pixels other than background
        /// </summary>
        public List<Point> NotbackgroundPixels { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public BitmapPixels(Bitmap bitmap)
        {
            this.Width = bitmap.Width;
            this.Height = bitmap.Height;
            IsBackground = new bool[Width, Height];
            Colors = new Color[Width, Height];
            NotbackgroundPixels = new List<Point>();
            caclulate(bitmap);
        }

        public bool this[int X, int Y]
        {
            get { return IsBackground[X, Y]; }
            set { IsBackground[X, Y] = value; }
        }



        private void caclulate(Bitmap bitmap)
        {
            var backgroundColors = new List<Color>();
            for (int i = 1; i < 30; i++)
            {
                var color = bitmap.GetPixel(bitmap.Width - i, bitmap.Height - i);
                var count = backgroundColors.Count;
                if (count == 0 || BitmapProcess.IsCloseColor(color, backgroundColors[count - 1]))
                    backgroundColors.Add(color);
            }

            for (int i = bitmap.Width - 1; i >= 0; i--)
            {
                for (int j = bitmap.Height - 1; j >= 0; j--)
                {
                    var color = bitmap.GetPixel(i, j);
                    Colors[i, j] = color;
                    IsBackground[i, j] = backgroundColors.Exists(c => BitmapProcess.IsCloseColor(c, color));
                    if (!IsBackground[i, j])
                        NotbackgroundPixels.Add(new Point(i, j));
                }
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
            
             
              bitmap = BitmapProcess.GetLastWidthHeight(bitmap, 70, 70); // crop the right button image
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
