using System.Windows.Forms;

namespace PaunPacker.Common
{
    /// <summary>
    /// This class exposes file filters in a format used by the <see cref="OpenFileDialog"/> / <see cref="SaveFileDialog"/>
    /// </summary>
    public static class FileFilters
    {
        /// <summary>
        /// An array containing all the supported file extensions
        /// </summary>
        public static readonly string[] SupportedImageFormatExtensions = { ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".ico" };

        /// <summary>
        /// File filter for filtering PaunPacker project files
        /// <remarks>PaunPacker project file has extension .ppproj</remarks>
        /// </summary>
        public const string ProjectExtensionFilter = "PaunPacker Project (*.ppproj)|*.ppproj";

        /// <summary>
        /// File filter for filtering images (in supported formats)
        /// <remarks>Supported formats (based on extension) are: .png, .jpg, .jpeg, .bmp, .gif, .ico</remarks>
        /// </summary>
        public const string ImageExtensionFilter = "Image files (*.png, *.jpg, *.jpeg, *.bmp, *.gif, *.ico) |*.png; *.jpg; *.jpeg; *.bmp; *.gif; *.ico";

        /// <summary>
        /// File filter for filtering texture atlas' metadata files
        /// <remarks>Supported metadata formats (based on extension) are file has extension ....... ???</remarks>
        /// </summary>
        public const string MetadataExtensionFilter = "Atlas files (*.xml, *.atlas) | *.xml; *.atlas";

        /// <summary>
        /// File filter for filtering texture atlas files
        /// <remarks>Supported texture atlas format is png, because it does not make sense to export into different format (almost always we need transparency)</remarks>
        /// </summary>
        public const string AtlasExtensionFilter = "Image files (*.png) | *.png";
    }
}
