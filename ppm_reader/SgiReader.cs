﻿using System;
using System.Drawing;
using System.IO;

/*

Decoder for Silicon Graphics SGI (.RGB, .BW) images.
Decodes all SGI images that I've found in the wild.  If you find
one that it fails to decode, let me know!

Copyright 2013-2016 Dmitry Brant
http://dmitrybrant.com

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

*/

namespace DmitryBrant.ImageFormats
{
    /// <summary>
    /// Handles reading Silicon Graphics SGI (.RGB, .BW) images
    /// </summary>
    public static class SgiReader
    {

        /// <summary>
        /// Reads a Silicon Graphics SGI (.RGB, .BW) image from a file.
        /// </summary>
        /// <param name="fileName">Name of the file to read.</param>
        /// <returns>Bitmap that contains the image that was read.</returns>
        public static Bitmap Load(string fileName){
            Bitmap bmp;
            using (var f = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                bmp = Load(f);
            }
            return bmp;
        }

        /// <summary>
        /// Reads a Silicon Graphics SGI (.RGB, .BW) image from a stream.
        /// </summary>
        /// <param name="stream">Stream from which to read the image.</param>
        /// <returns>Bitmap that contains the image that was read.</returns>
        public static Bitmap Load(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);

            UInt16 magic = BigEndian(reader.ReadUInt16());
            if (magic != 0x1DA)
                throw new ApplicationException("Not a valid SGI file.");

            int compressionType = stream.ReadByte();
            int bytesPerComponent = stream.ReadByte();
            UInt16 dimension = BigEndian(reader.ReadUInt16());

            if(compressionType > 1)
                throw new ApplicationException("Unsupported compression type.");
            if (bytesPerComponent != 1)
                throw new ApplicationException("Unsupported bytes per component.");
            if (dimension != 1 && dimension != 2 && dimension != 3)
                throw new ApplicationException("Unsupported dimension.");

            int imgWidth = BigEndian(reader.ReadUInt16());
            int imgHeight = BigEndian(reader.ReadUInt16());
            int zSize = BigEndian(reader.ReadUInt16());
            UInt32 pixMin = BigEndian(reader.ReadUInt32());
            UInt32 pixMax = BigEndian(reader.ReadUInt32());

            if ((imgWidth < 1) || (imgHeight < 1) || (imgWidth > 32767) || (imgHeight > 32767))
                throw new ApplicationException("This SGI file appears to have invalid dimensions.");

            stream.Seek(4, SeekOrigin.Current);

            string imgName = System.Text.Encoding.ASCII.GetString(reader.ReadBytes(80)).Replace("\0", "").Trim();

            UInt32 colorMapFormat = BigEndian(reader.ReadUInt32());

            stream.Seek(404, SeekOrigin.Current);

            UInt32[] offsets = null;
            if (compressionType == 1)
            {
                int offsetTableLen = imgHeight * zSize;
                offsets = new UInt32[offsetTableLen];
                for(int i=0; i<offsetTableLen; i++)
                    offsets[i] = BigEndian(reader.ReadUInt32());
                stream.Seek(offsets[0], SeekOrigin.Begin);
            }


            byte[] bmpData = new byte[imgWidth * 4 * imgHeight];

            try
            {

                if (compressionType == 1)
                {
                    if (zSize == 1)
                    {
                        int x = 0, i, j, k, b;
                        for (int y = imgHeight - 1; y >= 0; y--)
                        {
                            x = 0;
                            while (stream.Position < stream.Length)
                            {
                                i = stream.ReadByte();
                                j = i & 0x7F;
                                if (j == 0)
                                    break;

                                if ((i & 0x80) != 0)
                                {
                                    for (k = 0; k < j; k++)
                                    {
                                        b = stream.ReadByte();
                                        bmpData[4 * (y * imgWidth + x)] = (byte)b;
                                        bmpData[4 * (y * imgWidth + x) + 1] = (byte)b;
                                        bmpData[4 * (y * imgWidth + x) + 2] = (byte)b;
                                        x++;
                                    }
                                }
                                else
                                {
                                    b = stream.ReadByte();
                                    for (k = 0; k < j; k++)
                                    {
                                        bmpData[4 * (y * imgWidth + x)] = (byte)b;
                                        bmpData[4 * (y * imgWidth + x) + 1] = (byte)b;
                                        bmpData[4 * (y * imgWidth + x) + 2] = (byte)b;
                                        x++;
                                    }
                                }
                            }
                        }

                    }
                    else if (zSize == 3 || zSize == 4)
                    {
                        int lineCount = 0;
                        byte[,] scanline = new byte[zSize, imgWidth];
                        int i, j, k, b, scanPtr;

                        for (int y = imgHeight - 1; y >= 0; y--)
                        {
                            for (int scanLineIndex = 0; scanLineIndex < 3; scanLineIndex++)
                            {
                                scanPtr = 0;
                                stream.Seek(offsets[lineCount + scanLineIndex * imgHeight], SeekOrigin.Begin);
                                while (stream.Position < stream.Length)
                                {
                                    i = stream.ReadByte();
                                    j = i & 0x7F;
                                    if (j == 0)
                                        break;
                                    if ((i & 0x80) != 0)
                                    {
                                        for (k = 0; k < j; k++)
                                            scanline[scanLineIndex, scanPtr++] = (byte)stream.ReadByte();
                                    }
                                    else
                                    {
                                        b = stream.ReadByte();
                                        for (k = 0; k < j; k++)
                                            scanline[scanLineIndex, scanPtr++] = (byte)b;
                                    }
                                }
                            }

                            for (int x = 0; x < imgWidth; x++)
                            {
                                bmpData[4 * (y * imgWidth + x)] = scanline[2, x];
                                bmpData[4 * (y * imgWidth + x) + 1] = scanline[1, x];
                                bmpData[4 * (y * imgWidth + x) + 2] = scanline[0, x];
                            }

                            lineCount++;
                        }
                    }

                }
                else
                {
                    if (zSize == 1)
                    {
                        int i;
                        for (int y = imgHeight - 1; y >= 0; y--)
                        {
                            for (int x = 0; x < imgWidth; x++)
                            {
                                i = stream.ReadByte();
                                bmpData[4 * (y * imgWidth + x)] = (byte)i;
                                bmpData[4 * (y * imgWidth + x) + 1] = (byte)i;
                                bmpData[4 * (y * imgWidth + x) + 2] = (byte)i;
                            }
                        }
                    }
                    else if (zSize == 3)
                    {
                        int i;
                        for (int y = imgHeight - 1; y >= 0; y--)
                        {
                            for (int x = 0; x < imgWidth; x++)
                            {
                                i = stream.ReadByte();
                                bmpData[4 * (y * imgWidth + x)] = (byte)i;
                            }
                        }
                        for (int y = imgHeight - 1; y >= 0; y--)
                        {
                            for (int x = 0; x < imgWidth; x++)
                            {
                                i = stream.ReadByte();
                                bmpData[4 * (y * imgWidth + x) + 1] = (byte)i;
                            }
                        }
                        for (int y = imgHeight - 1; y >= 0; y--)
                        {
                            for (int x = 0; x < imgWidth; x++)
                            {
                                i = stream.ReadByte();
                                bmpData[4 * (y * imgWidth + x) + 2] = (byte)i;
                            }
                        }
                    }

                }

            }
            catch (Exception e)
            {
                //give a partial image in case of unexpected end-of-file

                System.Diagnostics.Debug.WriteLine("Error while processing SGI file: " + e.Message);
            }

            Bitmap theBitmap = new Bitmap((int)imgWidth, (int)imgHeight, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            System.Drawing.Imaging.BitmapData bmpBits = theBitmap.LockBits(new Rectangle(0, 0, theBitmap.Width, theBitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            System.Runtime.InteropServices.Marshal.Copy(bmpData, 0, bmpBits.Scan0, bmpData.Length);
            theBitmap.UnlockBits(bmpBits);
            return theBitmap;
        }


        private static UInt16 BigEndian(UInt16 val)
        {
            if (!BitConverter.IsLittleEndian) return val;
            return conv_endian(val);
        }
        private static UInt32 BigEndian(UInt32 val)
        {
            if (!BitConverter.IsLittleEndian) return val;
            return conv_endian(val);
        }

        private static UInt16 conv_endian(UInt16 val)
        {
            UInt16 temp;
            temp = (UInt16)(val << 8); temp &= 0xFF00; temp |= (UInt16)((val >> 8) & 0xFF);
            return temp;
        }
        private static UInt32 conv_endian(UInt32 val)
        {
            UInt32 temp = (val & 0x000000FF) << 24;
            temp |= (val & 0x0000FF00) << 8;
            temp |= (val & 0x00FF0000) >> 8;
            temp |= (val & 0xFF000000) >> 24;
            return (temp);
        }

    }
}
