using System;
using System.Linq;
using PaunPacker.GUI.WPF.Common.Attributes;

namespace PaunPacker.Plugins.ViewModels
{
    /// <summary>
    /// Represents a view model for RectAndPointPicker
    /// </summary>
    class FreeRectanglePostProcessorViewModel
    {
        /// <summary>
        /// Constructs new FreeRectanglePostProcessorViewModel from a FreeRectanglePostProcessor of type <paramref name="type"/>
        /// </summary>
        /// <param name="type">The type of the FreeRectanglePostProcessor</param>
        public FreeRectanglePostProcessorViewModel(Type type)
        {
            FreeRectanglePostProcessorType = type;
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
        /// The type of the FreeRectanglePostProcessor
        /// </summary>
        public Type FreeRectanglePostProcessorType { get; private set; }

        /// <summary>
        /// Tha name of the FreeRectanglePostProcessor
        /// </summary>
        /// <remarks>Possibly also contains a version number</remarks>
        public string Name { get; private set; }
    }
}
