using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogoDetector
{
    public static class BitmapUtilities
    {
        public static Bitmap ConvertToGrayScale(this Bitmap oldbmp)
        {
            //return  oldbmp.Clone(new Rectangle(0, 0, oldbmp.Width, oldbmp.Height), PixelFormat.Format8bppIndexed);
            //using (var ms = new MemoryStream())
            //{
            //    oldbmp.Save(ms, ImageFormat.Gif);
            //    ms.Position = 0;
            //    return (Bitmap)Image.FromStream(ms);
            //}

            for (int i = 0; i < oldbmp.Width; i++)
            {
                for (int x = 0; x < oldbmp.Height; x++)
                {
                    Color oc = oldbmp.GetPixel(i, x);
                    int grayScale = (int)((oc.R * 0.3) + (oc.G * 0.59) + (oc.B * 0.11));
                    Color nc = Color.FromArgb(oc.A, grayScale, grayScale, grayScale);
                    oldbmp.SetPixel(i, x, nc);
                }
            }
            return oldbmp;
        }
        /// <summary>
        /// Is number1 close to this number2 ?
        /// </summary>
        public static bool IsSimilarTo(this Color c1, Color c2,byte Threshold=10)
        {
            Threshold = 25;
           // return (Math.Abs(c1.R - c2.R) + Math.Abs(c1.G - c2.G)+ Math.Abs(c1.B - c2.B) )<= Threshold;
            int grayScale1 = (int)((c1.R * 0.3) + (c1.G * 0.59) + (c1.B * 0.11));
            int grayScale2 = (int)((c2.R * 0.3) + (c2.G * 0.59) + (c2.B * 0.11));
            return Math.Abs(grayScale1-grayScale2)<=Threshold;
        }
        /// <summary>
        /// Gets the average colors of this points
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="Points"></param>
        /// <returns></returns>
        public static Color AverageColor(this LockBitmap bitmap,IEnumerable<Point> Points)
        {
            //Used for tally
            int r = 0;
            int g = 0;
            int b = 0;

            int total = 0;
            foreach (var p in Points)
            {
                Color clr = bitmap[p.X,p.Y];
                r += clr.R;
                g += clr.G;
                b += clr.B;
                total++;
            }

            //Calculate average
            r /= total;
            g /= total;
            b /= total;

            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Resize the image
        /// </summary>
        public static Bitmap Crop(this Bitmap bitmap, int Width, int Height)
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
        public static List<Point> GetConnectedPixels(this LockBitmap bitmap, int X, int Y)
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
                        if (!path.Contains(p2) && bitmap[x2, y2].IsSimilarTo(pixelColor))
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
        public static List<List<Point>> FindClosedAreas(this LockBitmap bitmap, int PathPixelCount)
        {
            var paths = new List<List<Point>>();
            var pixels = bitmap.Points.ToList();
            
            while (pixels.Count > 0)
            {

                var path = GetConnectedPixels(bitmap, pixels[0].X, pixels[0].Y);
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
        public static double DistanceTo(this Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }
        /// <summary>
        /// Finds the distance between edges
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static double[] CalcEdgesQatars(this List<Point> Path)
        {
            var p1 = (from p in Path orderby p.X ascending select p).First();
            var p2 = (from p in Path orderby p.X descending select p).First();
            var p3 = (from p in Path orderby p.Y ascending select p).First();
            var p4 = (from p in Path orderby p.Y descending select p).First();

            var distances = new List<double>();
            var d1 = DistanceTo(p1, p2);
            var d2 = DistanceTo(p3, p4);
            return new double[] { d1, d2 };
        }

        /// <summary>
        /// Mark the path
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static void ChangeColor(this LockBitmap bitmap, IEnumerable<Point> Path, Color Color)
        {
            foreach (var item in Path)
                bitmap.SetPixel(item.X, item.Y, Color);
        }

        /// <summary>
        /// Finds how many each number repeated
        /// </summary>
        /// <param name="Numbers"></param>
        /// <returns></returns>
        public static List<int> CalculateRepeated(this IEnumerable<int> Numbers, int Threshold)
        {
            var list = new List<int>();
            foreach (var item in Numbers)
            {
                var n = Numbers.Count(c =>
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
}
