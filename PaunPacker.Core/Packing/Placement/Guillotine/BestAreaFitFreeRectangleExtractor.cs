using System;
using System.Collections.Generic;
using System.Linq;
using PaunPacker.Core.Types;

namespace PaunPacker.Core.Packing.Placement.Guillotine
{
    /// <summary>
    /// Extracts (picks) one of the free rectangles based on the "Best area fit" with the rectangle that should be placed to the free rectangle
    /// </summary>
    public class BestAreaFitFreeRectangleExtractor : IFreeRectangleExtractor
    {
        /// <summary>
        /// Selects free rectangle from freeLists.
        /// For BestAreaFit it means to select free rectangle of smallest area to which currentRectToPack can fit
        /// </summary>
        /// <param name="freeRects">Free rectangles</param>
        /// <param name="currentRectToPack">Rectangle that will be placed to the selected rectangle</param>
        /// <exception cref="ArgumentNullException">Is thrown when the <paramref name="freeRects"/> is null</exception>
        /// <returns>The best rectangle from <paramref name="freeRects"/> where the <paramref name="currentRectToPack"/> could be placed</returns>
        public PPRect? ExtractFreeRectangle(List<PPRect> freeRects, PPRect currentRectToPack)
        {
            if (freeRects == null)
            {
                throw new ArgumentNullException($"The {nameof(freeRects)} cannot be null");
            }

            PPRect selectedRect = freeRects
                .OrderBy(x => ((long)x.Width * x.Height) - ((long)currentRectToPack.Width) * currentRectToPack.Height)
                .FirstOrDefault(x => x.Width >= currentRectToPack.Width && x.Height >= currentRectToPack.Height);


            if (selectedRect == null || selectedRect.Width <= 0 || selectedRect.Height <= 0)
            {
                return null;
            }

            freeRects.Remove(selectedRect);
            return selectedRect;
        }
    }
}
