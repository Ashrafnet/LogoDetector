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
            if (er == null) return "";
            err = er.Message;
            while (er.InnerException!=null)
            {
                er = er.InnerException;
                err += " | " + er.Message;
            }
            return err;
        }

        public static int? ToIntOrNull(this string strint)
        {
            int  r;
            if (int.TryParse(strint, out r))
                return r;
            else
                return null;
        }
        public static float? ToFloatOrNull(this string strint)
        {
            float r;
            if (float.TryParse(strint, out r))
                return r;
            else
                return null;
        }
        public static double ? ToDoubleOrNull(this string strint)
        {
            double r;
            if (double.TryParse(strint, out r))
                return r;
            else
                return null;
        }

        public static string  ToPercentageString(this string strint)
        {
            double r;
            if (double.TryParse(strint, out r))
            {
                if (r <= 1)
                    return (Math.Round( r,5) * 100) + "%";
                else
                    return Math.Round(r, 5) + "%";
            }
            else
                return null;
        }

        public static float ToPercentagFloat(this string strint)
        {
            float r;
            if (float.TryParse(strint, out r))
            {
                if (r <= 1)
                    return (float)(Math.Round((decimal)r, 5) * 100);
                else
                    return (float) Math.Round((decimal)r, 5) ;
            }
            else
                return -1;
        }

        public static IEnumerable<IEnumerable<T>> Batch<T>(
        this IEnumerable<T> source, int size)
        {
            T[] bucket = null;
            var count = 0;

            foreach (var item in source)
            {
                if (bucket == null)
                    bucket = new T[size];

                bucket[count++] = item;

                if (count != size)
                    continue;

                yield return bucket.Select(x => x);

                bucket = null;
                count = 0;
            }

            // Return the last bucket with all remaining elements
            if (bucket != null && count > 0)
                yield return bucket.Take(count);
        }
    }
}
