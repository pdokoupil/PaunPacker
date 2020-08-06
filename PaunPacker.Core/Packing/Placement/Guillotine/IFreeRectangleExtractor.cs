using System.Collections.Generic;
using PaunPacker.Core.Types;

namespace PaunPacker.Core.Packing.Placement.Guillotine
{
    /// <summary>
    /// Allows to extract a free rectangle from a list of free rectangles
    /// </summary>
    public interface IFreeRectangleExtractor
    {
        /// <summary>
        /// Extracts a free rectangly (free rectangle "best" according to some (implementation specific) rules
        /// </summary>
        /// <param name="freeRects">The list of free rectangles to select from</param>
        /// <param name="currentRectToBePlaced">The current rectangle that will be placed to the selected rectangle</param>
        /// <returns></returns>
        PPRect? ExtractFreeRectangle(List<PPRect> freeRects, PPRect currentRectToBePlaced);
    }
}
