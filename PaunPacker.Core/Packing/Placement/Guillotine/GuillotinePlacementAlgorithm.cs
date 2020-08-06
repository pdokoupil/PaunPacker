using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using PaunPacker.Core.Packing.Sorting;
using PaunPacker.Core.Types;

namespace PaunPacker.Core.Packing.Placement.Guillotine
{
    public enum SplitDirection { VERTICAL, HORIZONTAL };

    /// <summary>
    /// Compares two rectangles based on their area
    /// </summary>
    class PPRectAreaComparer : IComparer<PPRect>
    {
        /// <summary>
        /// Compares two rectangles <paramref name="x"/> and <paramref name="y"/> based on their areas
        /// </summary>
        /// <param name="x">The first comparand</param>
        /// <param name="y">The second comparand</param>
        /// <returns>Negative number if the area of <paramref name="x"/> is lass than the area of <paramref name="y"/>, 0 if they are equal, positive number otherwise</returns>
        public int Compare(PPRect x, PPRect y)
        {
            long result = (long)x.Width * x.Height - (long)y.Width * y.Height;
            if (result < 0L)
            {
                return -1;
            }
            else if (result == 0L)
            {
                return 0;
            }
            return 1;
        }

    }

    /// <summary>
    /// The fully parameterizable implementation of the Guillotine algorithm (placement algorithm)
    /// </summary>
    public class GuillotinePlacementAlgorithm : IPlacementAlgorithm
    {
        /// <summary>
        /// Constructs a new Guillotine algorithm based on the passed parameters
        /// </summary>
        /// <param name="freeRectExtractor">Free rectangle extractor (<seealso cref="IFreeRectangleExtractor"/>) used by the Guillotine algorithm</param>
        /// <param name="freeRectangleSplitter">Free rectangle splitter (<seealso cref="IFreeRectangleSplitter"/>) used by the Guillotine algorithm</param>
        /// <param name="freeRectangleMerger">Free rectangle extractor (<seealso cref="IFreeRectangleMerger"/>) used by the Guillotine algorithm</param>
        /// <param name="rectOrientationSelector">Rectangle orientation selector (<seealso cref="IRectOrientationSelector"/>) used by the Guillotine algorithm</param>
        /// <param name="freeRectanglePostProcessor">Free post processor (<seealso cref="IFreeRectanglePostProcessor"/>) used by the Guillotine algorithm</param>
        public GuillotinePlacementAlgorithm(IFreeRectangleExtractor freeRectExtractor, IFreeRectangleSplitter freeRectangleSplitter, IFreeRectangleMerger freeRectangleMerger = null, IRectOrientationSelector rectOrientationSelector = null, IFreeRectanglePostProcessor freeRectanglePostProcessor = null)
            :this(freeRectExtractor, freeRectangleSplitter, freeRectangleMerger, rectOrientationSelector, freeRectanglePostProcessor, null)
        {
        }

        /// <summary>
        /// Constructs a new Guillotine algorithm based on the passed parameters and ImageSorter
        /// </summary>
        /// <param name="freeRectExtractor">Free rectangle extractor (<seealso cref="IFreeRectangleExtractor"/>) used by the Guillotine algorithm</param>
        /// <param name="freeRectangleSplitter">Free rectangle splitter (<seealso cref="IFreeRectangleSplitter"/>) used by the Guillotine algorithm</param>
        /// <param name="freeRectangleMerger">Free rectangle extractor (<seealso cref="IFreeRectangleMerger"/>) used by the Guillotine algorithm</param>
        /// <param name="rectOrientationSelector">Rectangle orientation selector (<seealso cref="IRectOrientationSelector"/>) used by the Guillotine algorithm</param>
        /// <param name="freeRectanglePostProcessor">Free post processor (<seealso cref="IFreeRectanglePostProcessor"/>) used by the Guillotine algorithm</param>
        /// <param name="sorter">The image sorter</param>
        public GuillotinePlacementAlgorithm(IFreeRectangleExtractor freeRectExtractor, IFreeRectangleSplitter freeRectangleSplitter, IFreeRectangleMerger freeRectangleMerger, IRectOrientationSelector rectOrientationSelector, IFreeRectanglePostProcessor freeRectanglePostProcessor, IImageSorter sorter)
        {
            this.freeRectExtractor = freeRectExtractor ?? new BestAreaFitFreeRectangleExtractor();
            this.freeRectangleSplitter = freeRectangleSplitter ?? new LongerAxisGuillotineFreeRectangleSplitter();
            this.rectOrientationSelector = rectOrientationSelector ?? new DummyRectOrientationSelector();
            this.freeRectangleMerger = freeRectangleMerger ?? new GuillotineFreeRectangleSortedMerger();
            this.freeRectanglePostProcessor = freeRectanglePostProcessor;
            this.imageSorter = sorter ?? new ByHeightAndWidthImageSorterDesc();
            freeRectanglesList = new List<PPRect>();
        }

        /// <summary>
        /// Default constructor for GuillotinePlacementAlgorithm
        /// </summary>
        public GuillotinePlacementAlgorithm(): this(null, null)
        {
        }

        /// <inheritdoc />
        public PackingResult PlaceRects(int width, int height, IEnumerable<PPRect> rects, CancellationToken token = default)
        {
            Progress = 0;
            if (rects == null)
            {
                throw new ArgumentNullException($"The {nameof(rects)} cannot be null");
            }

            var sortedRects = imageSorter.SortImages(rects);

            freeRectanglesList.Clear();
            freeRectanglesList.Add(new PPRect(0,0,width, height));

            List<PPRect> packing = new List<PPRect>(sortedRects.Count());

            int inputSize = packing.Count;
            int placedRects = 0;

            foreach (var rectToPlace in sortedRects)
            {
                if (token.IsCancellationRequested)
                {
                    Progress = 0;
                    return null;
                }

                PPRect? freeRectToUse = freeRectExtractor.ExtractFreeRectangle(freeRectanglesList, rectToPlace);  //parametrizable part
                if (freeRectToUse == null) //unable to pack
                {
                    Progress = 100;
                    return null;
                }
                var rectRotated = DecideOrientationOfRect(rectToPlace);
                var freeRects = freeRectangleSplitter.SplitFreeRectangle(freeRectToUse.Value, rectToPlace);  //parametrizable part
                freeRectangleMerger.MergeFreeRectangles(freeRectanglesList, freeRects); //parametrizable part
                var rectToPlaceOriented = rectOrientationSelector.DetermineAndApplyRectOrientation(rectToPlace); //parametrizable part
                packing.Add(new PPRect(freeRectToUse.Value.Left, freeRectToUse.Value.Top, freeRectToUse.Value.Left + rectToPlace.Width, freeRectToUse.Value.Top + rectToPlace.Height, rectToPlaceOriented.Image));
                freeRectanglePostProcessor?.PostProcess(freeRectanglesList, packing.Last());
                Progress = (int)((++placedRects / (double)inputSize) * 100.0);
                ProgressChange?.Invoke(this, Progress);
            }
            return new PackingResult(width, height, packing);
        }

        /// <inheritdoc />
        public int Progress { get; private set; }

        /// <inheritdoc />
        public bool ReportsProgress => true;

        /// <inheritdoc />
        public event Action<object, int> ProgressChange;

        /// <summary>
        /// Decides the orientation of the rectangle
        /// </summary>
        /// <remarks>
        /// The rectangle could either be rotated by 90 degrees, or not rotated at all
        /// This default orientation does not rotate
        /// </remarks>
        /// <param name="rect">The rect for which the orientation should be selected</param>
        /// <returns>The <paramref name="rect"/>, possibly rotated</returns>
        private PPRect DecideOrientationOfRect(PPRect rect)
        {
            return rect;
        }

        /// <summary>
        /// List of free rectangkes
        /// </summary>
        private readonly List<PPRect> freeRectanglesList;

        /// <summary>
        /// Free rectangle extractor (<seealso cref="IFreeRectangleExtractor"/>) used by the Guillotine algorithm
        /// </summary>
        private readonly IFreeRectangleExtractor freeRectExtractor;

        /// <summary>
        /// Free rectangle splitter (<seealso cref="IFreeRectangleSplitter"/>) used by the Guillotine algorithm
        /// </summary>
        private readonly IFreeRectangleSplitter freeRectangleSplitter;

        /// <summary>
        /// Free rectangle extractor (<seealso cref="IFreeRectangleMerger"/>) used by the Guillotine algorithm
        /// </summary>
        private readonly IFreeRectangleMerger freeRectangleMerger;

        /// <summary>
        /// Rectangle orientation selector (<seealso cref="IRectOrientationSelector"/>) used by the Guillotine algorithm
        /// </summary>
        private readonly IRectOrientationSelector rectOrientationSelector;

        /// <summary>
        /// Free post processor (<seealso cref="IFreeRectanglePostProcessor"/>) used by the Guillotine algorithm
        /// </summary>
        private readonly IFreeRectanglePostProcessor freeRectanglePostProcessor;

        /// <summary>
        /// Used image sorter
        /// </summary>
        private readonly IImageSorter imageSorter;
    }
}
