using System;
using System.Collections.Generic;
using System.Linq;
using PaunPacker.Core.Types;

namespace PaunPacker.Core.Packing.Placement.Guillotine
{
    /// <summary>
    /// Implementation of <see cref="IFreeRectangleSplitter"/> that splits a free rectangle by it's longer axis
    /// </summary>
    public class LongerAxisGuillotineFreeRectangleSplitter : IFreeRectangleSplitter
    {
        /// <inheritdoc />
        /// <summary>
        /// Split the <paramref name="selectedFreeRectangle"/> by it's longer axis
        /// </summary>
        /// <param name="selectedFreeRectangle">The free rectangle that was selected for a placement of <paramref name="rectToBePlaced"/></param>
        /// <param name="rectToBePlaced">The rectangle that should be placed</param>
        /// <returns>Free rectangle that results from the splitting of <paramref name="selectedFreeRectangle"/></returns>
        public IEnumerable<PPRect> SplitFreeRectangle(PPRect selectedFreeRectangle, PPRect rectToBePlaced)
        {
            SplitDirection splitDirection = SelectSplitDirection(/*selectedFreeRectangle, */rectToBePlaced);
            switch (splitDirection)
            {
                case SplitDirection.HORIZONTAL:
                    return SplitFreeRectangleHorizontal(ref selectedFreeRectangle, rectToBePlaced);
                case SplitDirection.VERTICAL:
                    return SplitFreeRectangleVertical(ref selectedFreeRectangle, rectToBePlaced);
                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Performs a split of <paramref name="freeRectangle"/> by a horizontal axis
        /// </summary>
        /// <param name="freeRectangle">The free rectangle that was selected for a placement of the free rectangle that should be placed</param>
        /// <param name="justPlacedRect">The rectangle that was just placed into <paramref name="freeRectangle"/></param>
        /// <returns></returns>
        private IEnumerable<PPRect> SplitFreeRectangleHorizontal(ref PPRect freeRectangle, PPRect justPlacedRect)
        {
            int splitY = freeRectangle.Top + justPlacedRect.Height;
            PPRect freeRect1 = new PPRect(freeRectangle.Left, splitY, freeRectangle.Right, freeRectangle.Bottom);
            PPRect freeRect2 = new PPRect(freeRectangle.Left + justPlacedRect.Width, freeRectangle.Top, freeRectangle.Right, justPlacedRect.Height);
            return new PPRect[] { freeRect1, freeRect2 }.Where(x => x.Width > 0 && x.Height > 0);
        }

        /// <summary>
        /// Performs a split of <paramref name="freeRectangle"/> by a vertical axis
        /// </summary>
        /// <param name="freeRectangle">The free rectangle that was selected for a placement of rectangle to be placed</param>
        /// <param name="justPlacedRect">The rectangle that was just placed into <paramref name="freeRectangle"/></param>
        /// <returns></returns>
        private IEnumerable<PPRect> SplitFreeRectangleVertical(ref PPRect freeRectangle, PPRect justPlacedRect)
        {
            int splitX = freeRectangle.Left + justPlacedRect.Width;
            PPRect freeRect1 = new PPRect(freeRectangle.Left, freeRectangle.Top + justPlacedRect.Height, splitX, freeRectangle.Bottom);
            PPRect freeRect2 = new PPRect(splitX, freeRectangle.Top, freeRectangle.Right, freeRectangle.Bottom);
            return new PPRect[] { freeRect1, freeRect2 }.Where(x => x.Width > 0 && x.Height > 0);
        }

        /// <summary>
        /// For a given rectangle that is being placed, selets the split direction (axis)
        /// </summary>
        /// <param name="rectToBePlaced">The rectangle that will be placed</param>
        /// <remarks>The direction is determined only be the <paramref name="rectToBePlaced"/> not by a free rectangle where it will be placed</remarks>
        /// <returns>The SplitDirection (the longer axis)</returns>
        private SplitDirection SelectSplitDirection(PPRect rectToBePlaced)
        {
            if (rectToBePlaced.Width > rectToBePlaced.Height)
            {
                return SplitDirection.HORIZONTAL;
            }
            else
            {
                return SplitDirection.VERTICAL;
            }
        }
    }
}
