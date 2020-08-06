using System;
using System.Collections.Generic;

namespace PaunPacker.GUI.WPF.Common.Attributes
{
    /// <summary>
    /// Metadata about a plugin
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PluginMetadataAttribute : Attribute
    {
        /// <summary>
        /// Constructs a new metadata about a plugin
        /// </summary>
        /// <param name="pluginType">The type of the plugin</param>
        /// <param name="name">User friendly name of the plugin</param>
        /// <param name="description">Description of the plugin</param>
        /// <param name="author">Author of the plugin</param>
        /// <param name="version">Version of the plugin</param>
        /// <param name="exportedTypes">The types exported from the plugin</param>
        public PluginMetadataAttribute(Type pluginType, string name, string description, string author, string version, params Type [] exportedTypes)
        {
            PluginType = pluginType;
            Name = name;
            Description = description;
            Author = author;
            Version = version;
            ExportedTypes = exportedTypes;
        }

        /// <summary>
        /// The name (user-friendly) of the decorated plugin
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The description of the decorated plugin
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// The author of the decorated plugin
        /// </summary>
        public string Author { get; private set; }

        /// <summary>
        /// The version of the decorated plugin
        /// </summary>
        public string Version { get; private set; }

        /// <summary>
        /// The types exported from the decorated plugin
        /// </summary>
        public IEnumerable<Type> ExportedTypes { get; private set; }

        /// <summary>
        /// The type of the plugin
        /// </summary>
        public Type PluginType { get; private set; }
    }
}
