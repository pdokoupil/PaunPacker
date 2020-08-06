using System;
using System.Collections.Generic;
using System.Linq;
using PaunPacker.Core.Types;

namespace PaunPacker.Core.Packing.Placement.Skyline
{
    /// <summary>
    /// Implementation of ISkylineFeasiblePointPicker that selects a feasible point that is most to the left and to the bottom
    /// </summary>
    public class BottomLeftSkylinePointPicker : ISkylineFeasiblePointPicker
    {
        /// <inheritdoc />
        /// <remarks>Selects a feasible point from <paramref name="feasiblePoints"/> which is most to the left and to the bottom</remarks>
        public int GetIndexOfOneOfFeasiblePoints(IEnumerable<(int x, int y)> feasiblePoints, PPRect rectToBePacked)
        {
            if (feasiblePoints == null)
            {
                throw new ArgumentNullException($"The {nameof(feasiblePoints)} cannot be null");
            }

            if (!feasiblePoints.Any())
            {
                return -1;
            }

            (int x, int y) currBestPoint = feasiblePoints.First();
            int index = 0;
            int currBestIndex = index;

            foreach ((int x, int y) feasiblePoint in feasiblePoints)
            {
                if (feasiblePoint.y < currBestPoint.y || (feasiblePoint.y == currBestPoint.y && feasiblePoint.x < currBestPoint.x))
                {
                    currBestPoint = feasiblePoint;
                    currBestIndex = index;
                }
                index++;
            }
            return currBestIndex;
        }
    }
}
