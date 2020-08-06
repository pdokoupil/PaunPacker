using System;
using System.Linq;
using PaunPacker.GUI.WPF.Common.Attributes;

namespace PaunPacker.Plugins.ViewModels
{
    /// <summary>
    /// Represents a view model for FreeRectangleSplitter
    /// </summary>
    class FreeRectangleSplitterViewModel
    {
        /// <summary>
        /// Constructs new FreeRectangleSplitterViewModel from a FreeRectangleSplitter of type <paramref name="type"/>
        /// </summary>
        /// <param name="type">The type of the FreeRectangleSplitter</param>
        public FreeRectangleSplitterViewModel(Type type)
        {
            FreeRectangleSplitterType = type;
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
        /// The type of the FreeRectangleSplitter
        /// </summary>
        public Type FreeRectangleSplitterType { get; private set; }

        /// <summary>
        /// Tha name of the FreeRectangleSplitter
        /// </summary>
        /// <remarks>Possibly also contains a version number</remarks>
        public string Name { get; private set; }
    }
}
