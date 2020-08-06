using PaunPacker.Core.Types;
using SkiaSharp;
using System;
using System.Threading;

namespace PaunPacker.Core.ImageProcessing.ImageProcessors
{
    /// <summary>
    /// Default implementation of the <see cref="TrimmerBase"/>
    /// </summary>
    /// <remarks>Trims transparent pixels from the border of the input image</remarks>
    public class Trimmer : TrimmerBase
    {
        /// <summary>
        /// Constructs a new Trimmer with alphaTolerance equal to 0
        /// </summary>
        public Trimmer() : this(0)
        {

        }

        /// <summary>
        /// Constructs a new Trimmer with a given <paramref name="alphaTolerance"/>
        /// </summary>
        /// <param name="alphaTolerance">The alpha tolerance to be used when trimming</param>
        public Trimmer(int alphaTolerance)
        {
            this.alphaTolerance = alphaTolerance;
        }

        /// <inheritdoc />
        public override PPImage Trim(PPImage input, CancellationToken token = default)
        {
            if (input == null)
            {
                throw new ArgumentNullException($"The {nameof(input)} cannot be null");
            }

            int origWidth = input.Bitmap.Width;
            int origHeight = input.Bitmap.Height;

            int left = 0, right = input.Bitmap.Width, top = input.Bitmap.Height, bottom = 0;
            var pixels = input.Bitmap.Pixels;
            bool topTransparent = true, bottomTransparent = true, leftTransparent = true, rightTransparent = true;
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    return input;
                }
                //process top and bottom lines (horizontal)
                for (int i = left; i < right; i++)
                {
                    if (pixels[bottom * input.Bitmap.Width + i].Alpha > alphaTolerance)
                        bottomTransparent = false;
                    if (pixels[(top - 1) * input.Bitmap.Width + i].Alpha > alphaTolerance)
                        topTransparent = false;
                }

                if (bottomTransparent)
                    bottom++;
                if (topTransparent)
                    top--;

                //process vertical lines
                for (int i = bottom; i < top; i++)
                {
                    if (pixels[i * input.Bitmap.Width + left].Alpha > alphaTolerance)
                        leftTransparent = false;
                    if (pixels[i * input.Bitmap.Width + (right - 1)].Alpha > alphaTolerance)
                        rightTransparent = false;
                }
                if (leftTransparent)
                    left++;
                if (rightTransparent)
                    right--;

                if (!(bottomTransparent || topTransparent || leftTransparent || rightTransparent))
                    break;
                if (left >= (input.Bitmap.Width - 1) && right <= 0 && top <= 0 && bottom >= (input.Bitmap.Height - 1))
                {
                    return new PPImage();
                }
            }
            SKBitmap result = new SKBitmap();
            input.Bitmap.ExtractSubset(result, new SKRectI(left, bottom, right, top));
            var resultImage = new PPImage(result, input.ImagePath)
            {
                ImageName = input.ImageName,
                OriginalWidth = origWidth,
                OriginalHeight = origHeight,
                OffsetX = left,
                OffsetY = bottom //actually top in "graphic field jargon" (i.e. the lower coordinates)
            };
            return resultImage;
        }

        /// <summary>
        /// A number between 0 and 255
        /// Pixels with alpha value lower than this tolerance are treated as transparent
        /// </summary>
        private readonly int alphaTolerance;
    }

}
