using LogoDetector.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogoDetector
{
    public class MyTemplateMatching
    {
        static MyTemplateMatching()
        {
            LoadLogoTemplates();
        }
        public static List<TemplateLogoInfo> LogoTemplates = new List<TemplateLogoInfo>();
        /// <summary>
        /// Loads a differnt sizes of the target logo
        /// </summary>
        private static void LoadLogoTemplates()
        {
            var scales = new double[] { 0.5, 0.4, 0.6, 0.7,0.8 };
            LogoTemplates.Clear();
            foreach (var item in scales)
            {
                var data = GetBitmapData(Resources.template_logo, item, c => c.R == 0 ? (byte)0 : (byte)1);
                LogoTemplates.Add(new TemplateLogoInfo(data));
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
        /// Compare the ImageData with this Template and return a number from 0-1 that describe how the image close to the template
        /// </summary>
        public static double Compare(TemplateLogoInfo Template, byte[,] ImageData, int Start_X, int Start_Y, int Threshold)
        {
            var width = Math.Min(ImageData.GetLength(0) - Start_X, Template.Data.GetLength(0));
            var height = Math.Min(ImageData.GetLength(1) - Start_Y, Template.Data.GetLength(1));

            if (width < Template.Data.GetLength(0) * 0.7) return 0;
            if (height < Template.Data.GetLength(1) * 0.7) return 0;

            int x, y;
            byte[] backgroundColors = new byte[Template.BackgroundPixelsCount];
            byte[] objectsColors = new byte[Template.ObjectPixelsCount];
            int backgrounCounter = 0, objectsCounter = 0;
            for (int i = 0; i < width; i++)
            {
                x = i + Start_X;
                for (int j = 0; j < height; j++)
                {
                    y = Start_Y + j;
                    if (Template.Data[i, j] == 0)//This is a background pixel
                    {
                        backgroundColors[backgrounCounter++] = ImageData[x, y];
                    }
                    else//This is an object pixel
                    {
                        objectsColors[objectsCounter++] = ImageData[x, y];
                    }
                }
            }
            var minObjectsPixels = (int)(objectsColors.Length * 0.65);
            var minBackgroundPixels = (int)(backgroundColors.Length * 0.8);
           

            Array.Sort(objectsColors, 0, objectsCounter);
            var compareColor = objectsColors[objectsColors.Length / 2];
            var similarObjectsCount = countCloseValues(objectsColors, compareColor, Threshold);
            if (similarObjectsCount < minObjectsPixels)
                return 0;

          //  Array.Sort(backgroundColors, 0, backgrounCounter);
            var differentBackgroundPixelsCount = countDifferentValues(backgroundColors, compareColor, minBackgroundPixels, Threshold);
            if (differentBackgroundPixelsCount < minBackgroundPixels)
                return 0;
            return (1.0 * similarObjectsCount / objectsColors.Length * 50) + (1.0 * differentBackgroundPixelsCount / backgroundColors.Length * 50);
        }
      

        /// <summary>
        /// Gets how many values not close to compareValue
        /// </summary>
        private static int countCloseValues(byte[] sortedArray, int compareValue, int threshold)
        {
            var counter = 0;
            var mid = sortedArray.Length / 2;
            for (int i = mid; i >= 0; i--)
            {
                if (compareValue-sortedArray[i]  <= threshold)
                    counter++;
                else break;
            }
            for (int i = mid+1; i < sortedArray.Length; i++)
            {
                if (sortedArray[i]-compareValue <= threshold)
                    counter++;
                else break;
            }
            return counter;
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
                if (counter >= stopCount) break;
            }
            return counter;
        }

        /// <summary>
        /// Compare the ImageData with this Template and return a number from 0-100 that describe how the image close to the template
        /// </summary>
        public static double DetectLogo(Bitmap source)
        {
            var imageData = GetBitmapData(source, 1, c => (byte)((c.R + c.G + c.B) / 3));

            double compareValue = 0;
            int i = 0, j = 0;
            TemplateLogoInfo validTemplate = null;
            foreach (var template in LogoTemplates)
            {
                var width = source.Width - template.Data.GetLength(0);
                var height = source.Height - template.Data.GetLength(1);
                for (i = 0; i < width; i += 2)
                {
                    for (j = 0; j < height; j += 2)
                    {
                        compareValue = Compare(template, imageData, i, j, 20);
                        if (compareValue < 0.5)
                            compareValue = Compare(template, imageData, i, j, 13);
                       
                        if (compareValue >= 0.5)
                            break;
                    }
                    if (compareValue >= 0.5) break;
                }
                if (compareValue >= 0.5)
                {
                    validTemplate = template;
                    break;
                }
            }

            if (compareValue >= 0.5)
                SetBitmapData(validTemplate.Data, source, i, j, c => c == 0 ? Color.Empty : Color.Red);
            return compareValue;
        }
    }

    public class TemplateLogoInfo
    {
        public byte[,] Data { get; private set; }
        /// <summary>
        /// Gets or Sets the background pixels count
        /// </summary>
        public int BackgroundPixelsCount { get; private set; }
        /// <summary>
        /// Gets or Sets the objects pixels count
        /// </summary>
        public int ObjectPixelsCount { get; private set; }
        public TemplateLogoInfo(byte[,] Data)
        {
            this.Data = Data;
            var width = Data.GetLength(0);
            var height = Data.GetLength(1);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (Data[i, j] == 0)
                        BackgroundPixelsCount++;
                    else ObjectPixelsCount++;
                }
            }
        }
    }
}
