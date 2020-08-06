using PaunPacker.Core.Types;
using System.Collections.Generic;
using System.Linq;

namespace PaunPacker.Core.Packing.Sorting
{
    /// <summary>
    /// ImageSorter that sorts by the height and then by the width of the rectangles, in ascending order
    /// </summary>
    public class ByHeightAndWidthImageSorter : IImageSorter
    {
        /// <summary>
        /// Constructs a <see cref="ByHeightAndWidthImageSorter"/>
        /// </summary>
        public ByHeightAndWidthImageSorter()
        {
            this.comparer = new ByHeightThenWidthComparer();
        }

        /// <inheritdoc />
        public IEnumerable<PPRect> SortImages(IEnumerable<PPRect> rects)
        {
            return rects.OrderBy(x => x, this.comparer);
        }

        /// <summary>
        /// The comparer used for sorting
        /// </summary>
        private readonly IComparer<PPRect> comparer;
    }
}
