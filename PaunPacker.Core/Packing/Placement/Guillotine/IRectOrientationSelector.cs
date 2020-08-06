using PaunPacker.Core.Types;

namespace PaunPacker.Core.Packing.Placement.Guillotine
{
    /// <summary>
    /// Allows to select an orientation of the rectangle that should be placed
    /// </summary>
    public interface IRectOrientationSelector
    {
        /// <summary>
        /// Selects an orientation of the rectangle that should be placed
        /// </summary>
        /// <param name="sourceRect">The rectangle to be placed</param>
        /// <returns>THe <paramref name="sourceRect"/>, possibly rotated</returns>
        PPRect DetermineAndApplyRectOrientation(PPRect sourceRect);
    }
}
