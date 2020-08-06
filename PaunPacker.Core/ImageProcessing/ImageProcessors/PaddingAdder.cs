using System;
using System.Threading;
using PaunPacker.Core.Types;
using SkiaSharp;

namespace PaunPacker.Core.ImageProcessing.ImageProcessors
{
    /// <summary>
    /// Class represeting an add padding operation
    /// </summary>
    public class PaddingAdder : IImageProcessor
    {
        /// <summary>
        /// Constructs a new padding adder
        /// </summary>
        /// <param name="amount">The amount of transparent pixels to add around the borders</param>
        public PaddingAdder(int amount)
        {
            Amount = amount;
        }

        /// <summary>
        /// Adds a padding, i.e. <see cref="Amount"/> of transparent pixels to each side of the <paramref name="input"/> image's border
        /// </summary>
        /// <param name="input">The input image</param>
        /// <param name="token">The cancellation token</param>
        /// <returns>The copy of the input texture with added padding or unmodified input texture if the cancellation was requested</returns>
        public PPImage AddPadding(PPImage input, CancellationToken token = default)
        {
            return AddPadding(input, Amount, token);
        }

        /// <summary>
        /// Adds a padding, i.e. <paramref name="amount"/> of transparent pixels to each side of the <paramref name="input"/> image's border
        /// </summary>
        /// <param name="input">The input image</param>
        /// <param name="amount">Amount of transparent pixels to add</param>
        /// <param name="token">The cancellation token</param>
        /// <remarks>Changes the <see cref="Amount"/> to <paramref name="amount"/></remarks>
        /// <returns>The copy of the input texture with added padding or unmodified input texture if the cancellation was requested</returns>
        public PPImage AddPadding(PPImage input, int amount, CancellationToken token = default)
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

            //for (int i = 0; i < Amount; i++)
            //{
            //    if (token.IsCancellationRequested)
            //    {
            //        return input;
            //    }

            //    for (int x = 0; x < input.Bitmap.Width; x++)
            //    {
            //        //The strip above
            //        result.SetPixel(x, i, SKColors.Transparent);
            //        //The strip below
            //        result.SetPixel(x, input.Bitmap.Height - 1 + i, SKColors.Transparent);
            //    }

            //    if (token.IsCancellationRequested)
            //    {
            //        return input;
            //    }

            //    for (int y = 0; y < input.Bitmap.Height; y++)
            //    {
            //        //The strip to the left
            //        result.SetPixel(i, y, SKColors.Transparent);
            //        //The strip to the right
            //        result.SetPixel(input.Bitmap.Width - 1 + i, y, SKColors.Transparent);
            //    }
            //}

            var resImage = new PPImage(result, input.ImagePath)
            {
                ImageName = input.ImageName,
            };

            resImage.NoWhiteSpaceXOffset += Amount;
            resImage.NoWhiteSpaceYOffset += Amount;
            resImage.FinalWidth -= 2 * Amount;
            resImage.FinalHeight -= 2 * Amount;

            return resImage;
        }

        /// <inheritdoc />
        public PPImage ProcessImage(PPImage input, CancellationToken token = default)
        {
            return AddPadding(input, token);
        }

        /// <summary>
        /// The amount of transparent pixels to padd around the input image
        /// </summary>
        public int Amount { get; private set; }
    }
}
