using System;
using System.Collections.Generic;
using System.Linq;
using PaunPacker.Core.Types;

namespace PaunPacker.Core.Packing.Placement.Skyline
{
    /// <summary>
    /// Implementation of ISkylineFeasiblePointPicker selecting randomly
    /// </summary>
    public class RandomPointPicker : ISkylineFeasiblePointPicker
    {
        /// <summary>
        /// Constructs a RandomPointPicker
        /// </summary>
        public RandomPointPicker()
        {
            rnd = new Random();
        }

        /// <summary>
        /// Constructs a RandomPointPicker using a given seed
        /// </summary>
        /// <param name="seed">Seed for the random generator</param>
        public RandomPointPicker(int seed)
        {
            rnd = new Random(seed);
        }

        /// <inheritdoc />
        /// <remarks>Selects the feasible point randomly</remarks>
        public int GetIndexOfOneOfFeasiblePoints(IEnumerable<(int x, int y)> feasiblePoints, PPRect rectToBePacked)
        {
            if (feasiblePoints == null)
            {
                throw new ArgumentNullException($"The {nameof(feasiblePoints)} cannot be null");
            }

            int numberOfPoints = feasiblePoints.Count();
            int randomIndex = rnd.Next(0, numberOfPoints);
            return randomIndex;
        }

        /// <summary>
        /// The random generator used for feasible point selection
        /// </summary>
        private readonly Random rnd;
    }
}
