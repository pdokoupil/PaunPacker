using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using PaunPacker.GUI.ViewModels;

namespace PaunPacker.GUI.Converters
{
    /// <summary>
    /// Converter for adding margin to tree view items based on their depth within the tree view
    /// </summary>
    class LeftMarginConverter : IValueConverter
    {
        /// <summary>
        /// Converts a Depth of a node inside a TreeView to left margin of the node (TreeViewItem)
        /// </summary>
        /// <param name="value">The value to be converted</param>
        /// <returns>The appropriate margin, 0 if the value is not <see cref="TreeViewItem"/> or it's DataContext is not <see cref="NodeVM"/></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TreeViewItem item) || !(item.DataContext is NodeVM nodeVM))
            {
                return new Thickness(0);
            }
            return new Thickness(Length * (nodeVM.Depth), 0, 0, 0);
        }

        /// <summary>
        /// Not supported
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        
        /// <summary>
        /// Length of the margin per level
        /// </summary>
        public double Length { get; set; }
    }
}
