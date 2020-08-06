using System;
using System.Composition;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using PaunPacker.GUI.WPF.Common.Attributes;

namespace PaunPacker.Core.Metadata
{
    //Unity used XML Format, but it has probably changed, however,  look at the https://assetstore.unity.com/packages/tools/utilities/xml-texture-atlas-slicer-36103 that allows to import .xml into unity
    /// <summary>
    /// The Unity metadata writer that writes the metadata about texture atlas in XML format
    /// </summary>
    [PluginMetadata(typeof(UnityMetadataWriter), nameof(UnityMetadataWriter), "Plugin containing Unity metadata writer implementation", "PaunPacker", "1.0.0.0", typeof(UnityMetadataWriter))]
    [ExportedTypeMetadata(typeof(UnityMetadataWriter), nameof(UnityMetadataWriter), "MetadataWriter implementation that targets Unity framework", "PaunPacker", "1.0.0.0")]
    [Export(typeof(IMetadataWriter))]
    [TargetFramework(FrameworkID.Unity)]
    public sealed class UnityMetadataWriter : IMetadataWriter
    {
        /// <inheritdoc />
        public Task WriteAsync(string path, string textureAtlasPath, MetadataCollection metadata, CancellationToken token = default)
        {
            return Task.Run(async () =>
            {
                var success = false;
                using (var xmlStream = XmlWriter.Create(path))
                {
                    success = await Write(textureAtlasPath, xmlStream, metadata, token).ConfigureAwait(false);
                }
                if (!success)
                {
                    try
                    {
                        File.Delete(path);
                    }
                    catch (IOException)
                    {

                    }
                }
            });
        }

        /// <summary>
        /// Internal implementation of the <see cref="WriteAsync(string, string, MetadataCollection, CancellationToken)"/> method that writes into a <paramref name="writer"/>
        /// </summary>
        /// <param name="writer">The XmlWriter to write to</param>
        /// <param name="metadata">The metadata to be written</param>
        /// <param name="token">The cancellation token</param>
        /// <param name="textureAtlasPath">Path of the texture atlas corresponding to this metadata, Unity format requires it to be specified</param>
        /// <remarks>When the cancellation is requested, the already written data is left in the stream</remarks>
        /// <returns>True on success, false on failure</returns>
        private Task<bool> Write(string textureAtlasPath, XmlWriter writer, MetadataCollection metadata, CancellationToken token = default)
        {
            //Does it make sense to use *Async ???
            return Task.Run(() =>
            {
                Progress = 0;
                int i = 0;
                writer.WriteStartDocument();
                writer.WriteRaw("\n");
                writer.WriteStartElement("TextureAtlas");
                writer.WriteAttributeString("imagePath", textureAtlasPath);
                writer.WriteRaw("\n"); //add new lines for beater readability ..
                foreach (var imgMetadata in metadata)
                {
                    if (token.IsCancellationRequested)
                    {
                        Progress = 0;
                        return false;
                    }
                    writer.WriteStartElement("SubTexture");
                    writer.WriteAttributeString("name", imgMetadata["name"].ToString());
                    writer.WriteAttributeString("x", imgMetadata["finalX"].ToString());
                    writer.WriteAttributeString("y", imgMetadata["finalY"].ToString());
                    writer.WriteAttributeString("width", imgMetadata["finalWidth"].ToString());
                    writer.WriteAttributeString("height", imgMetadata["finalHeight"].ToString());
                    writer.WriteEndElement();
                    writer.WriteRaw("\n");
                    Progress = (int)(i++ / metadata.Count * 100.0);
                    ProgressChange?.Invoke(this, Progress);
                }
                writer.WriteEndElement();
                return true;
            });
        }

        /// <inheritdoc />
        public int Progress { get; private set; }

        /// <inheritdoc />
        public bool ReportsProgress => true;

        /// <inheritdoc />
        public event Action<object, int> ProgressChange;
    }
}
