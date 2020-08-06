using PaunPacker.Core.Types;
using System.Collections.Generic;
using System.Linq;

namespace PaunPacker.Core.Packing.Utils
{
    /// <summary>
    /// For given rectangles, returns collection of possible widths / heights to try (for minimum bounding box of those rectangles)
    /// </summary>
    /// <example>
    /// Usability: Imagine situation of strip-packing, unlimited height and you need to fix width. In case of rectangle packing + strip packing algorithm, the width must be set, but the
    /// it is not knowh which width to use. This class could be used to enumerate several possible width that should be tried
    /// </example>
    public static class SingleDimensionEnumerator
    {
        /// <summary>
        /// Enumerates possible widths of a bounding box for given rectangles
        /// </summary>
        /// <param name="rects">The rectangles for which the widths of their bounding box should be enumerated</param>
        /// <returns>Possible widths of the bounding box</returns>
        public static IEnumerable<int> GetWidth(IEnumerable<PPRect> rects)
        {
            //if rotations enabled, calculate minumum of min(width,height)
            int minimumImageWidth = rects.Select(x => x.Width).Min(); //readability over performance
            int maximumImageWidth = rects.Select(x => x.Width).Max();

            int sumOfWidths = rects.Select(x => x.Width).Sum();

            //+= minimumImageWidth (might be too slow ...)
            int increment = (minimumImageWidth + maximumImageWidth) / 2;
            for (int width = maximumImageWidth; width <= sumOfWidths; width += increment)
                yield return width;
        }

        /// <summary>
        /// Enumerates possible heights of a bounding box for given rectangles
        /// </summary>
        /// <param name="rects">The rectangles for which the heights of their bounding box should be enumerated</param>
        /// <returns>Possible heights of the bounding box</returns>
        public static IEnumerable<int> GetHeight(IEnumerable<PPRect> rects)
        {
            //if rotations enabled, calculate minumum of min(width,height)
            int minimumImageHeight = rects.Select(x => x.Height).Min(); //readability over performance
            int maximumImageHeight = rects.Select(x => x.Height).Max();

            int sumOfHeights = rects.Select(x => x.Height).Sum();

            //+= minimumImageHeight (might be too slow ...)
            int increment = (minimumImageHeight + maximumImageHeight) / 2;
            for (int height = maximumImageHeight; height <= sumOfHeights; height += increment)
                yield return height;
        }
    }
}
