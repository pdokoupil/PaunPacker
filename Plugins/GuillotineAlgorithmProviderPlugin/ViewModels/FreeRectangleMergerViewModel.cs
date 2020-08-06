using System;
using System.Linq;
using PaunPacker.GUI.WPF.Common.Attributes;

namespace PaunPacker.Plugins.ViewModels
{
    /// <summary>
    /// Represents a view model for FreeRectangleMerger
    /// </summary>
    class FreeRectangleMergerViewModel
    {
        /// <summary>
        /// Constructs new FreeRectangleMergerViewModel from a FreeRectangleMerger of type <paramref name="type"/>
        /// </summary>
        /// <param name="type">The type of the FreeRectangleMerger</param>
        public FreeRectangleMergerViewModel(Type type)
        {
            FreeRectangleMergerType = type;
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
        /// The type of the FreeRectangleMerger
        /// </summary>
        public Type FreeRectangleMergerType { get; private set; }

        /// <summary>
        /// Tha name of the FreeRectangleMerger
        /// </summary>
        /// <remarks>Possibly also contains a version number</remarks>
        public string Name { get; private set; }
    }
}
