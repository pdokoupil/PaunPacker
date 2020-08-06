using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaunPacker.Core.Packing.Placement.Guillotine;
using PaunPacker.Core.Types;

namespace PaunPacker.Tests.Placement
{
    /// <summary>
    /// This class contains unit tests for <see cref="BestAreaFitFreeRectangleExtractor"/>
    /// </summary>
    [TestClass]
    public class BestAreaFitFreeRectangleExtractorTest
    {
        [TestMethod]
        public void BestAreaFitFreeRectangleExtractorTestEmpty()
        {
            //Arrange
            var rects = Enumerable.Empty<PPRect>().ToList();
            var currentRect = new PPRect(0,0,32,32);
            var extractor = new BestAreaFitFreeRectangleExtractor();

            //Act
            var result = extractor.ExtractFreeRectangle(rects, currentRect);

            //Assert
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        //Assert
        [ExpectedException(typeof(ArgumentNullException))]
        public void BestAreaFitFreeRectangleExtractorTestNull()
        {
            //Arrange
            var extractor = new BestAreaFitFreeRectangleExtractor();
            var currentRect = new PPRect(0, 0, 32, 32);

            //Act
            extractor.ExtractFreeRectangle(null, currentRect);
        }

        [TestMethod]
        public void BestAreaFitFreeRectangleExtractorTestFitNone()
        {
            //Arrange
            var rects = Enumerable.Repeat(new PPRect(0, 0, 64, 64), 16).ToList();
            var currentRect = new PPRect(0, 0, 64, 65);
            var extractor = new BestAreaFitFreeRectangleExtractor();

            //Act
            var result = extractor.ExtractFreeRectangle(rects, currentRect);

            //Assert
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void BestAreaFitFreeRectangleExtractorTestFitMany()
        {
            //Arrange
            var rects = new List<PPRect>()
            {
                new PPRect(0, 0, 32, 16),
                new PPRect(0, 0, 66, 65),
                new PPRect(0, 0, 65, 65),
                new PPRect(0, 0, 65, 65),
                new PPRect(0, 0, 32, 160),
                new PPRect(0, 0, 320, 16),
            };

            var currentRect = new PPRect(0, 0, 64, 65);
            var extractor = new BestAreaFitFreeRectangleExtractor();

            //Act
            var result = extractor.ExtractFreeRectangle(rects, currentRect);

            //Assert
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(65, result.Value.Width);
            Assert.AreEqual(65, result.Value.Height);
            Assert.AreEqual(0, result.Value.Left);
            Assert.AreEqual(0, result.Value.Top);
        }

        [TestMethod]
        public void BestAreaFitFreeRectangleExtractorTestFitMany2()
        {
            //Arrange
            var rects = new List<PPRect>()
            {
                new PPRect(7, 6, 7 + 32, 6 + 16),
                new PPRect(6, 1, 6 + 66, 1 + 65),
                new PPRect(14, 89, 14 + 65, 89 + 65),
                new PPRect(31, 124, 31 + 65, 124 + 65),
                new PPRect(89, 13, 89 + 32, 13 + 160),
                new PPRect(78, 500, 78 + 320, 500 + 16),
            };

            var currentRect = new PPRect(0, 0, 64, 65);
            var extractor = new BestAreaFitFreeRectangleExtractor();

            //Act
            var result = extractor.ExtractFreeRectangle(rects, currentRect);

            //Assert
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(65, result.Value.Width);
            Assert.AreEqual(65, result.Value.Height);
            Assert.IsTrue(14 == result.Value.Left || result.Value.Left == 31);
            Assert.IsTrue(89 == result.Value.Top || result.Value.Left == 124);
        }

        [TestMethod]
        public void BestAreaFitFreeRectangleExtractorTestFitSingle()
        {
            //Arrange
            var rects = new List<PPRect>()
            {
                new PPRect(7, 6, 7 + 32, 6 + 16),
                new PPRect(6, 1, 6 + 66, 1 + 65),
                new PPRect(14, 89, 14 + 65, 89 + 65),
                new PPRect(89, 13, 89 + 32, 13 + 160),
                new PPRect(78, 500, 78 + 320, 500 + 16),
            };

            var currentRect = new PPRect(0, 0, 64, 65);
            var extractor = new BestAreaFitFreeRectangleExtractor();

            //Act
            var result = extractor.ExtractFreeRectangle(rects, currentRect);

            //Assert
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(65, result.Value.Width);
            Assert.AreEqual(65, result.Value.Height);
            Assert.AreEqual(14, result.Value.Left);
            Assert.AreEqual(89, result.Value.Top);
        }
    }
}
