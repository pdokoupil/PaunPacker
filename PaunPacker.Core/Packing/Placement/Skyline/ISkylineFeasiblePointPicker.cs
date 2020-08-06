using System;
using System.Collections.Generic;
using PaunPacker.Core.Types;

namespace PaunPacker.Core.Packing.Placement.Skyline
{
    /// <summary>
    /// Allows to pick a feasible point from a sequence of feasible points
    /// </summary>
    public interface ISkylineFeasiblePointPicker
    {
        /// <summary>
        /// Returns an index of a feasible point from a sequence of feasible points <paramref name="feasiblePoints"/> where a <paramref name="rectToBePacked"/> should be placed
        /// </summary>
        /// <param name="feasiblePoints">A sequence of feasible points</param>
        /// <param name="rectToBePacked">A rectangle that will be placed</param>
        /// <remarks>
        /// In order to avoid namespace pollution with custom/helper type like FeasiblePoints, tuples are used instead
        /// </remarks>
        /// <exception cref="ArgumentNullException">Is thrown when the <paramref name="feasiblePoints"/> is null</exception>
        /// <returns>An index of the feasible point</returns>
        int GetIndexOfOneOfFeasiblePoints(IEnumerable<(int x, int y)> feasiblePoints, PPRect rectToBePacked);
    }
}
