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
            return Math.Abs(grayScale1 - grayScale2) <= Threshold;

            //int scale1 = (int)(c1.R + c1.G  + c1.B);
            //int scale2 = (int)(c2.R+ c2.G + c2.B);
            //return Math.Abs(scale1 - scale2) <= Threshold;
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
            return bitmap.Clone(new Rectangle(x, y, Width-2, Height-2), bitmap.PixelFormat);

        }

        /// <summary>
        /// Finds all areas with this color
        /// </summary>
        public static List<Point> GetConnectedPixels(this LockBitmap bitmap, int X, int Y,Predicate<Point> NavigateToThisPoint)
        {
            var path = new Dictionary<Point,int>();
            Queue q = new Queue();
            path[new Point(X, Y)]=0;
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
                        if (!path.ContainsKey(p2) && bitmap[x2, y2].IsSimilarTo(pixelColor)&&(NavigateToThisPoint==null|| NavigateToThisPoint(p2)))
                        {
                            q.Enqueue(p2);
                            path[p2]=0;
                        }
                    }

                }
            }
            return path.Keys.ToList();
        }
        /// <summary>
        /// Finds all closed paths with the same color
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static List<List<Point>> FindClosedAreas(this LockBitmap bitmap, int MinPixelsCount, int MaxPixelsCount)
        {
            var paths = new List<List<Point>>();
            var added = new Dictionary<Point, int>();
            var pixels = bitmap.Points.ToList();
            
            while (pixels.Count > 0)
            {
                var path = GetConnectedPixels(bitmap, pixels[0].X, pixels[0].Y,p=>!added.ContainsKey(p));
                foreach (var item in path)
                    added[item] = 0;
                paths.Add(path);
                pixels = pixels.Except(path).ToList();
            }
            paths.RemoveAll(c=>c.Count< MinPixelsCount||c.Count> MaxPixelsCount);
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
        /// Checks if all points are connected
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static bool IsConnectedPath(this List<Point> Path, int Threshold=2)
        {
             return true;
            Threshold = 0;
             var p1 = (from p in Path orderby p.X ascending select p).First();
            var p2 = (from p in Path orderby p.X descending select p).First();
            var p3 = (from p in Path orderby p.Y ascending select p).First();
            var p4 = (from p in Path orderby p.Y descending select p).First();

            return Path.IsConnectedPoints(p1, p2, Threshold) &&
                   Path.IsConnectedPoints(p3, p4, Threshold);
        }
        /// <summary>
        /// Checks if all points are connected
        /// </summary>
        /// <returns></returns>
        public static bool IsConnectedPoints(this List<Point> Path, Point p1, Point p2, int Threshold)
        {
            if (p1.X > p2.X)
                return IsConnectedPoints(Path, p2, p1, Threshold);
            if (p2.X == p1.X)
                return true;
            var distance = p2.X-p1.X;
            double slope = 1.0 * (p2.Y - p1.Y) / (p2.X - p1.X);
            var counter1 = 0;
            for (int x = p1.X; x <= p2.X; x++)
            {
                var y = slope * (x - p1.X) + p1.Y;
                if (Path.Contains(new Point(x, (int)y)))
                    counter1++;
            }
            return (distance - counter1) < Threshold;
        }

        public static double GetDistanceBetween(this List<Point> Shape1, List<Point> Shape2, bool CloseOrFar)
        {
            var min_x1 = (from p in Shape1 orderby p.X ascending select p).First();
            var max_x1 = (from p in Shape1 orderby p.X descending select p).First();
            var min_y1 = (from p in Shape1 orderby p.Y ascending select p).First();
            var max_y1 = (from p in Shape1 orderby p.Y descending select p).First();

            var distances = new List<double>();

            var min_x2 = (from p in Shape2 orderby p.X ascending select p).First();
            var max_x2 = (from p in Shape2 orderby p.X descending select p).First();
            var min_y2 = (from p in Shape2 orderby p.Y ascending select p).First();
            var max_y2 = (from p in Shape2 orderby p.Y descending select p).First();

            distances.Add(min_x1.DistanceTo(min_x2));
            distances.Add(min_x1.DistanceTo(max_x2));
            distances.Add(min_x1.DistanceTo(min_y2));
            distances.Add(min_x1.DistanceTo(max_y2));

            distances.Add(max_x1.DistanceTo(min_x2));
            distances.Add(max_x1.DistanceTo(max_x2));
            distances.Add(max_x1.DistanceTo(min_y2));
            distances.Add(max_x1.DistanceTo(max_y2));

            distances.Add(min_y1.DistanceTo(min_x2));
            distances.Add(min_y1.DistanceTo(max_x2));
            distances.Add(min_y1.DistanceTo(min_y2));
            distances.Add(min_y1.DistanceTo(max_y2));

            distances.Add(max_y1.DistanceTo(min_x2));
            distances.Add(max_y1.DistanceTo(max_x2));
            distances.Add(max_y1.DistanceTo(min_y2));
            distances.Add(max_y1.DistanceTo(max_y2));


            return CloseOrFar? distances.Min(): distances.Max();
        }

        /// <summary>
        /// Find the index of the closest shape to this shape
        /// </summary>
        public static List<Point> FindShape(this List<List<Point>> AllShapes, List<Point> TargetShape,bool CloseOrFar)
        {
            var minDistances = new Dictionary<List<Point>, double>();
            foreach (var item in AllShapes)
            {
                if (item == TargetShape) continue;
                minDistances[item] = GetDistanceBetween(TargetShape, item, CloseOrFar);
            }
            if (minDistances.Count == 0) return null;
            var misDistance = CloseOrFar? minDistances.Values.Min() : minDistances.Values.Max();
            var closestShape = minDistances.Where(c => c.Value == misDistance).Select(c => c.Key).First();
            return closestShape ;
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
