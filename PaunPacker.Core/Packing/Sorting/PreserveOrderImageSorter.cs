using PaunPacker.Core.Types;
using System.Collections.Generic;

namespace PaunPacker.Core.Packing.Sorting
{
    /// <summary>
    /// Represents an indentity sorter
    /// </summary>
    /// <remarks>
    ///  This sorter actually performs no sorting at all. It only offers a way to force placement algorithms / minimum bounding box finders (when they are parametrized by the sorter)
    ///  not to sort images and use the images in the order given to the algorithm
    /// </remarks>
    public class PreserveOrderImageSorter : IImageSorter
    {
        /// <inheritdoc />
        public IEnumerable<PPRect> SortImages(IEnumerable<PPRect> rects)
        {
            return rects;
        }
    }
}
