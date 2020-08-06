using System;
using System.Collections.Generic;
using System.Threading;
using PaunPacker.Core.Types;

namespace PaunPacker.Core.Packing.MBBF
{
    /// <summary>
    /// Represents the minimum bounding box finder
    /// </summary>
    /// <remarks>
    /// The "actual, top-level" packer
    /// Generally used in situations when neither W nor H of the minimum bounding box is known, thus the space (potentially somehow reduced space) of possible W x H bounding box
    /// has to be searched and the bounding box with minimum area = W*H is seeked.
    /// </remarks>
    public interface IMinimumBoundingBoxFinder : IProgressReporter
    {
        /// <summary>
        /// Finds the minimum bounding box for the rectangles given by <paramref name="rects"/>
        /// </summary>
        /// <param name="rects">The rectangle for which the bounding box is searched for</param>
        /// <param name="cancellationToken">Cancellation token to allow cancellation of the search</param>
        /// <returns><see cref="PackingResult"/> representing the minimum bounding box</returns>
        /// <exception cref="ArgumentNullException">Should be thrown when the <paramref name="rects"/> is null</exception>
        /// <remarks>When <paramref name="rects"/> is empty, the method should return valid <see cref="PackingResult"/></remarks>
        PackingResult FindMinimumBoundingBox(IEnumerable<PPRect> rects, CancellationToken cancellationToken = default);
    }
}
