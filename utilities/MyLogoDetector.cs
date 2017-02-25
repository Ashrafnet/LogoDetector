//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using AForge.Imaging.Filters;
//using AForge.Imaging;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using AForge;
//using System.Drawing.Imaging;
//using System.IO;
//using AForge.Math.Geometry;

//namespace LogoDetector
//{
//    public static class MyLogoDetector
//    {
//        static Color[] myPins = new Color[] { Color.Red, Color.Green, Color.Blue, Color.DarkGreen, Color.Brown, Color.Gold, Color.Black };
//        public static KeyValuePair<Bitmap,int> Process(Bitmap bitmap,byte grayThreshold,byte minShapes)
//        {
//            bitmap = Convert(bitmap);
//            var thresholdFilter = new Threshold() { ThresholdValue = grayThreshold };
//            var binaryImage = thresholdFilter.Apply(bitmap);
//            binaryImage = new ConnectedComponentsLabeling().Apply(binaryImage);
//            var blobCounter = new BlobCounter(binaryImage);
//            var shapes = blobCounter.GetObjectsInformation();
//            SimpleShapeChecker shapeChecker = new SimpleShapeChecker();
//            List<Blob> myShapes = new List<Blob>();
//            foreach (var item in shapes)
//            {
//               // item.Image.Save("d:\\shapes\\" + item.ID + ".png");
//                if (!isPartOfLogo(blobCounter,item)) continue;
//                myShapes.Add(item);
//            }
//            bool logoDetected = false;
//            List<Blob> foundShapes=new List<Blob>();
//            Permutations(myShapes, (s1, s2) => {
//                var rec1 = s1.Rectangle;
//                var rec2 = s2.Rectangle;
//                //Rectangles should be close
//                if (!isSimilarRectangles(rec1, rec2,4)) return true;

//                var p1 = s1.CenterOfGravity;
//                var p2 = s2.CenterOfGravity;
//                var mid = MidPoint(p1, p2);
//                foreach (var s in myShapes)
//                {
//                    if (s == s1 || s == s2 || !isSimilarRectangles(rec1, s.Rectangle, 25) || !isSimilarRectangles(rec2, s.Rectangle, 25)) continue;
//                    var center = MidPoint(mid, s.CenterOfGravity);
//                    var circleRoot = center.DistanceTo(p1);
//                    foundShapes= myShapes.Where(s3 =>isSimilarRectangles(rec1, s3.Rectangle, 25)&& Math.Abs(circleRoot - s3.CenterOfGravity.DistanceTo(center)) < 4).ToList();
//                    logoDetected = foundShapes.Count >= minShapes && foundShapes.Contains(s1) && foundShapes.Contains(s2) && foundShapes.Contains(s);
//                    if (logoDetected) break;
//                }
//                return !logoDetected;
//            });
//            Bitmap outputBitmap = binaryImage;
//            if (false&&logoDetected)
//            {
//                var foundRectangles = foundShapes.ConvertAll(c => c.Rectangle);
//                // foundShapes.ForEach(s => isPartOfLogo(blobCounter, s));

//                outputBitmap = new Bitmap(bitmap.Width, bitmap.Height);
//                for (int i = 0; i < foundShapes.Count; i++)
//                {
//                    var color = myPins[i % myPins.Length];
//                    blobCounter.GetBlobsEdgePoints(foundShapes[i]).ForEach(p => outputBitmap.SetPixel(p.X, p.Y, color));
//                }
//            }
           
           
//            var conf = !logoDetected ? 0 : 100 - Math.Abs(foundShapes.Count - 5) * 20;
//            return new KeyValuePair<Bitmap, int>(outputBitmap, conf);
//        }

//        static bool isSimilarRectangles(Rectangle rec1,Rectangle rec2,int distanceThreshold)
//        {
//            var max1 = Math.Max(rec1.Width, rec1.Height);
//            var max2 = Math.Max(rec2.Width, rec2.Height);
//            if (Math.Abs(max1 - max2) > 7) return false;

//            var min1 = Math.Min(rec1.Width, rec1.Height);
//            var min2 = Math.Min(rec2.Width, rec2.Height);
//            if (Math.Abs(min1 - min2) > 5) return false;

//            var p1 = new IntPoint(rec1.Location.X, rec1.Location.Y);
//            var s1 = rec1.Size;
//            var edges1 = new IntPoint[] {p1, new IntPoint(p1.X + s1.Width, p1.Y), new IntPoint(p1.X + s1.Width, p1.Y+s1.Height), new IntPoint(p1.X, p1.Y+s1.Height) };

//            var p2 = new IntPoint(rec2.Location.X, rec2.Location.Y);
//            var s2 = rec2.Size;
//            var edges2 = new IntPoint[] { p2, new IntPoint(p2.X + s2.Width, p2.Y), new IntPoint(p2.X + s2.Width, p2.Y + s2.Height), new IntPoint(p2.X, p1.Y + s2.Height) };

//            var minDistance = edges1.Min(c1 => edges2.Min(c2 => c1.DistanceTo(c2)));
//            if (minDistance >= distanceThreshold)
//                return false;
//            return true;
//        }

//        static bool isPartOfLogo(this BlobCounter blobCounter, Blob shape)
//        {
//            List<IntPoint> left, right,top,bottom;
//            blobCounter.GetBlobsLeftAndRightEdges(shape, out left, out right);

//            blobCounter.GetBlobsTopAndBottomEdges(shape, out top, out bottom);
//            //Check the distance between left and right edges
//            if (!checkIfEgdesParallel(left, right) || !checkIfEgdesParallel(top, bottom))
//                return false;

//            //Check the length of edges
//            var distances = new double[] { pathDistance(left), pathDistance(right), pathDistance(top), pathDistance(bottom) };
//            Array.Sort(distances);
//            if (distances[0] < 5) return false;
//            if (distances[3] > 25) return false;

//            if (distances[1] - distances[0] > 5) return false;
//            if (distances[3] - distances[2] > 5) return false;

//            return true;
//        }
//        static bool checkIfEgdesParallel(List<IntPoint> edges1, List<IntPoint>  edges2)
//        {
//            if (edges1.Count == 0 || edges2.Count == 0) return false;
//            var counter = 0;
//            for (int i = 0; i < edges1.Count; i++)
//            {
//                if (i >= edges2.Count) break;
//                if (edges1[i].DistanceTo(edges2[i]) > 3)
//                    counter++;
//            }
//            return counter > 0;
//        }
//        static double pathDistance(List<IntPoint> points)
//        {
//            double distance = 0;
//            Permutations(points, (p1, p2) => {
//                distance = Math.Max(distance, p1.DistanceTo(p2));
//                return true;
//            });
//            return distance;
//        }
//        static Bitmap Convert(Bitmap oldbmp)
//        {
//            using (var ms = new MemoryStream())
//            {
//                oldbmp.Save(ms, ImageFormat.Gif);
//                ms.Position = 0;
//                return (Bitmap)Bitmap.FromStream(ms);
//            }
//        }

//        static void Permutations<T>(this IEnumerable<T> source, Func<T, T, bool> fun)
//        {
//            var called = new List<T>();
//            foreach (var item1 in source)
//            {
//                called.Add(item1);
//                var stop = false;
//                foreach (var item2 in source)
//                {
//                    if (called.Contains(item2)) continue;
//                    stop = !fun(item1, item2);
//                    if (stop) break;
//                }
//                if (stop) break;
//            }
//        }

//        static IntPoint MidPoint(IntPoint p1, IntPoint p2)
//        {
//            return new IntPoint((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
//        }
//    }
//}
