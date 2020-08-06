namespace PaunPacker.GUI.WPF.Common
{
    /// <summary>
    /// Declares the names of the regions in the PaunPacker.GUI views and exposes them as constants
    /// </summary>
    public static class RegionNames
    {
        /// <summary>
        /// The name of the region for placement algorithms
        /// </summary>
        public const string PlacementAlgorithmsRegion = "PlacementAlgorithmsRegion";

        /// <summary>
        /// The name of the region for image sorters used by minimum bounding box finders
        /// </summary>
        public const string ImageSortersRegion = "ImageSortersRegion";

        /// <summary>
        /// The name of the region for image sorters used by placement algorithms
        /// </summary>
        public const string PlacementImageSortersRegion = "PlacementImageSortersRegion";

        /// <summary>
        /// The name of the region for minimum bounding box finders
        /// </summary>
        public const string MinimumBoundingBoxFinderRegion = "MinimumBoundingBoxFinderRegion";

        /// <summary>
        /// The name of the region for metadata writers
        /// </summary>
        public const string MetadataWritersRegion = "MetadataWritersRegion";

        /// <summary>
        /// The name of the region for image processors
        /// </summary>
        public const string ImageProcessorsRegion = "ImageProcessorsRegion";
    }
}
