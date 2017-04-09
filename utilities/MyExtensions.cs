using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogoDetector
{
  public static   class MyExtensions
    {
        public static string FullErrorMessage(this Exception er)
        {
            string err = "";
            err = er.Message;
            while (er.InnerException!=null)
            {
                er = er.InnerException;
                err += Environment.NewLine + er.Message;
            }
            return err;
        }
    }
}
