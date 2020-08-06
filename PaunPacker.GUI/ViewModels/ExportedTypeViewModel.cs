using System;
using System.ComponentModel;
using PaunPacker.GUI.WPF.Common;
using PaunPacker.GUI.WPF.Common.Attributes;

namespace PaunPacker.GUI.ViewModels
{
    /// <summary>
    /// View model for an exported type
    /// </summary>
    /// <remarks>
    /// Shown in Combo boxes (in the settings)
    /// </remarks>
    class ExportedTypeViewModel : ViewModelBase
    {
        /// <summary>
        /// Constructs a new View model from an exported type and metadata about that exported type
        /// </summary>
        /// <param name="exportedType">The exported type</param>
        /// <param name="metadata">Metadata about the exported type</param>
        public ExportedTypeViewModel(Type exportedType, ExportedTypeMetadataAttribute metadata)
        {
            ExportedType = exportedType;
            Metadata = metadata;
        }

        /// <summary>
        /// The name of the exported type
        /// </summary>
        public string Name => Metadata?.Name ?? ExportedType.Name;

        /// <summary>
        /// String containing name and the version of the exported type
        /// </summary>
        public string NameVersion
        {
            get
            { 
                if (Metadata != null)
                {
                    return Metadata.Name + $" (v {Metadata.Version})";
                }
                return ExportedType.Name;
            }
        }

        /// <summary>
        /// The metadata about the exported type
        /// </summary>
        public ExportedTypeMetadataAttribute Metadata
        {
            get => metadata;
            private set
            {
                metadata = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Metadata)));
            }
        }

        /// <summary>
        /// The exported type
        /// </summary>
        public Type ExportedType
        {
            get => exportedType;
            private set
            {
                exportedType = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ExportedType)));
            }
        }

        /// <summary>
        /// Overrides the equals method
        /// </summary>
        /// <param name="obj">The object to compare to</param>
        /// <returns>True if they equals, false otherwise</returns>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is ExportedTypeViewModel))
            {
                return false;
            }
            return (obj as ExportedTypeViewModel).ExportedType == ExportedType;
        }

        /// <summary>
        /// Overrides the GetHashCode method
        /// </summary>
        /// <returns>The hash code</returns>
        public override int GetHashCode()
        {
            return ExportedType.GetHashCode();
        }

        /// <inheritdoc />
        public override event PropertyChangedEventHandler PropertyChanged;
        
        /// <summary>
        /// The metadata about the exported type
        /// </summary>
        private ExportedTypeMetadataAttribute metadata;

        /// <summary>
        /// The exported type
        /// </summary>
        private Type exportedType;
    }
}
