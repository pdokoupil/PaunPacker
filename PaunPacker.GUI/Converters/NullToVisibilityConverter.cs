using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PaunPacker.GUI.Converters
{
    /// <summary>
    /// Converter from null to <see cref="Visibility"/>
    /// </summary>
    class NullToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value to the <see cref="Visibility"/>
        /// </summary>
        /// <param name="value">The value to be converted</param>
        /// <returns><see cref="Visibility.Collapsed"/> if the <paramref name="value"/> is null, <see cref="Visibility.Visible"/> otherwise</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
