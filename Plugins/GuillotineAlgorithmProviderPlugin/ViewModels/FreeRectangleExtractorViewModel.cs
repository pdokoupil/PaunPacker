using System;
using System.Linq;
using PaunPacker.GUI.WPF.Common.Attributes;

namespace PaunPacker.Plugins.ViewModels
{
    /// <summary>
    /// Represents a view model for FreeRectangleExtractor
    /// </summary>
    class FreeRectangleExtractorViewModel
    {
        /// <summary>
        /// Constructs new FreeRectangleExtractorViewModel from a FreeRectangleExtractor of type <paramref name="type"/>
        /// </summary>
        /// <param name="type">The type of the FreeRectangleExtractor</param>
        public FreeRectangleExtractorViewModel(Type type)
        {
            FreeRectangleExtractorType = type;
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
        /// The type of the FreeRectangleExtractor
        /// </summary>
        public Type FreeRectangleExtractorType { get; private set; }

        /// <summary>
        /// Tha name of the FreeRectangleExtractor
        /// </summary>
        /// <remarks>Possibly also contains a version number</remarks>
        public string Name { get; private set; }
    }
}
