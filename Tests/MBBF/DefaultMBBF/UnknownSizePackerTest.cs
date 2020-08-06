using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaunPacker.Core.Packing;
using PaunPacker.Core.Packing.Placement;
using PaunPacker.Core.Types;
using PaunPacker.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using System.Threading.Tasks;
using System.Threading;
using PaunPacker.Core.Packing.MBBF;

namespace PaunPacker.Tests.MBBF
{   

    [TestClass]
    /// <summary>
    /// This class contains tests for <see cref="UnknownSizePacker"/>
    /// Because <see cref="UnknownSizePacker"/>'s public interface has only a single method
    /// The one for packing and it's result highly depend on the used placement algorithm
    /// Only validity of the packing results is checked
    /// </summary>
    public class UnknownSizePackerTest
    {
        [TestMethod]
        public void UnknownSizePackerTestValidSquares1()
        {
            //Arrange
            var inputSequence = (TestUtil.Shuffle(TestUtil.GetIncreasingSquares(1)));
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
            var packer = new UnknownSizePacker(placementAlgorithmMock.Object);
            
            //Act
            var result = packer.FindMinimumBoundingBox(inputSequence);

            //Assert
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(true, TestUtil.IsPackingResultValid(result));
        }

        [TestMethod]
        public void UnknownSizePackerTestValidSquares2()
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
            var packer = new UnknownSizePacker(placementAlgorithmMock.Object);

            //Act
            var result = packer.FindMinimumBoundingBox(inputSequence);

            //Assert
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(true, TestUtil.IsPackingResultValid(result));
        }

        [TestMethod]
        public void UnknownSizePackerTestValidSquares3()
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
            var packer = new UnknownSizePacker(placementAlgorithmMock.Object);

            //Act
            var result = packer.FindMinimumBoundingBox(inputSequence);

            //Assert
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(true, TestUtil.IsPackingResultValid(result));
        }

        [TestMethod]
        public void UnknownSizePackerTestValidSquares4()
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
            var packer = new UnknownSizePacker(placementAlgorithmMock.Object);

            //Act
            var result = packer.FindMinimumBoundingBox(inputSequence);

            //Assert
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(true, TestUtil.IsPackingResultValid(result));
        }

        [TestMethod]
        public void UnknownSizePackerTestValidSquares5()
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
            var packer = new UnknownSizePacker(placementAlgorithmMock.Object);

            //Act
            var result = packer.FindMinimumBoundingBox(inputSequence);

            //Assert
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(true, TestUtil.IsPackingResultValid(result));
        }

        [TestMethod]
        public void UnknownSizePackerTestValidSquares10()
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
            var packer = new UnknownSizePacker(placementAlgorithmMock.Object);

            //Act
            var result = packer.FindMinimumBoundingBox(inputSequence);

            //Assert
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(true, TestUtil.IsPackingResultValid(result));
        }

        [TestMethod]
        public void UnknownSizePackerTestValidSquares15()
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
            var packer = new UnknownSizePacker(placementAlgorithmMock.Object);

            //Act
            var result = packer.FindMinimumBoundingBox(inputSequence);

            //Assert
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(true, TestUtil.IsPackingResultValid(result));
        }

        [TestMethod]
        public void UnknownSizePackerTestValidSquares20()
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
            var packer = new UnknownSizePacker(placementAlgorithmMock.Object);

            //Act
            var result = packer.FindMinimumBoundingBox(inputSequence);

            //Assert
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(true, TestUtil.IsPackingResultValid(result));
        }

        [TestMethod]
        public void UnknownSizePackerTestValidSquares50()
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
            var packer = new UnknownSizePacker(placementAlgorithmMock.Object);

            //Act
            var result = packer.FindMinimumBoundingBox(inputSequence);

            //Assert
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(true, TestUtil.IsPackingResultValid(result));
        }

        [TestMethod]
        //Assert
        [ExpectedException(typeof(ArgumentNullException))]
        public void UnknownSizePackerTestArgumentNull()
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
            var packer = new UnknownSizePacker(placementAlgorithmMock.Object);

            //Act
            var result = packer.FindMinimumBoundingBox(inputSequence);

            //Assert
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(true, TestUtil.IsPackingResultValid(result));
        }

        [TestMethod]
        public void UnknownSizePackerTestEmptyInput()
        {
            //Arrange 
            var inputSequence = Enumerable.Empty<PPRect>();
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
            var packer = new UnknownSizePacker(placementAlgorithmMock.Object);

            //Act
            var result = packer.FindMinimumBoundingBox(inputSequence);

            //Assert
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(0, result.Width);
            Assert.AreEqual(0, result.Height);
            Assert.AreEqual(0, result.Rects.Count());
        }

        [TestMethod]
        public async Task UnknownSizePackerTestCancellationWorks()
        {
            //Arrange 
            var inputSequence = (TestUtil.Shuffle(TestUtil.GetIncreasingSquares(50)));
            var placementAlgorithmMock = new Mock<IPlacementAlgorithm>();

            int calledTimes = 0;

            placementAlgorithmMock
                .Setup(x => x.PlaceRects(It.IsAny<int>(), It.IsAny<int>(), inputSequence, It.IsAny<CancellationToken>()))
                .Returns<int, int, IEnumerable<PPRect>, CancellationToken>((w, h, rects, _) =>
                {
                    calledTimes++;
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
            var packer = new UnknownSizePacker(placementAlgorithmMock.Object);

            //Act
            var result = packer.FindMinimumBoundingBox(inputSequence);

            //Assert
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(true, TestUtil.IsPackingResultValid(result));
            Assert.IsTrue(calledTimes > 2);



            //Now we know that it was called atleast three times, check that the use of cancellation reduce
            //it to two calls (it is called two times before the "main-loop" starts and the cancellation is
            //being checked in the main loop)
            calledTimes = 0;
            placementAlgorithmMock = new Mock<IPlacementAlgorithm>();

            placementAlgorithmMock
            .Setup(x => x.PlaceRects(It.IsAny<int>(), It.IsAny<int>(), inputSequence, It.IsAny<CancellationToken>()))
            .Returns<int, int, IEnumerable<PPRect>, CancellationToken>((w, h, rects, _) =>
            {
                //Ensure that it is not too fast ...
                System.Threading.Thread.Sleep(1000);
                calledTimes++;
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
            packer = new UnknownSizePacker(placementAlgorithmMock.Object);

            using (var source = new CancellationTokenSource())
            {
                var cancellationToken = source.Token;

                var task = Task.Run(() => packer.FindMinimumBoundingBox(inputSequence, cancellationToken));
                source.Cancel();

                result = await task.ConfigureAwait(false);

                //Check that even though it was canceled, some meaningful result was returned
                //This is property of UnknownSizePacker, not general property of all minimum bounding box finders
                Assert.AreNotEqual(null, result);
                //Check that the returned result is valid
                Assert.AreEqual(true, TestUtil.IsPackingResultValid(result));
            }
            
            //Check that it was called exactly two times
            //The cancellation check occurs after initial two calls within a loop
            Assert.IsTrue(calledTimes == 2);
        }

        [TestMethod]
        public async Task UnknownSizePackerTestRunCancelled()
        {
            //Now we know that it was called atleast three times (from the previous test case), check that the use of cancellation reduce
            //it to two calls (it is called two times before the "main-loop" starts and the cancellation is
            //being checked in the main loop)
            var calledTimes = 0;
            var inputSequence = (TestUtil.Shuffle(TestUtil.GetIncreasingSquares(50)));
            var placementAlgorithmMock = new Mock<IPlacementAlgorithm>();

            placementAlgorithmMock
            .Setup(x => x.PlaceRects(It.IsAny<int>(), It.IsAny<int>(), inputSequence, It.IsAny<CancellationToken>()))
            .Returns<int, int, IEnumerable<PPRect>, CancellationToken>((w, h, rects, _) =>
            {
                //Ensure that it is not too fast ...
                Thread.Sleep(1000);
                calledTimes++;
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

            var packer = new UnknownSizePacker(placementAlgorithmMock.Object);

            using var source = new CancellationTokenSource();
            var cancellationToken = source.Token;

            var task = Task.Run(() => packer.FindMinimumBoundingBox(inputSequence, cancellationToken));
            source.Cancel();

            var result = await task.ConfigureAwait(false);

            //Check that even though it was canceled, some meaningful result was returned
            //This is property of UnknownSizePacker, not general property of all minimum bounding box finders
            Assert.AreNotEqual(null, result);
            //Check that the returned result is valid
            Assert.AreEqual(true, TestUtil.IsPackingResultValid(result));

            //Check that it was called exactly two times
            //The cancellation check occurs after initial two calls within a loop
            Assert.IsTrue(calledTimes == 2);

            calledTimes = 0;
            var result2 = packer.FindMinimumBoundingBox(inputSequence, cancellationToken);
            //Check that even though it was canceled, some meaningful result was returned
            //This is property of UnknownSizePacker, not general property of all minimum bounding box finders
            Assert.AreNotEqual(null, result2);
            //Check that the returned result is valid
            Assert.AreEqual(true, TestUtil.IsPackingResultValid(result2));
            //Check that it was called exactly two times
            //The cancellation check occurs after initial two calls within a loop
            Assert.IsTrue(calledTimes == 2);
        }

        [TestMethod]
        public async Task UnknownSizePackerTestRunAfterCancellation()
        {
            //Now we know that it was called atleast three times (from the previous test case), check that the use of cancellation reduce
            //it to two calls (it is called two times before the "main-loop" starts and the cancellation is
            //being checked in the main loop)
            var calledTimes = 0;
            var inputSequence = (TestUtil.Shuffle(TestUtil.GetIncreasingSquares(50)));
            var placementAlgorithmMock = new Mock<IPlacementAlgorithm>();

            placementAlgorithmMock
            .Setup(x => x.PlaceRects(It.IsAny<int>(), It.IsAny<int>(), inputSequence, It.IsAny<CancellationToken>()))
            .Returns<int, int, IEnumerable<PPRect>, CancellationToken>((w, h, rects, _) =>
            {
                calledTimes++;
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

            var packer = new UnknownSizePacker(placementAlgorithmMock.Object);

            using (var source = new CancellationTokenSource())
            {
                var cancellationToken = source.Token;

                var task = Task.Run(() => packer.FindMinimumBoundingBox(inputSequence, cancellationToken));
                source.Cancel();

                var result = await task.ConfigureAwait(false);
            }


            //Obtain new cancellation token
            using (var source = new CancellationTokenSource())
            {
                var cancellationToken = source.Token;
                calledTimes = 0;
                var result2 = packer.FindMinimumBoundingBox(inputSequence, cancellationToken);
                Assert.AreNotEqual(null, result2);
                Assert.AreEqual(true, TestUtil.IsPackingResultValid(result2));
            }

            //Check that it was not cancelled
            Assert.IsTrue(calledTimes > 2);
        }

        [TestMethod]
        public void UnknownSizePackerTestInstanceReusability()
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
            var packer = new UnknownSizePacker(placementAlgorithmMock.Object);

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
    }
}
