using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using PaunPacker.Core.Types;
using SkiaSharp;

namespace PaunPacker.Core.ImageProcessing
{
    /// <summary>
    /// Default implementation of the <see cref="BackgroundRemoverBase"/>
    /// </summary>
    /// <remarks>
    /// Very simple implementation that tries to guess a background color from the corners of the image and then
    /// removes all the pixels that have the guessed color
    /// An idea for improvement: Introduce a "tolerance" value
    /// Assumes single colored background
    /// </remarks>
    public class HeuristicBackgroundRemover : BackgroundRemoverBase
    {
        /// <inheritdoc />
        public override PPImage RemoveBackground(PPImage input, CancellationToken token = default)
        {
            if (input == null)
            {
                throw new ArgumentNullException($"The {nameof(input)} cannot be null");
            }

            var component = SelectBackground(input);

            //Cancellation was requested during the SelectBackground method
            if (component == null)
            {
                return input;
            }

#pragma warning disable CA2000 // The resulting PPImage is the owner of the bitmap
            SKBitmap bmp2 = new SKBitmap(input.Bitmap.Width, input.Bitmap.Height, input.Bitmap.ColorType, SKAlphaType.Premul)
            {
                Pixels = input.Bitmap.Pixels
            };
#pragma warning restore CA2000 // So it should not get disposed there
            bmp2.Erase(SKColors.Transparent);

            foreach (var pix in component)
            {
                if (token.IsCancellationRequested)
                {
                    return input;
                }
                bmp2.SetPixel(pix % input.Bitmap.Width, pix / input.Bitmap.Width, SKColors.Transparent);
            }

            return new PPImage(bmp2, input.ImagePath)
            {
                ImageName = input.ImageName
            };
        }

        /// <summary>
        /// Selects a guessed background of the input image
        /// </summary>
        /// <param name="input">The input image</param>
        /// <param name="token">The cancellation token</param>
        /// <remarks>Performs a BFS</remarks>
        /// <returns>Returns the list of pixels having the same color as the color of the guessed background, null when cancellation was requested</returns>
        private List<int> SelectBackground(PPImage input, CancellationToken token = default)
        {
            SKColor bgColor = SKColors.Transparent;

            Dictionary<SKColor, List<int>> components = new Dictionary<SKColor, List<int>>();
            int[] cornerIndices = new int[]{ 0, //top-left
                                              input.Bitmap.Width - 1, //top-right
                                              (input.Bitmap.Height - 1) * input.Bitmap.Width, //bottom-left
                                              input.Bitmap.Width * input.Bitmap.Height - 1 }; //bottom-right};

            var pixels = input.Bitmap.Pixels;

            SKColor[] cornerColors = new SKColor[4]
            {
                pixels[cornerIndices[0]],
                pixels[cornerIndices[1]],
                pixels[cornerIndices[2]],
                pixels[cornerIndices[3]]
            };

            bool[] visited = new bool[input.Bitmap.Width * input.Bitmap.Height];
            Dictionary<SKColor, float> colorCounts = new Dictionary<SKColor, float>();

            Queue<int> toVisit = new Queue<int>();
            foreach (var corner in cornerIndices)
            {
                if (token.IsCancellationRequested)
                {
                    return null;
                }
                toVisit.Enqueue(corner);
                visited[corner] = true;
            }

            if (cornerColors.All(x => x == cornerColors[0]))
            {
                colorCounts.Add(cornerColors[0], 0.0f);
                components.Add(cornerColors[0], new List<int>());
                int totalVisited = 0;
                while (toVisit.Count > 0)
                {
                    if (token.IsCancellationRequested)
                    {
                        return null;
                    }
                    var curr = toVisit.Dequeue();
                    totalVisited++;
                    foreach (var neigh in GetNeighborhood(curr, input).Where(x => x != curr && x >= 0 && x < input.Bitmap.Width * input.Bitmap.Height))
                    {
                        if (visited[neigh] || pixels[neigh] != cornerColors[0])
                            continue;
                        toVisit.Enqueue(neigh);
                        visited[neigh] = true;
                    }
                    if (pixels[curr] == cornerColors[0])
                    {
                        components[cornerColors[0]].Add(curr);
                        colorCounts[cornerColors[0]] += 1.0f;
                    }

                }

                return components[cornerColors[0]];
            }
            else
            {
                var uniqueColors = cornerColors.Distinct();
                foreach (var x in uniqueColors)
                {
                    components.Add(x, new List<int>());
                    colorCounts.Add(x, 0.0f);
                }

                while (toVisit.Count > 0)
                {
                    if (token.IsCancellationRequested)
                    {
                        return null;
                    }
                    var curr = toVisit.Dequeue();
                    foreach (var neigh in GetNeighborhood(curr, input).Where(z => z != curr && z >= 0 && z < input.Bitmap.Width * input.Bitmap.Height))
                    {
                        if (visited[neigh] || !cornerColors.Contains(pixels[neigh]))
                            continue;
                        toVisit.Enqueue(neigh);
                        visited[neigh] = true;
                    }
                    //if (uniqueColors.Contains(input.Pixels[curr]))
                    //{
                    components[pixels[curr]].Add(curr);
                    float x = curr % input.Bitmap.Width;
                    float y = curr / input.Bitmap.Width;
                    float middleX = input.Bitmap.Width / 2.0f;
                    float middleY = input.Bitmap.Height / 2.0f;
                    colorCounts[pixels[curr]] += Max(Abs(middleX - x), Abs(middleY - y));
                    //colorCounts[input.Pixels[curr]] += 1.0f;
                    //}
                    visited[curr] = true;
                }
                
                bgColor = colorCounts.OrderByDescending(x => x.Value).First().Key;
                return components[bgColor];
            }

            //return bgColor;
        }

        /// <summary>
        /// Returns a neighborhood pixels of the pixel at a given index (index in the flat 1D instead of 2D array) within the given image
        /// </summary>
        /// <remarks>The neighborhood is "Moore neighborhood"</remarks>
        /// <param name="index">The index of the pixel for which the neighborhood should be returned</param>
        /// <param name="image">The image containing the pixel at the given index</param>
        /// <returns>The indices of the neighborhood pixels</returns>
        private static IEnumerable<int> GetNeighborhood(int index, PPImage image)
        {
            yield return index - 1;
            yield return index + 1;
            yield return index - image.Bitmap.Width;
            yield return index + image.Bitmap.Width;
            yield return index - image.Bitmap.Width - 1;
            yield return index - image.Bitmap.Width + 1;
            yield return index + image.Bitmap.Width - 1;
            yield return index + image.Bitmap.Width + 1;
        }

        /// <summary>
        /// Simple implementation of absolute value for float values only
        /// </summary>
        /// <param name="a">A value for which an absolute value should be found</param>
        /// <returns>The absolute value of <paramref name="a"/></returns>
        private static float Abs(float a)
        {
            return a < 0.0f ? -1.0f * a : a;
        }

        /// <summary>
        /// Simple implementation of max(a,b) for float values only
        /// </summary>
        /// <param name="a">First comparand</param>
        /// <param name="b">Second comparand</param>
        /// <returns>The greater of the two numbers, or <paramref name="a"/> if both comparands are equal</returns>
        private static float Max(float a, float b)
        {
            return a >= b ? a : b;
        }
    }

}
