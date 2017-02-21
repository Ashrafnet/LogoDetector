using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogoDetector
{
    public static class EdgeDetector
    {

        public static Bitmap Sobel(Bitmap source)
        {
            int[,] gx = new int[,] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };   //  The matrix Gx
            int[,] gy = new int[,] { { 1, 2, 1 }, { 0, 0, 0 }, { -1, -2, -1 } };  //  The matrix Gy
            Bitmap b1 = new Bitmap(source);
            for (int i = 1; i < source.Height - 1; i++)   // loop for the image pixels height
            {
                for (int j = 1; j < source.Width - 1; j++) // loop for image pixels width    
                {
                    float new_x = 0, new_y = 0;
                    float c;
                    Color color;
                    for (int hw = -1; hw < 2; hw++)  //loop for cov matrix
                    {
                        for (int wi = -1; wi < 2; wi++)
                        {
                            color = source.GetPixel(j + wi, i + hw);
                            c = (color.B + color.R + color.G) / 3;
                            new_x += gx[hw + 1, wi + 1] * c;
                            new_y += gy[hw + 1, wi + 1] * c;
                        }
                    }

                    if (new_x * new_x + new_y * new_y > 128 * 128)
                        b1.SetPixel(j, i, Color.Black);
                    else
                        b1.SetPixel(j, i, Color.White);
                }
            }
            return b1;
        }


        public static Bitmap Prewitt(Bitmap source)
        {
            int[,] gx = new int[,] { { 1, 1, 1 }, { 0, 0, 0 }, { -1, -1, -1 } };   //  The matrix Gx
            int[,] gy = new int[,] { { -1, 0, 1 }, { -1, 0, 1 }, { -1, 0, 1 } };  //  The matrix Gy
            Bitmap b1 = new Bitmap(source.Width, source.Height);
            for (int i = 1; i < source.Height - 1; i++)   // loop for the image pixels height
            {
                for (int j = 1; j < source.Width - 1; j++) // loop for image pixels width    
                {
                    float new_x = 0, new_y = 0;
                    float c;
                    Color color;
                    for (int hw = -1; hw < 2; hw++)  //loop for cov matrix
                    {
                        for (int wi = -1; wi < 2; wi++)
                        {
                            color = source.GetPixel(j + wi, i + hw);
                            c = (color.B + color.R + color.G) / 3;

                            new_x += gx[hw + 1, wi + 1] * c;
                            new_y += gy[hw + 1, wi + 1] * c;
                        }
                    }
                    if (new_x * new_x + new_y * new_y > 128 * 100)
                        b1.SetPixel(j, i, Color.Black);
                    else
                        b1.SetPixel(j, i, Color.White);
                }
            }
            return b1;
        }

        public static Bitmap ProposedEdgeDetection(Bitmap source, int _proposedThresoldValue = 90)
        {
            var sourceData = new LockBitmap(source);
            var target = new Bitmap(source.Width, source.Height);
            var targetData = new LockBitmap(target);
            sourceData.LockBits();
            targetData.LockBits();
            try
            {
                int total = 0;
                Color color1, color2;
                for (int i = 0; i < sourceData.Width; i++)
                {
                    for (int j = 0; j < sourceData.Height; j++)
                        targetData.SetPixel(i, j, Color.White);
                }
                for (int i = 1; i < sourceData.Width - 1; i++)
                    for (int j = 1; j < sourceData.Height - 1; j++)
                    {
                        color1 = sourceData.GetPixel(i, j);
                        color2 = sourceData.GetPixel(i + 1, j + 1);
                        total = Math.Abs(color2.B - color1.B) + Math.Abs(color2.R - color1.R) + Math.Abs(color2.G - color1.G);
                        if (total > _proposedThresoldValue)
                        { targetData.SetPixel(i, j, Color.Black); continue; }

                        color2 = sourceData.GetPixel(i, j + 1);
                        total = Math.Abs(color2.B - color1.B) + Math.Abs(color2.R - color1.R) + Math.Abs(color2.G - color1.G);
                        if (total > _proposedThresoldValue)
                        { targetData.SetPixel(i, j, Color.Black); continue; }

                        color2 = sourceData.GetPixel(i + 1, j);
                        total = Math.Abs(color2.B - color1.B) + Math.Abs(color2.R - color1.R) + Math.Abs(color2.G - color1.G);
                        if (total > _proposedThresoldValue)
                        { targetData.SetPixel(i, j, Color.Black); continue; }

                        color2 = sourceData.GetPixel(i - 1, j - 1);
                        total = Math.Abs(color2.B - color1.B) + Math.Abs(color2.R - color1.R) + Math.Abs(color2.G - color1.G);
                        if (total > _proposedThresoldValue)
                        { targetData.SetPixel(i, j, Color.Black); continue; }


                        color2 = sourceData.GetPixel(i, j - 1);
                        total = Math.Abs(color2.B - color1.B) + Math.Abs(color2.R - color1.R) + Math.Abs(color2.G - color1.G);
                        if (total > _proposedThresoldValue)
                        { targetData.SetPixel(i, j, Color.Black); continue; }

                        color2 = sourceData.GetPixel(i - 1, j);
                        total = Math.Abs(color2.B - color1.B) + Math.Abs(color2.R - color1.R) + Math.Abs(color2.G - color1.G);
                        if (total > _proposedThresoldValue)
                        { targetData.SetPixel(i, j, Color.Black); continue; }

                        color2 = sourceData.GetPixel(i - 1, j + 1);
                        total = Math.Abs(color2.B - color1.B) + Math.Abs(color2.R - color1.R) + Math.Abs(color2.G - color1.G);
                        if (total > _proposedThresoldValue)
                        { targetData.SetPixel(i, j, Color.Black); continue; }

                        color2 = sourceData.GetPixel(i + 1, j - 1);
                        total = Math.Abs(color2.B - color1.B) + Math.Abs(color2.R - color1.R) + Math.Abs(color2.G - color1.G);
                        if (total > _proposedThresoldValue)
                        { targetData.SetPixel(i, j, Color.Black); continue; }

                    }
                return target;
            }
            finally
            {
                sourceData.UnlockBits();
                targetData.UnlockBits();
            }
        }


        public static Bitmap Compare(Bitmap b1,Bitmap b2)
        {
            Bitmap result = new Bitmap(b1.Width, b1.Height);
            Color color1,color2;
            for (int i = 1; i < b1.Width - 1; i++)
            {
                for (int j = 1; j < b1.Height - 1; j++)
                {
                    color1 = b1.GetPixel(i, j);
                    color2 = b2.GetPixel(i, j);
                    if (color1.R == 0 && color2.R == 0)
                        result.SetPixel(i, j, Color.Black);
                    else if (color1.R == 0)
                        result.SetPixel(i, j, Color.Black);
                    else if (color2.R == 0)
                        result.SetPixel(i, j, Color.Black);
                }
            }
            return result;
        }

    }



}
