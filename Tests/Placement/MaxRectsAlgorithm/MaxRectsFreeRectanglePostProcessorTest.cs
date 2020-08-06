using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaunPacker.Core.Packing.Placement.MaximalRectangles;
using PaunPacker.Core.Types;

namespace PaunPacker.Tests.Placement
{
    /// <summary>
    /// This class contains unit tests for <see cref="MaxRectsFreeRectanglePostProcessor"/>
    /// </summary>
    [TestClass]
    public class MaxRectsFreeRectanglePostProcessorTest
    {
        [TestMethod]
        public void MaxRectsFreeRectanglePostProcessorTestSingleIntersectionInCorner()
        {
            //Arrange
            var postProcessor = new MaxRectsFreeRectanglePostProcessor();
            var freeRects = new List<PPRect>()
            {
                new PPRect(0, 0, 50, 50),
                new PPRect(50, 0, 100, 50)
            };
            var rectJustPlaced = new PPRect(0, 0, 20, 20);

            //Act
            postProcessor.PostProcess(freeRects, rectJustPlaced);

            //Assert
            Assert.AreEqual(3, freeRects.Count);
            var sortedList = freeRects.OrderBy(x => x.Left).ThenBy(x => x.Top).ToList();
            Assert.AreEqual(sortedList[0].Left, 0);
            Assert.AreEqual(sortedList[0].Top, 20);
            Assert.AreEqual(sortedList[0].Width, 50);
            Assert.AreEqual(sortedList[0].Height, 30);
            Assert.AreEqual(sortedList[1].Left, 20);
            Assert.AreEqual(sortedList[1].Top, 0);
            Assert.AreEqual(sortedList[1].Width, 30);
            Assert.AreEqual(sortedList[1].Height, 50);
            Assert.AreEqual(sortedList[2].Left, 50);
            Assert.AreEqual(sortedList[2].Top, 0);
            Assert.AreEqual(sortedList[2].Width, 50);
            Assert.AreEqual(sortedList[2].Height, 50);
        }

        [TestMethod]
        public void MaxRectsFreeRectanglePostProcessorTestSingleIntersectionExactFit()
        {
            //Arrange
            var postProcessor = new MaxRectsFreeRectanglePostProcessor();
            var freeRects = new List<PPRect>()
            {
                new PPRect(0, 0, 100, 100)
            };
            var rectJustPlaced = new PPRect(0, 0, 100, 100);

            //Act
            postProcessor.PostProcess(freeRects, rectJustPlaced);

            //Assert
            Assert.AreEqual(0, freeRects.Count);
        }

        [TestMethod]
        public void MaxRectsFreeRectanglePostProcessorTestSingleIntersectionInMiddle()
        {
            //Arrange
            var postProcessor = new MaxRectsFreeRectanglePostProcessor();
            var freeRects = new List<PPRect>()
            {
                new PPRect(0, 0, 50, 50),
                new PPRect(50, 0, 50 + 50, 50)
            };
            var rectJustPlaced = new PPRect(10, 20, 10 + 20, 20 + 20);

            //Act
            postProcessor.PostProcess(freeRects, rectJustPlaced);

            //Assert
            Assert.AreEqual(5, freeRects.Count); //First free rectangle was divided into four parts
            var sortedList = freeRects.OrderBy(x => x.Left).ThenBy(x => x.Top).ThenBy(x => x.Right).ToList();
            Assert.AreEqual(0, sortedList[0].Left);
            Assert.AreEqual(0, sortedList[0].Top);
            Assert.AreEqual(10, sortedList[0].Width);
            Assert.AreEqual(50, sortedList[0].Height);

            Assert.AreEqual(0, sortedList[1].Left);
            Assert.AreEqual(0, sortedList[1].Top);
            Assert.AreEqual(50, sortedList[1].Width);
            Assert.AreEqual(20, sortedList[1].Height);

            Assert.AreEqual(0, sortedList[2].Left);
            Assert.AreEqual(40, sortedList[2].Top);
            Assert.AreEqual(50, sortedList[2].Width);
            Assert.AreEqual(10, sortedList[2].Height);

            Assert.AreEqual(30, sortedList[3].Left);
            Assert.AreEqual(0, sortedList[3].Top);
            Assert.AreEqual(20, sortedList[3].Width);
            Assert.AreEqual(50, sortedList[3].Height);
        }

        [TestMethod]
        public void MaxRectsFreeRectanglePostProcessorTestMultiIntersection()
        {
            //Arrange
            var postProcessor = new MaxRectsFreeRectanglePostProcessor();
            var freeRects = new List<PPRect>()
            {
                new PPRect(0, 0, 50, 50),
                new PPRect(40, 40, 40 + 50, 40 + 50)
            };
            var rectJustPlaced = new PPRect(40, 40, 50, 50);

            //Act
            postProcessor.PostProcess(freeRects, rectJustPlaced);

            //Assert
            Assert.AreEqual(4, freeRects.Count);
            var sortedList = freeRects.OrderBy(x => x.Left).ThenBy(x => x.Top).ThenBy(x => x.Right).ToList();
            Assert.AreEqual(0, sortedList[0].Left);
            Assert.AreEqual(0, sortedList[0].Top);
            Assert.AreEqual(40, sortedList[0].Width);
            Assert.AreEqual(50, sortedList[0].Height);

            Assert.AreEqual(0, sortedList[1].Left);
            Assert.AreEqual(0, sortedList[1].Top);
            Assert.AreEqual(50, sortedList[1].Width);
            Assert.AreEqual(40, sortedList[1].Height);

            Assert.AreEqual(40, sortedList[2].Left);
            Assert.AreEqual(50, sortedList[2].Top);
            Assert.AreEqual(50, sortedList[2].Width);
            Assert.AreEqual(40, sortedList[2].Height);

            Assert.AreEqual(50, sortedList[3].Left);
            Assert.AreEqual(40, sortedList[3].Top);
            Assert.AreEqual(40, sortedList[3].Width);
            Assert.AreEqual(50, sortedList[3].Height);
        }
    }
}
