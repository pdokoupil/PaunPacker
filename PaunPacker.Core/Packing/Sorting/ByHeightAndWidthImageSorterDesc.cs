using PaunPacker.Core.Types;
using System.Collections.Generic;
using System.Linq;

namespace PaunPacker.Core.Packing.Sorting
{

    /// <summary>
    /// ImageSorter that sorts by the height and then by the width of the rectangles, in descending order
    /// </summary>
    public class ByHeightAndWidthImageSorterDesc : IImageSorter
    {
        /// <summary>
        /// Constructs a <see cref="ByHeightAndWidthImageSorterDesc"/>
        /// </summary>
        public ByHeightAndWidthImageSorterDesc()
        {
            this.comparer = new ByHeightThenWidthComparer();
        }
        
        /// <inheritdoc />
        public IEnumerable<PPRect> SortImages(IEnumerable<PPRect> rects)
        {
            return rects.OrderByDescending(x => x, this.comparer);
        }

        /// <summary>
        /// The comparer used for sorting
        /// </summary>
        private readonly IComparer<PPRect> comparer;
    }

}
