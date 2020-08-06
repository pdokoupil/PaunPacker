using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaunPacker.Core.Packing.Placement.MaximalRectangles;
using PaunPacker.Core.Types;

namespace PaunPacker.Tests.Placement
{
    /// <summary>
    /// This class contains unit tests for <see cref="MaxRectsFreeRectangleSplitter"/>
    /// </summary>
    [TestClass]
    public class MaxRectsFreeRectangleSplitterTest
    {
        [TestMethod]
        public void MaxRectsFreeRectangleSplitterTestNotFull()
        {
            //Arrange
            var splitter = new MaxRectsFreeRectangleSplitter();
            var freeRect = new PPRect(0, 0, 100, 100);
            var rectJustPlaced = new PPRect(0, 0, 50, 50);
            var expectedFreeRects = new List<PPRect>()
            {
                new PPRect(0, 50, 50, 100),
                new PPRect(50, 0, 100, 100),
                new PPRect(0, 50, 100, 100),
                new PPRect(50, 0, 100, 50)
            };

            //Act
            var result = splitter.SplitFreeRectangle(freeRect, rectJustPlaced);

            //Assert
            CollectionAssert.AreEquivalent(expectedFreeRects, result.ToList());
        }

        [TestMethod]
        public void MaxRectsFreeRectangleSplitterTestFullWidth()
        {
            //Arrange
            var splitter = new MaxRectsFreeRectangleSplitter();
            var freeRect = new PPRect(0, 0, 100, 100);
            var rectJustPlaced = new PPRect(0, 0, 100, 50);
            var expectedFreeRects = new List<PPRect>()
            {
                new PPRect(0, 50, 100, 100),
            };

            //Act
            var result = splitter.SplitFreeRectangle(freeRect, rectJustPlaced);

            //Assert
            CollectionAssert.AreEquivalent(expectedFreeRects, result.ToList());
        }

        [TestMethod]
        public void MaxRectsFreeRectangleSplitterTestFullHeight()
        {
            //Arrange
            var splitter = new MaxRectsFreeRectangleSplitter();
            var freeRect = new PPRect(0, 0, 100, 100);
            var rectJustPlaced = new PPRect(0, 0, 50, 100);
            var expectedFreeRects = new List<PPRect>()
            {
                new PPRect(50, 0, 100, 100)
            };

            //Act
            var result = splitter.SplitFreeRectangle(freeRect, rectJustPlaced);

            //Assert
            CollectionAssert.AreEquivalent(expectedFreeRects, result.ToList());
        }

        [TestMethod]
        public void MaxRectsFreeRectangleSplitterTestFullBoth()
        {
            //Arrange
            var splitter = new MaxRectsFreeRectangleSplitter();
            var freeRect = new PPRect(0, 0, 100, 100);
            var rectJustPlaced = new PPRect(0, 0, 100, 100);

            //Act
            var result = splitter.SplitFreeRectangle(freeRect, rectJustPlaced);

            //Assert
            Assert.AreEqual(0, result.Count());
        }
    }
}
