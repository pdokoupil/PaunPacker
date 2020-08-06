using PaunPacker.Core.Packing;
using PaunPacker.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PaunPacker.Tests
{
    /// <summary>
    /// This class contains several utility methods for unit testing
    /// of placement algorithms <see cref="Core.Packing.Placement.IPlacementAlgorithm"/>
    /// and minimum bounding box finders <see cref="Core.Packing.MBBF.IMinimumBoundingBoxFinder"/>
    /// </summary>
    public static class TestUtil
    {
        /// <summary>
        /// Swaps two items at positions given by <paramref name="i"/> and <paramref name="j"/> in array <paramref name="array"/>
        /// </summary>
        /// <typeparam name="T">Type of the elements within the array</typeparam>
        /// <param name="i">Position of first item that has to be swapped</param>
        /// <param name="j">Position of second item that has to be swapped</param>
        /// <param name="array">Array where they should be swapped</param>
        /// <exception cref="ArgumentNullException">Is thrown when the <paramref name="array"/> is null</exception>
        public static void Swap<T>(int i, int j, T[] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException($"The {nameof(array)} cannot be null");
            }

            T tmp = array[i];
            array[i] = array[j];
            array[j] = tmp;
        }

        /// <summary>
        /// Shuffles (randomly permutes) the input sequence <paramref name="input"/>
        /// </summary>
        /// <typeparam name="T">The types of items in the sequence</typeparam>
        /// <param name="input">The input sequence</param>
        /// <returns>Shuffled <paramref name="input"/></returns>
        /// <remarks>Uses Fischer-Yates O(n) algorithm</remarks>
        public static IEnumerable<T> Shuffle<T>(IEnumerable<T> input)
        {  // How to shuffle without .ToArray() ??? this implementation without ToArray() caused reordering of elements after another linq calls (deffered execution..)
           // so It caused problems in places where I relied on order of elements (i.e. permutations in Genetic Packer..)
           // return input.OrderBy(x => rnd.Next()).ToArray().AsEnumerable();


            //Use Fischer-Yates (O(n) algorithm)
            var inputArray = input.ToArray();
            for (int i = inputArray.Length - 1; i > 0; i--)
            {
                int j = rnd.Next(i + 1);
                //while (i == j)
                //    j = rnd.Next(i + 1);
                Swap(i, j, inputArray);
            }
            return inputArray.AsEnumerable();
        }

        /// <summary>
        /// Returns a sequence of squares (<see cref="PPRect"/>) of sizes 1x1, 2x2, ..., nxn (<paramref name="n"/>)
        /// </summary>
        /// <param name="n">How much squares should be generated</param>
        /// <returns>The sequence of squares</returns>
        public static IEnumerable<PPRect> GetIncreasingSquares(int n)
        {
            for (int i = 1; i <= n; i++)
                yield return new PPRect(0, 0, i, i);
        }

        /// <summary>
        /// Checks whether the area of two rectangles where the first has dimensions <paramref name="actualW"/>x<paramref name="actualH"/>
        /// and the second has dimensions <paramref name="expW"/>x<paramref name="expH"/> are the same
        /// </summary>
        /// <param name="actualW">Width of the first rectangle</param>
        /// <param name="actualH">Height of the first rectangle</param>
        /// <param name="expW">Width of the second rectangle</param>
        /// <param name="expH">Height of the second rectangle</param>
        /// <returns></returns>
        public static bool Succeeded(int actualW, int actualH, int expW, int expH)
        {
            return (actualW == expW && actualH == expH) || (actualW == expH && actualH == expW);
        }

        /// <summary>
        /// Checkes whether the packing result is valid
        /// I.e. it checks whether there are no overlaps between the rectangles
        /// And that it fit into result.Width x result.Height rectangle
        /// </summary>
        /// <param name="result">Packing result to be checked</param>
        /// <exception cref="ArgumentNullException">Is thrown when the <paramref name="result"/> is null</exception>
        /// <returns>true if the <paramref name="result"/> is valid, false otherwise</returns>
        public static bool IsPackingResultValid(PackingResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException($"The {nameof(result)} cannot be null");
            }

            var tmp = result.Rects.ToList();
            var indices = Enumerable.Range(0, tmp.Count);
            foreach (var i in indices)
            {
                foreach (var j in indices)
                {
                    if (j < i)
                    {
                        if (tmp[i].IntersectsWith(tmp[j]))
                        {
                            return false;
                        }
                    }
                }
            }

            return tmp.Max(x => x.Right) <= result.Width && tmp.Max(x => x.Bottom) <= result.Height;
        }

        private static readonly Random rnd = new Random(1234);
    }
}
