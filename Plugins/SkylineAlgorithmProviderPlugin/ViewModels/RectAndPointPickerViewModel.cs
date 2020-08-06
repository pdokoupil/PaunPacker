using System;
using System.Linq;
using PaunPacker.GUI.WPF.Common.Attributes;

namespace PaunPacker.Plugins.ViewModels
{
    /// <summary>
    /// Represents a view model for RectAndPointPicker
    /// </summary>
    class RectAndPointPickerViewModel
    {
        /// <summary>
        /// Constructs new RectAndPointPickerViewModel from a RectAndPointPicker of type <paramref name="type"/>
        /// </summary>
        /// <param name="type">The type of the RectAndPointPicker</param>
        public RectAndPointPickerViewModel(Type type)
        {
            RectAndPointPickerType = type;
            var exportedTypeAttributes = type.GetCustomAttributes(typeof(ExportedTypeMetadataAttribute), true);
            if (exportedTypeAttributes.Any())
            {
                var attr = exportedTypeAttributes.First() as ExportedTypeMetadataAttribute;
                Name = attr.Name + $" (v {attr.Version})";
            }
            else
            {
                Name = type.Name.Split(".").LastOrDefault();
            }
        }

        /// <summary>
        /// The type of the RectAndPointPicker
        /// </summary>
        public Type RectAndPointPickerType { get; private set; }

        /// <summary>
        /// Tha name of the RectAndPointPicker
        /// </summary>
        /// <remarks>Possibly also contains a version number</remarks>
        public string Name { get; private set; }
    }
}
