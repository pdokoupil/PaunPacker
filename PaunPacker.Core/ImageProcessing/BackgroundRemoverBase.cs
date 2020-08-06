using System;
using System.Threading;
using PaunPacker.Core.Types;

namespace PaunPacker.Core.ImageProcessing
{
    /// <summary>
    /// Base class for implementations of background remove image processor
    /// </summary>
    public abstract class BackgroundRemoverBase : IImageProcessor
    {
        /// <inheritdoc />
        public PPImage ProcessImage(PPImage input, CancellationToken token = default)
        {
            return RemoveBackground(input, token);
        }

        /// <summary>
        /// Performs the remove background operation
        /// </summary>
        /// <param name="input">Input image</param>
        /// <param name="token">The cancellation token</param>
        /// <remarks>The input image is copied, processed and the processed version is then returned</remarks>
        /// <exception cref="ArgumentNullException">Is thrown when the <paramref name="input"/> is null</exception>
        /// <returns>An image with background removed, unmodified <paramref name="input"/> when cancellation was requested</returns>
        public abstract PPImage RemoveBackground(PPImage input, CancellationToken token = default);
    }
}
