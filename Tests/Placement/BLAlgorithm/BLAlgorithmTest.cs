using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaunPacker.Core.Packing.Placement;
using PaunPacker.Core.Types;
using PaunPacker.Tests;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PaunPacker.Tests.Placement
{
    /// <summary>
    /// This class contains unit tests for <see cref="BLAlgorithmPacker"/>
    /// </summary>
    [TestClass]
    public class BLAlgorithmTest
    {
        [TestMethod]
        public void BLAlgorithmTestNoFit()
        {
            //Arrange
            var blAlgorithm = new BLAlgorithmPacker();
            var rects = new List<PPRect>()
            {
                new PPRect(0, 0, 20, 20)
            };

            //Act
            var result = blAlgorithm.PlaceRects(10, 10, rects);

            //Assert
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void BLAlgorithmTestNoEmptyRect()
        {
            //Arrange
            var blAlgorithm = new BLAlgorithmPacker();
            var rects = new List<PPRect>()
            {
                new PPRect(0, 0, 10, 100),
                new PPRect(0, 0, 10, 100),
                new PPRect(0, 0, 10, 100),
                new PPRect(0, 0, 10, 100),
                new PPRect(0, 0, 70, 100),
            };

            //Act
            var result = blAlgorithm.PlaceRects(100, 100, rects);

            //Assert
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        //Assert
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void BLAlgorithmTestNegativeParameters()
        {
            //Arrange
            var blAlgorithm = new BLAlgorithmPacker();
            var rects = new List<PPRect>()
            {
                new PPRect(0, 0, 20, 20)
            };

            //Act
            blAlgorithm.PlaceRects(-1, -10, rects);
        }

        [TestMethod]
        public void BLAlgorithmTestFullHeight()
        {
            //Arrange
            var blAlgorithm = new BLAlgorithmPacker();
            var rects = new List<PPRect>()
            {
                new PPRect(0, 0, 10, 100),
                new PPRect(0, 0, 10, 100),
                new PPRect(0, 0, 10, 100),
                new PPRect(0, 0, 10, 100)
            };

            //Act
            var result = blAlgorithm.PlaceRects(100, 100, rects);

            //Assert
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(true, TestUtil.IsPackingResultValid(result));
            var actualW = result.Rects.Max(x => x.Right);
            var actualH = result.Rects.Max(x => x.Bottom);
            Assert.AreEqual(100, actualH);
            Assert.AreEqual(40, actualW);
        }

        [TestMethod]
        public void BLAlgorithmTestFullWidth()
        {
            //Arrange
            var blAlgorithm = new BLAlgorithmPacker();
            var rects = new List<PPRect>()
            {
                new PPRect(0, 0, 100, 10),
                new PPRect(0, 0, 100, 10),
                new PPRect(0, 0, 100, 10),
                new PPRect(0, 0, 100, 10)
            };

            //Act
            var result = blAlgorithm.PlaceRects(100, 100, rects);

            //Assert
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(true, TestUtil.IsPackingResultValid(result));
            var actualW = result.Rects.Max(x => x.Right);
            var actualH = result.Rects.Max(x => x.Bottom);
            Assert.AreEqual(100, actualW);
            Assert.AreEqual(40, actualH);
        }


    }
}
