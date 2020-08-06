using SkiaSharp;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PaunPacker.GUI.Workarounds
{
    /// <summary>Provides a functionality for conversions between <see cref="WriteableBitmap"/> and <see cref="SKBitmap"/></summary>
    /// <remarks>
    /// This workaround was needed in order to remove the dependency on SkiaSharp.Views NuGet package that is platform specific and .NET Framework dependent
    /// Because the only functionality needed from this package is the functionality for conversions between <see cref="WriteableBitmap"/> and <see cref="SKBitmap"/>
    /// It was decided to take these parts from the SkiaSharp's GitHub repository: https://github.com/mono/SkiaSharp/blob/master/source/SkiaSharp.Views/SkiaSharp.Views.WPF/WPFExtensions.cs
    /// The documentation to these methods could be found in the SkiaSharp's documentation
    /// </remarks>
    static class SkiaSharpExtensions
    {
        public static WriteableBitmap ToWriteableBitmap(this SKBitmap skBitmap)
        {
            using var image = SKImage.FromPixels(skBitmap.PeekPixels());
            var info = new SKImageInfo(image.Width, image.Height);
            var bmp = new WriteableBitmap(info.Width, info.Height, 96, 96, PixelFormats.Pbgra32, null);
            bmp.Lock();

            using (var pixmap = new SKPixmap(info, bmp.BackBuffer, bmp.BackBufferStride))
            {
                image.ReadPixels(pixmap, 0, 0);
            }

            bmp.AddDirtyRect(new Int32Rect(0, 0, info.Width, info.Height));
            bmp.Unlock();

            return bmp;
        }

        public static SKBitmap ToSKBitmap(this BitmapSource bitmap)
        {
            var info = new SKImageInfo(bitmap.PixelWidth, bitmap.PixelHeight);
            var skiaBitmap = new SKBitmap(info);
            using (var pixmap = skiaBitmap.PeekPixels())
            {
                bitmap.ToSKPixmap(pixmap);
            }
            return skiaBitmap;
        }

        public static SKImage ToSKImage(this BitmapSource bitmap)
        {
            // TODO: maybe keep the same color types where we can, instead of just going to the platform default

            var info = new SKImageInfo(bitmap.PixelWidth, bitmap.PixelHeight);
            var image = SKImage.Create(info);
            using (var pixmap = image.PeekPixels())
            {
                bitmap.ToSKPixmap(pixmap);
            }
            return image;
        }

        public static void ToSKPixmap(this BitmapSource bitmap, SKPixmap pixmap)
        {
            if (pixmap.ColorType == SKImageInfo.PlatformColorType)
            {
                var info = pixmap.Info;
                var converted = new FormatConvertedBitmap(bitmap, PixelFormats.Pbgra32, null, 0);
                converted.CopyPixels(new Int32Rect(0, 0, info.Width, info.Height), pixmap.GetPixels(), info.BytesSize, info.RowBytes);
            }
            else
            {
                // we have to copy the pixels into a format that we understand
                // and then into a desired format
                // TODO: we can still do a bit more for other cases where the color types are the same
                using var tempImage = bitmap.ToSKImage();
                tempImage.ReadPixels(pixmap, 0, 0);
            }
        }
    }
}
