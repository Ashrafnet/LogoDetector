using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LogoDetector
{
    public class LockBitmap
    {
        Bitmap source = null;
        IntPtr Iptr = IntPtr.Zero;
        BitmapData bitmapData = null;

        public byte[] Pixels { get; set; }
        /// <summary>
        /// Map the indexs to it's points
        /// </summary>
        public Point[] Points { get; set; }
        public int Depth { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public LockBitmap(Bitmap source)
        {
            this.source = source;
        }

        /// <summary>
        /// Lock bitmap data
        /// </summary>
        public void LockBits()
        {
            // Get width and height of bitmap
            Width = source.Width;
            Height = source.Height;

            // get total locked pixels count
            int PixelCount = Width * Height;

            // Create rectangle to lock
            Rectangle rect = new Rectangle(0, 0, Width, Height);

            // get source bitmap pixel format size
            Depth = System.Drawing.Bitmap.GetPixelFormatSize(source.PixelFormat);

            // Lock bitmap and return bitmap data
            bitmapData = source.LockBits(rect, ImageLockMode.ReadWrite,
                                         source.PixelFormat);

            // Declare an array to hold the bytes of the bitmap.
            int bytes = Math.Abs(bitmapData.Stride) * source.Height;
            Pixels = new byte[bytes];
            Iptr = bitmapData.Scan0;

            // Copy data from pointer to array
            Marshal.Copy(Iptr, Pixels, 0, Pixels.Length);

            //Map indexs to points
            Points = new Point[Pixels.Length];
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                    Points[PixelToIndex(i, j)] = new Point(i, j);
            }
        }

        /// <summary>
        /// Unlock bitmap data and save the changes
        /// </summary>
        public void UnlockBits()
        {
            // Copy data from byte array to pointer
            Marshal.Copy(Pixels, 0, Iptr, Pixels.Length);
            // Unlock bitmap data
            source.UnlockBits(bitmapData);
        }
        /// <summary>
        /// Gets the Pixel's index
        /// </summary>
        public int PixelToIndex(int x,int y)
        {
            // Get color components count
            int cCount = Depth / 8;
            // Get start index of the specified pixel
            int i = y * bitmapData.Stride + x * cCount;

            if (i > Pixels.Length - cCount)
                throw new IndexOutOfRangeException();

            return i;
        }

        /// <summary>
        /// Get the color of the specified pixel
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Color GetPixel(int x, int y)
        {
            Color clr = Color.Empty;
            var i = PixelToIndex(x, y);
            if (Depth == 32) // For 32 bpp get Red, Green, Blue and Alpha
            {
                byte b = Pixels[i];
                byte g = Pixels[i + 1];
                byte r = Pixels[i + 2];
                byte a = Pixels[i + 3]; // a
                clr = Color.FromArgb(a, r, g, b);
            }
            else if (Depth == 24) // For 24 bpp get Red, Green and Blue
            {
                byte b = Pixels[i];
                byte g = Pixels[i + 1];
                byte r = Pixels[i + 2];
                clr = Color.FromArgb(r, g, b);
            }
            else if (Depth == 8)
            // For 8 bpp get color value (Red, Green and Blue values are the same)
            {
                byte c = Pixels[i];
                clr = Color.FromArgb(c, c, c);
            }
            return clr;
        }

        /// <summary>
        /// Set the color of the specified pixel
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        public void SetPixel(int x, int y, Color color)
        {
            // Get color components count
            var i = PixelToIndex(x, y);
            if (Depth == 32) // For 32 bpp set Red, Green, Blue and Alpha
            {
                Pixels[i] = color.B;
                Pixels[i + 1] = color.G;
                Pixels[i + 2] = color.R;
                Pixels[i + 3] = color.A;
            }
            else if (Depth == 24) // For 24 bpp set Red, Green and Blue
            {
                Pixels[i] = color.B;
                Pixels[i + 1] = color.G;
                Pixels[i + 2] = color.R;
            }
            else if (Depth == 8)
            // For 8 bpp set color value (Red, Green and Blue values are the same)
            {
                Pixels[i] = color.B;
            }
        }
        /// <summary>
        /// Gets or Sets the Color at this pixel
        /// </summary>
        public Color this[int X, int Y]
        {
            get { return GetPixel(X,Y); }
            set { SetPixel(X, Y, value); }
        }


    }
}
