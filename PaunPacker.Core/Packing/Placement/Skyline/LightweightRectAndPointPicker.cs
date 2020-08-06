using System.Collections.Generic;
using System.Linq;
using PaunPacker.Core.Types;

namespace PaunPacker.Core.Packing.Placement.Skyline
{
    /// <summary>
    /// Very simple implementation of ISkylineRectAndPointPicker that simply selects the first rectangle in the sequence and first of the feasible points for that rectangle
    /// </summary>
    public class LightweightRectAndPointPicker : ISkylineRectAndPointPicker
    {
        /// <summary>
        /// Constructs a new LightweightRectAndPointPicker
        /// </summary>
        public LightweightRectAndPointPicker()
        {
            feasiblePointPicker = new BottomLeftSkylinePointPicker();
        }

        /// <inheritdoc />
        public (PPRect Rect, int FeasiblePointIndex) PickRectAndPoint(IEnumerable<PPRect> rects, IEnumerable<(int X, int Y, int verticalEdgeLength, int horizontalEdgeLength)> feasiblePoints, int width, int height)
        {
            return (rects.First(), feasiblePointPicker.GetIndexOfOneOfFeasiblePoints(feasiblePoints.Where(x => IsReallyFeasibleForGivenRect(x.X, x.Y, width, height, rects.First())).Select(feasiblePoint => (feasiblePoint.X, feasiblePoint.Y)), rects.First()));
        }

        /// <summary>
        /// Checks whether the point with coordinates <paramref name="x"/>, <paramref name="y"/> within a bounding box with dimensions <paramref name="width"/> x <paramref name="height"/> is feasible for a rectangle <paramref name="rect"/>
        /// </summary>
        /// <param name="x">The X coordinate of the feasible point</param>
        /// <param name="y">The Y coordinate of the feasible point</param>
        /// <param name="width">The width of the bounding box</param>
        /// <param name="height">The height of the bounding box</param>
        /// <param name="rect">The rectangle that should be tested</param>
        /// <returns>True if the point is feasible, false otherwise</returns>
        private bool IsReallyFeasibleForGivenRect(int x, int y, int width, int height, PPRect rect)
        {
            //we can do this because of the property that for an image p, being packed, all of the images i that are already packed, i must be to the left or below
            //so basically when placing image to feasiblePoint, there will be no images to the right (at the same y)
            return (x + rect.Width <= width && y + rect.Height <= height);
        }

        /// <summary>
        /// Internally used for feasible point picking
        /// </summary>
        private readonly ISkylineFeasiblePointPicker feasiblePointPicker;
    }
}
