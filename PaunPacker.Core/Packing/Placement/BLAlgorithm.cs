using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using PaunPacker.Core.Packing.Sorting;
using PaunPacker.Core.Types;
using SkiaSharp;

namespace PaunPacker.Core.Packing.Placement
{
    /// <summary>
    /// Implementation of Bottom-Left algorithm
    /// </summary>
    public class BLAlgorithmPacker : IPlacementAlgorithm
    {
        /// <summary>
        /// Constructs a BLAlgorithmPacker
        /// </summary>
        public BLAlgorithmPacker()
        {
            this.sorter = new ByHeightAndWidthImageSorterDesc();
        }

        /// <summary>
        /// Constructs a BLAlgorithmPacker using a given image sorter
        /// </summary>
        /// <param name="sorter">The image sorter to be used</param>
        public BLAlgorithmPacker(IImageSorter sorter)
        {
            this.sorter = sorter;
        }

        /// <inheritdoc />
        public PackingResult PlaceRects(int width, int height, IEnumerable<PPRect> rects, CancellationToken token = default)
        {
            Progress = 0;
            if (width < 0 || height < 0)
            {
                throw new ArgumentOutOfRangeException($"The {nameof(width)} and {nameof(height)} should be non-negative");
            }

            var sortedInput = sorter.SortImages(rects);

            int inputSize = rects.Count();
            int placedRects = 0;

            int actualWidth = 0;
            int actualHeight = 0;

            RectComparer rectComparer = new RectComparer();
            PointComparer ptComparer = new PointComparer();
            SortedSet<PPRect> currentPacking = new SortedSet<PPRect>(rectComparer);
            SortedDictionary<SKPointI, int> pointsToTry = new SortedDictionary<SKPointI, int>(ptComparer)
            {
                { new SKPointI(0, 0), -1 } //the current packing is empty, so only point to try is point [0,0]
            };

            SKPointI[] pointsToAdd = new SKPointI[2];
            foreach (var x in sortedInput)
            {
                if (token.IsCancellationRequested)
                {
                    Progress = 0;
                    return null;
                }
                SKPointI? pointToRemove = null;
                foreach (var ptToTry in pointsToTry)
                {
                    PPRect tested = new PPRect(ptToTry.Key.X, ptToTry.Key.Y, ptToTry.Key.X + x.Width, ptToTry.Key.Y + x.Height);
                    var possibleIntersections = currentPacking.AsEnumerable(); //have to test everything
                    if (ptToTry.Key.X + x.Width <= width && ptToTry.Key.Y + x.Height <= height && !Intersects(tested, possibleIntersections)) //safe to pack here
                    {
                        if (ptToTry.Key.X + x.Width > actualWidth)
                        {
                            actualWidth = ptToTry.Key.X + x.Width;
                        }

                        if (ptToTry.Key.Y + x.Height > actualHeight)
                        {
                            actualHeight = ptToTry.Key.Y + x.Height;
                        }

                        int improved = 0;
                        if (TryImprove(ref tested, currentPacking, 0)) //Try to position it further to the top / left
                            improved++;

                        //Add it to the packing
                        tested.Image = x.Image;
                        currentPacking.Add(tested);
                        if (improved == 0)
                            pointToRemove = ptToTry.Key;

                        pointsToAdd[0] = new SKPointI(ptToTry.Key.X + x.Width, ptToTry.Key.Y);
                        pointsToAdd[1] = new SKPointI(ptToTry.Key.X, ptToTry.Key.Y + x.Height);

                        break;
                    }
                }

                if (pointToRemove != null)
                {
                    pointsToTry.Remove(pointToRemove.Value);
                    pointsToTry[pointsToAdd[0]] = -1;
                    pointsToTry[pointsToAdd[1]] = -1;

                    Progress = (int)((++placedRects / (double)inputSize) * 100.0);
                    ProgressChange?.Invoke(this, Progress);
                }
                else
                {
                    Progress = 100;
                    return null; //we cannot pack it anywhere
                }
            }


            //var result = new PackingResult(width, height, currentPacking.Select(x => (x.Value, x.Key))); // probably better to return result with actual width & height instead of those needed
            //actual height can be lower than height specified, width also BUT THIS IS NOT DESIRED, BECAUSE THIS CAN BE CALLED FROM FIXEDSIZE..? OR chhange size in FixedSize..
            var result = new PackingResult(actualWidth, actualHeight, currentPacking);
            return result;
        }

        /// <summary>
        /// Tries to improve (decrease its area) the current minimum bounding box
        /// </summary>
        /// <param name="currentRect"></param>
        /// <param name="possibleIntersections"></param>
        /// <param name="direction">0 for try to move up, 1 for try to move left</param>
        /// <returns></returns>
        private bool TryImprove(ref PPRect currentRect, IEnumerable<PPRect> possibleIntersections, int direction)
        {
            //adjust the improvemet to improve by higher steps (using bin search ?)
            bool ret = false;
            PPRect toTest = currentRect;

            if (direction == 0)
            {
                toTest.Top--;
                while (toTest.Top >= 0 && !Intersects(toTest, possibleIntersections))
                {
                    toTest.Top--;
                    ret = true;
                }
                toTest.Top++;
            }
            else
            {
                toTest.Left--;
                while (toTest.Left >= 0 && !Intersects(toTest, possibleIntersections))
                {
                    toTest.Left--;
                    ret = true;

                }
                toTest.Left++;

            }
            currentRect = toTest;
            if (ret)
                TryImprove(ref currentRect, possibleIntersections, (direction + 1) % 2);

            return ret;
        }

        private class PointComparer : IComparer<SKPointI>
        {
            public int Compare(SKPointI x, SKPointI y)
            {
                int cmp = x.Y.CompareTo(y.Y);
                if (cmp == 0)
                    cmp = x.X.CompareTo(y.X);
                return cmp;
            }
        }

        //reimplement it using more efficient sweep a line algorithm
        private static bool Intersects(PPRect tested, IEnumerable<PPRect> possibleIntersections)
        {
            foreach (var x in possibleIntersections)
                if (tested.IntersectsWith(x))
                    return true;

            return false;
        }

        private class RectComparer : IComparer<PPRect>
        {
            public int Compare(PPRect x, PPRect y)
            {
                int comp = x.Left.CompareTo(y.Left);
                if (comp == 0)
                    comp = x.Top.CompareTo(y.Top);
                return comp; //equality should never happen in my case?
            }
        }

        private readonly IImageSorter sorter;

        /// <inheritdoc />
        public int Progress { get; private set; }

        /// <inheritdoc />
        public bool ReportsProgress => true;

        /// <inheritdoc />
        public event Action<object, int> ProgressChange;
    }
}
