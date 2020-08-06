using System;

namespace PaunPacker.Core.Types
{
    /// <summary>
    /// Represents a 2D point with integer coordinates
    /// </summary>
    public struct PPPoint : IEquatable<PPPoint>
    {
        /// <summary>
        /// Creates a new point
        /// </summary>
        /// <param name="left">The x coordinate of the point</param>
        /// <param name="top">The y coordinate of the point</param>
        public PPPoint(int left, int top)
        {
            Left = left;
            Top = top;
        }

        /// <summary>
        /// The x coordinate of the point
        /// </summary>
        public int Left
        {
            get; set;
        }

        /// <summary>
        /// The y coordinate of the point
        /// </summary>
        public int Top
        {
            get; set;
        }

        /// <summary>
        /// Implementation of IEquatable interface
        /// </summary>
        /// <param name="obj">The object that is compared to this object</param>
        /// <remarks>An object is equal to this PPPoint if it is also an instance of PPPoint and they have same coordinates</remarks>
        /// <returns>True if the objects are equal, false otherwise</returns>
        public override bool Equals(object obj)
        {
            if (obj is PPPoint point)
            {
                return this.Equals(point);
            }
            return false;
        }

        /// <summary>
        /// Overrides the GetHashCode method
        /// </summary>
        /// <returns>Hash code for this point</returns>
        public override int GetHashCode()
        {
            return Left.GetHashCode() ^ Top.GetHashCode();
        }

        /// <summary>
        /// Override of equality operator
        /// </summary>
        /// <param name="left">The left operand</param>
        /// <param name="right">The right operand</param>
        /// <returns>True if the operands are equal, false otherwise</returns>
        public static bool operator ==(PPPoint left, PPPoint right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Override of non-equality operator
        /// </summary>
        /// <param name="left">The left operand</param>
        /// <param name="right">The right operand</param>
        /// <returns>True if the operands are not equal, false otherwise</returns>
        public static bool operator !=(PPPoint left, PPPoint right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Strongly typed version of the equals method
        /// </summary>
        /// <param name="other">Two PPPoints are equal if they have same coordinates</param>
        /// <returns>True if the <paramref name="other"/> is equal to this object on which the method was called, false otherwise</returns>
        public bool Equals(PPPoint other)
        {
            return other.Top == this.Top && other.Left == this.Left;
        }
    }
}
