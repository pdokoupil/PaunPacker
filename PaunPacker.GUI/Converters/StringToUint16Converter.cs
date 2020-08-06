using System;
using System.Globalization;
using System.Windows.Data;
namespace PaunPacker.GUI.Converters
{
    /// <summary>
    /// Converter from <see cref="string"/> to <see cref="ushort"/>
    /// </summary>
    class StringToUint16Converter : IValueConverter
    {
        /// <summary>
        /// Converts a value to <see cref="ushort"/>
        /// </summary>
        /// <param name="value">The value to be converted</param>
        /// <exception cref="InvalidCastException">Is thrown when the <see cref="ushort"/> cannot be parsed from the <paramref name="value"/></exception>
        /// <returns>The <see cref="ushort"/> result</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (ushort.TryParse(value.ToString(), out ushort res))
                return res;
            throw new InvalidCastException();
        }

        /// <summary>
        /// Converts a value to string by simply calling it's .ToString() method
        /// </summary>
        /// <param name="value">The value to be converted to the string</param>
        /// <returns>The string representation of <paramref name="value"/></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString();
        }
    }
}
