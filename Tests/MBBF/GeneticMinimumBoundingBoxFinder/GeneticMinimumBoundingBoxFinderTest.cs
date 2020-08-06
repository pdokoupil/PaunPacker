using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaunPacker.Core.Types;
using PaunPacker.Plugins;

namespace PaunPacker.Tests.MBBF
{
    /// <summary>
    /// This class contains unit tests for <see cref="GeneticMinimumBoundingBoxFinder"/>
    /// </summary>
    [TestClass]
    public class GeneticMinimumBoundingBoxFinderTest
    {
        [TestMethod]
        public void GeneticMinimumBoundingBoxFinderTestZeroIterations()
        {
            //Arrange
            var inputSequence = (TestUtil.Shuffle(TestUtil.GetIncreasingSquares(1)));
            var packer = new GeneticMinimumBoundingBoxFinder(0, 2);

            //Act
            var result = packer.FindMinimumBoundingBox(inputSequence);

            //Assert
            Assert.AreEqual(true, TestUtil.IsPackingResultValid(result));
        }

        [TestMethod]
        //Assert
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GeneticMinimumBoundingBoxFinderTestZeroPopulation()
        {
            //Arrange
            var inputSequence = (TestUtil.Shuffle(TestUtil.GetIncreasingSquares(1)));
            var packer = new GeneticMinimumBoundingBoxFinder(10, 0);

            //Act
            packer.FindMinimumBoundingBox(inputSequence);
        }

        [TestMethod]
        //Assert
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GeneticMinimumBoundingBoxFinderTestNegativeIterations()
        {
            //Arrange
            var inputSequence = (TestUtil.Shuffle(TestUtil.GetIncreasingSquares(1)));
            var packer = new GeneticMinimumBoundingBoxFinder(-10, 100);

            //Act
            var result = packer.FindMinimumBoundingBox(inputSequence);

            //Assert
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void GeneticMinimumBoundingBoxFinderTestInstanceReusability()
        {
            //Arrange
            var inputSequence = (TestUtil.Shuffle(TestUtil.GetIncreasingSquares(1)));
            var packer = new GeneticMinimumBoundingBoxFinder(10, 10);

            //Results might be different because of the random factor
            //Act
            packer.FindMinimumBoundingBox(inputSequence);
            packer.FindMinimumBoundingBox(inputSequence);
            packer.FindMinimumBoundingBox(inputSequence);

            //Assert -> no exception
        }

        [TestMethod]
        //Assert
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GeneticMinimumBoundingBoxFinderTestNegativePopulation()
        {
            //Arrange
            var inputSequence = (TestUtil.Shuffle(TestUtil.GetIncreasingSquares(1)));
            var packer = new GeneticMinimumBoundingBoxFinder(100, -10);

            //Act
            packer.FindMinimumBoundingBox(inputSequence);
        }

        [TestMethod]
        //Assert
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GeneticMinimumBoundingBoxFinderTestLessThanTwoPopulation()
        {
            //Arrange
            var inputSequence = (TestUtil.Shuffle(TestUtil.GetIncreasingSquares(1)));
            var packer = new GeneticMinimumBoundingBoxFinder(100, 1);

            //Act
            packer.FindMinimumBoundingBox(inputSequence);
        }

        [TestMethod]
        public void GeneticMinimumBoundingBoxFinderTestValidSquares1()
        {
            //Arrange
            var inputSequence = (TestUtil.Shuffle(TestUtil.GetIncreasingSquares(1)));
            var packer = new GeneticMinimumBoundingBoxFinder(1, 2);

            //Act
            var result = packer.FindMinimumBoundingBox(inputSequence);

            //Assert
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(true, TestUtil.IsPackingResultValid(result));
        }

        [TestMethod]
        public void GeneticMinimumBoundingBoxFinderTestValidSquares2()
        {
            //Arrange
            var inputSequence = (TestUtil.Shuffle(TestUtil.GetIncreasingSquares(2)));
            var packer = new GeneticMinimumBoundingBoxFinder(100, 100);

            //Act
            var result = packer.FindMinimumBoundingBox(inputSequence);

            //Assert
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(true, TestUtil.IsPackingResultValid(result));
        }

        [TestMethod]
        public void GeneticMinimumBoundingBoxFinderTestValidSquares3()
        {
            //Arrange 
            var inputSequence = (TestUtil.Shuffle(TestUtil.GetIncreasingSquares(3)));
            var packer = new GeneticMinimumBoundingBoxFinder(100, 100);

            //Act
            var result = packer.FindMinimumBoundingBox(inputSequence);

            //Assert
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(true, TestUtil.IsPackingResultValid(result));
        }

        [TestMethod]
        public void GeneticMinimumBoundingBoxFinderTestValidSquares4()
        {
            //Arrange 
            var inputSequence = (TestUtil.Shuffle(TestUtil.GetIncreasingSquares(4)));
            var packer = new GeneticMinimumBoundingBoxFinder(100, 100);

            //Act
            var result = packer.FindMinimumBoundingBox(inputSequence);

            //Assert
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(true, TestUtil.IsPackingResultValid(result));
        }

        [TestMethod]
        public void GeneticMinimumBoundingBoxFinderTestValidSquares5()
        {
            //Arrange 
            var inputSequence = (TestUtil.Shuffle(TestUtil.GetIncreasingSquares(5)));
            var packer = new GeneticMinimumBoundingBoxFinder(100, 100);

            //Act
            var result = packer.FindMinimumBoundingBox(inputSequence);

            //Assert
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(true, TestUtil.IsPackingResultValid(result));
        }

        [TestMethod]
        public void GeneticMinimumBoundingBoxFinderTestValidSquares10()
        {
            //Arrange 
            var inputSequence = (TestUtil.Shuffle(TestUtil.GetIncreasingSquares(10)));
            var packer = new GeneticMinimumBoundingBoxFinder(100, 100);

            //Act
            var result = packer.FindMinimumBoundingBox(inputSequence);

            //Assert
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(true, TestUtil.IsPackingResultValid(result));
        }

        [TestMethod]
        public void GeneticMinimumBoundingBoxFinderTestValidSquares15()
        {
            //Arrange 
            var inputSequence = (TestUtil.Shuffle(TestUtil.GetIncreasingSquares(15)));
            var packer = new GeneticMinimumBoundingBoxFinder(100, 100);

            //Act
            var result = packer.FindMinimumBoundingBox(inputSequence);

            //Assert
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(true, TestUtil.IsPackingResultValid(result));
        }

        [TestMethod]
        public void GeneticMinimumBoundingBoxFinderTestValidSquares20()
        {
            //Arrange 
            var inputSequence = (TestUtil.Shuffle(TestUtil.GetIncreasingSquares(20)));
            var packer = new GeneticMinimumBoundingBoxFinder(100, 100);

            //Act
            var result = packer.FindMinimumBoundingBox(inputSequence);

            //Assert
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(true, TestUtil.IsPackingResultValid(result));
        }

        [TestMethod]
        public void GeneticMinimumBoundingBoxFinderTestValidSquares50()
        {
            //Arrange 
            var inputSequence = (TestUtil.Shuffle(TestUtil.GetIncreasingSquares(50)));
            var packer = new GeneticMinimumBoundingBoxFinder(100, 100);

            //Act
            var result = packer.FindMinimumBoundingBox(inputSequence);

            //Assert
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(true, TestUtil.IsPackingResultValid(result));
        }

        [TestMethod]
        //Assert
        [ExpectedException(typeof(ArgumentNullException))]
        public void GeneticMinimumBoundingBoxFinderTestArgumentNull()
        {
            //Arrange 
            IEnumerable<PPRect> inputSequence = null;
            var packer = new GeneticMinimumBoundingBoxFinder(100, 100);
            
            //Act
            var result = packer.FindMinimumBoundingBox(inputSequence);

            //Assert
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(true, TestUtil.IsPackingResultValid(result));
        }

        [TestMethod]
        public void GeneticMinimumBoundingBoxFinderTestEmptyInput()
        {
            //Arrange 
            var inputSequence = Enumerable.Empty<PPRect>();
            var packer = new GeneticMinimumBoundingBoxFinder(100, 100);

            //Act
            var result = packer.FindMinimumBoundingBox(inputSequence);

            //Assert
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(0, result.Width);
            Assert.AreEqual(0, result.Height);
            Assert.AreEqual(0, result.Rects.Count());
        }

        [TestMethod]
        public async Task GeneticMinimumBoundingBoxFinderTestCancellationWorks()
        {
            //Arrange 
            var inputSequence = (TestUtil.Shuffle(TestUtil.GetIncreasingSquares(50)));
            var packer = new GeneticMinimumBoundingBoxFinder(100000, 10);
            var source = new CancellationTokenSource();
            var token = source.Token;

            //Act
            var resultTask = Task.Run(() => packer.FindMinimumBoundingBox(inputSequence, token));
            //Wait some time so that some iteration has completed
            await Task.Delay(10000).ConfigureAwait(false);
            source.Cancel();
            var result = await resultTask.ConfigureAwait(false);

            source.Dispose();

            //Assert
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(true, TestUtil.IsPackingResultValid(result));
            Assert.IsTrue(packer.Progress > 0 && packer.Progress < 100);
        }

        [TestMethod]
        public async Task GeneticMinimumBoundingBoxFinderTestRunCancelled()
        {
            //Arrange 
            var inputSequence = (TestUtil.Shuffle(TestUtil.GetIncreasingSquares(50)));
            var packer = new GeneticMinimumBoundingBoxFinder(100000, 100);
            var source = new CancellationTokenSource();
            var token = source.Token;

            //Act
            var resultTask = Task.Run(() => packer.FindMinimumBoundingBox(inputSequence, token));
            //Wait some time so that some iteration has completed
            await Task.Delay(3000).ConfigureAwait(false);
            source.Cancel();
            var result = await resultTask.ConfigureAwait(false);
            packer.FindMinimumBoundingBox(inputSequence, token);

            source.Dispose();

            //Assert
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(true, TestUtil.IsPackingResultValid(result));
            Assert.AreEqual(0, packer.Progress);

        }

        [TestMethod]
        public async Task GeneticMinimumBoundingBoxFinderTestRunAfterCancellation()
        {
            //Arrange 
            var inputSequence = (TestUtil.Shuffle(TestUtil.GetIncreasingSquares(50)));
            var packer = new GeneticMinimumBoundingBoxFinder(1000, 100);
            var source = new CancellationTokenSource();
            var token = source.Token;

            //Act
            var resultTask = Task.Run(() => packer.FindMinimumBoundingBox(inputSequence, token));
            //Wait some time so that some iteration has completed
            await Task.Delay(3000).ConfigureAwait(false);
            source.Cancel();
            source.Dispose();
            var result = await resultTask.ConfigureAwait(false);
            source = new CancellationTokenSource();
            token = source.Token;
            result = packer.FindMinimumBoundingBox(inputSequence, token);

            source.Dispose();

            //Assert
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(true, TestUtil.IsPackingResultValid(result));
            Assert.IsTrue(packer.Progress > 0);
        }

    }
}
