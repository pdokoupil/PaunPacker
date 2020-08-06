using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaunPacker.Core.Packing.Placement.Guillotine;
using PaunPacker.Core.Types;

namespace PaunPacker.Tests.Placement
{
    /// <summary>
    /// This class contains unit tests for <see cref="LongerAxisGuillotineFreeRectangleSplitter"/>
    /// </summary>
    [TestClass]
    public class LongerAxisGuillotineFreeRectangleSplitterTest
    {
        [TestMethod]
        public void LongerAxisGuillotineFreeRectangleSplitterTestSplitByHorizontalAxis()
        {
            //Arrange
            var splitter = new LongerAxisGuillotineFreeRectangleSplitter();
            var freeRectangle = new PPRect(0, 0, 100, 200);
            var rectToBePlaced = new PPRect(0, 0, 20, 10);

            //Act
            var result = splitter.SplitFreeRectangle(freeRectangle, rectToBePlaced);

            //Assert
            Assert.AreEqual(2, result.Count());
            var item1 = result.First();
            var item2 = result.Skip(1).First();
            Assert.IsTrue((item1.Width == 100 && item1.Height == 190 && item2.Width == 80 && item2.Height == 10) ||
                (item2.Width == 100 && item2.Height == 190 && item2.Width == 80 && item2.Height == 10));
        }

        [TestMethod]
        public void LongerAxisGuillotineFreeRectangleSplitterTestSplitByVerticalAxis()
        {
            //Arrange
            var splitter = new LongerAxisGuillotineFreeRectangleSplitter();
            var freeRectangle = new PPRect(0, 0, 100, 200);
            var rectToBePlaced = new PPRect(0, 0, 10, 20);

            //Act
            var result = splitter.SplitFreeRectangle(freeRectangle, rectToBePlaced);

            //Assert
            Assert.AreEqual(2, result.Count());
            var item1 = result.First();
            var item2 = result.Skip(1).First();
            Assert.IsTrue((item1.Height == 180 && item1.Width == 10 && item2.Height == 200 && item2.Width == 90) ||
                (item2.Height == 180 && item2.Width == 10 && item1.Height == 200 && item1.Width == 90));
        }

        [TestMethod]
        public void LongerAxisGuillotineFreeRectangleSplitterTestFullWidth()
        {
            //Arrange
            var splitter = new LongerAxisGuillotineFreeRectangleSplitter();
            var freeRectangle = new PPRect(0, 0, 100, 200);
            var rectToBePlaced = new PPRect(0, 0, 100, 20);

            //Act
            var result = splitter.SplitFreeRectangle(freeRectangle, rectToBePlaced);

            //Assert
            Assert.AreEqual(1, result.Count());
            var item1 = result.First();
            Assert.AreEqual(100, item1.Width);
            Assert.AreEqual(180, item1.Height);
        }

        [TestMethod]
        public void LongerAxisGuillotineFreeRectangleSplitterTestFullHeight()
        {
            //Arrange
            var splitter = new LongerAxisGuillotineFreeRectangleSplitter();
            var freeRectangle = new PPRect(0, 0, 200, 100);
            var rectToBePlaced = new PPRect(0, 0, 20, 100);

            //Act
            var result = splitter.SplitFreeRectangle(freeRectangle, rectToBePlaced);

            //Assert
            Assert.AreEqual(1, result.Count());
            var item1 = result.First();
            Assert.AreEqual(180, item1.Width);
            Assert.AreEqual(100, item1.Height);
        }

        //Tests exact fit
        [TestMethod]
        public void LongerAxisGuillotineFreeRectangleSplitterTestFullBoth()
        {
            //Arrange
            var splitter = new LongerAxisGuillotineFreeRectangleSplitter();
            var freeRectangle = new PPRect(0, 0, 100, 100);
            var rectToBePlaced = new PPRect(0, 0, 100, 100);

            //Act
            var result = splitter.SplitFreeRectangle(freeRectangle, rectToBePlaced);

            //Assert
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void LongerAxisGuillotineFreeRectangleSplitterTestPositionHasNoEffect()
        {
            //Arrange
            var splitter = new LongerAxisGuillotineFreeRectangleSplitter();
            var freeRectangle = new PPRect(0, 0, 100, 100);
            var freeRectangle2 = new PPRect(76, 54, 76 + 100, 54 + 100);
            var freeRectangle3 = new PPRect(220, 11, 220 + 100, 11 + 100);
            var rectToBePlaced = new PPRect(0, 0, 50, 50);

            //Act
            var result = splitter.SplitFreeRectangle(freeRectangle, rectToBePlaced);
            var result2 = splitter.SplitFreeRectangle(freeRectangle2, rectToBePlaced);
            var result3 = splitter.SplitFreeRectangle(freeRectangle3, rectToBePlaced);

            //Assert
            Assert.AreEqual(2, result.Count());
            var item1 = result.First();
            var item2 = result.Skip(1).First();
            Assert.AreEqual(50, item1.Width);
            Assert.AreEqual(50, item2.Width);
            Assert.IsTrue(item1.Height == 50 || item1.Height == 100);
            if (item1.Height == 50)
            {
                //Then the second must have 100
                Assert.AreEqual(100, item2.Height);
            }
            else
            {
                Assert.AreEqual(50, item2.Height);
            }

            Assert.AreEqual(2, result3.Count());
            item1 = result2.First();
            item2 = result2.Skip(1).First();
            Assert.AreEqual(item1.Width, item1.Width);
            Assert.AreEqual(item1.Height, item1.Height);
            Assert.AreEqual(item2.Width, item2.Width);
            Assert.AreEqual(item2.Height, item2.Height);

            Assert.AreEqual(2, result3.Count());
            item1 = result3.First();
            item2 = result3.Skip(1).First();
            Assert.AreEqual(item1.Width, item1.Width);
            Assert.AreEqual(item1.Height, item1.Height);
            Assert.AreEqual(item2.Width, item2.Width);
            Assert.AreEqual(item2.Height, item2.Height);
        }
    }
}
