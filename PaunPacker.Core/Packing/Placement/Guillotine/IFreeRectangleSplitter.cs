using System.Collections.Generic;
using PaunPacker.Core.Types;

namespace PaunPacker.Core.Packing.Placement.Guillotine
{
    /// <summary>
    /// Allows to split a free rectangle after placing a rectangle into it
    /// </summary>
    public interface IFreeRectangleSplitter
    {
        /// <summary>
        /// Splits the <paramref name="freeRect"/> after placing <paramref name="rectJustPlaced"/> into it
        /// </summary>
        /// <param name="freeRect">The free rectangle where the <paramref name="rectJustPlaced"/> was placed</param>
        /// <param name="rectJustPlaced">The just placed rectangle</param>
        /// <returns>Free rectangles that resulted from the split</returns>
        IEnumerable<PPRect> SplitFreeRectangle(PPRect freeRect, PPRect rectJustPlaced);
    }
}
