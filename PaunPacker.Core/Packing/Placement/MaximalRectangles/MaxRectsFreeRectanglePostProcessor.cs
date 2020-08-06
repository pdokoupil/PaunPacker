using System;
using System.Collections.Generic;
using System.Linq;
using PaunPacker.Core.Packing.Placement.Guillotine;
using PaunPacker.Core.Types;

namespace PaunPacker.Core.Packing.Placement.MaximalRectangles
{
    /// <summary>
    /// Post processor for a MaxRects (Maximal rectangles) algorithm
    /// </summary>
    public class MaxRectsFreeRectanglePostProcessor : IFreeRectanglePostProcessor
    {
        /// <inheritdoc />
        /// <remarks>
        /// Performs the post-processing after placing the rectangle <paramref name="rectJustPlaced"/>
        /// This includes a "repair" of all the free rectangles that intersects with a <paramref name="rectJustPlaced"/> and removal of free rectangles that are fully included in other free rectangle
        /// </remarks>
        /// <exception cref="ArgumentNullException">Is thrown when the <paramref name="freeRectangles"/> is null</exception>
        public void PostProcess(List<PPRect> freeRectangles, PPRect rectJustPlaced)
        {
            if (freeRectangles == null)
            {
                throw new ArgumentNullException($"The {nameof(freeRectangles)} cannot be null");
            }

            List<PPRect> rectsToAdd = new List<PPRect>();
            PPRect[] resultingFreeRects = new PPRect[4];
            for (int i = 0; i < freeRectangles.Count;)
            {
                if (freeRectangles[i].IntersectsWith(rectJustPlaced))
                {
                    //This is very similar to splitting a free rectangle but it is not the same, because this code does not assume (unlike the IFreeRectangleSplitter) that a rectangles are placed into a bottom-left of the free rectangle
                    //Calculate FreeRect \ rectJustPlaced and split it into at most four free rects
                    //make it always four (save NEW's and just set 0 dimension to nonused, then filter these upon addition)
                    resultingFreeRects[0] = new PPRect(freeRectangles[i].Left, freeRectangles[i].Top, rectJustPlaced.Left, freeRectangles[i].Bottom); //LEFT
                    resultingFreeRects[1] = new PPRect(freeRectangles[i].Left, freeRectangles[i].Top, freeRectangles[i].Right, rectJustPlaced.Top); //TOP
                    resultingFreeRects[2] = new PPRect(rectJustPlaced.Right, freeRectangles[i].Top, freeRectangles[i].Right, freeRectangles[i].Bottom); //RIGHT
                    resultingFreeRects[3] = new PPRect(freeRectangles[i].Left, rectJustPlaced.Bottom, freeRectangles[i].Right, freeRectangles[i].Bottom); //BOTTOM

                    //(PPRect freeRect1, PPRect freeRect2) = splitter.SplitFreeRectangle(freeRectangles[i], rectJustPlaced);
                    freeRectangles.RemoveAt(i);
                    //rectsToAdd.Add(freeRect1);
                    //rectsToAdd.Add(freeRect2);
                    rectsToAdd.AddRange(resultingFreeRects.Where(x => x.Width > 0 && x.Height > 0));
                }
                else
                {
                    i++;
                }
            }

            freeRectangles.AddRange(rectsToAdd);

            List<PPRect> rectsToRemove = new List<PPRect>();

            for (int i = 0; i < freeRectangles.Count; i++)
            {
                for (int j = 0; j < freeRectangles.Count; j++)
                {
                    if (i != j && freeRectangles[i].Contains(freeRectangles[j]))
                    {
                        rectsToRemove.Add(freeRectangles[j]);
                    }
                }
            }

            foreach (var toRemove in rectsToRemove)
            {
                freeRectangles.Remove(toRemove);
            }
        }
    }
}
