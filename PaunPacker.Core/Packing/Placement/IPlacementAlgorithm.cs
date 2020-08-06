using System;
using System.Collections.Generic;
using System.Threading;
using PaunPacker.Core.Types;

namespace PaunPacker.Core.Packing.Placement
{
    /// <summary>
    /// Represents a placement algorithm
    /// </summary>
    /// <remarks>
    /// Generally used in situations where the W and H of the bounding box is known and the input only has to be placed into this bounding box
    /// </remarks>
    public interface IPlacementAlgorithm : IProgressReporter
    {
        /// <summary>
        /// Tries to place the <paramref name="rects"/> into a bounding box with width <paramref name="width"/>
        /// and height <paramref name="height"/>
        /// </summary>
        /// <param name="width">The width of the bounding box</param>
        /// <param name="height">The height of the bounding box</param>
        /// <param name="rects">The rectangles to be placed</param>
        /// <param name="token">The cancellation token</param>
        /// <returns>The packing result, null if the placement is not possible or if the cancellation was requested</returns>
        /// <exception cref="ArgumentNullException">Is thrown when <paramref name="rects"/> is null</exception>
        /// <remarks>If the <paramref name="rects"/> is empty then a valid result should be returned</remarks>
        PackingResult PlaceRects(int width, int height, IEnumerable<PPRect> rects, CancellationToken token = default);
    }
}
