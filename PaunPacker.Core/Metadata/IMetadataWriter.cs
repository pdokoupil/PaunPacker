using System.Threading;
using System.Threading.Tasks;

namespace PaunPacker.Core.Metadata
{
    /// <summary>
    /// Interface representing a metadata writer
    /// </summary>
    public interface IMetadataWriter : IProgressReporter
    {
        /// <summary>
        /// Writes the <paramref name="metadata"/> into a file at path <paramref name="path"/>
        /// </summary>
        /// <param name="path">The path of the metadata file</param>
        /// <param name="metadata">The metadata to be written to the file</param>
        /// <param name="token">The cancellation token</param>
        /// <param name="textureAtlasPath">The path of the texture atlas, this is required by some metadata writers</param>
        /// <remarks>When cancellation is requested, the partially written file is deleted</remarks>
        /// <returns>A task of the writing</returns>
        Task WriteAsync(string path, string textureAtlasPath, MetadataCollection metadata, CancellationToken token = default);
    }
}
