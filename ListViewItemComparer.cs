using System;
using System.Collections;
using System.Windows.Forms;

namespace LogoDetector
{
    class ListViewItemComparer : IComparer
    {
        private int col;
        private SortOrder order;
        public ListViewItemComparer()
        {
            col = 0;
            order = SortOrder.Ascending;
        }
        public ListViewItemComparer(int column, SortOrder order)
        {
            col = column;
            this.order = order;
        }
        public int Compare(object x, object y)
        {
            int returnVal = -1;
            string itemx = ((ListViewItem)x).SubItems[col].Text;
            string itemy = ((ListViewItem)y).SubItems[col].Text;
            if (col == 0 || col == 1)
                returnVal = string.CompareOrdinal(itemx, itemy);
            else if (col == 2)
            {
                long numx = long.Parse(itemx.Replace("ms", ""));
                long numy = long.Parse(itemy.Replace("ms", ""));
                returnVal = numx > numy ? 1 : numx == numy ? 0 : -1; //string.CompareOrdinal(itemx.Replace("ms", ""), itemy.Replace("ms", ""));
            }
            else if (col == 3)
            {
                long numx = long.Parse(itemx.Replace("%", ""));
                long numy = long.Parse(itemy.Replace("%", ""));
                returnVal = numx > numy ? 1 : numx == numy ? 0 : -1;
                //   returnVal = string.CompareOrdinal(itemx.Replace("%", ""), itemy.Replace("%", ""));
            }

            // Determine whether the sort order is descending.
            if (order == SortOrder.Descending)
                // Invert the value returned by String.Compare.
                returnVal *= -1;
            return returnVal;
        }
    }
}