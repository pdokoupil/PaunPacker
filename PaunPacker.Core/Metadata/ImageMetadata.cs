using System;
using System.Collections.Generic;

namespace PaunPacker.Core.Metadata
{
    /// <summary>
    /// Represents a metadata of a single image
    /// </summary>
    public class ImageMetadata
    {
        /// <summary>
        /// Creates a metadata for a single image
        /// </summary>
        /// <param name="props">An array of key value pairs</param>
        /// <exception cref="ArgumentNullException">Is thrown when the <paramref name="props"/> is null</exception>
        public ImageMetadata(params (string name, object value)[] props)
        {
            if (props == null)
            {
                throw new ArgumentNullException($"The {nameof(props)} cannot be null");
            }

            properties = new Dictionary<string, object>();
            foreach (var (name, value) in props)
            {
                this[name] = value;
            }
        }

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="x">The attribute name</param>
        /// <returns>The value of the attribute <paramref name="x"/></returns>
        public object this[string x]
        {
            get => properties[x];
            set => properties[x] = value;
        }

        /// <summary>
        /// Properties (the key, value pairs)
        /// </summary>
        private readonly Dictionary<string, object> properties;
    }
}
