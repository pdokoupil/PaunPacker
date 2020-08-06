using PaunPacker.GUI.WPF.Common.Attributes;
using System;
using System.Composition;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PaunPacker.Core.Metadata
{
    /// <summary>
    /// This Plugin exports a <see cref="LibGDXMetadataWriter"/> which exports <see cref="MetadataCollection"/> about
    /// Texture atlas into a ".atlas" format. 
    /// Libgdx Texture Atlas Description (i.e. ".atlas" format) http://esotericsoftware.com/spine-atlas-format
    /// </summary>
    [PluginMetadata(typeof(LibGDXMetadataWriter), nameof(LibGDXMetadataWriter), "Plugin containing LibGDX metadata writer implementation", "PaunPacker", "1.0.0.0", typeof(LibGDXMetadataWriter))]
    [ExportedTypeMetadata(typeof(LibGDXMetadataWriter), nameof(LibGDXMetadataWriter), "MetadataWriter implementation that targets libGDX library", "PaunPacker", "1.0.0.0")]
    [Export(typeof(IMetadataWriter))]
    [TargetFramework(FrameworkID.libGDX)]
    public sealed class LibGDXMetadataWriter : IMetadataWriter
    {
        /// <inheritdoc />
        public Task WriteAsync(string path, string textureAtlasPath, MetadataCollection metadata, CancellationToken token = default)
        {
            return Task.Run(async () =>
            {
                var success = false;
                using (var writer = new StreamWriter(path))
                {
                    success = await Write(textureAtlasPath, writer, metadata, token).ConfigureAwait(false);
                }
                if (!success)
                {
                    try
                    {
                        //Delete the partially saved file
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
        /// <param name="writer">The stream to write to</param>
        /// <param name="metadata">The metadata to be written</param>
        /// <param name="token">The cancellation token</param>
        /// <param name="textureAtlasPath">Path of the texture atlas corresponding to this metadata, LibGDX format requires it to be specified (needed for "page section" within the metadata file)</param>
        /// <remarks>When the cancellation is requested, the already written data is left in the stream</remarks>
        /// <returns>True on success, false on failure</returns>
        private Task<bool> Write(string textureAtlasPath, StreamWriter writer, MetadataCollection metadata, CancellationToken token = default)
        {
            return Task.Run(() =>
            {
                Progress = 0;
                int i = 0;
                //Page section starts
                writer.WriteLine(Path.GetFileName(textureAtlasPath));
                writer.WriteLine("size: {0}, {1}", metadata.Width, metadata.Height);
                writer.WriteLine("format: {0}", metadata.Format);
                writer.WriteLine("filter: Linear,Linear");
                writer.WriteLine("repeat: none");
                foreach (var imgMetadata in metadata)
                {
                    if (token.IsCancellationRequested)
                    {
                        Progress = 0;
                        return false;
                    }
                    writer.WriteLine(Path.GetFileNameWithoutExtension(imgMetadata["path"].ToString()));
                    writer.WriteLine(" rotate: {0}", imgMetadata["rotate"]);
                    writer.WriteLine(" xy: {0}, {1}", imgMetadata["finalX"].ToString(), imgMetadata["finalY"].ToString());
                    writer.WriteLine(" size: {0}, {1}", imgMetadata["finalWidth"].ToString(), imgMetadata["finalHeight"].ToString());
                    //writer.WriteLine(" split: {0}, {1}, {2}, {3}", , , , ); //not mandatory, but TODO ?
                    //writer.WriteLine(" pad: {0}, {1}, {2}, {3}", , , , );
                    writer.WriteLine(" orig: {0}, {1}", imgMetadata["origWidth"], imgMetadata["origHeight"]);
                    writer.WriteLine(" offset: {0}, {1}", imgMetadata["offsetX"], (int)imgMetadata["origHeight"] - ((int)imgMetadata["offsetY"] + (int)imgMetadata["height"])); //LibGDX wants offset from bottom-left instead of a top-left corner
                    writer.WriteLine(" index: -1"); //Would be useful for animations
                    Progress = (int)(i++ / metadata.Count * 100.0);
                    ProgressChange?.Invoke(this, Progress);
                }
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
