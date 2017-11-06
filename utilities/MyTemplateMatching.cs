using LogoDetector.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogoDetector
{
    public class MyTemplateMatching
    {
        public static List<TemplateLogoInfo> LogoTemplates = new List<TemplateLogoInfo>();

        static MyTemplateMatching()
        {
            LoadLogoTemplates();
        }
        /// <summary>
        /// Loads a differnt sizes of the target logo
        /// </summary>
        private static void LoadLogoTemplates()
        {
            LogoTemplates.Clear();
            var scales = new double[] { 28, 32, 24, 36, 42 }.Select(c=> 1.0 / (56.0 / c));
            foreach (var item in scales)
            {
                var data = GetBitmapData(Resources.template_logo, item, c => c.R == 0 ? (byte)0 : (byte)1);
                LogoTemplates.Add(new TemplateLogoInfo(data));
                data = GetBitmapData(Resources.logo_costar_40, item, c => c.R == 0 ? (byte)0 : (byte)1);
                LogoTemplates.Add(new TemplateLogoInfo(data));

                //data = GetBitmapData(Resources.logo_costar_word, item, c => c.R == 0 ? (byte)0 : (byte)1);
                //LogoTemplates.Add(new TemplateLogoInfo(data));
            }
        }
        /// <summary>
        /// Convert the bitmap pixels into two dimensional array
        /// </summary>
        public static byte[,] GetBitmapData(Bitmap Source, double Scale, Func<Color, byte> ColorConverter)
        {
            var width = (int)(Source.Width * Scale);
            var height = (int)(Source.Height * Scale);
            byte[,] result = new byte[width, height];
            int x, y;
            for (int i = 0; i < width; i++)
            {
                x = (int)(i / Scale) % Source.Width;
                for (int j = 0; j < height; j++)
                {
                    y = (int)(j / Scale) % Source.Height;
                    result[i, j] = ColorConverter(Source.GetPixel(x, y));
                }
            }
            return result;
        }
        /// <summary>
        /// Sets the bitmap pixels with the data from the two dimensional array
        /// </summary>
        public static void SetBitmapData(byte[,] Data,Bitmap Target, int Start_X, int Start_Y, Func<byte, Color> ColorConverter)
        {
            var width = Math.Min(Target.Width - Start_X, Data.GetLength(0));
            var height = Math.Min(Target.Height - Start_Y, Data.GetLength(1));
            int x, y;
            Color color;
            for (int i = 0; i < width; i++)
            {
                x = i + Start_X;
                for (int j = 0; j < height; j++)
                {
                    y = Start_Y + j;
                    color = ColorConverter(Data[i, j]);
                    if (!color.IsEmpty) Target.SetPixel(x, y, color);
                }
            }
        }


        /// <summary>
        /// Compare the ImageData with this Template and return a number from 0-100 that describe how the image close to the template
        /// </summary>
        public static double Compare(TemplateLogoInfo Template, byte[,] ImageData, int Start_X, int Start_Y, int Threshold)
        {
            var width = Math.Min(ImageData.GetLength(0) - Start_X, Template.Width);
            var height = Math.Min(ImageData.GetLength(1) - Start_Y, Template.Height);

            if (width < Template.Width * 0.7) return 0;
            if (height < Template.Height * 0.7) return 0;

            byte[] backgroundColors = new byte[Template.BackgroundPixels.Count];
            byte[] objectsColors = new byte[Template.ObjectsPixels.Count];
            int backgrounCounter = 0, objectsCounter = 0;

            foreach (var item in Template.BackgroundPixels)
                backgroundColors[backgrounCounter++] = ImageData[item.X + Start_X, item.Y + Start_Y];
            foreach (var item in Template.ObjectsPixels)
                objectsColors[objectsCounter++] = ImageData[item.X + Start_X, item.Y + Start_Y];

            var minObjectsPixels = (int)(objectsColors.Length * 0.55);
            var minBackgroundPixels = (int)(backgroundColors.Length * 0.8);


            Array.Sort(objectsColors, 0, objectsCounter);
            var compareColor = objectsColors[objectsColors.Length / 2];

            var similarObjectsCount = countCloseValues(objectsColors, compareColor, Threshold);
            var differentBackgroundPixelsCount = countDifferentValues(backgroundColors, compareColor, minBackgroundPixels, Threshold+3);

            var failRatio = Math.Min(50.0 * similarObjectsCount / minObjectsPixels, 50.0 * differentBackgroundPixelsCount / minBackgroundPixels);
            if (failRatio > 40)
            { }
            if (failRatio < 50)
                return failRatio;
            
            var vaildRatio = (65.0 * similarObjectsCount / objectsColors.Length) + (35.0 * differentBackgroundPixelsCount / backgroundColors.Length);
            return vaildRatio;
        }
      

        /// <summary>
        /// Gets how many values not close to compareValue
        /// </summary>
        private static int countCloseValues(byte[] sortedArray, byte compareValue, int threshold)
        {
            var counter = 0;
            var mid = sortedArray.Length / 2;
            var shift = 1;
            for (int i = mid; i >= 0; i-= shift)
            {
                if (compareValue-sortedArray[i]  <= threshold)
                    counter++;
                else break;
            }
            for (int i = mid+1; i < sortedArray.Length; i+= shift)
            {
                if (sortedArray[i]-compareValue <= threshold)
                    counter++;
                else break;
            }
            return counter * shift;
        }

        /// <summary>
        /// Gets how many values not close to compareValue
        /// </summary>
        private static int countDifferentValues(byte[] array, int compareValue,int stopCount, int threshold)
        {
            var counter = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if (Math.Abs(array[i] - compareValue) > threshold)
                    counter++;
                //if (counter >= stopCount) break;
            }
            return counter;
        }
       public static byte RGBtoYUV(Color rgb)
        {
            double y = rgb.R * .299000 + rgb.G * .587000 + rgb.B * .114000;
            //double u = rgb.R * -.168736 + rgb.G * -.331264 + rgb.B * .500000 + 128;
            //double v = rgb.R * .500000 + rgb.G * -.418688 + rgb.B * -.081312 + 128;

            //return new double[] { y, u, v };
            return (byte)y;
        }
        /// <summary>
        /// Compare the ImageData with this Template and return a number from 0-100 that describe how the image close to the template
        /// </summary>
        public static double DetectLogo(Bitmap source)
        {
            var imageData = GetBitmapData(source, 1, RGBtoYUV);

            TemplateLogoInfo bestTemplate = null;
            int bestTemplate_x=0, bestTemplate_y=0;
            double bestCompareValue = 0,temp;
            foreach (var template in LogoTemplates)
            {
                var width = source.Width - template.Width;
                var height = source.Height - template.Height;
                for (int i = 0; i < width; i += 2)
                {
                    temp = 0;
                    for (int j = 0; j < height; j += 2)
                    {
                        var compare = Compare(template, imageData, i, j, 20);
                        if (compare < 50)
                            compare = Math.Max(compare, Compare(template, imageData, i, j, 13));
                        if (compare > temp) temp = compare;
                        if (compare > bestCompareValue)
                        {
                            bestCompareValue = compare;
                            bestTemplate = template;
                            bestTemplate_x = i;
                            bestTemplate_y = j;
                        }
                        if (bestCompareValue >= 50)
                            break;
                        if (compare < 25) j+=2;
                    }
                    if (bestCompareValue >= 50) break;
                    if (temp < 25) i += 2;
                }
               
                if (bestCompareValue >= 50)
                    break;
            }
            // 
            if (bestTemplate != null && bestCompareValue >= 50)
            {
                var data = bestTemplate.GetTemplateData(0,1,0);
                SetBitmapData(data, source, bestTemplate_x, bestTemplate_y, c =>c==0?Color.Empty: Color.Red);
            }
            return bestCompareValue;
        }
    }

    public class TemplateLogoInfo
    {
        //public byte[,] Data { get; private set; }

        /// <summary>
        /// Gets or Sets the background pixels 
        /// </summary>
        public List<Point> BackgroundPixels { get; private set; }
        /// <summary>
        /// Gets or Sets the objects pixels 
        /// </summary>
        public List<Point> ObjectsPixels { get; private set; }
        /// <summary>
        /// Gets or Sets the pixels between backgound and objects
        /// </summary>
        public List<Point> BorderPixels { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public TemplateLogoInfo(byte[,] Data)
        {
            //this.Data = Data;
            Width = Data.GetLength(0);
            Height = Data.GetLength(1);
            BackgroundPixels = new List<Point>();
            ObjectsPixels = new List<Point>();
            BorderPixels = new List<Point>();
            Point p;
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    p = new Point(i,j);
                    if (i > 0 && Data[i, j]==1&& Data[i - 1, j]==0)
                        BorderPixels.Add(p);

                    else if (Data[i, j] == 0)
                        BackgroundPixels.Add(p);
                    else ObjectsPixels.Add(p);
                }
            }
        }
        /// <summary>
        /// Gets the two dimentional array that represent the template data
        /// </summary>
        /// <returns></returns>
        public byte[,] GetTemplateData(byte Background,byte Objects,byte Border)
        {
            var data = new byte[Width, Height];

            foreach (var item in BackgroundPixels)
                data[item.X, item.Y] = Background;
            foreach (var item in ObjectsPixels)
                data[item.X, item.Y] = Objects;
            foreach (var item in BorderPixels)
                data[item.X, item.Y] = Border;
            return data;
        } 
    }

  public  enum LogoType
    {
        MainLogo,
        CostarLogo
    }

}
