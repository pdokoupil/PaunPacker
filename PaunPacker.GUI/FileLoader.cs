using PaunPacker.Core.Types;
using SkiaSharp;
using System.IO;

namespace PaunPacker
{
    /// <summary>
    /// Handles loading of image files
    /// </summary>
    class FileLoader
    {
        /// <summary>
        /// Loads an image at a given path
        /// </summary>
        /// <param name="path">Path of the image to be loaded</param>
        /// <returns>The loaded image</returns>
        public static PPImage Load(string path)
        {
            if (!File.Exists(path))
                return null;
            return FastLoad(path);
        }

        /// <summary>
        /// Loads an image at a given path
        /// </summary>
        /// <remarks>Does not check for the existence of the image</remarks>
        /// <param name="path">Path of the image to be loaded</param>
        /// <returns>The loaded image</returns>
        private static PPImage FastLoad(string path)
        {
            SKBitmap result = null;

            using (var sr = File.OpenRead(path))
            using (var stream = new SKManagedStream(sr))
            {
                result = SKBitmap.Decode(stream);
                result = result.Copy();
            }
            
            return new PPImage(result, path);

        }
    }
}
