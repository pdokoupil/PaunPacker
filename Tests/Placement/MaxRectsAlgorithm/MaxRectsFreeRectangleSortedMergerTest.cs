using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaunPacker.Core.Packing.Placement.MaximalRectangles;
using PaunPacker.Core.Types;

namespace PaunPacker.Tests.Placement
{
    /// <summary>
    /// This class contains basic unit tests for <see cref="MaxRectsFreeRectangleSortedMerger"/>
    /// </summary>
    [TestClass]
    public class MaxRectsFreeRectangleSortedMergerTest
    {
        [TestMethod]
        public void MaxRectsFreeRectangleSortedMergerTestMergeToEmpty()
        {
            //Arrange
            var freeRectangles = new List<PPRect>();
            var merger = new MaxRectsFreeRectangleSortedMerger();
            var freeRect1 = new PPRect(0, 0, 100, 100);
            var freeRect2 = new PPRect(0, 0, 100, 50);
            var oldSize = freeRectangles.Count;

            //Act
            merger.MergeFreeRectangles(freeRectangles, new PPRect[] { freeRect1, freeRect2 });

            //Assert
            Assert.AreEqual(oldSize + 2, freeRectangles.Count);
            Assert.AreEqual(100, freeRectangles[0].Width);
            Assert.AreEqual(50, freeRectangles[0].Height);
            Assert.AreEqual(100, freeRectangles[1].Width);
            Assert.AreEqual(100, freeRectangles[1].Height);
        }

        [TestMethod]
        public void MaxRectsFreeRectangleSortedMergerTestMergeOneHasZeroSize()
        {
            //Arrange
            var freeRectangles = new List<PPRect>();
            var merger = new MaxRectsFreeRectangleSortedMerger();
            var freeRect1 = new PPRect(0, 0, 100, 20);
            var freeRect2 = new PPRect(0, 0, 100, 0);
            var oldSize = freeRectangles.Count;

            //Act
            merger.MergeFreeRectangles(freeRectangles, new PPRect[] { freeRect1, freeRect2 });

            //Assert
            Assert.AreEqual(oldSize + 1, freeRectangles.Count);
            Assert.AreEqual(100, freeRectangles[0].Width);
            Assert.AreEqual(20, freeRectangles[0].Height);
        }

        [TestMethod]
        public void MaxRectsFreeRectangleSortedMergerTestMergeSimple()
        {
            //Arrange
            var freeRectangles = new List<PPRect>()
            {
                new PPRect(0, 0, 10, 12),
                new PPRect(0, 0, 10, 13),
                new PPRect(0, 0, 1000, 200)
            };

            var merger = new MaxRectsFreeRectangleSortedMerger();
            var freeRect1 = new PPRect(0, 0, 10, 14);
            var freeRect2 = new PPRect(0, 0, 1000, 201);
            var oldSize = freeRectangles.Count;

            //Act
            merger.MergeFreeRectangles(freeRectangles, new PPRect[] { freeRect1, freeRect2 });

            //Assert
            Assert.AreEqual(oldSize + 2, freeRectangles.Count);
            Assert.AreEqual(10, freeRectangles[0].Width);
            Assert.AreEqual(12, freeRectangles[0].Height);
            Assert.AreEqual(10, freeRectangles[1].Width);
            Assert.AreEqual(13, freeRectangles[1].Height);
            Assert.AreEqual(10, freeRectangles[2].Width);
            Assert.AreEqual(14, freeRectangles[2].Height);
            Assert.AreEqual(1000, freeRectangles[3].Width);
            Assert.AreEqual(200, freeRectangles[3].Height);
            Assert.AreEqual(1000, freeRectangles[4].Width);
            Assert.AreEqual(201, freeRectangles[4].Height);
        }
    }
}
