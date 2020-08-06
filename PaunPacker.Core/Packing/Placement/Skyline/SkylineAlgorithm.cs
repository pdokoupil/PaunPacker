using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MoreLinq.Extensions;
using PaunPacker.Core.Packing.Sorting;
using PaunPacker.Core.Types;

namespace PaunPacker.Core.Packing.Placement.Skyline
{
    /// <summary>
    /// Implementation of Skyline algorithm
    /// </summary>
    /// <remarks>
    /// Implementation is based on: <externalLink>
    /// <linkText>This paper</linkText>
    /// <linkUri>http://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.724.1363&rep=rep1&type=pdf</linkUri>
    /// </externalLink>
    /// Does not perform rotations
    /// </remarks>
    public class SkylineAlgorithm : IPlacementAlgorithm
    {
        /// <summary>
        /// Constructs a SkylineAlgorithm with a given image sorter and ISkylineRectAndPointPicker
        /// </summary>
        /// <param name="imageSorter">The image sorter to be used</param>
        /// <param name="rectangleAndPointPicker">The rectangle and point picker to be used</param>
        /// <remarks>
        /// When <paramref name="imageSorter"/> is null, the <see cref="ByHeightAndWidthImageSorterDesc"/> is used as a default image sorter
        /// When <paramref name="rectangleAndPointPicker"/> is null, the <see cref="MinimalAreaWasteRectAndPointPicker"/> is used as a default rect and point picker
        /// </remarks>
        public SkylineAlgorithm(IImageSorter imageSorter = null, ISkylineRectAndPointPicker rectangleAndPointPicker = null)
        {
            feasiblePoints = new LinkedList<FeasiblePoint>();
            this.imageSorter = imageSorter ?? new ByHeightAndWidthImageSorterDesc();
            this.rectAndPointPicker = rectangleAndPointPicker ?? new MinimalAreaWasteRectAndPointPicker();
        }

        /// <inheritdoc />
        public PackingResult PlaceRects(int width, int height, IEnumerable<PPRect> rects, CancellationToken token = default)
        {
            InitializeLists(width, height);
            var sortedRects = imageSorter.SortImages(rects);
            var resultingPacking = PlaceRectsImpl(width, height, sortedRects, token).ToList();
            if (resultingPacking.Count() != sortedRects.Count())
                return null;
            return new PackingResult(width, height, resultingPacking);
        }

        /// <inheritdoc />
        /// <remarks>Private implementation</remarks>
        private IEnumerable<PPRect> PlaceRectsImpl(int width, int height, IEnumerable<PPRect> sortedRects, CancellationToken token = default)
        {
            Progress = 0;

            var rects = sortedRects.GroupBy(key => key, new PPRectDimensionComparer())
                .Select<IGrouping<PPRect,PPRect>, (PPRect Rect, List<PPRect> Rects)>(groupping => (groupping.Key, groupping.ToList()))
                .ToDictionary(pair => pair.Rect, pair => pair.Rects, new PPRectDimensionComparer());
            
            var sortedRectsList = sortedRects.ToList();
            int rectCount = sortedRectsList.Count;
            int placedRects = 0;


            while (rectCount > 0)
            {
                if (token.IsCancellationRequested)
                {
                    Progress = 0;
                    yield break;
                }
                ////There is not enough feasible points
                if (feasiblePoints.Count == 0)
                {
                    Progress = 100;
                    yield break;
                }

                var (Rect, FeasiblePointIndex) = rectAndPointPicker.PickRectAndPoint(sortedRectsList, feasiblePoints.Select(x => (x.Point2D.X, x.Point2D.Y, x.VerticalEdge.Length, x.HorizontalEdge.Length)), width, height);

                //Check if there is even some feasible solution ..
                if (FeasiblePointIndex < 0)
                {
                    Progress = 100;
                    yield break;
                }

                var feasiblePoint = feasiblePoints.ElementAt(FeasiblePointIndex);
                var rect = Rect;

                var placedImage = PlaceRect(feasiblePoint.Point2D, rect);
                
                rects[rect].Remove(rect);
                sortedRectsList.Remove(rect);
                rectCount--;
                
                if (rectCount > 0)
                {
                    int minWidthOfRemaining = int.MaxValue;
                    int minHeightOfRemaining = int.MaxValue;
                    
                    foreach(var keyValue in rects)
                    {
                        foreach (var rectangle in keyValue.Value)
                        {
                            if (rectangle.Width < minWidthOfRemaining)
                            {
                                minWidthOfRemaining = rectangle.Width;
                            }
                            if (rectangle.Height < minHeightOfRemaining)
                            {
                                minHeightOfRemaining = rectangle.Height;
                            }
                        }
                    }
                 
                    AdjustEnvelope(width, height, feasiblePoint, placedImage, minWidthOfRemaining, minHeightOfRemaining);
                }
                Progress = (int)((++placedRects / (double)sortedRectsList.Count) * 100.0);
                ProgressChange?.Invoke(this, Progress);
                yield return placedImage;
            }
        }


        /// <summary>
        /// Comparer for PPRect based on their dimensions
        /// </summary>
        private class PPRectDimensionComparer : IEqualityComparer<PPRect>
        {
            /// <summary>
            /// Checks whether two compared rectangles (comparands) have equal dimensions
            /// </summary>
            /// <param name="x">First comparand</param>
            /// <param name="y">Second comparand</param>
            /// <returns>True if the comparands have equal dimensions, false otherwise</returns>
            public bool Equals(PPRect x, PPRect y)
            {
                return x.Width == y.Width && x.Height == y.Height;
            }

            /// <summary>
            /// Returns the hash code for a given rectangle
            /// </summary>
            /// <param name="obj">The rectangle for which the hash code is calculated</param>
            /// <returns>The hash code of <paramref name="obj"/></returns>
            public int GetHashCode(PPRect obj)
            {
                return obj.Width.GetHashCode() ^ obj.Height.GetHashCode();
            }
        }

        /// <summary>
        /// Initializes the list of feasible points
        /// </summary>
        /// <param name="width">The width of the bounding box</param>
        /// <param name="height">The height of the bounding box</param>
        private void InitializeLists(int width, int height)
        {
            feasiblePoints.Clear();
            feasiblePoints.AddFirst(new FeasiblePoint(new Edge((0, 0), (0, height)), new Edge((0, 0), (width, 0)), (0, 0)));
        }

        /// <summary>
        /// Places a rectangle onto a feasible point
        /// </summary>
        /// <param name="feasiblePoint">Feasible point where the rectangle should be placed</param>
        /// <param name="rect">The rectangle that should be placed</param>
        /// <returns>A new rectangle that is has same dimensions and corresponds to the same image but has different position (is placed onto the <paramref name="feasiblePoint"/>)</returns>
        private static PPRect PlaceRect((int x, int y) feasiblePoint, PPRect rect)
        {
            return new PPRect(feasiblePoint.x, feasiblePoint.y, feasiblePoint.x + rect.Width, feasiblePoint.y + rect.Height, rect.Image);
        }

        /// <summary>
        /// Adjusts (repairs) the envelope after a placement of rectangle <paramref name="rectPlaced"/> onto a feasible point <paramref name="feasiblePointUsed"/>
        /// </summary>
        /// <param name="width">The width of the bounding box</param>
        /// <param name="height">The height of the bounding box</param>
        /// <param name="feasiblePointUsed">The feasible point that was used for a placement</param>
        /// <param name="rectPlaced">The rectangle that was placed</param>
        /// <param name="minWidth">Minimum width of the rectangles that remain to be placed</param>
        /// <param name="minHeight">Minimum height of the rectangles that remain to be placed</param>
        private void AdjustEnvelope(int width, int height, FeasiblePoint feasiblePointUsed, PPRect rectPlaced, int minWidth, int minHeight)
        {
            //adjustEdges(width, height, feasiblePointUsed, rectPlaced);
            RecalculateFeasiblePoints(width, height, feasiblePointUsed, rectPlaced, minWidth, minHeight);
        }

        /// <summary>
        /// Recalculates the feasible points after a placement of rectangle <paramref name="rectPlaced"/> onto a feasible point <paramref name="feasiblePointUsed"/>
        /// </summary>
        /// <param name="width">The width of the bounding box</param>
        /// <param name="height">The height of the bounding box</param>
        /// <param name="feasiblePointUsed">The feasible point that was used for a placement</param>
        /// <param name="rectPlaced">The rectangle that was placed</param>
        /// <param name="minWidth">Minimum width of the rectangles that remain to be placed</param>
        /// <param name="minHeight">Minimum height of the rectangles that remain to be placed</param>
        private void RecalculateFeasiblePoints(int width, int height, FeasiblePoint feasiblePointUsed, PPRect rectPlaced, int minWidth, int minHeight)
        {
            //Find feasible points, and go to the left
            //delete all the feasible points that are to the left to this feasible point
            //and also below feasiblePoint.y + image height
            //Add new feasible points (those that originate due to the placement of new image)

            var currFeasiblePoint = feasiblePoints.First;

            var feasiblePointsNew = new LinkedList<FeasiblePoint>();

            while (true)
            {
                (int x, int y) = currFeasiblePoint.Value.Point2D;

                feasiblePoints.RemoveFirst();

                if (currFeasiblePoint.Value == feasiblePointUsed)
                {
                    break;
                }

                //If still feasible, add it
                if (y >= rectPlaced.Bottom || x >= rectPlaced.Right)
                {
                    feasiblePointsNew.AddLast(currFeasiblePoint);
                }

                currFeasiblePoint = feasiblePoints.First;
            }

            //The part prior to feasiblePointSelected is repaired, currFeasiblePoint points to feasiblePointSelected
            //Add two new feasible points that arise when rectPlaced is placed


            var previousFeasiblePoint = feasiblePointsNew.Last;
            var nextFeasiblePoint = currFeasiblePoint.Next;

            Edge e1 = null;

            if (previousFeasiblePoint != null)
            {
                e1 = new Edge((rectPlaced.Left, rectPlaced.Top), 
                                (previousFeasiblePoint.Value.HorizontalEdge.Second.X, previousFeasiblePoint.Value.HorizontalEdge.Second.Y));
            }
            else
            {
                e1 = new Edge((rectPlaced.Left, rectPlaced.Top), (rectPlaced.Left, height));
            }

            Edge e4 = null;

            if (nextFeasiblePoint != null)
            {
                e4 = new Edge((nextFeasiblePoint.Value.VerticalEdge.First.X, nextFeasiblePoint.Value.VerticalEdge.First.Y), (rectPlaced.Left, rectPlaced.Bottom));
            }
            else
            {
                e4 = new Edge((rectPlaced.Right, rectPlaced.Top), (width, rectPlaced.Top));
            }

            var topLeftFeasiblePoint = new FeasiblePoint(e1, new Edge((rectPlaced.Left, rectPlaced.Top), (rectPlaced.Right, rectPlaced.Top)), (rectPlaced.Left, rectPlaced.Bottom));
            var bottomRightFeasiblePoint = new FeasiblePoint(new Edge((rectPlaced.Right, rectPlaced.Top), (rectPlaced.Right, rectPlaced.Bottom)), e4, (rectPlaced.Right, rectPlaced.Top));

            feasiblePointsNew.AddLast(topLeftFeasiblePoint);
            feasiblePointsNew.AddLast(bottomRightFeasiblePoint);

            currFeasiblePoint = feasiblePoints.First;

            //Restore part after feasiblePointSelected (some feasible points may lie below rectPlaced
            while (currFeasiblePoint != null)
            {
                (int x, int y) = currFeasiblePoint.Value.Point2D;

                feasiblePoints.RemoveFirst();
                
                //If still feasible, add it
                if ((y < rectPlaced.Top || x > rectPlaced.Right) && !(feasiblePointsNew.Any(f => (f.Point2D.X > x && f.Point2D.Y > y)))) //!(feasiblePointsNew.Any(f => (f.Point2D.X > x && f.Point2D.Y < y)) should not be there but unfortunately there was some bug which cause
                {                                                                                                                          //the textures in result to overlap. But this condition decreased packing optimality quite a lot for this algorithm
                    feasiblePointsNew.AddLast(currFeasiblePoint);
                }

                currFeasiblePoint = feasiblePoints.First;
            }


            //filter out "bad positions"
            feasiblePoints = new LinkedList<FeasiblePoint>(feasiblePointsNew.Where(x => width - x.Point2D.X >= minWidth && height - x.Point2D.Y >= minHeight));
        }

        /// <inheritdoc />
        public int Progress { get; private set; }

        /// <inheritdoc />
        public bool ReportsProgress => true;

        /// <inheritdoc />
        public event Action<object, int> ProgressChange;

        /// <summary>
        /// Represents a feasible point
        /// </summary>
        /// <remarks>
        /// Points are defined by two edges that are adjacent to it
        /// Always specify it in a way that: vertical top, vertical bottom, horizontal left, horizontal right ! in order to avoid checks each time
        /// </remarks>
        private class FeasiblePoint
        {
            /// <summary>
            /// Constructs a feasible point from it's adjacent edges and coordinates of this point
            /// </summary>
            /// <param name="verticalEdge"></param>
            /// <param name="horizontalEdge"></param>
            /// <param name="point2d"></param>
            public FeasiblePoint(Edge verticalEdge, Edge horizontalEdge, (int x, int y) point2d)
            {
                VerticalEdge = verticalEdge;
                HorizontalEdge = horizontalEdge;
                Point2D = point2d;
            }

            /// <summary>
            /// The 2D coordinates of this feasible point
            /// </summary>
            public (int X, int Y) Point2D { get; private set; }
            
            /// <summary>
            /// Vertical edge (from the envelope / skyline) adjacent to this feasible point
            /// </summary>
            public Edge VerticalEdge { get; private set; }

            /// <summary>
            /// Horizontal edge (from the envelope / skyline) adjacent to this feasible point
            /// </summary>
            public Edge HorizontalEdge { get; private set; }
        }

        /// <summary>
        /// Represents an edge of the envelope
        /// </summary>
        private class Edge
        {
            /// <summary>
            /// Constructs a new edge from coordinates of two points
            /// </summary>
            /// <param name="x1">The X coordinate of the first point</param>
            /// <param name="y1">The Y coordinate of the first point</param>
            /// <param name="x2">The X coordinate of the second point</param>
            /// <param name="y2">The Y coordinate of the second point</param>
            /// <remarks>
            /// Always specify it in a way that: vertical top, vertical bottom, horizontal left, horizontal right ! in order to avoid checks each time
            /// </remarks>
            public Edge(int x1, int y1, int x2, int y2): this((x1, y1), (x2, y2))
            {
                
            }

            /// <summary>
            /// Constructs a new edge from coordinates of two points (given as tuples)
            /// </summary>
            /// <param name="first">The coordinates of the first point</param>
            /// <param name="second">The coordinates of the second point</param>
            public Edge((int X, int Y) first, (int X, int Y) second)
            {
                this.First = first;
                this.Second = second;
            }

            /// <summary>
            /// Indicates whether this edge is vertical
            /// </summary>
            public bool IsVertical
            {
                get => First.X == Second.X;
            }

            /// <summary>
            /// The length of the edge
            /// </summary>
            public int Length
            {
                get => Second.X - First.X + Second.Y - First.Y;
            }

            /// <summary>
            /// The first point of the edge
            /// </summary>
            public (int X, int Y) First { get; set; }

            /// <summary>
            /// The second point of the edge
            /// </summary>
            public (int X, int Y) Second { get; set; }

        }

        //We only care about points where envelope line change from vertical to horizonal
        private LinkedList<FeasiblePoint> feasiblePoints;
        private readonly IImageSorter imageSorter;
        private readonly ISkylineRectAndPointPicker rectAndPointPicker;

    }
}
