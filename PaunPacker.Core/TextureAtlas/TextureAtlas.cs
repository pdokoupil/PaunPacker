using PaunPacker.Core.Packing;
using PaunPacker.Core.Types;
using System;
using SkiaSharp;
using System.Collections.Generic;

namespace PaunPacker.Core.Atlas
{
    /// <summary>
    /// Represents the texture atlas
    /// </summary>
    public class TextureAtlas
    {
        /// <summary>
        /// Creates a texture atlas from a given packing result
        /// </summary>
        /// <param name="packing"></param>
        /// <exception cref="ArgumentNullException">Is thrown when the <paramref name="packing"/> is null</exception>
        public TextureAtlas(PackingResult packing)
        {
            if (packing == null)
            {
                throw new ArgumentNullException($"The {nameof(packing)} cannot be null");
            }

            Width = packing.Width;
            Height = packing.Height; 
            Rects = packing.Rects;
            bmp = new Lazy<SKBitmap>(() => GetBitmap());
            image = new Lazy<PPImage>(GetImage);
        }

        /// <summary>
        /// Returns Texture Atlas' Bitmap
        /// </summary>
        /// <param name="colorType">The color type of the resulting bitmap</param>
        /// <returns>The texture atlas' bitmap</returns>
        private SKBitmap GetBitmap(SKColorType colorType = SKColorType.Rgba8888)
        {
            using (SKBitmap bmp = new SKBitmap(Width, Height, colorType, SKAlphaType.Premul)) //Premul needed for transparency !
            { 
                bmp.Erase(SKColors.Transparent); //DIDNT WORK WITHOUT THIS !!!! ctor of SKBitmap sometimes returned bitmap that was not clear ..
                                                 //When clicked several times on Generate atlas (with for example genetic, maxIterations 12300)

                using (SKCanvas canvas = new SKCanvas(bmp))
                {
                    foreach (var img in Rects)
                    {
                        canvas.DrawBitmap(img.Image.Bitmap, img.Rect);
                    }
                }

                //It may return Null, if SKColorType == Unknown, as it is not supported now
                var res = bmp.Copy();  //We have to return copy because we do not want to allow someone who got the bitmap to write into it
                if (res == null)
                {
                    res = new SKBitmap(Width, Height);
                    res.Erase(SKColors.Transparent);
                }

                return res;
            }
        }

        /// <summary>
        /// Returns Texture Atlas' Image
        /// </summary>
        /// <param name="colorType">The color type of the resulting image</param>
        /// <returns>The texture atlas' image</returns>
        public PPImage GetImageWithColorType(SKColorType colorType)
        {
            return new PPImage(GetBitmapWithColorType(colorType));
        }

        /// <summary>
        /// Returns Texture Atlas' Bitmap
        /// </summary>
        /// <remarks>
        /// Same as TextureAtlas.Bitmap, but allows a parametrization by <paramref name="colorType"/>
        /// </remarks>
        /// <param name="colorType">The color type of the resulting bitmap</param>
        /// <returns>The texture atlas' bitmap</returns>
        public SKBitmap GetBitmapWithColorType(SKColorType colorType)
        {
            if (colorType == SKColorType.Rgba8888)
            {
                return Bitmap;
            }
            else
            {
                return GetBitmap(colorType);
            }
        }

        /// <summary>
        /// Rectangles contained within the texture atlas
        /// </summary>
        public IEnumerable<PPRect> Rects { get; private set; }
        
        /// <summary>
        /// Width of the texture atlas
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Height of the texture atlas
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Bitmap of the texture atlas
        /// </summary>
        public SKBitmap Bitmap => bmp.Value;

        /// <summary>
        /// Image of the texture atlas
        /// </summary>
        public PPImage Image => image.Value;

        /// <summary>
        /// Returns an image of the texture atlas
        /// </summary>
        /// <returns>The image of the texture atlas</returns>
        private PPImage GetImage()
        {
            PPImage image = new PPImage(Bitmap);
            return image;
        }

        /// <summary>
        /// Color type of the texture atlas
        /// </summary>
        public SKColorType Format { get; private set; } //In UI, there will be a possibility to change SKColorType (in order to compress the size of atlass, so we should specify Format upon creation)

        private readonly Lazy<SKBitmap> bmp;
        private readonly Lazy<PPImage> image;
    }
}
