using System;
using System.Collections.Generic;
using System.Text;
using PaunPacker.Core.Types;

namespace PaunPacker.Core.Packing.Placement.Guillotine
{
    /// <summary>
    /// Allows to perform post-processing of free rectangles after a placement of a rectangle to one of the free rectangles
    /// </summary>
    public interface IFreeRectanglePostProcessor
    {
        /// <summary>
        /// Post process the free rectangles after select, split, merge i.e. when the rectangle was placed
        /// </summary>
        /// <param name="freeRectangles">The list of free rectangles</param>
        /// <param name="rectJustPlaced">The rectangle that just was placed</param>
        void PostProcess(List<PPRect> freeRectangles, PPRect rectJustPlaced);
    }
}
