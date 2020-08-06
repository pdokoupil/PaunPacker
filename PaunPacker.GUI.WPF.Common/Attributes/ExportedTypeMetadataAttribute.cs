using System;

namespace PaunPacker.GUI.WPF.Common.Attributes
{
    /// <summary>
    /// Metadata about a type exported from a plugin
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class ExportedTypeMetadataAttribute : Attribute
    {
        /// <summary>
        /// Constructs a new metadata about a type exported from a plugin
        /// </summary>
        /// <param name="name">User friendly name of the type</param>
        /// <param name="description">Description of the type</param>
        /// <param name="author">Author of the type</param>
        /// <param name="version">Version of the type</param>
        /// <param name="exportedType">The type being exported</param>
        /// <remarks>The <paramref name="exportedType"/> is useful when it is not possible to mark the exported type with <see cref="ExportedTypeMetadataAttribute"/> because it allows to mark only the main plugin with this attribute</remarks>
        public ExportedTypeMetadataAttribute(Type exportedType, string name, string description, string author, string version)
        {
            Name = name;
            Description = description;
            Author = author;
            Version = version;
            ExportedType = exportedType;
        }

        /// <summary>
        /// Name (user-friendly) of the decorated type
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Description of the decorated type
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Author of the decorated type
        /// </summary>
        public string Author { get; private set; }

        /// <summary>
        /// Version of the decorated type
        /// </summary>
        public string Version { get; private set; }

        /// <summary>
        /// The exported type
        /// </summary>
        public Type ExportedType { get; private set; }
    }
}
