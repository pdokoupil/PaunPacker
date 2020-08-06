using System;
using System.Collections.Generic;
using System.Threading;
using PaunPacker.Core.Packing.Sorting;
using PaunPacker.Core.Types;

namespace PaunPacker.Core.Packing.Placement.Guillotine
{
    /// <summary>
    /// Parametrized/Specific implementation of the Guillotine algorithm called BestAreaFit
    /// </summary>
    /// <remarks>
    /// Free rectangles are selected based on the "best area fit" using the <see cref="BestAreaFitFreeRectangleExtractor"/>
    /// </remarks>
    public class GuillotineBestAreaFitAlgorithm : IPlacementAlgorithm
    {
        /// <summary>
        /// Constructs a new GuillotineBestAreaFitAlgorithm
        /// </summary>
        public GuillotineBestAreaFitAlgorithm()
        {
            this.placementAlgorithm = new GuillotinePlacementAlgorithm(new BestAreaFitFreeRectangleExtractor(), new LongerAxisGuillotineFreeRectangleSplitter());
        }

        /// <summary>
        /// Constructs a new GuillotineBestAreaFitAlgorithm with a given image sorter
        /// </summary>
        /// <param name="imageSorter">Image sorter to be used</param>
        public GuillotineBestAreaFitAlgorithm(IImageSorter imageSorter)
        {
            this.placementAlgorithm = new GuillotinePlacementAlgorithm(new BestAreaFitFreeRectangleExtractor(), new LongerAxisGuillotineFreeRectangleSplitter(), null, null, null, imageSorter);
        }

        /// <inheritdoc />
        public PackingResult PlaceRects(int width, int height, IEnumerable<PPRect> rects, CancellationToken token = default)
        {
            return placementAlgorithm.PlaceRects(width, height, rects, token);
        }

        /// <summary>
        /// The guillotine placement algorithm used by this version of placement algorithm
        /// </summary>
        /// <remarks>
        /// The only difference between the current implementation and GuillotinePlacementAlgorithm is in its parameterization
        /// </remarks>
        private readonly GuillotinePlacementAlgorithm placementAlgorithm;

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
    }
}
