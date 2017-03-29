using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;

//using XnaColor = Microsoft.Xna.Framework.Graphics.Color;
//using XnaRectangle = Microsoft.Xna.Framework.Rectangle;
using SysColor = System.Drawing.Color;
using SysRectangle = System.Drawing.Rectangle;

namespace LogoDetector
{
    /// <summary>
    /// This class provides a managed C# abstraction for Portable Bit Map (.pbm), Portable Grey Map (.pgm) and 
    /// Portable Pixel Map (.ppm)images.  The class is able to handle P1 (ASCII .pbm), P2 (ASCII .pgm) P3 (ASCII .ppm),
    /// P5 (binary .pgm), and P6 (binary .ppm) files and streams. P4 (binary .pbm) is not supported.  The class can 
    /// process normal images, which have widths that are multiples of 4 pixels wide, and can also handle "off-size"
    /// images using a less efficient algorithm.  It presents .pbm and .pgm images using the 8bppIndexed pixel format,
    /// and presents .ppm images using the 24bppRGB pixel format.  It can convert these images into a visually pleasing
    /// greyscale bitmap image (in a less efficient 24bppRGB pixel format), which is accessible using the GreyMap 
    /// property.
    /// 
    /// To use this class:
    /// 1. Copy the PixelMap.cs file into your own project.
    /// 2. Change the namespace of the class (above this summary) from "PixelMap" to the namespace of your project.
    /// 3. Compile.
    /// 
    /// This class is used by the PixelMapViewer application.
    /// </summary>
    public class PixelMap
    {
        private PixelMapHeader header;
        /// <summary>
        /// The header portion of the PixelMap.
        /// </summary>
        public PixelMapHeader Header
        {
            get { return header; }
            set { header = value; }
        }

        private byte[] imageData;
        /// <summary>
        /// The data portion of the PixelMap.
        /// </summary>
        public byte[] ImageData
        {
            get { return imageData; }
            set { imageData = value; }
        }

        private PixelFormat pixelFormat;
        /// <summary>
        /// The pixel format used by the BitMap generated by this PixelMap.
        /// </summary>
        public PixelFormat PixelFormat
        {
            get { return pixelFormat; }
        }

        private int bytesPerPixel;
        /// <summary>
        /// The number of bytes per pixel.
        /// </summary>
        public int BytesPerPixel
        {
            get { return bytesPerPixel; }
        }

        private int stride;
        /// <summary>
        /// The stride of the scan across the image.  Typically this is width * bytesPerPixel, and is a multiple of 4.
        /// </summary>
        public int Stride
        {
            get { return stride; }
            set { stride = value; }
        }

        private Bitmap bitmap;
        /// <summary>
        /// The Bitmap created from the PixelMap.
        /// </summary>
        public Bitmap BitMap
        {
            get { return bitmap; }
        }

        /// <summary>
        /// A 24bpp Grey map (really only 8bpp of information, since R,G,B bytes are identical) created from the PixelMap.
        /// </summary>
        public Bitmap GreyMap
        {
            get { return CreateGreyMap(); }
        }

        /// <summary>
        /// File-based constructor.
        /// </summary>
        /// <param name="filename">The name of the .pbm, .pbm, or .ppm file.</param>
        public PixelMap(string filename)
        {
            if (File.Exists(filename))
            {
                Stream  stream = new MemoryStream(File.ReadAllBytes(filename));// new FileStream(filename, FileMode.Open);
                
                this.FromStream(stream);
                stream.Close();
            }
            else
            {
                throw new FileNotFoundException("The file " + filename + " does not exist", filename);
            }
        }

        /// <summary>
        /// Stream-based constructor. Typically, the stream will be a FileStream, but it may also be a MemoryStream
        /// or other object derived from the Stream class.
        /// </summary>
        /// <param name="stream">A stream containing the header and data portions of the .pbm, .pgm, or .ppm image.</param>
        public PixelMap(Stream stream)
        {
            this.FromStream(stream);
        }

        private void FromStream(Stream stream)
        {
            int index;
            this.header = new PixelMapHeader();
            int headerItemCount = 0;
            BinaryReader binReader = new BinaryReader(stream);
            try
            {
                //1. Read the Header.
                while (headerItemCount < 4)
                {
                    char nextChar = (char)binReader.PeekChar();
                    if (nextChar == '#')    // comment
                    {
                        while (binReader.ReadChar() != '\n') ;  // ignore the rest of the line.
                    }
                    else if (Char.IsWhiteSpace(nextChar))   // whitespace
                    {
                        binReader.ReadChar();   // ignore whitespace
                    }
                    else
                    {
                        switch (headerItemCount)
                        {
                            case 0: // next item is Magic Number
                                // Read the first 2 characters and determine the type of pixelmap.
                                char[] chars = binReader.ReadChars(2);
                                this.header.MagicNumber = chars[0].ToString() + chars[1].ToString();
                                headerItemCount++;
                                break;
                            case 1: // next item is the width.
                                this.header.Width = ReadValue(binReader);
                                headerItemCount++;
                                break;
                            case 2: // next item is the height.
                                this.header.Height = ReadValue(binReader);
                                headerItemCount++;
                                break;
                            case 3: // next item is the depth.
                                if (this.header.MagicNumber == "P1" | this.header.MagicNumber == "P4")
                                {
                                    // no depth value for PBM type.
                                    headerItemCount++;
                                }
                                else
                                {
                                    this.header.Depth = ReadValue(binReader);
                                    headerItemCount++;
                                }
                                break;
                            default:
                                throw new Exception("Error parsing the file header.");
                        }
                    }
                }

                // 2. Read the image data.
                // 2.1 Size the imageData array to hold the image bytes.
                switch (this.header.MagicNumber)
                {
                    case "P1": // 1 byte per pixel
                        this.pixelFormat = PixelFormat.Format8bppIndexed;
                        this.bytesPerPixel = 1;
                        break;
                    case "P2": // 1 byte per pixel
                        this.pixelFormat = PixelFormat.Format8bppIndexed;
                        this.bytesPerPixel = 1;
                        break;
                    case "P3": // 3 bytes per pixel
                        this.pixelFormat = PixelFormat.Format24bppRgb;
                        this.bytesPerPixel = 3;
                        break;
                    case "P4":
                        throw new Exception("Binary .pbm (Magic Number P4) is not supported at this time.");
                    case "P5":  // 1 byte per pixel
                        this.pixelFormat = PixelFormat.Format8bppIndexed;
                        this.bytesPerPixel = 1;
                        break;
                    case "P6":  // 3 bytes per pixel
                        this.pixelFormat = PixelFormat.Format24bppRgb;
                        this.bytesPerPixel = 3;
                        break;
                    default:
                        throw new Exception("Unknown Magic Number: " + this.header.MagicNumber);
                }
                this.imageData = new byte[this.header.Width * this.header.Height * this.bytesPerPixel];
                this.stride = this.header.Width * this.bytesPerPixel;
                //while ((this.stride % 4) != 0)
                //{
                //    this.stride++;
                //}
                if (this.header.MagicNumber == "P1" | this.header.MagicNumber == "P2" | this.header.MagicNumber == "P3") // ASCII Encoding
                {
                    int charsLeft = (int)(binReader.BaseStream.Length - binReader.BaseStream.Position);
                    char[] charData = binReader.ReadChars(charsLeft);   // read all the data into an array in one go, for efficiency.
                    string valueString = string.Empty;
                    index = 0;
                    for (int i = 0; i < charData.Length; i++)
                    {
                        if (Char.IsWhiteSpace(charData[i])) // value is ignored if empty, or converted to byte and added to array otherwise.
                        {
                            if (valueString != string.Empty)
                            {
                                this.imageData[index] = (byte)int.Parse(valueString);
                                valueString = string.Empty;
                                index++;
                            }
                        }
                        else // add the character to the end of the valueString.
                        {
                            valueString += charData[i];
                        }
                    }
                }
                else // binary encoding.
                {
                    int bytesLeft = (int)(binReader.BaseStream.Length - binReader.BaseStream.Position);
                    this.imageData = binReader.ReadBytes(bytesLeft);
                }

                // 3. We need to change the byte order of this.imageData from BGR to RGB.
                ReorderBGRtoRGB();

                // 4. Create the BitMap
                if (this.stride % 4 == 0)
                {
                    this.bitmap = CreateBitMap();
                }
                else
                {
                    this.bitmap = CreateBitmapOffSize();
                }

                // 5. Rotate the BitMap by 180 degrees so it is in the same orientation as the original image.
                this.bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
            }

            // If the end of the stream is reached before reading all of the expected values raise an exception.
            catch (EndOfStreamException e)
            {
                Console.WriteLine(e.Message);
                throw new Exception("Error reading the stream! ", e);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("Error reading the stream! ", ex);
            }
            finally
            {
               binReader.Close();
            }
        }

        /// <summary>
        /// As it stands, the native byte order in .pbm, .pgm, and .ppm images is BGR.  We need to reverse the order
        /// into RGB as this is what is needed for 24bppRGB  bitmap pixel format on Windows.
        /// </summary>
        private void ReorderBGRtoRGB()
        {
            byte[] tempData = new byte[this.imageData.Length];
            for (int i = 0; i < this.imageData.Length; i++)
            {
                tempData[i] = this.imageData[this.imageData.Length - 1 - i];
            }
            this.imageData = tempData;
        }

        private int ReadValue(BinaryReader binReader)
        {
            string value = string.Empty;
            while (!Char.IsWhiteSpace((char)binReader.PeekChar()))
            {
                value += binReader.ReadChar().ToString();
            }
            binReader.ReadByte();   // get rid of the whitespace.
            return int.Parse(value);
        }
        
        private Bitmap CreateBitMap()
        {
            IntPtr pImageData = Marshal.AllocHGlobal(this.imageData.Length);
            Marshal.Copy(this.imageData, 0, pImageData, this.imageData.Length);
            Bitmap bitmap = new Bitmap(this.header.Width, this.header.Height, this.stride, this.pixelFormat, pImageData);
            return bitmap;
        }

        private Bitmap CreateGreyMap()
        {
            byte[] greyData = new byte[this.header.Width * this.header.Height * 3];
            int stride = this.header.Width * 3;
            //while (stride % 4 != 0)
            //{
            //    stride++;
            //}
            if (stride % 4 != 0)
            {
                return CreateGreyMapOffSize(greyData);
            }
            byte grey;
            switch (this.pixelFormat)
            {
                case PixelFormat.Format24bppRgb:
                    int red, green, blue;
                    for (int i = 0; i < this.imageData.Length; i = i + 3)
                    {
                        red = (int)this.imageData[i];
                        green = (int)this.imageData[i + 1];
                        blue = (int)this.imageData[i + 2];
                        grey = (byte)((red + green + blue) / 3);
                        greyData[i] = grey;
                        greyData[i + 1] = grey;
                        greyData[i + 2] = grey;
                    }
                    break;
                case PixelFormat.Format8bppIndexed:
                    int index = 0;
                    for (int i = 0; i < this.imageData.Length; i++)
                    {
                        index = 3 * i;
                        grey = this.imageData[i];
                        if (this.header.MagicNumber == "P1")
                        {
                            if ((int)grey == 1)
                            {
                                grey = (byte)255;
                            }
                        }
                        greyData[index] = grey;
                        greyData[index + 1] = grey;
                        greyData[index + 2] = grey;
                    }
                    break;
                default:
                    throw new Exception("Unsupported Pixel Format: " + this.pixelFormat.ToString());
            }
            IntPtr pGreyData = Marshal.AllocHGlobal(greyData.Length);
            Marshal.Copy(greyData, 0, pGreyData, greyData.Length);
            Bitmap bitmap = new Bitmap(this.header.Width, this.header.Height, stride, PixelFormat.Format24bppRgb, pGreyData);
            bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
            return bitmap;
        }

        /// <summary>
        /// This method is able to handle the process of creating Bitmaps that are not sized in widths that are multiples
        /// of 4 pixels (which is needed for the rapid drawing using the Marshal class.)  Unfortunately, this is a very slow
        /// method, since it uses SetPixel().
        /// </summary>
        /// <returns></returns>
        private Bitmap CreateBitmapOffSize()
        {
            Bitmap bitmap = new Bitmap(this.header.Width, this.header.Height, PixelFormat.Format24bppRgb);
            SysColor sysColor = new SysColor();
            int red, green, blue, grey;
            int index;

            for (int x = 0; x < this.header.Width; x++)
            {
                for (int y = 0; y < this.header.Height; y++)
                {
                    index = x + y * this.header.Width;

                    switch (this.header.MagicNumber)
                    {
                        case "P1":
                            grey = (int)this.imageData[index];
                            if (grey == 1)
                            {
                                grey = 255;
                            }
                            sysColor = SysColor.FromArgb(grey, grey, grey);
                            break;
                        case "P2":
                            grey = (int)this.imageData[index];
                            sysColor = SysColor.FromArgb(grey, grey, grey);
                            break;
                        case "P3":
                            index = 3 * index;
                            blue = (int)this.imageData[index];
                            green = (int)this.imageData[index + 1];
                            red = (int)this.imageData[index + 2];
                            sysColor = SysColor.FromArgb(red, green, blue);
                            break;
                        case "P4":
                            break;
                        case "P5":
                            grey = (int)this.imageData[index];
                            sysColor = SysColor.FromArgb(grey, grey, grey);
                            break;
                        case "P6":
                            index = 3 * index;
                            blue = (int)this.imageData[index];
                            green = (int)this.imageData[index + 1];
                            red = (int)this.imageData[index + 2];
                            sysColor = SysColor.FromArgb(red, green, blue);
                            break;
                        default:
                            break;
                    }
                    bitmap.SetPixel(x, y, sysColor);
                }
            }
            return bitmap;
        }
        
        private Bitmap CreateGreyMapOffSize(byte[] greyData)
        {        
            Bitmap bitmap = new Bitmap(this.header.Width, this.header.Height, PixelFormat.Format24bppRgb);
            SysColor sysColor = new SysColor();
            int index = 0;
            int x = 0;
            int y = -1;
            switch (this.pixelFormat)
            {
                case PixelFormat.Format24bppRgb:
                    int red, green, blue, grey;
                    for (int i = 0; i < this.imageData.Length; i = i + 3)
                    {
                        index = i / 3;  // the current pixel
                        if (index%this.header.Width == 0) y++;   // the current row = y;
                        x = index - y * this.header.Width; // the current column = x;
                        red = (int)this.imageData[i];
                        green = (int)this.imageData[i + 1];
                        blue = (int)this.imageData[i + 2];
                        grey = (byte)((red + green + blue) / 3);
                        sysColor = SysColor.FromArgb(grey, grey, grey);
                        bitmap.SetPixel(x, y, sysColor);
                    }
                    break;
                case PixelFormat.Format8bppIndexed:
                    for (int i = 0; i < this.imageData.Length; i++)
                    {
                        index = i;  // the current pixel
                        if (index % this.header.Width == 0) y++;   // the current row = y;
                        x = index - y * this.header.Width; // the current column = x;

                        grey = this.imageData[i];
                        if (this.header.MagicNumber == "P1")
                        {
                            if ((int)grey == 1)
                            {
                                grey = (byte)255;
                            }
                        }
                        sysColor = SysColor.FromArgb(grey, grey, grey);
                        bitmap.SetPixel(x, y, sysColor);
                    }
                    break;
                default:
                    throw new Exception("Unsupported Pixel Format: " + this.pixelFormat.ToString());
            }
            bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
            return bitmap;
        }

        /// <summary>
        /// This struct contains the objects that are found in the header of .pbm, .pgm, and .ppm files.
        /// </summary>
        [Serializable]
        public struct PixelMapHeader
        {
            private string magicNumber;
            /// <summary>
            /// The "Magic Number" that identifies the type of Pixelmap. P1 = PBM (ASCII); P2 = PGM (ASCII); P3 = PPM (ASCII); P4 is not used;
            /// P5 = PGM (Binary); P6 = PPM (Binary).
            /// </summary>
            public string MagicNumber
            {
                get { return magicNumber; }
                set { magicNumber = value; }
            }

            private int width;
            /// <summary>
            /// The width of the image.
            /// </summary>
            public int Width
            {
                get { return width; }
                set { width = value; }
            }

            private int height;
            /// <summary>
            /// The height of the image.
            /// </summary>
            public int Height
            {
                get { return height; }
                set { height = value; }
            }

            private int depth;
            /// <summary>
            /// The depth (maximum color value in each channel) of the image.  This allows the format to represent 
            /// more than a single byte (0-255) for each color channel.
            /// </summary>
            public int Depth
            {
                get { return depth; }
                set { depth = value; }
            }
        }
    }
}
