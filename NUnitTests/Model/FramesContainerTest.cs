using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using DiffusionCurves.Diffusion;
using OpenTK;
using DiffusionCurves.Model;
using Rhino.Mocks;

namespace NUnitTests.Model
{
    /// <summary>
    /// Test class for the FramesContainer class
    /// </summary>
    [TestFixture]
    class FramesContainerTest
    {
        FramesContainer testFramesContainer;
        bool called;

        /// <summary>
        /// Set up instance
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            called = false;
            testFramesContainer = new FramesContainer();
            this.testFramesContainer.CollectionChanged += (o, e) => { called = true; };
        }

        /// <summary>
        /// Tear down instance
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            testFramesContainer = null;
            called = false;
        }


        [Test]
        public void TestConstructor()
        {
            Assert.NotNull(testFramesContainer);
        }

        /// <summary>
        /// Test that addFrame correctly adds frames
        /// </summary>
        [Test]
        public void TestAddFrame()
        {
            Frame testframe = MockRepository.GenerateMock<Frame>("");
            Frame testframe2 = MockRepository.GenerateMock<Frame>("");
            Frame testframe3 = MockRepository.GenerateMock<Frame>("");
            int countFrames = testFramesContainer.Count;

            testFramesContainer.AddFrame(testframe);
            Assert.AreEqual(countFrames + 1, testFramesContainer.Count);

            testFramesContainer.AddFrame(testframe2);
            Assert.AreEqual(countFrames + 2, testFramesContainer.Count);

            testFramesContainer.AddFrame(testframe3);
            Assert.AreEqual(countFrames + 3, testFramesContainer.Count);

            Assert.True(called);
        }

        /// <summary>
        /// Test that setting a cursor index greater than count throws an exception
        /// </summary>
        [Test]
        [ExpectedException("System.IndexOutOfRangeException")]
        public void TestCursorIndexExceptionPositive()
        {
            testFramesContainer.CursorIndex = 10;
        }

        /// <summary>
        /// Test that setting a negative cursor index throws an exception
        /// </summary>
        [Test]
        [ExpectedException("System.IndexOutOfRangeException")]
        public void TestCursorIndexExceptionNegative()
        {
            testFramesContainer.CursorIndex = -10;
        }

        /// <summary>
        /// Testsetting a valid cursorindex
        /// </summary>
        [Test]
        public void TestCursorIndex()
        {
            Frame testframe = MockRepository.GenerateMock<Frame>("");
            Frame testframe2 = MockRepository.GenerateMock<Frame>("");
            Frame testframe3 = MockRepository.GenerateMock<Frame>("");
            testFramesContainer.AddFrame(testframe);
            testFramesContainer.AddFrame(testframe2);
            testFramesContainer.AddFrame(testframe3);

            testFramesContainer.CursorIndex = 2;

            Assert.AreEqual(2, testFramesContainer.CursorIndex);
            Assert.True(called);
        }

        /// <summary>
        /// Test that addRange correctly adds a collection of frames
        /// </summary>
        [Test]
        public void AddRangeTest()
        {
            Frame testframe = MockRepository.GenerateMock<Frame>("");
            Frame testframe2 = MockRepository.GenerateMock<Frame>("");
            Frame testframe3 = MockRepository.GenerateMock<Frame>("");
            List<DiffusionCurves.Model.Frame> frameCollection = new List<DiffusionCurves.Model.Frame>();
            frameCollection.Add(testframe);
            frameCollection.Add(testframe2);
            frameCollection.Add(testframe3);

            int count = testFramesContainer.Count;

            testFramesContainer.AddRange(frameCollection);
            Assert.AreEqual(count + 3, testFramesContainer.Count);
        }

        /// <summary>
        /// Check get for frameslist
        /// </summary>
        [Test]
        public void GetFramesListTest()
        {
            Frame f1 = MockRepository.GenerateMock<Frame>("");
            Frame f2 = MockRepository.GenerateMock<Frame>("");
            Frame f3 = MockRepository.GenerateMock<Frame>("");

            testFramesContainer.AddFrame(f1);
            testFramesContainer.AddFrame(f2);
            testFramesContainer.AddFrame(f3);

            ObservableRangeCollection<Frame> fl = new ObservableRangeCollection<Frame>();
            fl.Add(f1);
            fl.Add(f2);
            fl.Add(f3);

            Assert.AreEqual(testFramesContainer.FramesList.ElementAt(0), fl.ElementAt(0));
            Assert.AreEqual(testFramesContainer.FramesList.ElementAt(1), fl.ElementAt(1));
            Assert.AreEqual(testFramesContainer.FramesList.ElementAt(2), fl.ElementAt(2));

        }
    }
}
