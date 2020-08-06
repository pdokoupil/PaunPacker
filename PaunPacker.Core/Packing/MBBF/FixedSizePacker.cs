using System;
using System.Collections.Generic;
using System.Threading;
using PaunPacker.Core.Packing.Placement;
using PaunPacker.Core.Types;

namespace PaunPacker.Core.Packing.MBBF
{
    /// <summary>
    /// Implementation of <see cref="IMinimumBoundingBoxFinder"/> that allows to pack the rectangles into a bounding box of a fixed size
    /// </summary>
    /// <remarks>
    /// This class only wraps the <see cref="IPlacementAlgorithm"/> and the reason for introducing this wrapper is to have an ability to use
    /// placement into fixed size bounding box at places where <see cref="IMinimumBoundingBoxFinder"/> is expected
    /// If we know we are packing into W x H sized bounding box -> then this is simply a wrapper around placement algorithm
    /// </remarks>
    public class FixedSizePacker : IMinimumBoundingBoxFinder
    {
        /// <summary>
        /// Constructs a FixedSizePacker which uses a given placement algorithm <paramref name="placementAlgo"/> and packs the rectangles into a bounding box with dimensions of <paramref name="width"/> x <paramref name="height"/>
        /// </summary>
        /// <param name="width">The width of the bounding box</param>
        /// <param name="height">The height of the bounding box</param>
        /// <param name="placementAlgo">The placement algorithm to be used</param>
        public FixedSizePacker(int width, int height, IPlacementAlgorithm placementAlgo)
        {
            this.placementAlgo = placementAlgo;
            this.Width = width;
            this.Height = height;
        }

        /// <inheritdoc />
        /// <remarks>
        /// Does not find the minimum bounding box in the right sense, but instead it tries to place (with the underlying placement algorithm)
        /// into a bounding box with dimensions <see cref="Width"/> x <see cref="Height"/>
        /// </remarks>
        public PackingResult FindMinimumBoundingBox(IEnumerable<PPRect> rects, CancellationToken cancellationToken = default)
        {
            var result = FindMinimumBoundingBox(Width, Height, rects, cancellationToken);
            if (result != null && (result.Width != Width || result.Height != Height)) //placement did some optimizations to minimize packing, but user wanted fixed size, so change size..
            {
                return new PackingResult(Width, Height, result.Rects);
            }
            return result;
        }

        /// <see cref="FindMinimumBoundingBox(IEnumerable{PPRect}, CancellationToken)"/>
        /// <remarks>
        /// Does not find the minimum bounding box in the right sense, but instead it tries to place (with the underlying placement algorithm)
        /// into a bounding box with dimensions <paramref name="width"/> x <paramref name="height"/>
        /// </remarks>
        /// <param name="width">The width of the bounding box, overrides the <see cref="Width"/></param>
        /// <param name="height">The height of the bounding box, overrides the <see cref="Height"/></param>
        /// <param name="rects">The rectangles that should be packed into the bounding box of size width * height</param>
        /// <param name="token">The cancellation token</param>
        public PackingResult FindMinimumBoundingBox(int width, int height, IEnumerable<PPRect> rects, CancellationToken token = default)
        {
            return placementAlgo.PlaceRects(width, height, rects, token);
        }

        /// <summary>
        /// The width of the bounding box
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// The height of the bounding box
        /// </summary>
        public int Height { get; set; }

        /// <inheritdoc />
        public int Progress => placementAlgo?.Progress ?? 0;

        /// <inheritdoc />
        public bool ReportsProgress => placementAlgo?.ReportsProgress ?? false;

        /// <inheritdoc />
        public event Action<object, int> ProgressChange
        {
            add
            {
                if (placementAlgo != null)
                {
                    placementAlgo.ProgressChange += value;
                }
            }

            remove
            {
                if (placementAlgo != null)
                {
                    placementAlgo.ProgressChange -= value;
                }
            }
        }

        private readonly IPlacementAlgorithm placementAlgo;
    }
}
