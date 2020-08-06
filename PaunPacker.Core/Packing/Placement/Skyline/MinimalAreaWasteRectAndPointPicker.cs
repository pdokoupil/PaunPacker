using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using PaunPacker.Core.Types;

namespace PaunPacker.Core.Packing.Placement.Skyline
{
    /// <summary>
    /// Implementation of ISkylineRectAndPointPicker that selects based on the mininal area waste
    /// </summary>
    public class MinimalAreaWasteRectAndPointPicker : ISkylineRectAndPointPicker
    {
        /// <summary>
        /// Selects a pair (rect, index of a feasible point) for which the wasted area is minimal
        /// </summary>
        /// <param name="rects">Rectangles to be placed</param>
        /// <param name="feasiblePoints">Feasible points</param>
        /// <param name="width">Width of the bounding box</param>
        /// <param name="height">Height of the bounding box</param>
        /// <returns>The pair (rect, index of a feasible point) for which the wasted area is minimal</returns>
        public (PPRect Rect, int FeasiblePointIndex) PickRectAndPoint(IEnumerable<PPRect> rects, IEnumerable<(int X, int Y, int verticalEdgeLength, int horizontalEdgeLength)> feasiblePoints, int width, int height)
        {
            var tmpResult = default(((PPRect rect, int index) rectIndex, ((int X, int Y, int verticalEdgeLength, int horizontalEdgeLength) feasiblePoint, int index) feasiblePointIndex));

            var pairsOfRectAndFeasiblePoint = from rectIndexPair in rects.Select<PPRect, (PPRect Rect, int Index)>((x, index) => (x, index))
                                              from feasiblePointPair in feasiblePoints.Select((y, index2) => (y, index2)).Where(feasibleAndIndex => IsReallyFeasibleForGivenRect(feasibleAndIndex.y.X, feasibleAndIndex.y.Y, width, height, rectIndexPair.Rect))
                                              select (rectIndexPair, ((feasiblePointPair.y.X, feasiblePointPair.y.Y, feasiblePointPair.y.verticalEdgeLength, feasiblePointPair.y.horizontalEdgeLength), feasiblePointPair.index2));
            
            var pairsWithMinimalAreaWaste = pairsOfRectAndFeasiblePoint.MinBy<((PPRect rect, int index) rectIndex, ((int X, int Y, int verticalEdgeLength, int horizontalEdgeLength) point, int index) feasiblePoint), long>(pair => CalculateWastedArea(ref pair.rectIndex.rect, pair.feasiblePoint.point));

            if (pairsOfRectAndFeasiblePoint.Count() < rects.Count()) //tighter upperbound would be nice (something like if there is rect without feasible point .. but we need to keep performance)
            {
                return (default(PPRect), -1);
            }

            if (pairsWithMinimalAreaWaste.Count() > 1)
            {
                var pairsWithMaxGn = pairsWithMinimalAreaWaste.MaxBy(pair => CalculateGN(ref pair.rectIndex.rect, pair.feasiblePoint.point.verticalEdgeLength, pair.feasiblePoint.point.horizontalEdgeLength));

                if (pairsWithMaxGn.Count() > 1)
                {
                    var pairsWithMinLeft = pairsWithMaxGn.MinBy(pair => pair.feasiblePoint.point.X).MinBy(pair => pair.feasiblePoint.point.Y);
                    if (pairsWithMinLeft.Count() > 1)
                    {
                        tmpResult = pairsWithMinLeft.MinBy(pair => pair.rectIndex.index).First();
                        return (tmpResult.rectIndex.rect, tmpResult.feasiblePointIndex.index);
                    }
                    else
                    {
                        tmpResult = pairsWithMinLeft.First();
                        return (tmpResult.rectIndex.rect, tmpResult.feasiblePointIndex.index);
                    }
                }
                else
                {
                    tmpResult = pairsWithMaxGn.First();
                    return (tmpResult.rectIndex.rect, tmpResult.feasiblePointIndex.index);
                }
            }
            else
            {
                tmpResult = pairsWithMinimalAreaWaste.First();
                return (tmpResult.rectIndex.rect, tmpResult.feasiblePointIndex.index);
            }
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
            //This can be done because of the property that for a rectangle R being placed, all of the rectangles Q that are already placed, Q must be to the left or below R
            //so basically when placing a rectangle to a feasiblePoint, there will be no rectangle to the right (at the same y)
            return (x + rect.Width <= width && y + rect.Height <= height);
        }

        /// <summary>
        /// Calculates the "goodness number" value of a given rectangle
        /// </summary>
        /// <param name="rect">The rectangle for which the GN will be calculated</param>
        /// <param name="verticalEdgeLength">The length of the vertical edge pointing out (to the top) from the feasible point</param>
        /// <param name="horizontalEdgeLength">The length of the horizontal edge pointing out (to the right) from the feasible point</param>
        /// <returns>The goodness number</returns>
        private int CalculateGN(ref PPRect rect, int verticalEdgeLength, int horizontalEdgeLength)
        {
            if (rect.Width == horizontalEdgeLength && rect.Height == verticalEdgeLength)
            {
                return 2;
            }
            else if (rect.Width == horizontalEdgeLength || rect.Height == verticalEdgeLength)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Calculates the wasted area
        /// </summary>
        /// <param name="rect">The rectangle to be placed</param>
        /// <param name="feasiblePoint">The feasible point where the rectangle should be placed</param>
        /// <returns>The wasted area that would result from placing the <paramref name="rect"/> at the feasible point <paramref name="feasiblePoint"/></returns>
        private long CalculateWastedArea(ref PPRect rect, (int X, int Y, int verticalEdgeLength, int horizontalEdgeLength) feasiblePoint)
        {
            return ((long)feasiblePoint.verticalEdgeLength - rect.Height) * (feasiblePoint.horizontalEdgeLength - rect.Width);
        }

    }
}
