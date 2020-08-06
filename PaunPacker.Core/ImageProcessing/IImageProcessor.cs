using System;
using System.Threading;
using PaunPacker.Core.Types;

namespace PaunPacker.Core.ImageProcessing
{
    /// <summary>
    /// Interface representing an image processor
    /// </summary>
    public interface IImageProcessor
    {
        /// <summary>
        /// Takes an image on the input and returns a modified (processed) copy of it on it's output
        /// </summary>
        /// <param name="input">The input image</param>
        /// <param name="token">The cancellation token</param>
        /// <exception cref="ArgumentNullException">Is thrown when the <paramref name="input"/> is null</exception>
        /// <remarks>Whenever a cancellation is requested via the <paramref name="token"/>, the corresponding image processor should stop in a consistent state, returning the input, unmodified image</remarks>
        /// <returns>Modified image</returns>
        PPImage ProcessImage(PPImage input, CancellationToken token = default);   
    }
}
