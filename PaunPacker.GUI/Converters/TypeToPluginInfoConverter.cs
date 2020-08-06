using PaunPacker.GUI.WPF.Common.Attributes;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace PaunPacker.GUI.Converters
{
    /// <summary>
    /// Converter from a <see cref="Type"/> to the <see cref="ExportedTypeMetadataAttribute"/>
    /// Is used when displaying Types in MainWindowView
    /// Checks whether the Type is decorated with ExportedTypeMetadataAttribute and if yes, it extracts the name from it
    /// Otherwise string representation (.ToString() is returned
    /// </summary>
    class TypeToPluginInfoConverter : IValueConverter
    {
        /// <summary>
        /// Extracts (converts to) a name from the <see cref="ExportedTypeMetadataAttribute"/> that decorates the <see cref="Type"/> of the <paramref name="value"/>
        /// </summary>
        /// <param name="value">The value to be converted</param>
        /// <returns>The name of the exported metadata attribute if the <see cref="Type"/> of <paramref name="value"/> is decorated with the <see cref="ExportedTypeMetadataAttribute"/></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Type t)
            {
                var attribs = t.GetCustomAttributes(typeof(ExportedTypeMetadataAttribute), true);
                return (attribs.FirstOrDefault() as ExportedTypeMetadataAttribute)?.Name ?? value.ToString();
            }
            return value?.ToString() ?? "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
