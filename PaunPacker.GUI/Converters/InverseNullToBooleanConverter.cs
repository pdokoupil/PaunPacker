using System;
using System.Globalization;
using System.Windows.Data;
namespace PaunPacker.GUI.Converters
{
    /// <summary>
    /// Converter from null to boolean
    /// </summary>
    /// <remarks>Non-null value is converted to false and null value to true</remarks>
    class InverseNullToBooleanConverter : IValueConverter
    {
        /// <summary>
        /// Converts a <paramref name="value"/> to boolean
        /// </summary>
        /// <param name="value">The value to be converted</param>
        /// <returns>True if the <paramref name="value"/> is null, false otherwise</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var converter = new NullToBooleanConverter();
            return !(bool)converter.Convert(value, targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
