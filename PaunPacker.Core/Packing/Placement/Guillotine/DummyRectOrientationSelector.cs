using PaunPacker.Core.Types;

namespace PaunPacker.Core.Packing.Placement.Guillotine
{
    /// <summary>
    /// Default/dummy implementation of <see cref="IRectOrientationSelector"/> i.e. no orientation chage
    /// </summary>
    public class DummyRectOrientationSelector : IRectOrientationSelector
    {
        /// <inheritdoc />
        public PPRect DetermineAndApplyRectOrientation(PPRect sourceRect)
        {
            return sourceRect;
        }
    }
}
