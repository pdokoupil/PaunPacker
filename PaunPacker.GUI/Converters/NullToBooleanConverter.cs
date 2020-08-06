using System;
using System.Globalization;
using System.Windows.Data;

namespace PaunPacker.GUI.Converters
{
    /// <summary>
    /// Converter for converting (reference type) value to boolean
    /// </summary>
    class NullToBooleanConverter : IValueConverter
    {
        /// <summary>
        /// Converts a null to boolean
        /// </summary>
        /// <param name="value">The value to be converted</param>
        /// <returns>True if <paramref name="value"/> is not null, false otherwise</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(value == null);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
