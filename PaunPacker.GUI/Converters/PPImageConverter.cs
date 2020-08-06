using PaunPacker.Core.Types;
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using PaunPacker.GUI.Workarounds;

namespace PaunPacker.GUI.Converters
{
    /// <summary>
    /// Converter from <see cref="PPImage"/> to <see cref="Image"/>
    /// </summary>
    class PPImageConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value to the <see cref="Image"/>
        /// </summary>
        /// <param name="value">The value to be converted</param>
        /// <exception cref="ArgumentException">Is thrown when the <paramref name="value"/> is not <see cref="PPImage"/></exception>
        /// <returns>The resulting <see cref="Image"/></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is PPImage img)
            {
                var resultImage = new Image();
                var bmp = img.Bitmap.ToWriteableBitmap();
                resultImage.Source = bmp;
                resultImage.Stretch = System.Windows.Media.Stretch.None;
                return resultImage;
            }
            throw new ArgumentException($"The {nameof(value)} is not of type PPImage");
        }

        /// <summary>
        /// Converts a value to the <see cref="PPImage"/>
        /// </summary>
        /// <param name="value">The value to be converted</param>
        /// <exception cref="ArgumentException">Is thrown when <paramref name="value"/> is not <see cref="Image"/> or it's source is not <see cref="BitmapSource"/></exception>
        /// <returns>The resulting <see cref="PPImage"/></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Image img)
            {
                if (img.Source is BitmapSource bmp)
                {
                    var tmp = bmp.ToSKBitmap();
                    var resultPPImage = new PPImage(tmp);
                    tmp.Dispose();
                    return resultPPImage;
                }
            }
            throw new ArgumentException($"{nameof(value)} is not Image or it's source is not BitmapSource");
        }
    }
}
