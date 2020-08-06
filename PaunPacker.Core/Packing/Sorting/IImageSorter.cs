using System.Collections.Generic;
using PaunPacker.Core.Types;

namespace PaunPacker.Core.Packing.Sorting
{
    /// <summary>
    /// Represents an image sorter
    /// </summary>
    public interface IImageSorter //Rename to rectangle sorter ?
    {
        /// <summary>
        /// Sorts the images according to rules that are specific for a particular implementation
        /// </summary>
        /// <param name="rects">The rectangles to be sorted</param>
        /// <returns>Sorted rectangles</returns>
        IEnumerable<PPRect> SortImages(IEnumerable<PPRect> rects);
    }
}
