using System;
using System.Collections.Generic;
using PaunPacker.Core.Types;

namespace PaunPacker.Core.Packing.Placement.Guillotine
{
    /// <summary>
    /// Implementation of <see cref="IFreeRectangleMerger"/> that represents a merger which merges a free rectangle
    /// into a collection of free rectangle in sorted manner where the order is determined based on the <see cref="PPRectAreaComparer"/>
    /// </summary>
    public class GuillotineFreeRectangleSortedMerger : IFreeRectangleMerger
    {
        /// <summary>
        /// Merges a <paramref name="freeRectsObtained"/> into <paramref name="freeRectangles"/> at a position so that the updated collection
        /// of free rectangles is sorted according to <see cref="PPRectAreaComparer"/>
        /// </summary>
        /// <param name="freeRectangles">List of free rectangles</param>
        /// <param name="freeRectsObtained">New free rectangle that should be merged to the <paramref name="freeRectangles"/></param>
        /// <exception cref="ArgumentNullException">Is thrown when any of the <paramref name="freeRectangles"/>, <paramref name="freeRectsObtained"/> is null</exception>
        public void MergeFreeRectangles(List<PPRect> freeRectangles, IEnumerable<PPRect> freeRectsObtained)
        {
            if (freeRectsObtained == null)
            {
                throw new ArgumentNullException($"The {nameof(freeRectsObtained)} cannot be null");
            }

            if (freeRectangles == null)
            {
                throw new ArgumentNullException($"The {nameof(freeRectangles)} cannot be null");
            }

            foreach (var freeRect in freeRectsObtained)
            {
                if (freeRect.Width > 0 && freeRect.Height > 0)
                {
                    SortedInsertion(freeRectangles, freeRect);
                }
            }
        }

        /// <summary>
        /// Utility method for sorted (based on order given by <see cref="PPRectAreaComparer"/>) insertion
        /// </summary>
        /// <param name="list">List of rectangles</param>
        /// <param name="item">Rectangle that should be inserted</param>
        private void SortedInsertion(List<PPRect> list, PPRect item)
        {
            int position = list.BinarySearch(item, new PPRectAreaComparer());
            if (position < 0)
            {
                position = ~position;
            }
            list.Insert(position, item);
        }
    }
}
