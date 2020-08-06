using PaunPacker.Core.Types;
using System.Collections.Generic;

namespace PaunPacker.Core.Packing
{
    /// <summary>
    /// Represents a result of a packing algorithm (Placement algorithm / Minimum bounding box finder)
    /// </summary>
    /// <remarks>
    /// This class is an immutable dataholder, but class is used instead of struct because it is very often useful to use null value (indicate impossible packing)
    /// without polluting the code with Nullable (code would be more verbose)
    /// </remarks>
    public class PackingResult
    {
        /// <summary>
        /// Constructs a packing result that represents a bounding box of a given dimensions containing the given rectangles (at specified positions)
        /// </summary>
        /// <param name="width">The width of the bounding box</param>
        /// <param name="height">The height of the bounding box</param>
        /// <param name="rects">The rectangles within the bounding box</param>
        public PackingResult(int width, int height, IEnumerable<PPRect> rects)
        {
            Width = width;
            Height = height;
            Rects = rects;
        }

        /// <summary>
        /// The width of the bounding box
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// The height of the bounding box
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// The rectangles within the bounding box
        /// </summary>
        public IEnumerable<PPRect> Rects { get; private set; }
    }
}
