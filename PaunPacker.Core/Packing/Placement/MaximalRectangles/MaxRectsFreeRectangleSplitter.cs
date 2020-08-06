using System.Collections.Generic;
using System.Linq;
using PaunPacker.Core.Packing.Placement.Guillotine;
using PaunPacker.Core.Types;

namespace PaunPacker.Core.Packing.Placement.MaximalRectangles
{
    /// <summary>
    /// Implementation of <see cref="IFreeRectangleSplitter"/> for a MaxRects algorithm
    /// </summary>
    public class MaxRectsFreeRectangleSplitter : IFreeRectangleSplitter
    {
        /// <inheritdoc />
        /// <summary>
        /// Splits a free rectangle by both vertical and horizontal axis
        /// </summary>
        /// <param name="freeRect">Free rectangle that should be splitted</param>
        /// <param name="rectJustPlaced">The rectangle that should be placed to the <paramref name="freeRect"/></param>
        /// <returns></returns>
        public IEnumerable<PPRect> SplitFreeRectangle(PPRect freeRect, PPRect rectJustPlaced)
        {
            int splitX = freeRect.Left + rectJustPlaced.Width;
            int splitY = freeRect.Top + rectJustPlaced.Height;
            //Perform the split by horizontal axis
            //Rect above the horizontal axis
            var freeRectAbove = new PPRect(freeRect.Left, splitY, freeRect.Right, freeRect.Bottom);
            //Rect below the horizontal axis
            var freeRectBelow = new PPRect(freeRect.Left + rectJustPlaced.Width, freeRect.Top, freeRect.Right, splitY);

            //Perform the split by vertical axis
            //Rect to the right of the vertical axis
            var freeRectRight = new PPRect(splitX, freeRect.Top, freeRect.Right, freeRect.Bottom);
            //Rect to the left of the vertical axis
            var freeRectLeft = new PPRect(freeRect.Left, freeRect.Top + rectJustPlaced.Height, splitX, freeRect.Bottom);

            return new PPRect[] { freeRectAbove, freeRectRight, freeRectBelow, freeRectLeft }.Where(x => x.Width > 0 && x.Height > 0).Distinct();
        }
    }
}
