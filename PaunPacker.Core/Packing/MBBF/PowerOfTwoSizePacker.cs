using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using PaunPacker.Core.Packing.Placement;
using PaunPacker.Core.Types;

namespace PaunPacker.Core.Packing.MBBF
{
    /// <summary>
    /// Implementation of <see cref="IMinimumBoundingBoxFinder"/> that packs (and tries to find MBBF) the rectangles into a minimum bounding box
    /// that has dimensions which are powers of two
    /// </summary>
    public class PowerOfTwoSizePacker : IMinimumBoundingBoxFinder
    {
        /// <summary>
        /// Constructs a PowerOfTwoSizePacker that finds a minimum bounding box whose dimensions are in powers of two and tries to pack (using the <paramref name="placementAlgorithm"/>) the rectangles into that bounding box
        /// </summary>
        /// <param name="placementAlgorithm">The placement algorithm to be used</param>
        public PowerOfTwoSizePacker(IPlacementAlgorithm placementAlgorithm)
        {
            this.placementAlgorithm = placementAlgorithm;
        }

        /// <inheritdoc />
        public PackingResult FindMinimumBoundingBox(IEnumerable<PPRect> rects, CancellationToken cancellationToken = default)
        {
            if (!rects.Any())
            {
                return new PackingResult(0, 0, rects);
            }

            PackingResult bestPacking = null;
            foreach ((int W, int H) in GetDimensionsToTry(rects))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return bestPacking;
                }
                PackingResult currPacking = placementAlgorithm.PlaceRects(W, H, rects, cancellationToken);
                if (bestPacking == null || (currPacking != null && currPacking.Width * currPacking.Height < bestPacking.Width * bestPacking.Height))
                {
                    bestPacking = currPacking;
                }
            }

            if (bestPacking != null && (!IsPoT(bestPacking.Width) || !IsPoT(bestPacking.Height)))
            {
                return new PackingResult(NextPoT(bestPacking.Width), NextPoT(bestPacking.Height), bestPacking.Rects);
            }
            return bestPacking;
        }
        
        /// <summary>
        /// Enumerates the dimensions of the bounding box finder that the algorithm will try pack the rectangles into
        /// </summary>
        /// <remarks>Only dimensions that are in powers of two are considered</remarks>
        /// <param name="rects">Rectangles that should be contained within the bounding box</param>
        /// <returns>Possible, power of two dimensions of the bounding box</returns>
        private IEnumerable<(int W, int H)> GetDimensionsToTry(IEnumerable<PPRect> rects)
        {
            int w = 2;
            int h = 2;
            int sumW = rects.Select(x => x.Width).Sum();
            int sumH = rects.Select(x => x.Height).Sum();
            int sumWPoT = NextPoT(sumW);
            int sumHPoT = NextPoT(sumH);
            for (; w <= sumWPoT; w = NextPoT(w))
            {
                for (; h <= sumHPoT; h = NextPoT(h))
                {
                    yield return (w, h);
                }
                h = 2;
            }
        }

        /// <summary>
        /// Checks whether a given value is in power of two
        /// </summary>
        /// <param name="value">Value to be checked</param>
        /// <returns>True if the <paramref name="value"/> is a power of two, false otherwise</returns>
        private static bool IsPoT(int value)
        {
            int v = value;
            v--;
            v |= v >> 1;
            v |= v >> 2;
            v |= v >> 4;
            v |= v >> 8;
            v |= v >> 16;
            v++;
            return v == value;
        }

        /// <summary>
        /// Returns the smallest number that is a power of two and that is (strictly) greater than <paramref name="value"/>
        /// </summary>
        /// <remarks>Based on https://graphics.stanford.edu/~seander/bithacks.html#RoundUpPowerOf2 </remarks>
        /// <param name="value">The lower bound</param>
        /// <returns>The smallest number greater than <paramref name="value"/> which is a power of two</returns>
        private static int NextPoT(int value)
        {
            int v = value;
            while (true)
            {
                v--;
                v |= v >> 1;
                v |= v >> 2;
                v |= v >> 4;
                v |= v >> 8;
                v |= v >> 16;
                v++;

                if (v == value)
                    v++;
                else break;
            }
            return v;
        }

        private readonly IPlacementAlgorithm placementAlgorithm;

        /// <inheritdoc />
        public int Progress => 0;

        /// <inheritdoc />
        public bool ReportsProgress => false;

        /// <inheritdoc />
        public event Action<object, int> ProgressChange;
    }
}
