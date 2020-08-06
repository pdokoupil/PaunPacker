using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PaunPacker.Core.Packing;
using PaunPacker.Core.Packing.MBBF;
using PaunPacker.Core.Packing.Placement;
using PaunPacker.Core.Types;

namespace PaunPacker.Tests.MBBF
{
    [TestClass]
    public class FixedSizePackerTest
    {
        [TestMethod]
        public void FixedSizePackerTestValidSquares1()
        {
            //Arrange
            var inputSequence = (TestUtil.Shuffle(TestUtil.GetIncreasingSquares(1)));
            var placementAlgorithmMock = new Mock<IPlacementAlgorithm>();

            placementAlgorithmMock
                .Setup(x => x.PlaceRects(It.IsAny<int>(), It.IsAny<int>(), inputSequence, It.IsAny<CancellationToken>()))
                .Returns<int, int, IEnumerable<PPRect>, CancellationToken> ((w, h, rects, _) =>
                {
                    //Assume "dumb" placement algorithm that simple places all the rects consecutively next to each other
                    int maxH = rects.Max(x => x.Height);
                    int sumW = rects.Sum(x => x.Width);
                    if (sumW > w || maxH > h)
                        return null;
                    int left = 0;
                    return new PackingResult(sumW, maxH, rects.Select(x =>
                    {
                        left += x.Width;
                        return new PPRect(left, 0, left, x.Height);
                    })); ;
                });

            var packer = new FixedSizePacker(1, 1, placementAlgorithmMock.Object);

            //Act
            var result = packer.FindMinimumBoundingBox(inputSequence);

            //Assert
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(true, TestUtil.IsPackingResultValid(result));
        }

        [TestMethod]
        public void FixedSizePackerTestValidSquares2()
        {
            //Arrange
            var inputSequence = (TestUtil.Shuffle(TestUtil.GetIncreasingSquares(2)));
            var placementAlgorithmMock = new Mock<IPlacementAlgorithm>();

            placementAlgorithmMock
                .Setup(x => x.PlaceRects(It.IsAny<int>(), It.IsAny<int>(), inputSequence, It.IsAny<CancellationToken>()))
                .Returns<int, int, IEnumerable<PPRect>, CancellationToken>((w, h, rects, _) =>
                {
                    //Assume "dumb" placement algorithm that simple places all the rects consecutively next to each other
                    int maxH = rects.Max(x => x.Height);
                    int sumW = rects.Sum(x => x.Width);
                    if (sumW > w || maxH > h)
                        return null;
                    int left = 0;
                    return new PackingResult(sumW, maxH, rects.Select(x =>
                    {
                        left += x.Width;
                        return new PPRect(left, 0, left, x.Height);
                    })); ;
                });
            var packer = new FixedSizePacker(3, 3, placementAlgorithmMock.Object);

            //Act
            var result = packer.FindMinimumBoundingBox(inputSequence);
            
            //Assert
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(true, TestUtil.IsPackingResultValid(result));
        }

        [TestMethod]
        public void FixedSizePackerTestValidSquares3()
        {
            //Arrange 
            var inputSequence = (TestUtil.Shuffle(TestUtil.GetIncreasingSquares(3)));
            var placementAlgorithmMock = new Mock<IPlacementAlgorithm>();

            placementAlgorithmMock
                .Setup(x => x.PlaceRects(It.IsAny<int>(), It.IsAny<int>(), inputSequence, It.IsAny<CancellationToken>()))
                .Returns<int, int, IEnumerable<PPRect>, CancellationToken>((w, h, rects, _) =>
                {
                    //Assume "dumb" placement algorithm that simple places all the rects consecutively next to each other
                    int maxH = rects.Max(x => x.Height);
                    int sumW = rects.Sum(x => x.Width);
                    if (sumW > w || maxH > h)
                        return null;
                    int left = 0;
                    return new PackingResult(sumW, maxH, rects.Select(x =>
                    {
                        left += x.Width;
                        return new PPRect(left, 0, left, x.Height);
                    })); ;
                });
            var packer = new FixedSizePacker(6, 6, placementAlgorithmMock.Object);

            //Act
            var result = packer.FindMinimumBoundingBox(inputSequence);

            //Assert
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(true, TestUtil.IsPackingResultValid(result));
        }

        [TestMethod]
        public void FixedSizePackerTestValidSquares3NoFit()
        {
            //Arrange 
            var inputSequence = (TestUtil.Shuffle(TestUtil.GetIncreasingSquares(3)));
            var placementAlgorithmMock = new Mock<IPlacementAlgorithm>();

            placementAlgorithmMock
                .Setup(x => x.PlaceRects(It.IsAny<int>(), It.IsAny<int>(), inputSequence, It.IsAny<CancellationToken>()))
                .Returns<int, int, IEnumerable<PPRect>, CancellationToken>((w, h, rects, _) =>
                {
                    //Assume "dumb" placement algorithm that simple places all the rects consecutively next to each other
                    int maxH = rects.Max(x => x.Height);
                    int sumW = rects.Sum(x => x.Width);
                    if (sumW > w || maxH > h)
                        return null;
                    int left = 0;
                    return new PackingResult(sumW, maxH, rects.Select(x =>
                    {
                        left += x.Width;
                        return new PPRect(left, 0, left, x.Height);
                    })); ;
                });
            var packer = new FixedSizePacker(5, 2, placementAlgorithmMock.Object);

            //Act
            var result = packer.FindMinimumBoundingBox(inputSequence);

            //Assert
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void FixedSizePackerTestValidSquares4()
        {
            //Arrange 
            var inputSequence = (TestUtil.Shuffle(TestUtil.GetIncreasingSquares(4)));
            var placementAlgorithmMock = new Mock<IPlacementAlgorithm>();

            placementAlgorithmMock
                .Setup(x => x.PlaceRects(It.IsAny<int>(), It.IsAny<int>(), inputSequence, It.IsAny<CancellationToken>()))
                .Returns<int, int, IEnumerable<PPRect>, CancellationToken>((w, h, rects, _) =>
                {
                    //Assume "dumb" placement algorithm that simple places all the rects consecutively next to each other
                    int maxH = rects.Max(x => x.Height);
                    int sumW = rects.Sum(x => x.Width);
                    if (sumW > w || maxH > h)
                        return null;
                    int left = 0;
                    return new PackingResult(sumW, maxH, rects.Select(x =>
                    {
                        left += x.Width;
                        return new PPRect(left, 0, left, x.Height);
                    })); ;
                });

            var packer = new FixedSizePacker(12, 12, placementAlgorithmMock.Object);

            //Act
            var result = packer.FindMinimumBoundingBox(inputSequence);
            var mockResult = placementAlgorithmMock.Object.PlaceRects(12, 12, inputSequence);

            //Assert (Check that it only wraps the placement algorithm)
            var mockActualW = mockResult.Rects.Max(x => x.Right);
            var mockActualH = mockResult.Rects.Max(x => x.Bottom);
            var actualW = result.Rects.Max(x => x.Right);
            var actualH = result.Rects.Max(x => x.Bottom);
            Assert.AreEqual(mockActualW, actualW);
            Assert.AreEqual(mockActualH, actualH);
        }

        [TestMethod]
        public void FixedSizePackerTestValidSquares5()
        {
            //Arrange 
            var inputSequence = (TestUtil.Shuffle(TestUtil.GetIncreasingSquares(5)));
            var placementAlgorithmMock = new Mock<IPlacementAlgorithm>();

            placementAlgorithmMock
                .Setup(x => x.PlaceRects(It.IsAny<int>(), It.IsAny<int>(), inputSequence, It.IsAny<CancellationToken>()))
                .Returns<int, int, IEnumerable<PPRect>, CancellationToken>((w, h, rects, _) =>
                {
                    //Assume "dumb" placement algorithm that simple places all the rects consecutively next to each other
                    int maxH = rects.Max(x => x.Height);
                    int sumW = rects.Sum(x => x.Width);
                    if (sumW > w || maxH > h)
                        return null;
                    int left = 0;
                    return new PackingResult(sumW, maxH, rects.Select(x =>
                    {
                        left += x.Width;
                        return new PPRect(left, 0, left, x.Height);
                    })); ;
                });
            var packer = new FixedSizePacker(20, 20, placementAlgorithmMock.Object);

            //Act
            var result = packer.FindMinimumBoundingBox(inputSequence);
            var mockResult = placementAlgorithmMock.Object.PlaceRects(20, 20, inputSequence);

            //Assert (Check that it only wraps the placement algorithm)
            var mockActualW = mockResult.Rects.Max(x => x.Right);
            var mockActualH = mockResult.Rects.Max(x => x.Bottom);
            var actualW = result.Rects.Max(x => x.Right);
            var actualH = result.Rects.Max(x => x.Bottom);
            Assert.AreEqual(mockActualW, actualW);
            Assert.AreEqual(mockActualH, actualH);
        }

        [TestMethod]
        public void FixedSizePackerTestValidSquares10()
        {
            //Arrange 
            var inputSequence = (TestUtil.Shuffle(TestUtil.GetIncreasingSquares(10)));
            var placementAlgorithmMock = new Mock<IPlacementAlgorithm>();

            placementAlgorithmMock
                .Setup(x => x.PlaceRects(It.IsAny<int>(), It.IsAny<int>(), inputSequence, It.IsAny<CancellationToken>()))
                .Returns<int, int, IEnumerable<PPRect>, CancellationToken>((w, h, rects, _) =>
                {
                    //Assume "dumb" placement algorithm that simple places all the rects consecutively next to each other
                    int maxH = rects.Max(x => x.Height);
                    int sumW = rects.Sum(x => x.Width);
                    if (sumW > w || maxH > h)
                        return null;
                    int left = 0;
                    return new PackingResult(sumW, maxH, rects.Select(x =>
                    {
                        left += x.Width;
                        return new PPRect(left, 0, left, x.Height);
                    })); ;
                });
            var packer = new FixedSizePacker(100, 100, placementAlgorithmMock.Object);

            //Act
            var result = packer.FindMinimumBoundingBox(inputSequence);
            var mockResult = placementAlgorithmMock.Object.PlaceRects(100, 100, inputSequence);

            //Assert (Check that it only wraps the placement algorithm)
            var mockActualW = mockResult.Rects.Max(x => x.Right);
            var mockActualH = mockResult.Rects.Max(x => x.Bottom);
            var actualW = result.Rects.Max(x => x.Right);
            var actualH = result.Rects.Max(x => x.Bottom);
            Assert.AreEqual(mockActualW, actualW);
            Assert.AreEqual(mockActualH, actualH);
        }

        [TestMethod]
        public void FixedSizePackerTestValidSquares15()
        {
            //Arrange 
            var inputSequence = (TestUtil.Shuffle(TestUtil.GetIncreasingSquares(15)));
            var placementAlgorithmMock = new Mock<IPlacementAlgorithm>();

            placementAlgorithmMock
                .Setup(x => x.PlaceRects(It.IsAny<int>(), It.IsAny<int>(), inputSequence, It.IsAny<CancellationToken>()))
                .Returns<int, int, IEnumerable<PPRect>, CancellationToken>((w, h, rects, _) =>
                {
                    //Assume "dumb" placement algorithm that simple places all the rects consecutively next to each other
                    int maxH = rects.Max(x => x.Height);
                    int sumW = rects.Sum(x => x.Width);
                    if (sumW > w || maxH > h)
                        return null;
                    int left = 0;
                    return new PackingResult(sumW, maxH, rects.Select(x =>
                    {
                        left += x.Width;
                        return new PPRect(left, 0, left, x.Height);
                    })); ;
                });
            var packer = new FixedSizePacker(1000, 1000, placementAlgorithmMock.Object);

            //Act
            var result = packer.FindMinimumBoundingBox(inputSequence);
            var mockResult = placementAlgorithmMock.Object.PlaceRects(1000, 1000, inputSequence);

            //Assert (Check that it only wraps the placement algorithm)
            var mockActualW = mockResult.Rects.Max(x => x.Right);
            var mockActualH = mockResult.Rects.Max(x => x.Bottom);
            var actualW = result.Rects.Max(x => x.Right);
            var actualH = result.Rects.Max(x => x.Bottom);
            Assert.AreEqual(mockActualW, actualW);
            Assert.AreEqual(mockActualH, actualH);
        }

        [TestMethod]
        public void FixedSizePackerTestValidSquares20()
        {
            //Arrange 
            var inputSequence = (TestUtil.Shuffle(TestUtil.GetIncreasingSquares(20)));
            var placementAlgorithmMock = new Mock<IPlacementAlgorithm>();

            placementAlgorithmMock
                .Setup(x => x.PlaceRects(It.IsAny<int>(), It.IsAny<int>(), inputSequence, It.IsAny<CancellationToken>()))
                .Returns<int, int, IEnumerable<PPRect>, CancellationToken>((w, h, rects, _) =>
                {
                    //Assume "dumb" placement algorithm that simple places all the rects consecutively next to each other
                    int maxH = rects.Max(x => x.Height);
                    int sumW = rects.Sum(x => x.Width);
                    if (sumW > w || maxH > h)
                        return null;
                    int left = 0;
                    return new PackingResult(sumW, maxH, rects.Select(x =>
                    {
                        left += x.Width;
                        return new PPRect(left, 0, left, x.Height);
                    })); ;
                });
            var packer = new FixedSizePacker(1000, 1000, placementAlgorithmMock.Object);

            //Act
            var result = packer.FindMinimumBoundingBox(inputSequence);
            var mockResult = placementAlgorithmMock.Object.PlaceRects(1000, 1000, inputSequence);

            //Assert (Check that it only wraps the placement algorithm)
            var mockActualW = mockResult.Rects.Max(x => x.Right);
            var mockActualH = mockResult.Rects.Max(x => x.Bottom);
            var actualW = result.Rects.Max(x => x.Right);
            var actualH = result.Rects.Max(x => x.Bottom);
            Assert.AreEqual(mockActualW, actualW);
            Assert.AreEqual(mockActualH, actualH);
        }

        [TestMethod]
        public void FixedSizePackerTestValidSquares50()
        {
            //Arrange 
            var inputSequence = (TestUtil.Shuffle(TestUtil.GetIncreasingSquares(50)));
            var placementAlgorithmMock = new Mock<IPlacementAlgorithm>();

            placementAlgorithmMock
                .Setup(x => x.PlaceRects(It.IsAny<int>(), It.IsAny<int>(), inputSequence, It.IsAny<CancellationToken>()))
                .Returns<int, int, IEnumerable<PPRect>, CancellationToken>((w, h, rects, _) =>
                {
                    //Assume "dumb" placement algorithm that simple places all the rects consecutively next to each other
                    int maxH = rects.Max(x => x.Height);
                    int sumW = rects.Sum(x => x.Width);
                    if (sumW > w || maxH > h)
                        return null;
                    int left = 0;
                    return new PackingResult(sumW, maxH, rects.Select(x =>
                    {
                        left += x.Width;
                        return new PPRect(left, 0, left, x.Height);
                    })); ;
                });
            var packer = new FixedSizePacker(2000, 2000, placementAlgorithmMock.Object);

            //Act
            var result = packer.FindMinimumBoundingBox(inputSequence);
            var mockResult = placementAlgorithmMock.Object.PlaceRects(2000, 2000, inputSequence);

            //Assert (Check that it only wraps the placement algorithm)
            var mockActualW = mockResult.Rects.Max(x => x.Right);
            var mockActualH = mockResult.Rects.Max(x => x.Bottom);
            var actualW = result.Rects.Max(x => x.Right);
            var actualH = result.Rects.Max(x => x.Bottom);
            Assert.AreEqual(mockActualW, actualW);
            Assert.AreEqual(mockActualH, actualH);
        }

        [TestMethod]
        //Assert
        [ExpectedException(typeof(ArgumentNullException))]
        public void FixedSizePackerTestArgumentNull()
        {
            //Arrange 
            IEnumerable<PPRect> inputSequence = null;
            var placementAlgorithmMock = new Mock<IPlacementAlgorithm>();

            placementAlgorithmMock
                .Setup(x => x.PlaceRects(It.IsAny<int>(), It.IsAny<int>(), inputSequence, It.IsAny<CancellationToken>()))
                .Returns<int, int, IEnumerable<PPRect>, CancellationToken>((w, h, rects, _) =>
                {
                    //Assume "dumb" placement algorithm that simple places all the rects consecutively next to each other
                    int maxH = rects.Max(x => x.Height);
                    int sumW = rects.Sum(x => x.Width);
                    if (sumW > w || maxH > h)
                        return null;
                    int left = 0;
                    return new PackingResult(sumW, maxH, rects.Select(x =>
                    {
                        left += x.Width;
                        return new PPRect(left, 0, left, x.Height);
                    })); ;
                });
            var packer = new FixedSizePacker(100, 100, placementAlgorithmMock.Object);

            //Act
            var result = packer.FindMinimumBoundingBox(inputSequence);

            //Assert
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(true, TestUtil.IsPackingResultValid(result));
        }

        [TestMethod]
        public void FixedSizePackerTestInstanceReusability()
        {
            //Arrange 
            var inputSequence = (TestUtil.Shuffle(TestUtil.GetIncreasingSquares(20)));
            var placementAlgorithmMock = new Mock<IPlacementAlgorithm>();

            placementAlgorithmMock
                .Setup(x => x.PlaceRects(It.IsAny<int>(), It.IsAny<int>(), inputSequence, It.IsAny<CancellationToken>()))
                .Returns<int, int, IEnumerable<PPRect>, CancellationToken>((w, h, rects, _) =>
                {
                    //Assume "dumb" placement algorithm that simple places all the rects consecutively next to each other
                    int maxH = rects.Max(x => x.Height);
                    int sumW = rects.Sum(x => x.Width);
                    if (sumW > w || maxH > h)
                        return null;
                    int left = 0;
                    return new PackingResult(sumW, maxH, rects.Select(x =>
                    {
                        left += x.Width;
                        return new PPRect(left, 0, left, x.Height);
                    })); ;
                });
            var packer = new FixedSizePacker(1000, 1000, placementAlgorithmMock.Object);

            //Act
            var result1 = packer.FindMinimumBoundingBox(inputSequence);
            var result2 = packer.FindMinimumBoundingBox(inputSequence);
            var result3 = packer.FindMinimumBoundingBox(inputSequence);

            //Assert
            Assert.AreNotEqual(null, result1);
            Assert.AreEqual(true, TestUtil.IsPackingResultValid(result1));
            Assert.AreEqual(true, TestUtil.Succeeded(result1.Width, result1.Height, result2.Width, result2.Height));
            Assert.AreEqual(true, TestUtil.Succeeded(result1.Width, result1.Height, result3.Width, result3.Height));
        }

        [TestMethod]
        public void FixedSizePackerTestSimilarToUnknown4()
        {
            //Arrange 
            var inputSequence = (TestUtil.Shuffle(TestUtil.GetIncreasingSquares(4)));
            var placementAlgorithmMock = new Mock<IPlacementAlgorithm>();

            placementAlgorithmMock
                .Setup(x => x.PlaceRects(It.IsAny<int>(), It.IsAny<int>(), inputSequence, It.IsAny<CancellationToken>()))
                .Returns<int, int, IEnumerable<PPRect>, CancellationToken>((w, h, rects, _) =>
                {
                    //Assume "dumb" placement algorithm that simple places all the rects consecutively next to each other
                    int maxH = rects.Max(x => x.Height);
                    int sumW = rects.Sum(x => x.Width);
                    if (sumW > w || maxH > h)
                        return null;
                    int left = 0;
                    return new PackingResult(sumW, maxH, rects.Select(x =>
                    {
                        left += x.Width;
                        return new PPRect(left, 0, left, x.Height);
                    })); ;
                });

            var packer = new FixedSizePacker(12, 12, placementAlgorithmMock.Object);
            var packer2 = new UnknownSizePacker(placementAlgorithmMock.Object);

            //Act
            var result = packer.FindMinimumBoundingBox(inputSequence);
            var unkResult = packer2.FindMinimumBoundingBox(inputSequence);

            //Assert (Check that for "unlimited" dimensions, it behaves same
            // to unknown size packer)
            var actualW = result.Rects.Max(x => x.Right);
            var actualH = result.Rects.Max(x => x.Bottom);
            Assert.AreEqual(unkResult.Width, actualW);
            Assert.AreEqual(unkResult.Height, actualH);
        }

        [TestMethod]
        public void FixedSizePackerTestSimilarToUnknown5()
        {
            //Arrange 
            var inputSequence = (TestUtil.Shuffle(TestUtil.GetIncreasingSquares(5)));
            var placementAlgorithmMock = new Mock<IPlacementAlgorithm>();

            placementAlgorithmMock
                .Setup(x => x.PlaceRects(It.IsAny<int>(), It.IsAny<int>(), inputSequence, It.IsAny<CancellationToken>()))
                .Returns<int, int, IEnumerable<PPRect>, CancellationToken>((w, h, rects, _) =>
                {
                    //Assume "dumb" placement algorithm that simple places all the rects consecutively next to each other
                    int maxH = rects.Max(x => x.Height);
                    int sumW = rects.Sum(x => x.Width);
                    if (sumW > w || maxH > h)
                        return null;
                    int left = 0;
                    return new PackingResult(sumW, maxH, rects.Select(x =>
                    {
                        left += x.Width;
                        return new PPRect(left, 0, left, x.Height);
                    })); ;
                });
            var packer = new FixedSizePacker(20, 20, placementAlgorithmMock.Object);
            var packer2 = new UnknownSizePacker(placementAlgorithmMock.Object);

            //Act
            var result = packer.FindMinimumBoundingBox(inputSequence);
            var unkResult = packer2.FindMinimumBoundingBox(inputSequence);

            //Assert (Check that for "unlimited" dimensions, it behaves same
            // to unknown size packer)
            var actualW = result.Rects.Max(x => x.Right);
            var actualH = result.Rects.Max(x => x.Bottom);
            Assert.AreEqual(unkResult.Width, actualW);
            Assert.AreEqual(unkResult.Height, actualH);
        }

        [TestMethod]
        public void FixedSizePackerTestSimilarToUnknown10()
        {
            //Arrange 
            var inputSequence = (TestUtil.Shuffle(TestUtil.GetIncreasingSquares(10)));
            var placementAlgorithmMock = new Mock<IPlacementAlgorithm>();

            placementAlgorithmMock
                .Setup(x => x.PlaceRects(It.IsAny<int>(), It.IsAny<int>(), inputSequence, It.IsAny<CancellationToken>()))
                .Returns<int, int, IEnumerable<PPRect>, CancellationToken>((w, h, rects, _) =>
                {
                    //Assume "dumb" placement algorithm that simple places all the rects consecutively next to each other
                    int maxH = rects.Max(x => x.Height);
                    int sumW = rects.Sum(x => x.Width);
                    if (sumW > w || maxH > h)
                        return null;
                    int left = 0;
                    return new PackingResult(sumW, maxH, rects.Select(x =>
                    {
                        left += x.Width;
                        return new PPRect(left, 0, left, x.Height);
                    })); ;
                });
            var packer = new FixedSizePacker(1000, 1000, placementAlgorithmMock.Object);
            var packer2 = new UnknownSizePacker(placementAlgorithmMock.Object);

            //Act
            var result = packer.FindMinimumBoundingBox(inputSequence);
            var unkResult = packer2.FindMinimumBoundingBox(inputSequence);

            //Assert (Check that for "unlimited" dimensions, it behaves same
            // to unknown size packer)
            var actualW = result.Rects.Max(x => x.Right);
            var actualH = result.Rects.Max(x => x.Bottom);
            Assert.AreEqual(unkResult.Width, actualW);
            Assert.AreEqual(unkResult.Height, actualH);
        }

        [TestMethod]
        public void FixedSizePackerTestSimilarToUnknown15()
        {
            //Arrange 
            var inputSequence = (TestUtil.Shuffle(TestUtil.GetIncreasingSquares(15)));
            var placementAlgorithmMock = new Mock<IPlacementAlgorithm>();

            placementAlgorithmMock
                .Setup(x => x.PlaceRects(It.IsAny<int>(), It.IsAny<int>(), inputSequence, It.IsAny<CancellationToken>()))
                .Returns<int, int, IEnumerable<PPRect>, CancellationToken>((w, h, rects, _) =>
                {
                    //Assume "dumb" placement algorithm that simple places all the rects consecutively next to each other
                    int maxH = rects.Max(x => x.Height);
                    int sumW = rects.Sum(x => x.Width);
                    if (sumW > w || maxH > h)
                        return null;
                    int left = 0;
                    return new PackingResult(sumW, maxH, rects.Select(x =>
                    {
                        left += x.Width;
                        return new PPRect(left, 0, left, x.Height);
                    })); ;
                });
            var packer = new FixedSizePacker(1000, 1000, placementAlgorithmMock.Object);
            var packer2 = new UnknownSizePacker(placementAlgorithmMock.Object);

            //Act
            var result = packer.FindMinimumBoundingBox(inputSequence);
            var unkResult = packer2.FindMinimumBoundingBox(inputSequence);

            //Assert (Check that for "unlimited" dimensions, it behaves same
            // to unknown size packer)
            var actualW = result.Rects.Max(x => x.Right);
            var actualH = result.Rects.Max(x => x.Bottom);
            Assert.AreEqual(unkResult.Width, actualW);
            Assert.AreEqual(unkResult.Height, actualH);
        }

        [TestMethod]
        public void FixedSizePackerTestSimilarToUnknown20()
        {
            //Arrange 
            var inputSequence = (TestUtil.Shuffle(TestUtil.GetIncreasingSquares(20)));
            var placementAlgorithmMock = new Mock<IPlacementAlgorithm>();

            placementAlgorithmMock
                .Setup(x => x.PlaceRects(It.IsAny<int>(), It.IsAny<int>(), inputSequence, It.IsAny<CancellationToken>()))
                .Returns<int, int, IEnumerable<PPRect>, CancellationToken>((w, h, rects, _) =>
                {
                    //Assume "dumb" placement algorithm that simple places all the rects consecutively next to each other
                    int maxH = rects.Max(x => x.Height);
                    int sumW = rects.Sum(x => x.Width);
                    if (sumW > w || maxH > h)
                        return null;
                    int left = 0;
                    return new PackingResult(sumW, maxH, rects.Select(x =>
                    {
                        left += x.Width;
                        return new PPRect(left, 0, left, x.Height);
                    })); ;
                });
            var packer = new FixedSizePacker(1000, 1000, placementAlgorithmMock.Object);
            var packer2 = new UnknownSizePacker(placementAlgorithmMock.Object);

            //Act
            var result = packer.FindMinimumBoundingBox(inputSequence);
            var unkResult = packer2.FindMinimumBoundingBox(inputSequence);

            //Assert (Check that for "unlimited" dimensions, it behaves same
            // to unknown size packer)
            var actualW = result.Rects.Max(x => x.Right);
            var actualH = result.Rects.Max(x => x.Bottom);
            Assert.AreEqual(unkResult.Width, actualW);
            Assert.AreEqual(unkResult.Height, actualH);
        }

        [TestMethod]
        public void FixedSizePackerTestSimilarToUnknown50()
        {
            //Arrange 
            var inputSequence = (TestUtil.Shuffle(TestUtil.GetIncreasingSquares(50)));
            var placementAlgorithmMock = new Mock<IPlacementAlgorithm>();

            placementAlgorithmMock
                .Setup(x => x.PlaceRects(It.IsAny<int>(), It.IsAny<int>(), inputSequence, It.IsAny<CancellationToken>()))
                .Returns<int, int, IEnumerable<PPRect>, CancellationToken>((w, h, rects, _) =>
                {
                    //Assume "dumb" placement algorithm that simple places all the rects consecutively next to each other
                    int maxH = rects.Max(x => x.Height);
                    int sumW = rects.Sum(x => x.Width);
                    if (sumW > w || maxH > h)
                        return null;
                    int left = 0;
                    return new PackingResult(sumW, maxH, rects.Select(x =>
                    {
                        left += x.Width;
                        return new PPRect(left, 0, left, x.Height);
                    })); ;
                });
            var packer = new FixedSizePacker(2000, 2000, placementAlgorithmMock.Object);
            var packer2 = new UnknownSizePacker(placementAlgorithmMock.Object);

            //Act
            var result = packer.FindMinimumBoundingBox(inputSequence);
            var unkResult = packer2.FindMinimumBoundingBox(inputSequence);

            //Assert (Check that for "unlimited" dimensions, it behaves same
            // to unknown size packer)
            var actualW = result.Rects.Max(x => x.Right);
            var actualH = result.Rects.Max(x => x.Bottom);
            Assert.AreEqual(unkResult.Width, actualW);
            Assert.AreEqual(unkResult.Height, actualH);
        }
    }
}
