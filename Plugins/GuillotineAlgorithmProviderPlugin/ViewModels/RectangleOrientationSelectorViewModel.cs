using System;
using System.Linq;
using PaunPacker.GUI.WPF.Common.Attributes;

namespace PaunPacker.Plugins.ViewModels
{
    /// <summary>
    /// Represents a view model for RectangleOrientationSelector
    /// </summary>
    class RectangleOrientationSelectorViewModel
    {
        /// <summary>
        /// Constructs new RectangleOrientationSelectorViewModel from a RectangleOrientationSelector of type <paramref name="type"/>
        /// </summary>
        /// <param name="type">The type of the RectangleOrientationSelector</param>
        public RectangleOrientationSelectorViewModel(Type type)
        {
            RectangleOrientationSelectorType = type;
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
        /// The type of the RectangleOrientationSelector
        /// </summary>
        public Type RectangleOrientationSelectorType { get; private set; }

        /// <summary>
        /// Tha name of the RectangleOrientationSelector
        /// </summary>
        /// <remarks>Possibly also contains a version number</remarks>
        public string Name { get; private set; }
    }
}
