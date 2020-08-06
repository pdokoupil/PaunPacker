using System;
using System.Threading;
using PaunPacker.Core.Types;

namespace PaunPacker.Core.ImageProcessing
{
    /// <summary>
    /// Base class for implementations of Trim image processor
    /// </summary>
    public abstract class TrimmerBase : IImageProcessor
    {
        /// <inheritdoc />
        public PPImage ProcessImage(PPImage input, CancellationToken token = default)
        {
            return Trim(input, token);
        }

        /// <summary>
        /// Performs the Trim operation
        /// </summary>
        /// <param name="input">Input image</param>
        /// <param name="token">The cancellation token</param>
        /// <remarks>The input image is copied, processed and the processed version is then returned</remarks>
        /// <exception cref="ArgumentNullException">Is thrown when the <paramref name="input"/> is null</exception>
        /// <returns>Trimmed image, null if the cancellation was requested</returns>
        public abstract PPImage Trim(PPImage input, CancellationToken token = default);
    }
}
