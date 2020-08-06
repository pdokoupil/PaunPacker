using System;
using System.Collections.Generic;
using System.Threading;
using PaunPacker.Core.Packing.Placement.Guillotine;
using PaunPacker.Core.Packing.Sorting;
using PaunPacker.Core.Types;

namespace PaunPacker.Core.Packing.Placement.MaximalRectangles
{
    /// <summary>
    /// Implementation of MaxRects algorithm
    /// </summary>
    public class MaximalRectanglesAlgorithm : IPlacementAlgorithm
    {
        /// <summary>
        /// Constructs a MaximalRectanglesAlgorithm
        /// </summary>
        /// <remarks>
        /// The MaximalRectanglesAlgorithm is only a special case of <see cref="GuillotinePlacementAlgorithm"/> with a proper parameterization
        /// </remarks>
        public MaximalRectanglesAlgorithm()
        {
            this.placementAlgorithm = new GuillotinePlacementAlgorithm(new BestAreaFitFreeRectangleExtractor(),
                new MaxRectsFreeRectangleSplitter(),
                new MaxRectsFreeRectangleSortedMerger(),
                rectOrientationSelector: null,
                new MaxRectsFreeRectanglePostProcessor());
        }

        /// <summary>
        /// Constructs a MaxRects algorithm with a given image sorter
        /// </summary>
        /// <param name="sorter">Image sorter to be used</param>
        public MaximalRectanglesAlgorithm(IImageSorter sorter)
        {
            this.placementAlgorithm = new GuillotinePlacementAlgorithm(new BestAreaFitFreeRectangleExtractor(),
                new MaxRectsFreeRectangleSplitter(),
                new MaxRectsFreeRectangleSortedMerger(),
                rectOrientationSelector: null,
                new MaxRectsFreeRectanglePostProcessor(),
                sorter);
        }

        /// <inheritdoc />
        public PackingResult PlaceRects(int width, int height, IEnumerable<PPRect> rects, CancellationToken token = default)
        {
            return placementAlgorithm.PlaceRects(width, height, rects, token);
        }

        /// <inheritdoc />
        public int Progress => placementAlgorithm?.Progress ?? 0;

        /// <inheritdoc />
        public bool ReportsProgress => placementAlgorithm?.ReportsProgress ?? false;

        /// <inheritdoc />
        public event Action<object, int> ProgressChange
        {
            add
            {
                if (placementAlgorithm != null)
                {
                    placementAlgorithm.ProgressChange += value;
                }
            }
            remove
            {
                if (placementAlgorithm != null)
                {
                    placementAlgorithm.ProgressChange -= value;
                }
            }
        }

        /// <summary>
        /// The underlying guillotine algorithm
        /// </summary>
        private readonly GuillotinePlacementAlgorithm placementAlgorithm;
    }
}
