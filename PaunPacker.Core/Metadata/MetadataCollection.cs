using MoreLinq.Extensions;
using PaunPacker.Core.Atlas;
using SkiaSharp;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PaunPacker.Core.Metadata
{
    /// <summary>
    /// Represents metadata of a whole texture atlas
    /// </summary>
    public class MetadataCollection : IEnumerable<ImageMetadata>
    {
        /// <summary>
        /// Constructs a metadata for a given texture atlas
        /// </summary>
        /// <param name="textureAtlas">Texture atlas for which a metadata should be created</param>
        /// <exception cref="ArgumentNullException">Is thrown when the <paramref name="textureAtlas"/> is null</exception>
        public MetadataCollection(TextureAtlas textureAtlas)
        {
            if (textureAtlas == null)
            {
                throw new ArgumentNullException($"The {nameof(textureAtlas)} cannot be null");
            }

            metadata = new List<ImageMetadata>();
            foreach (var rect in textureAtlas.Rects)
            {
                foreach ((string ImageName, string ImagePath) in rect.Image.Aliases.Append((rect.Image.ImageName, rect.Image.ImagePath)))
                {
                    metadata.Add(new ImageMetadata(("path", ImagePath),
                    ("name", ImageName),
                    ("x", rect.Left),
                    ("y", rect.Right),
                    ("width", rect.Image.Bitmap.Width),
                    ("height", rect.Image.Bitmap.Height),
                    ("origWidth", rect.Image.OriginalWidth),
                    ("origHeight", rect.Image.OriginalHeight),
                    ("finalWidth", rect.Image.FinalWidth),
                    ("finalHeight", rect.Image.FinalHeight),
                    ("offsetX", rect.Image.OffsetX),
                    ("offsetY", rect.Image.OffsetY),
                    ("finalX", rect.Image.NoWhiteSpaceXOffset + rect.Left),
                    ("finalY", rect.Image.NoWhiteSpaceYOffset + rect.Top),
                    ("rotate", rect.Image.IsRotated)));
                }
            }
            this.textureAtlas = textureAtlas;
        }

        /// <summary>
        /// The number of images within the texture atlas
        /// </summary>
        public int Count => metadata.Count;

        /// <summary>
        /// The metadata about images within the texture atlas
        /// </summary>
        private readonly List<ImageMetadata> metadata;

        /// <summary>
        /// Width of the texture atlas
        /// </summary>
        public int Width { get => textureAtlas.Width; }

        /// <summary>
        /// Height of the texture atlas
        /// </summary>
        public int Height { get => textureAtlas.Height; }

        /// <summary>
        /// Color format of the texture atlas
        /// </summary>
        public SKColorType Format { get => textureAtlas.Format; }

        /// <summary>
        /// Returns an enumerator for enumeration of the metadata of the texture atlas' individual images
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ImageMetadata> GetEnumerator()
        {
            return metadata.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private readonly TextureAtlas textureAtlas;
    }

}
