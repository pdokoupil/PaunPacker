using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace PaunPacker.Core.Types
{
    /// <summary>
    /// Represents an image
    /// </summary>
    /// <remarks>
    /// Wrapper around <see cref="SKBitmap"/> with some additional members
    /// About sizes: Width is the width of the "raw" bitmap, i.e. including any added pixels, exluding removed pixels .. i.e. the actual (real) width
    ///              OriginalWidth is the width before image processing i.e. the width of the corresponding texture on the disk
    ///              FinalWidth is the width that will be written to the metadata (which might differ from the previous two)
    ///              Similarly for the Height
    /// </remarks>
    public sealed class PPImage : IDisposable
    {
        /// <summary>
        /// Constructs an empty image
        /// </summary>
        public PPImage()
        {

        }

        /// <summary>
        /// Constructs an image from a given SKBitmap
        /// </summary>
        /// <param name="skBmp">The SKBitmap from which an image should be created</param>
        public PPImage(SKBitmap skBmp) : this(skBmp, null)
        {
            
        }

        /// <summary>
        /// Constructs an image from a given SKBitmap and obtains it's file name from a given pat
        /// </summary>
        /// <param name="skBmp">The SKBitmap from which an image should be created</param>
        /// <param name="filePath">The path of the image</param>
        /// <param name="isRotated">Whether the image is rotated by 90 degrees</param>
        public PPImage(SKBitmap skBmp, string filePath, bool isRotated = false)
        {
            if (skBmp == null)
            {
                throw new ArgumentNullException($"The {nameof(skBmp)} cannot be null");
            }

            Bitmap = skBmp.Copy();
            ImagePath = filePath;
            ImageName = System.IO.Path.GetFileName(filePath);
            OriginalWidth = FinalWidth = skBmp.Width;
            OriginalHeight = FinalHeight = skBmp.Height;
            NoWhiteSpaceXOffset = NoWhiteSpaceYOffset = 0;
            IsRotated = isRotated;
            Aliases = Enumerable.Empty<(string AliasedName, string AliasedPath)>();
            bitmapHash = new Lazy<string>(() =>
            {
                using (var sha = new SHA256Managed())
                {
                    return Convert.ToBase64String(sha.ComputeHash(Bitmap.Bytes));
                }

            }, true);
        }

        /// <summary>
        /// Constructor that creates an image which aliases the other images from the sequence of identical images
        /// </summary>
        /// <remarks>Duplicates the first image of the sequence and makes the rest as it's aliases</remarks>
        /// <param name="sameImages">The images that are same (the have same BitmapHash <see cref="BitmapHash"/></param>
        public PPImage(IEnumerable<PPImage> sameImages)
        {
            var imageToDuplicate = sameImages.First();
            Bitmap = imageToDuplicate.Bitmap.Copy();
            ImagePath = imageToDuplicate.ImagePath;
            ImageName = imageToDuplicate.ImageName;
            OriginalHeight = imageToDuplicate.OriginalHeight;
            OriginalWidth = imageToDuplicate.OriginalWidth;
            FinalHeight = imageToDuplicate.FinalHeight;
            FinalWidth = imageToDuplicate.FinalWidth;
            IsRotated = imageToDuplicate.IsRotated;
            NoWhiteSpaceXOffset = imageToDuplicate.NoWhiteSpaceXOffset;
            NoWhiteSpaceYOffset = imageToDuplicate.NoWhiteSpaceYOffset;
            OffsetX = imageToDuplicate.OffsetX;
            OffsetY = imageToDuplicate.OffsetY;
            Aliases = sameImages.Skip(1).Select(x => (x.ImageName, x.ImagePath));
        }

        /// <summary>
        /// Contains file names and file paths of all the same (aliased) images
        /// </summary>
        public IEnumerable<(string AliasedName, string AliasedPath)> Aliases { get; private set; }

        /// <summary>
        /// The underlying SKBitmap
        /// </summary>
        public SKBitmap Bitmap { get; private set; }

        /// <summary>
        /// Determines whether the image is rotated (relative to the original image)
        /// </summary>
        public bool IsRotated { get; private set; }

        /// <summary>
        /// The image path
        /// </summary>
        public string ImagePath
        {
            get; private set;
        }

        /// <summary>
        /// The image name
        /// </summary>
        public string ImageName
        {
            get; set; //set not private, because of possible aliases (alias)?
        }

        /// <summary>
        /// Determines X offset of the current image inside an image with size originalWidth x originalHeight
        /// </summary>
        /// <remarks>
        /// Taken from top-left corner
        /// When the OriginalWidth == Width then the X offset should be 0
        /// </remarks>
        public int OffsetX
        {
            get; set;
        }

        /// <summary>
        /// Determines Y offset of the current image inside an image with size originalWidth x originalHeight
        /// </summary>
        /// <remarks>
        /// Taken from top-left corner
        ///  When the OriginalHeight == Height then the Y offset should be 0
        /// </remarks>
        public int OffsetY
        {
            get; set;
        }

        /// <summary>
        /// Rotates the image by 90 degrees
        /// </summary>
        /// <remarks>
        /// Only rotation by 90 degrees affect
        /// If the image is not rotated (i.e. it has the same orientation as the original (with the given path)) then the rotation performs clock-wise rotation by 90 degrees
        /// Otherwise if the image is already rotated, the rotation is taken by -90 degrees to turn it into the original position
        /// This ensures that it is sufficient to only store flag "rotate" in the metadata and not additional degrees
        /// More info at: https://docs.microsoft.com/cs-cz/xamarin/xamarin-forms/user-interface/graphics/skiasharp/transforms/rotate
        /// </remarks>
        /// <returns>The rotated image</returns>
        public PPImage RotateBy90Degrees()
        {
            SKBitmap rotatedBitmap;

            if (IsRotated)
            {
                rotatedBitmap = RotateClockWiseByDegrees(Bitmap, -90.0f);
                IsRotated = false;
            }
            else
            {
                rotatedBitmap = RotateClockWiseByDegrees(Bitmap, 90.0f);
                IsRotated = true;
            }

            var result = new PPImage(rotatedBitmap, ImagePath, true)
            {
                ImageName = this.ImageName
            };
            rotatedBitmap.Dispose();
            return result;
        }

        /// <summary>
        /// Rotates a SKBitmap clock-wise by <paramref name="n"/> degrees
        /// </summary>
        /// <param name="bmp">The SKBitmap to be rotated</param>
        /// <param name="n">The amount of degrees for clock-wise rotation</param>
        /// <remarks>Both negative degrees and degrees greater than 360 are allowed</remarks>
        /// <returns>The rotated SKBitmap</returns>
        private static SKBitmap RotateClockWiseByDegrees(SKBitmap bmp, float n)
        {
            SKBitmap result = new SKBitmap(bmp.Height, bmp.Width);
            using (var surface = new SKCanvas(result))
            {
                surface.Translate(result.Width, 0);
                surface.RotateDegrees(n);
                surface.DrawBitmap(bmp, 0, 0);
            }
            return result;
        }

        /// <summary>
        /// The original width of the image (i.e. before possibly changing the bitmap in the image processing)
        /// </summary>
        /// <remarks>Is used when creating the metadata</remarks>
        public int OriginalWidth
        {
            get; set;
        }

        /// <summary>
        /// The original height of the image (i.e. before possibly changing the bitmap in the image processing)
        /// </summary>
        /// <remarks>Is used when creating the metadata</remarks>
        public int OriginalHeight
        {
            get; set;
        }

        // About sizes: Width is the width of the "raw" bitmap, i.e. including any added pixels, exluding removed pixels .. i.e. the actual (real) width
        //              OriginalWidth is the width before image processing i.e. the width of the corresponding texture on the disk
        //              FinalWidth is the width that will be written to the metadata (which might differ from the previous two)
        //              Similarly for the Height
        // </remarks>
        //
        
        /// <summary>
        /// The final width that will be stored in the metadata
        /// </summary>
        public int FinalWidth
        {
            get;
            set;
        }

        /// <summary>
        /// The final height that will be stored in the metadata
        /// </summary>
        public int FinalHeight
        {
            get;
            set;
        }

        /// <summary>
        /// The "final" X coordinate that will be used when creating the texture atlas
        /// </summary>
        /// <remarks>
        /// Used by for example Extruder / PaddingAdder, which adds some pixels to the texture but in the metadata, these pixels should not be considered as a part of the texture
        /// Not handled by the OffsetX beacuse the OFfsetX is responsible only for whitespace (transparent pixels) (same approach is taken by TexturePacker when creating LibGDX metadata)
        /// </remarks>
        public int NoWhiteSpaceXOffset
        {
            get; set;
        }

        /// <summary>
        /// The "final" Y coordinate that will be used when creating the texture atlas
        /// </summary>
        /// <remarks>
        /// Used by for example Extruder / PaddingAdder, which adds some pixels to the texture but in the metadata, these pixels should not be considered as a part of the texture
        /// Not handled by the OffsetY beacuse the OFfsetY is responsible only for whitespace (transparent pixels) (same approach is taken by TexturePacker when creating LibGDX metadata)
        /// </remarks>
        public int NoWhiteSpaceYOffset
        {
            get; set;
        }

        /// <summary>
        /// Implementation of the IDisposable interface
        /// </summary>
        public void Dispose()
        {
            Bitmap.Dispose();
        }

        /// <summary>
        /// The hash of the bitmap pixels (bytes)
        /// </summary>
        public string BitmapHash => bitmapHash.Value;

        /// <summary>
        /// The lazy evaluated hash of the bitmap
        /// </summary>
        private readonly Lazy<string> bitmapHash;
    }
}
