using System.Collections.Generic;
using PaunPacker.Core.Types;

namespace PaunPacker.Core.Packing.Placement.Guillotine
{
    /// <summary>
    /// Allows to merge new free rectangles into a list of free rectangles
    /// </summary>
    public interface IFreeRectangleMerger
    {
        /// <summary>
        /// Merges a free rectangles from <paramref name="freeRectsObtained"/> into the <paramref name="freeRectangles"/>
        /// </summary>
        /// <param name="freeRectangles">Current, already existing free rectangles</param>
        /// <param name="freeRectsObtained">Newly obtained free rectangles</param>
        void MergeFreeRectangles(List<PPRect> freeRectangles, IEnumerable<PPRect> freeRectsObtained);
    }
}
