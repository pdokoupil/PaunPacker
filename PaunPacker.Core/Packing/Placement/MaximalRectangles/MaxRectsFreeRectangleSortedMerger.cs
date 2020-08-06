using System.Collections.Generic;
using PaunPacker.Core.Packing.Placement.Guillotine;
using PaunPacker.Core.Types;

namespace PaunPacker.Core.Packing.Placement.MaximalRectangles
{
    /// <summary>
    /// Implementation of <see cref="IFreeRectangleMerger"/> for MaxRects algorithm
    /// </summary>
    /// <seealso cref="GuillotineFreeRectangleSortedMerger"/>
    /// <remarks>Currently behaves same as the <see cref="GuillotineFreeRectangleSortedMerger"/></remarks>
    public class MaxRectsFreeRectangleSortedMerger : IFreeRectangleMerger
    {
        /// <summary>
        /// Constructs a <see cref="MaxRectsFreeRectangleSortedMerger"/>
        /// </summary>
        public MaxRectsFreeRectangleSortedMerger()
        {
            guillotineMerger = new GuillotineFreeRectangleSortedMerger();
        }

        /// <summary>
        /// Merges a <paramref name="freeRectsObtained"/> into <paramref name="freeRectangles"/> at a position so that the updated collection
        /// of free rectangles is sorted according to <see cref="PPRectAreaComparer"/>
        /// </summary>
        /// <param name="freeRectangles">List of free rectangles</param>
        /// <param name="freeRectsObtained">New free rectangle that should be merged to the <paramref name="freeRectangles"/></param>
        public void MergeFreeRectangles(List<PPRect> freeRectangles, IEnumerable<PPRect> freeRectsObtained)
        {
            guillotineMerger.MergeFreeRectangles(freeRectangles, freeRectsObtained);
        }

        /// <summary>
        /// The IFreeRectangleMerger used by this merger
        /// </summary>
        private readonly IFreeRectangleMerger guillotineMerger;
    }
}
