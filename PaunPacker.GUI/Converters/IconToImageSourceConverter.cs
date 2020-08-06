using System;
using System.Drawing;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace PaunPacker.GUI.Converters
{
    /// <summary>
    /// Convertor from <see cref="Icon"/> to <see cref="BitmapSource"/>
    /// </summary>
    class IconToImageSourceConverter : IValueConverter
    {
        /// <summary>
        /// Converts <see cref="Icon"/> to <see cref="BitmapSource"/>
        /// </summary>
        /// <param name="value">The value to be converted</param>
        /// <exception cref="InvalidCastException">Is thrown when <paramref name="value"/> cannot be casted to the <see cref="BitmapSource"/></exception>
        /// <returns>The <see cref="BitmapSource"/></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Icon icon))
            {
                throw new InvalidCastException();
            }

            var imgSource = Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            return imgSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
