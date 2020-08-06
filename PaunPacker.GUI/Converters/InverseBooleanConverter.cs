using System;
using System.Globalization;
using System.Windows.Data;

namespace PaunPacker.GUI.Converters
{
    /// <summary>
    /// Converter from boolean to its inverse (negation)
    /// </summary>
    class InverseBooleanConverter : IValueConverter
    {
        /// <summary>
        /// Converts boolean to its negation
        /// </summary>
        /// <param name="value">The value to be converted</param>
        /// <exception cref="InvalidCastException">Is thrown when the <paramref name="value"/> is not bool</exception>
        /// <returns>Negation of the <paramref name="value"/></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }

            throw new InvalidCastException($"The value given by {nameof(value)} parameter is not bool");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }
    }
}
