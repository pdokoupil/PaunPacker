using System.Collections.Generic;
using PaunPacker.Core.Types;

namespace PaunPacker.Core.Packing.Placement.Skyline
{
    /// <summary>
    /// Allows to pick a rectangle and index of feasible point where the rectangle should be placed
    /// </summary>
    public interface ISkylineRectAndPointPicker
    {
        /// <summary>
        /// Returns a selected rectangle and selected feasible point where the selected rectangle should be placed
        /// </summary>
        /// <param name="rects">The input (to be placed) rectangles</param>
        /// <param name="feasiblePoints">Sequence of feasible points</param>
        /// <param name="width">Width of the bounding box</param>
        /// <param name="height">Height of the bounding box</param>
        /// <returns>The rectangle from <paramref name="rects"/> and a point from <paramref name="feasiblePoints"/> where the selected rectangle should be placed</returns>
        (PPRect Rect, int FeasiblePointIndex) PickRectAndPoint(IEnumerable<PPRect> rects, IEnumerable<(int X, int Y, int verticalEdgeLength, int horizontalEdgeLength)> feasiblePoints, int width, int height);
    }
}
