using PaunPacker.Core.Types;
using SkiaSharp;
using System.Collections.Generic;
using System.IO;
using System.Collections.Concurrent;
using System;
using System.Linq;

namespace PaunPacker
{
    /// <summary>
    /// Provides a functionality for loading and saving bitmaps
    /// </summary>
    static class BitmapManager
    {
        /// <summary>
        /// Loads the bitmap from a file given by <paramref name="path"/>
        /// </summary>
        /// <param name="path">Path of the bitmap file</param>
        /// <returns>The loaded image</returns>
        public static PPImage LoadBitmap(string path/*, int degreesRotated = 0*/)
        {
            if (bitmapCache.ContainsKey(path))
                return bitmapCache[path];

            var loadedBmp = FileLoader.Load(path);

            while (!bitmapCache.TryAdd(path, loadedBmp))
            { }

            return loadedBmp;
        }

        /// <summary>
        /// Saves an image at a given path using a format given by <paramref name="format"/>
        /// </summary>
        /// <param name="image">The image to be saved</param>
        /// <param name="path">The path where the image should be saved</param>
        /// <param name="format">Format of the image</param>
        /// <param name="overwrite">Determines whether the file should be overriden if it already exists</param>
        /// <returns></returns>
        public static bool SaveBitmap(PPImage image, string path, SKEncodedImageFormat format, bool overwrite = true)
        {
            if (!overwrite && File.Exists(path))
                return false;

            using (var img = SKImage.FromBitmap(image.Bitmap))
            using (var sw = File.OpenWrite(path))
            using (var data = img.Encode(format, 100))
            {
                data.SaveTo(sw);
            }

            return true;
        }

        /// <summary>
        /// Recursively loads all the images starting in a directory at a given path
        /// </summary>
        /// <param name="path">Path of a root directory</param>
        /// <returns>All the loaded images</returns>
        public static IEnumerable<PPImage> LoadDir(string path)
        {
            if (!Directory.Exists(path))
                return Enumerable.Empty<PPImage>();

            return Common.IOUtilities.GetAllFilesRecursively(path)
                                     .Where(file => Common.FileFilters.SupportedImageFormatExtensions.Contains(file.Extension))
                                     .Select(file => LoadBitmap(file.FullName));
        }

        private static readonly ConcurrentDictionary<string, PPImage> bitmapCache = new ConcurrentDictionary<string, PPImage>();
    }
}
