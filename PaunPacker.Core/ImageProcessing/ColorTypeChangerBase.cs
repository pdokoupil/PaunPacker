using PaunPacker.Core.Types;
using SkiaSharp;
using System;
using System.Threading;

namespace PaunPacker.Core.ImageProcessing
{
    /// <summary>
    /// Base class for implementations of ColorType changer image processor
    /// </summary>
    public abstract class ColorTypeChangerBase : IImageProcessor
    {
        /// <summary>
        /// Constructor initializing a target color type
        /// </summary>
        /// <param name="type">The target color type</param>
        public ColorTypeChangerBase(SKColorType type)
        {
            NewColorType = type;
        }
                
        /// <inheritdoc />
        public PPImage ProcessImage(PPImage input, CancellationToken token = default)
        {
            return ChangeColorType(input, NewColorType, token);
        }

        /// <summary>
        /// Performs the ColorType change operation
        /// </summary>
        /// <param name="input">Input image</param>
        /// <param name="token">The cancellation token</param>
        /// <remarks>
        /// The input image is copied, processed and the processed version is then returned
        /// The target color type is given by the <see cref="NewColorType"/>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Is thrown when the <paramref name="input"/> is null</exception>
        /// <returns>An image with a color type changed to the <see cref="NewColorType"/>, null when the cancellation was requested</returns>
        public abstract PPImage ChangeColorType(PPImage input, CancellationToken token = default);

        /// <summary>
        /// Performs the ColorType change operation
        /// </summary>
        /// <param name="input">Input image</param>
        /// <param name="targetColorType">The target color type</param>
        /// <param name="token">The cancellation token</param>
        /// <remarks>The input image is copied, processed and the processed version is then returned</remarks>
        /// <exception cref="ArgumentNullException">Is thrown when the <paramref name="input"/> is null</exception>
        /// <returns>An image with a color type changed to the target color type, unmodified <paramref name="input"/> when the cancellation was requested</returns>
        public abstract PPImage ChangeColorType(PPImage input, SKColorType targetColorType, CancellationToken token = default);

        /// <summary>
        /// Determines the new color type for the image
        /// </summary>
        public SKColorType NewColorType { get; set; }
    }

}
