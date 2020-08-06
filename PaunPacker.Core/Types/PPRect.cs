using SkiaSharp;
using System;

namespace PaunPacker.Core.Types
{
    /// <summary>
    /// Represents a rectangle with integer coordinates (thus also dimensions) in 2D space with a (possible) reference to a corresponding image
    /// </summary>
    /// <remarks>
    /// A wrapper around <see cref="SKRect"/> with some additional members
    /// </remarks>
    public struct PPRect : IEquatable<PPRect>
    {
        /// <summary>
        /// Constructs a new PPRect from coordinates
        /// </summary>
        /// <param name="left">X coordinate of the top-left corner of the rectangle</param>
        /// <param name="top">Y coordinate of the top-left corner of the rectangle</param>
        /// <param name="right">X coordinate of the bottom-right corner of the rectangle</param>
        /// <param name="bottom">Y coordinate of the bottom-right corner of the rectangle</param>
        /// <param name="image">A corresponding image</param>
        public PPRect(int left, int top, int right, int bottom, PPImage image = null)
        {
            rect = new SKRectI(left, top, right, bottom);
            Image = image;
        }

        /// <summary>
        /// Constructs a new PPRect from a given image
        /// </summary>
        /// <remarks>The top-left corner is set to point (0, 0) and it's dimensions are equal to the dimensions of the <paramref name="image"/></remarks>
        /// <param name="image">The image from which the rectangle is constructed</param>
        public PPRect(PPImage image): this(0, 0, image?.Bitmap?.Width ?? 0, image?.Bitmap?.Height ?? 0, image)
        {
            
        }

        /// <summary>
        /// Checkes whether two PPRects intersect
        /// </summary>
        /// <param name="rect">A rectangle which is checked for intersection</param>
        /// <returns>True if the rectangles intersect, false otherwise</returns>
        public bool IntersectsWith(PPRect rect)
        {
            return Rect.IntersectsWith(rect.Rect);
        }

        /// <summary>
        /// Checkes whether this PPRect intersects (contains) with a given point
        /// </summary>
        /// <param name="point">A point which is checked for intersection</param>
        /// <returns>True if the rectangle intersects with the point, false otherwise</returns>
        public bool IntersectsWith(PPPoint point)
        {
            return (point.Left >= Rect.Left && point.Left <= Rect.Right &&
                point.Top >= Rect.Top && point.Top <= Rect.Bottom);
        }

        /// <summary>
        /// Checkes whether this PPRect contains a given rectangle
        /// </summary>
        /// <param name="rect">The rectangle which is checked for containment</param>
        /// <returns>True if this rectangle contains the <paramref name="rect"/></returns>
        public bool Contains(PPRect rect)
        {
            return Rect.Contains(rect.Rect);
        }

        //Wrap some of the frequently used Properties

        /// <summary>
        /// Determines if the rectangle is rotated (relative to the original)
        /// </summary>
        public bool IsRotated => Image?.IsRotated ?? false;

        /// <summary>
        /// X coordinate of the top-left corner of this rectangle
        /// </summary>
        public int Left { get => rect.Left; set => rect.Left = value; }

        /// <summary>
        /// X coordinate of the bottom-right corner of this rectangle
        /// </summary>
        public int Right { get => rect.Right; set => rect.Right = value; }

        /// <summary>
        /// Y coordinate of the top-left corner of this rectangle
        /// </summary>
        public int Top { get => rect.Top; set => rect.Top = value; }

        /// <summary>
        /// Y coordinate of the bottom-right corner of this rectangle
        /// </summary>
        public int Bottom { get => rect.Bottom; set => rect.Bottom = value; }

        /// <summary>
        /// Width of the rectangle
        /// </summary>
        public int Width { get => rect.Width; }

        /// <summary>
        /// Height of the rectangle
        /// </summary>
        public int Height { get => rect.Height; }

        /// <summary>
        /// Indicates whether the rectangle is empty
        /// </summary>
        public bool IsEmpty { get => rect.IsEmpty; }

        /// <summary>
        /// Rotates the rectangle by 90 degrees clock-wise
        /// </summary>
        /// <returns></returns>
        public PPRect RotateClockWiseBy90Degrees()
        {
            return new PPRect(Right, Top, Left, Bottom, Image.RotateBy90Degrees());
        }

        /// <summary>
        /// The underlying SKRectI
        /// </summary>
        public SKRectI Rect { get => rect; }

        /// <summary>
        /// The corresponding image
        /// </summary>
        public PPImage Image { get; set; }

        /// <see cref="Rect"/>
        private SKRectI rect;

        /// <summary>
        /// Implementation of IEquatable interface
        /// </summary>
        /// <param name="obj">The object that is compared to this object</param>
        /// <remarks>An object is equal to this PPRect if it is also an instance of PPRect and they have same coordinates</remarks>
        /// <returns>True if the objects are equal, false otherwise</returns>
        public override bool Equals(object obj)
        {
            if (obj is PPRect rect)
            {
                return this.Equals(rect);
            }
            return false;
        }

        /// <summary>
        /// Overrides the GetHashCode method
        /// </summary>
        /// <returns>Hash code for this rectangle</returns>
        public override int GetHashCode()
        {
            return Top.GetHashCode() ^ Left.GetHashCode() ^ Right.GetHashCode() ^ Bottom.GetHashCode();
        }

        /// <summary>
        /// Override of equality operator
        /// </summary>
        /// <param name="left">The left operand</param>
        /// <param name="right">The right operand</param>
        /// <returns>True if the operands are equal, false otherwise</returns>
        public static bool operator ==(PPRect left, PPRect right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Override of non-equality operator
        /// </summary>
        /// <param name="left">The left operand</param>
        /// <param name="right">The right operand</param>
        /// <returns>True if the operands are not equal, false otherwise</returns>
        public static bool operator !=(PPRect left, PPRect right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Strongly typed version of the equals method
        /// </summary>
        /// <param name="other">Two PPRects are equal if they have same coordinates</param>
        /// <returns>True if the <paramref name="other"/> is equal to this object on which the method was called, false otherwise</returns>
        public bool Equals(PPRect other)
        {
            return this.Top == other.Top && this.Left == other.Left && this.Right == other.Right && this.Bottom == other.Bottom;
        }
    }
}
