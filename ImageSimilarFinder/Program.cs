using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApplication1;

namespace ImageSimilarFinder
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
           
           
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += Application_ThreadException;
          
                       Application.Run(new Form1());
           // Application.Run(new ImageSimilarFinder.ReviewImageSimilarity());


        }

        public static  Icon GetExecutableIcon()
        {
            IntPtr large;
            IntPtr small;
            ExtractIconEx(Application.ExecutablePath, 0, out large, out small, 1);
            return Icon.FromHandle(small);
        }

        [DllImport("Shell32")]
        public static extern int ExtractIconEx(
            string sFile,
            int iIndex,
            out IntPtr piLargeVersion,
            out IntPtr piSmallVersion,
            int amountIcons);

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            
        }
    }
}
