using System;
using System.Threading;
using PaunPacker.Core.Types;
using SkiaSharp;

namespace PaunPacker.Core.ImageProcessing
{
    /// <summary>
    /// The image processor performing an Extrude operation
    /// </summary>
    public sealed class Extruder : IImageProcessor
    {
        /// <summary>
        /// Constructs a new Extruder with a specified amount of pixels to be added
        /// </summary>
        /// <param name="amount">Amount of pixels to be added</param>
        public Extruder(int amount)
        {
            Amount = amount;
        }

        /// <inheritdoc />
        public PPImage ProcessImage(PPImage input, CancellationToken token = default)
        {
            return Extrude(input, Amount, token);
        }

        /// <summary>
        /// Performs the Extrude operation
        /// </summary>
        /// <param name="input">Input image</param>
        /// <param name="token">The cancellation token</param>
        /// <remarks>The input image is copied, processed and the processed version is then returned</remarks>
        /// <returns>An image extruded by a <see cref="Amount"/> of pixels</returns>
        public PPImage Extrude(PPImage input, CancellationToken token = default)
        {
            return Extrude(input, Amount, token);
        }

        /// <summary>
        /// Performs the Trim operation with a specified amount
        /// </summary>
        /// <param name="input">Input image</param>
        /// <param name="amount">Amount of pixels to be added</param>
        /// <param name="token">The cancellation token</param>
        /// <remarks>
        /// The input image is copied, processed and the processed version is then returned
        /// Changes the <see cref="Amount"/> to <paramref name="amount"/>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Is thrown when the <paramref name="input"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">Is thrown when the <paramref name="amount"/> is negative</exception>
        /// <returns>An image extruded by a <paramref name="amount"/> of pixels, unmodified <paramref name="input"/> if cancellation was requested</returns>
        public PPImage Extrude(PPImage input, int amount, CancellationToken token = default)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException($"The {nameof(amount)} has to be non-negative integer");
            }

            if (input == null)
            {
                throw new ArgumentNullException($"The {nameof(input)} cannot be null");
            }
            
            //Do not allow to go beyond this size (because the use of Int32.MaxValue >> 1 in the packing algorithms
            //And because width > height must fit into Int32.MaxValue
            var area = (long)(input.Bitmap.Width + amount) * (input.Bitmap.Height + amount);
            if (area > int.MaxValue)
            {
                return input;
            }

            Amount = amount;
#pragma warning disable CA2000 // The resulting PPImage is the owner of the bitmap
            SKBitmap result = new SKBitmap(input.Bitmap.Width + 2 * amount, input.Bitmap.Height + 2 * amount);
#pragma warning restore CA2000 // So it should not get disposed there
            result.Erase(SKColors.Transparent);
            var srcPixels = input.Bitmap.Pixels;

            for (int i = 0; i < input.Bitmap.Height; i++)
            {
                for (int j = 0; j < input.Bitmap.Width; j++)
                {
                    result.SetPixel(j + amount, i + amount, srcPixels[i * input.Bitmap.Width + j]); //copy
                }
                //Do not ask too often, that is the reason why the check is after the inner loop
                if (token.IsCancellationRequested)
                {
                    return input;
                }
            }

            //top & bottow rows
            for (int i = 0; i < input.Bitmap.Width; i++)
            {
                for (int j = 0; j < amount; j++)
                {
                    result.SetPixel(i + amount, j, srcPixels[i]);
                    result.SetPixel(i + amount, input.Bitmap.Height + j + amount, srcPixels[srcPixels.Length - 1 - ((input.Bitmap.Width - 1) - i)]);
                }
                //Do not ask too often, that is the reason why the check is after the inner loop
                if (token.IsCancellationRequested)
                {
                    return input;
                }
            }

            //left & right columns
            for (int i = 0; i < input.Bitmap.Height; i++)
            {
                for (int j = 0; j < amount; j++)
                {
                    result.SetPixel(j, i + amount, srcPixels[i * input.Bitmap.Width]);
                    result.SetPixel(j + input.Bitmap.Width + amount, i + amount, srcPixels[i * input.Bitmap.Width + input.Bitmap.Width - 1]);
                }
                //Do not ask too often, that is the reason why the check is after the inner loop
                if (token.IsCancellationRequested)
                {
                    return input;
                }
            }

            //diags
            for (int i = 0; i < amount; i++)
            {
                for (int j = 0; j < amount; j++)
                {
                    result.SetPixel(i, j, srcPixels[0]); //left top
                    result.SetPixel(i, result.Height - j - 1, srcPixels[input.Bitmap.Width * (input.Bitmap.Height - 1)]); //left bottom
                    result.SetPixel(result.Width - i - 1, j, srcPixels[input.Bitmap.Width - 1]); //top right
                    result.SetPixel(result.Width - i - 1, result.Height - j - 1, srcPixels[input.Bitmap.Width * input.Bitmap.Height - 1]); //right bottom
                }
                //Do not ask too often, that is the reason why the check is after the inner loop
                if (token.IsCancellationRequested)
                {
                    return input;
                }
            }

            var resImage = new PPImage(result, input.ImagePath)
            {
                ImageName = input.ImageName
            };

            resImage.NoWhiteSpaceXOffset += Amount;
            resImage.NoWhiteSpaceYOffset += Amount;
            resImage.FinalWidth -= 2 * Amount;
            resImage.FinalHeight -= 2 * Amount;

            return resImage;
        }

        /// <summary>
        /// The amount of pixels to be added around the sides of the image
        /// </summary>
        public int Amount { get; set; }
    }
}
