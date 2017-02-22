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
            if (c1.A != c2.A)
                return false;
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
        public static Bitmap Crop(this Bitmap bitmap, int Width, int Height, float scale = 1)
        {
            var x = Math.Max(0, bitmap.Width - Width);
            var y = Math.Max(0, bitmap.Height - Height);
            Width = Math.Min(Width, bitmap.Width);
            Height = Math.Min(Height, bitmap.Height);
            var newBitmap = bitmap.Clone(new Rectangle(x, y, Width, Height), PixelFormat.Format24bppRgb);
            if (scale != 1)
            {
                newBitmap = new Bitmap(newBitmap, new Size((int)(Width * scale), (int)(Height * scale)));
                newBitmap = Crop(newBitmap,Width,Height);
               // newBitmap.Save("d:\\dsdsad.png");
            }
            return newBitmap;
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
            paths.RemoveAll(c => c.Count < MinPixelsCount || c.Count > MaxPixelsCount);
         //   paths.Sort((c1, c2) => c2.Count.CompareTo(c1.Count));
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
        /// Finds the edges
        /// </summary>
        /// <returns></returns>
        public static ShapeEdge CalcEdges(this List<Point> Path)
        {
            var min_x1 = (from p in Path orderby p.X ascending select p).First();
            var max_x1 = (from p in Path orderby p.X descending select p).First();
            var min_y1 = (from p in Path orderby p.Y ascending select p).First();
            var max_y1 = (from p in Path orderby p.Y descending select p).First();

            var left_top = new Point(min_x1.X,min_y1.Y);
            var left_bottom = new Point(min_x1.X, max_y1.Y);
            var right_top = new Point(max_x1.X, min_y1.Y);
            var right_bottom = new Point(max_x1.X, max_y1.Y);

            return new ShapeEdge(left_top,left_bottom,right_top,right_bottom);
        }

        /// <summary>
        /// Gets the start and end point
        /// </summary>
        /// <returns></returns>
        public static Point[] GetPointsBetween(this IEnumerable<Point> Shape1, IEnumerable<Point> Shape2, bool CloseOrFar)
        {
            var min_x1 = (from p in Shape1 orderby p.X ascending select p).First();
            var max_x1 = (from p in Shape1 orderby p.X descending select p).First();
            var min_y1 = (from p in Shape1 orderby p.Y ascending select p).First();
            var max_y1 = (from p in Shape1 orderby p.Y descending select p).First();

            var min_x2 = (from p in Shape2 orderby p.X ascending select p).First();
            var max_x2 = (from p in Shape2 orderby p.X descending select p).First();
            var min_y2 = (from p in Shape2 orderby p.Y ascending select p).First();
            var max_y2 = (from p in Shape2 orderby p.Y descending select p).First();

            var pointsList = new List<Point[]>();
            pointsList.Add(new Point[] { min_x1, min_x2 });
            pointsList.Add(new Point[] { min_x1, max_x2 });
            pointsList.Add(new Point[] { min_x1, min_y2 });
            pointsList.Add(new Point[] { min_x1, max_y2 });

            pointsList.Add(new Point[] { max_x1, min_x2 });
            pointsList.Add(new Point[] { max_x1, max_x2 });
            pointsList.Add(new Point[] { max_x1, min_y2 });
            pointsList.Add(new Point[] { max_x1, max_y2 });

            pointsList.Add(new Point[] { min_y1, min_x2 });
            pointsList.Add(new Point[] { min_y1, max_x2 });
            pointsList.Add(new Point[] { min_y1, min_y2 });
            pointsList.Add(new Point[] { min_y1, max_y2 });

            pointsList.Add(new Point[] { max_y1, min_x2 });
            pointsList.Add(new Point[] { max_y1, max_x2 });
            pointsList.Add(new Point[] { max_y1, min_y2 });
            pointsList.Add(new Point[] { max_y1, max_y2 });

          var distances=  pointsList.ConvertAll(c => c[0].DistanceTo(c[1]));
            var value = CloseOrFar ? distances.Min() : distances.Max();
            var index = distances.IndexOf(value);
            return pointsList[index];
        }

        public static double GetDistanceBetween(this IEnumerable<Point> Shape1, IEnumerable<Point> Shape2, bool CloseOrFar)
        {
            var points = GetPointsBetween(Shape1, Shape2, CloseOrFar);
            return points[0].DistanceTo(points[1]);
        }

        public static Point MidPoint(this Point p1,Point p2)
        {
            return new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
        }

        /// <summary>
        /// Find the index of the closest shape to this shape
        /// </summary>
        public static List<Point> FindShape(this List<List<Point>> AllShapes, List<Point> TargetShape, bool CloseOrFar)
        {
            var minDistances = new Dictionary<List<Point>, double>();
            foreach (var item in AllShapes)
            {
                if (item == TargetShape) continue;
                minDistances[item] = GetDistanceBetween(TargetShape, item, CloseOrFar);
            }
            if (minDistances.Count == 0) return null;
            var misDistance = CloseOrFar ? minDistances.Values.Min() : minDistances.Values.Max();
            var closestShape = minDistances.Where(c => c.Value == misDistance).Select(c => c.Key).First();
            return closestShape;
        }

        /// <summary>
        /// Find all shapes that can show up in a circle broder
        /// </summary>
        public static List<List<Point>> FindShapesInCirclesBorder(this List<List<Point>> AllShapes, int MinShapes)
        {
            var shapeEdges = new Dictionary<List<Point>,ShapeEdge>();
            foreach (var item in AllShapes)
                shapeEdges[item] = item.CalcEdges();

            AllShapes=AllShapes.FindAll(c=>
            {
                var rec = shapeEdges[c].Sides();
                return rec.Width > 1 && rec.Height > 1 &&(rec.Width/rec.Height)<4 && (rec.Height / rec.Width) < 4;
            });
            var thresholdDev = 3;
            var bestDeviation = double.MaxValue;
            var bestResult = new Dictionary<List<Point>,double>();
            AllShapes.Permutations((c1, c2) =>
            {
                if (c1 == c2) return true;
                var edge1 = shapeEdges[c1];
                var edge2 = shapeEdges[c2];
                var p1 = edge1.MidPoint();
                var p2 = edge2.MidPoint();
                
                AllShapes.ForEach(c3 =>
                {
                    if (c1 == c3 || c2 == c3) return ;
                    var shapeCenter = p1.MidPoint(p2).MidPoint(shapeEdges[c3].MidPoint());
                    var root = shapeCenter.DistanceTo(p2) ;
                    var roots = AllShapes.Select(c4 => shapeEdges[c4].MidPoint().DistanceTo(shapeCenter));

                    var deviation = roots.Select(r => Math.Abs(r - root)).ToArray();
                    var items = AllShapes.ToArray();
                    Array.Sort(deviation, items);
                    deviation = deviation.Take(5).ToArray() ;
                    var avgDev = deviation.Average();
                    if (avgDev < bestDeviation && deviation.Count(c => c < thresholdDev) >= MinShapes)
                    {
                        bestDeviation = avgDev;
                        bestResult.Clear();
                        for (int i = 0; i < deviation.Length; i++)
                        {
                            if (deviation[i] >= thresholdDev) break;
                            bestResult[items[i]] = deviation[i];
                        }
                    }
                    
                });
                return true;
            });
            AllShapes = bestResult.Keys.ToList();

            if (AllShapes.Count >= MinShapes)
            {
                var sortedShapes = new List<List<Point>>();
                var distances = new List<double>();
                sortedShapes.Add(AllShapes[0]);
                while(sortedShapes.Count< AllShapes.Count)
                {
                    var lastShape = sortedShapes.Last();
                    var remainShapes = AllShapes.Except(sortedShapes).ToList();
                    //Try to sort the shapes
                    var shapeDistances = remainShapes.ConvertAll(c2 =>
                    {
                        var p1 = shapeEdges[lastShape].MidPoint();
                        var p2 = shapeEdges[c2].MidPoint();
                        return p1.DistanceTo(p2);
                    });
                    distances.Add(shapeDistances.Min());
                    var nextShape = remainShapes[shapeDistances.IndexOf(shapeDistances.Min())];
                    sortedShapes.Add(nextShape);
                }
                distances.Add(shapeEdges[sortedShapes.Last()].MidPoint().DistanceTo(shapeEdges[sortedShapes.First()].MidPoint()));
                for (int i = 0; i < sortedShapes.Count; i++)
                {
                    var d = distances[i];
                    if (d > 5&&d<=40) continue;
                    sortedShapes.Clear();
                    break;
                }
                var dev = new List<double>();
                foreach (var item in distances)
                {
                    if (dev.Count>0 && Math.Abs(dev.First() - item) <= 10) continue;
                    dev.Add(item);
                }
                if (dev.Count > 2 || dev.Count == 0 || (distances.Count==5&&dev.Count>1 ) || ( dev.Count > 1&&dev.Max()/dev.Min()>3 ))
                    sortedShapes.Clear();
                if (sortedShapes.Count >= MinShapes)
                {
                    var rec1 = shapeEdges[sortedShapes[0]].Sides();
                    sortedShapes = sortedShapes.FindAll(c =>
                    {
                        var rec2 = shapeEdges[c].Sides();
                        var max1 = Math.Max(rec1.Width, rec1.Height);
                        var max2 = Math.Max(rec2.Width, rec2.Height);
                        return Math.Abs(max1 - max2) < 7;
                    });
                }
                AllShapes = sortedShapes;
            }
            return AllShapes;
        }
    
        static void Permutations<T>(this IEnumerable<T> source,Func<T,T,bool> fun)
        {
            var called = new List<T>();
            foreach (var item1 in source)
            {
                called.Add(item1);
                   var stop = false;
                foreach (var item2 in source)
                {
                    if (called.Contains(item2)) continue;
                    stop = !fun(item1,item2);
                    if (stop) break;
                }
                if (stop) break;
            }
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

    public struct ShapeEdge
    {
        public Point LeftTop { get; set; }
        public Point LeftBottom { get; set; }
        public Point RightTop { get; set; }
        public Point RightBottom { get; set; }

        public ShapeEdge(Point LeftTop, Point LeftBottom, Point RightTop, Point RightBottom)
        {
            this.LeftTop = LeftTop;
            this.LeftBottom = LeftBottom;
            this.RightTop = RightTop;
            this.RightBottom = RightBottom;
        }

        public double[] Qatars()
        {
            var d1 = LeftTop.DistanceTo(RightBottom);
            var d2 = LeftBottom.DistanceTo(RightTop);
            return new double[] {d1,d2 };
        }

        public RectangleF Sides()
        {
            var leftSide = LeftTop.DistanceTo(LeftBottom);
            var topSide = LeftTop.DistanceTo(RightTop);
            return new RectangleF(LeftTop, new SizeF((float)topSide, (float)leftSide));
        }

        public Point MidPoint()
        {
            var q = Qatars();
            if (q[0] >= q[1])
                return LeftTop.MidPoint(RightBottom);
            else return LeftBottom.MidPoint(RightTop);
        }
    }
}
