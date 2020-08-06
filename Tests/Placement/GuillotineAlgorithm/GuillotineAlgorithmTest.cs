using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaunPacker.Core.Packing.Placement.Guillotine;
using PaunPacker.Core.Types;

namespace PaunPacker.Tests.Placement
{
    /// <summary>
    /// This class contains basic tests for <see cref="GuillotinePlacementAlgorithm"/>
    /// </summary>
    [TestClass]
    public class GuillotineAlgorithmTest
    {
        [TestMethod]
        public void GuillotineAlgorithmTestSingleSquareFit()
        {
            //Arrange
            var algo = new GuillotinePlacementAlgorithm(new BestAreaFitFreeRectangleExtractor(), new LongerAxisGuillotineFreeRectangleSplitter());
            var rects = Enumerable.Repeat(new PPRect(0, 0, 64, 64), 1);

            //Act
            var result = algo.PlaceRects(500, 500, rects);

            //Assert
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(500, result.Width);
            Assert.AreEqual(500, result.Height);
            Assert.AreEqual(true, TestUtil.IsPackingResultValid(result));
            CollectionAssert.AreEqual(rects.ToList(), result.Rects.ToList());
        }

        [TestMethod]
        public void GuillotineAlgorithmTestSingleSquareExactFit()
        {
            //Arrange
            var algo = new GuillotinePlacementAlgorithm(new BestAreaFitFreeRectangleExtractor(), new LongerAxisGuillotineFreeRectangleSplitter());
            var rects = Enumerable.Repeat(new PPRect(0, 0, 64, 64), 1);

            //Act
            var result = algo.PlaceRects(64, 64, rects);

            //Assert
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(64, result.Width);
            Assert.AreEqual(64, result.Height);
            Assert.AreEqual(true, TestUtil.IsPackingResultValid(result));
            CollectionAssert.AreEqual(rects.ToList(), result.Rects.ToList());
        }

        [TestMethod]
        public void GuillotineAlgorithmTestSingleSquareNoFitInHorizontalDirection()
        {
            //Arrange
            var algo = new GuillotinePlacementAlgorithm(new BestAreaFitFreeRectangleExtractor(), new LongerAxisGuillotineFreeRectangleSplitter());
            var rects = Enumerable.Repeat(new PPRect(0, 0, 64, 64), 1);

            //Act
            var result = algo.PlaceRects(63, 500, rects);

            //Assert
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void GuillotineAlgorithmTestSingleSquareNoFitInVerticalDirection()
        {
            //Arrange
            var algo = new GuillotinePlacementAlgorithm(new BestAreaFitFreeRectangleExtractor(), new LongerAxisGuillotineFreeRectangleSplitter());
            var rects = Enumerable.Repeat(new PPRect(0, 0, 64, 64), 1);

            //Act
            var result = algo.PlaceRects(500, 63, rects);

            //Assert
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void GuillotineAlgorithmTestSingleSquareNoFitInBothDirections()
        {
            //Arrange
            var algo = new GuillotinePlacementAlgorithm(new BestAreaFitFreeRectangleExtractor(), new LongerAxisGuillotineFreeRectangleSplitter());
            var rects = Enumerable.Repeat(new PPRect(0, 0, 64, 64), 1);

            //Act
            var result = algo.PlaceRects(63, 63, rects);

            //Assert
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void GuillotineAlgorithmTestSingleSquareFitSideBySideHorizontally()
        {
            //Arrange
            var algo = new GuillotinePlacementAlgorithm(new BestAreaFitFreeRectangleExtractor(), new LongerAxisGuillotineFreeRectangleSplitter());
            var rects = Enumerable.Repeat(new PPRect(0, 0, 64, 64), 10);

            //Act
            var result = algo.PlaceRects(1000, 64, rects);

            //Assert
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(1000, result.Width);
            Assert.AreEqual(64, result.Height);
            Assert.AreEqual(true, TestUtil.IsPackingResultValid(result));
            CollectionAssert.AreEqual(Enumerable.Range(0, 10).Select(i => new PPRect(i * 64, 0, i * 64 + 64, 64)).ToList(), result.Rects.ToList());
        }

        [TestMethod]
        public void GuillotineAlgorithmTestSingleSquareFitSideBySideVertically()
        {
            //Arrange
            var algo = new GuillotinePlacementAlgorithm(new BestAreaFitFreeRectangleExtractor(), new LongerAxisGuillotineFreeRectangleSplitter());
            var rects = Enumerable.Repeat(new PPRect(0, 0, 64, 64), 10);

            //Act
            var result = algo.PlaceRects(64, 1000, rects);

            //Assert
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(64, result.Width);
            Assert.AreEqual(1000, result.Height);
            Assert.AreEqual(true, TestUtil.IsPackingResultValid(result));
            CollectionAssert.AreEqual(Enumerable.Range(0, 10).Select(i => new PPRect(0, i * 64, 64, i * 64 + 64)).ToList(), result.Rects.ToList());
        }

        [TestMethod]
        public void GuillotineAlgorithmTestSingleSquarePackFourSquaresIntoLarge2x2Square()
        {
            //Arrange
            var algo = new GuillotinePlacementAlgorithm(new BestAreaFitFreeRectangleExtractor(), new LongerAxisGuillotineFreeRectangleSplitter());
            var rects = Enumerable.Repeat(new PPRect(0, 0, 64, 64), 4);

            //Act
            var result = algo.PlaceRects(128, 128, rects);

            //Assert
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(128, result.Width);
            Assert.AreEqual(128, result.Height);
            Assert.AreEqual(true, TestUtil.IsPackingResultValid(result));
            Assert.AreEqual(4, result.Rects.Count());

            List<PPRect> expectedSuperset = new List<PPRect>()
            {
                new PPRect(0, 0, 64, 64),
                new PPRect(0, 64, 64, 128),
                new PPRect(64, 0, 128, 64),
                new PPRect(64, 64, 128, 128)
            };

            CollectionAssert.IsSubsetOf(result.Rects.ToList(), expectedSuperset);
        }

        [TestMethod]
        public void GuillotineAlgorithmTestSingleSquarePackSixteenSquaresIntoLarge4x4Square()
        {
            //Arrange
            var algo = new GuillotinePlacementAlgorithm(new BestAreaFitFreeRectangleExtractor(), new LongerAxisGuillotineFreeRectangleSplitter());
            var rects = Enumerable.Repeat(new PPRect(0, 0, 64, 64), 16);

            //Act
            var result = algo.PlaceRects(256, 256, rects);

            //Assert
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(256, result.Width);
            Assert.AreEqual(256, result.Height);
            Assert.AreEqual(true, TestUtil.IsPackingResultValid(result));
            Assert.AreEqual(16, result.Rects.Count());

            List<PPRect> expectedSuperset = new List<PPRect>();

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    expectedSuperset.Add(new PPRect(i * 64, j * 64, i * 64 + 64, j * 64 + 64));
                }
            }

            CollectionAssert.IsSubsetOf(result.Rects.ToList(), expectedSuperset);
        }

        [TestMethod]
        public void GuillotineAlgorithmTestSingleSquarePackSeventeenSquaresIntoLarge4x4Square()
        {
            //Arrange
            var algo = new GuillotinePlacementAlgorithm(new BestAreaFitFreeRectangleExtractor(), new LongerAxisGuillotineFreeRectangleSplitter());
            var rects = Enumerable.Repeat(new PPRect(0, 0, 64, 64), 17);

            //Assert
            var result = algo.PlaceRects(256, 256, rects);

            //Act
            Assert.AreEqual(null, result);

        }

        //Also used to test code in MinimalAreaWasteRectangleAndPointExtractor
        [TestMethod]
        public void GuillotineAlgorithmTestTwoDistinctRectsThatDoesNotFit()
        {
            //Arrange
            var algo = new GuillotinePlacementAlgorithm(new BestAreaFitFreeRectangleExtractor(), new LongerAxisGuillotineFreeRectangleSplitter());
            var rects = new List<PPRect>
            {
                new PPRect(0, 0, 32, 32),
                new PPRect(0, 0, 32, 32),
                new PPRect(0, 0, 64, 64),
                new PPRect(0, 0, 300, 300),
                new PPRect(0, 0, 512, 512)
            };

            //Act
            var result = algo.PlaceRects(256, 256, rects);

            //Assert
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        //Assert
        [ExpectedException(typeof(ArgumentNullException))]
        public void GuillotineAlgorithmTestNullInput()
        {
            //Arrange
            var algo = new GuillotinePlacementAlgorithm(new BestAreaFitFreeRectangleExtractor(), new LongerAxisGuillotineFreeRectangleSplitter());
            IEnumerable<PPRect> rects = null;

            //Act
            algo.PlaceRects(63, 63, rects);
        }

        [TestMethod]
        public void GuillotineAlgorithmTestEmptyInput()
        {
            //Arrange
            var algo = new GuillotinePlacementAlgorithm(new BestAreaFitFreeRectangleExtractor(), new LongerAxisGuillotineFreeRectangleSplitter());
            var rects = Enumerable.Empty<PPRect>();

            //Act
            var result = algo.PlaceRects(63, 63, rects);

            //Assert
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(63, result.Width);
            Assert.AreEqual(63, result.Height);
            int actualWidth = result.Rects.Any() ? result.Rects.Max(x => x.Right) : 0;
            int actualHeight = result.Rects.Any() ? result.Rects.Max(x => x.Bottom) : 0;
            Assert.AreEqual(0, actualWidth);
            Assert.AreEqual(0, actualHeight);
            Assert.AreEqual(0, result.Rects?.Count() ?? 0);
        }
    }
}
