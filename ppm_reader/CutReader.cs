﻿using System;
using System.Drawing;
using System.IO;

/*

Decoder for Dr. Halo CUT (.CUT) images.
Decodes all CUT images that I've found in the wild.  If you find
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
    /// Handles reading Dr. Halo (.CUT) images
    /// </summary>
    public static class CutReader
    {

        /// <summary>
        /// Reads a Dr. Halo (.CUT) image from a file.
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
        /// Reads a Dr. Halo (.CUT) image from a stream.
        /// </summary>
        /// <param name="stream">Stream from which to read the image.</param>
        /// <returns>Bitmap that contains the image that was read.</returns>
        public static Bitmap Load(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);

            int imgWidth = LittleEndian(reader.ReadUInt16());
            int imgHeight = LittleEndian(reader.ReadUInt16());
            LittleEndian(reader.ReadUInt16()); //reserved word

            if ((imgWidth < 1) || (imgHeight < 1) || (imgWidth > 32767) || (imgHeight > 32767))
                throw new ApplicationException("This CUT file appears to have invalid dimensions.");

            byte[] bmpData = new byte[imgWidth * 4 * imgHeight];

            //create a grayscale color palette, since CUT files don't contain a
            //palette of their own. Replace this if you like...
            byte[] colorPalette = new byte[256];
            for (int i = 0; i < colorPalette.Length; i++)
                colorPalette[i] = (byte)i;

            try
            {
                int x = 0, y = 0;
                int i, j, k, b;
                int lineLen;

                while (y < imgHeight && stream.Position < stream.Length)
                {
                    lineLen = reader.ReadUInt16();

                    while(stream.Position < stream.Length){
                        
                        i = stream.ReadByte();
                        j = i & 0x7F;

                        if (j == 0)
                        {
                            x = 0; y++;
                            break;
                        }
                        if (i > 127)
                        {
                            k = stream.ReadByte();
                            for (b = 0; b < j; b++)
                            {
                                bmpData[4 * (y * imgWidth + x)] = colorPalette[k];
                                bmpData[4 * (y * imgWidth + x) + 1] = colorPalette[k];
                                bmpData[4 * (y * imgWidth + x) + 2] = colorPalette[k];
                                x++;
                            }
                        }
                        else
                        {
                            for (b = 0; b < j; b++)
                            {
                                k = stream.ReadByte();
                                bmpData[4 * (y * imgWidth + x)] = colorPalette[k];
                                bmpData[4 * (y * imgWidth + x) + 1] = colorPalette[k];
                                bmpData[4 * (y * imgWidth + x) + 2] = colorPalette[k];
                                x++;
                            }
                        }
                    }

                }

            }
            catch (Exception e)
            {
                //give a partial image in case of unexpected end-of-file

                System.Diagnostics.Debug.WriteLine("Error while processing CUT file: " + e.Message);
            }

            Bitmap theBitmap = new Bitmap((int)imgWidth, (int)imgHeight, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            System.Drawing.Imaging.BitmapData bmpBits = theBitmap.LockBits(new Rectangle(0, 0, theBitmap.Width, theBitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            System.Runtime.InteropServices.Marshal.Copy(bmpData, 0, bmpBits.Scan0, bmpData.Length);
            theBitmap.UnlockBits(bmpBits);
            return theBitmap;
        }


        private static UInt16 LittleEndian(UInt16 val)
        {
            if (BitConverter.IsLittleEndian) return val;
            return conv_endian(val);
        }
        private static UInt32 LittleEndian(UInt32 val)
        {
            if (BitConverter.IsLittleEndian) return val;
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