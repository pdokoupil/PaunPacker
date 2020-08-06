using System;
using System.Collections.Generic;
using System.Linq;
using PaunPacker.Core.Packing.Placement;
using PaunPacker.Core.Types;

namespace PaunPacker.Core.Packing.MBBF
{
    /// <summary>
    /// Implementation of minimum bounding box finders which for a given rectangles tries to find a minimum bounding box of these rectangles
    /// </summary>
    public class UnknownSizePacker : IMinimumBoundingBoxFinder
    {
        /// <summary>
        /// Constructs an UnknownSizePacker with a given placement algorithm
        /// </summary>
        /// <param name="placementAlgo">The placement algorithm to be used</param>
        public UnknownSizePacker(IPlacementAlgorithm placementAlgo)
        {
            this.placementAlgo = placementAlgo;
        }

        /// <inheritdoc />
        public PackingResult FindMinimumBoundingBox(IEnumerable<PPRect> rects, System.Threading.CancellationToken cancellationToken = default)
        {
            if (!rects.Any())
            {
                return new PackingResult(0, 0, Enumerable.Empty<PPRect>());
            }

            (int currBestW, int currBestH, int area, int maxRectW) = GetInitialBoundary(rects);
            int currW = currBestW - 1, currH = currBestH;
            PackingResult bestPacking = placementAlgo.PlaceRects(currBestW, currBestH, rects);
            
            while (currW > maxRectW)
            {

                if (cancellationToken.IsCancellationRequested)
                {
                    return bestPacking;
                }

                if (currW * currH > currBestW * currBestH)
                {
                    currW--;
                    continue;
                }

                PackingResult currentPacking;
                if ((currentPacking = placementAlgo.PlaceRects(currW, currH, rects, cancellationToken)) != null)
                {
                    if (currW * currH < currBestH * currBestW)
                    {
                        currBestW = currW;
                        currBestH = currH;
                        bestPacking = currentPacking;
                    }
                    currW--;

                }
                else
                {
                    currH++;
                    continue;
                }

                if (currW * currH < area)
                    currH++;
            }


            return bestPacking;
        }

        /// <summary>
        /// Finds initial boundary (bounding box), that is somemething like upperbound on the area of packing
        /// </summary>
        /// <param name="rects">The rectangles for which the initial bounding box will be searched</param>
        /// <returns>The initial bounding box</returns>
        private (int w, int h, int area, int maxRectW) GetInitialBoundary(IEnumerable<PPRect> rects)
        {
            //int bestEnclosingArea = Int32.MaxValue;
            //int area = 0;
            int maxH = rects.Max(x => x.Height);

            //MaxValue / 2 should be enough (we do not expect someone to put that large textures into our program, as it would already consume at least approx. 2^30 * 4 bytes (4GB for a single texture !)
            var result = placementAlgo.PlaceRects(int.MaxValue >> 1, maxH, rects); //Int32.MaxValue caused problems, because when Width was MaxValue, then it was possible for Rect's Right to overflow ..
            int it = 0;
            while (result == null)
            {
                result = placementAlgo.PlaceRects(int.MaxValue >> 1, maxH * (++it + 1), rects);
            }

            int w = 0; int h = 0; int area = 0; int maxRectW = 0;
            foreach (var x in result.Rects)
            {
                area += x.Width * x.Height;
                maxRectW = (maxRectW > x.Width ? maxRectW : x.Width);
                w = (w > x.Right ? w : x.Right);
                h = (h > x.Bottom ? h : x.Bottom);
            }
            return (w, h, area, maxRectW);

        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        private readonly IPlacementAlgorithm placementAlgo;

        /// <inheritdoc />
        public int Progress => 0;

        /// <inheritdoc />
        public bool ReportsProgress => false;

        /// <inheritdoc />
        public event Action<object, int> ProgressChange;
    }
}
