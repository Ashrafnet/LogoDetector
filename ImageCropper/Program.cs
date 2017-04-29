using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace ImageCropper
{
    class Program
    {
        static void Main(string[] args)
        {
            string folder_path = @"C:\My Files\work\Ashraf\LogoDetection\testimg";
            var imgPaths = Directory.GetFiles(folder_path, "*.*", SearchOption.AllDirectories).ToList();
            foreach (string imgPath in imgPaths)
            {
                Bitmap source = new Bitmap(imgPath);
                int x = source.Width - 120;
                int y = source.Height - 120;
                Bitmap CroppedImage = source.Clone(new Rectangle(x, y, 120, 120), source.PixelFormat);
                MemoryStream ms = new MemoryStream();
                CroppedImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                Image img = System.Drawing.Image.FromStream(ms);
                img.Save(Path.Combine(System.IO.Path.GetDirectoryName(imgPath),"cropped", System.IO.Path.GetFileName(imgPath)), System.Drawing.Imaging.ImageFormat.Jpeg);
            }
        }
    }
}
