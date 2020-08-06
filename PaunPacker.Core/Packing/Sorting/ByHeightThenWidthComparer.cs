using PaunPacker.Core.Types;
using System.Collections.Generic;

namespace PaunPacker.Core.Packing.Sorting
{
    /// <summary>
    /// Implementation of <see cref="IComparer{T}"/> for PPRect
    /// Compares two rectangles based on their height and then on their width (in default, ascending order)
    /// </summary>
    public class ByHeightThenWidthComparer : IComparer<PPRect>
    {
        /// <summary>
        /// Compares two rectangles based on their height and then on their width
        /// </summary>
        /// <param name="x">First comparand</param>
        /// <param name="y">Second comparand</param>
        /// <returns>Negative number if <paramref name="x"/> is smaller than <paramref name="y"/>, 0 if they are equal and positive number otherwise</returns>
        public int Compare(PPRect x, PPRect y)
        {
            int tmp = x.Height - y.Height;
            if (tmp == 0)
                tmp = x.Width - y.Width;
            return tmp;
        }
    }
}
