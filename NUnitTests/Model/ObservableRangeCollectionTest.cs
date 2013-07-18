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
    /// Test class for the ObservableRangeCollection class
    /// </summary>
    [TestFixture]
    class ObservableRangeCollectionTest
    {
        ObservableRangeCollection<Frame> obs;
        bool called;

        /// <summary>
        /// Set up instance
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            called = false;
            obs = new ObservableRangeCollection<Frame>();
            this.obs.CollectionChanged += (o, e) => { called = true; };

        }

        /// <summary>
        /// Tear down instance
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            obs = null;
            called = false;
        }

        /// <summary>
        /// Test the no-argument constructor
        /// </summary>
        [Test]
        public void ConstructorNoArgumentTest()
        {
            Assert.NotNull(obs);
            Assert.AreEqual(obs.Count, 0);
        }

        /// <summary>
        /// Test the one-argument constructor
        /// </summary>
        [Test]
        public void ConstructorOneArgumentTest()
        {
            Frame testframe = MockRepository.GenerateMock<Frame>("");
            Frame testframe2 = MockRepository.GenerateMock<Frame>("");
            Frame testframe3 = MockRepository.GenerateMock<Frame>("");
            List<Frame> frameslist = new List<Frame>();
            frameslist.Add(testframe);
            frameslist.Add(testframe2);
            frameslist.Add(testframe3);
           
            ObservableRangeCollection<Frame> obs2 = new ObservableRangeCollection<Frame>(frameslist);

            Assert.NotNull(obs2);
            Assert.AreEqual(obs2.Count, 3);
        }

        /// <summary>
        /// Test AddRange functionality
        /// </summary>
        [Test]
        public void AddRangeTest()
        {
            Frame testframe = MockRepository.GenerateMock<Frame>("");
            Frame testframe2 = MockRepository.GenerateMock<Frame>("");
            Frame testframe3 = MockRepository.GenerateMock<Frame>("");

            List<Frame> frameslist = new List<Frame>();

            frameslist.Add(testframe);
            frameslist.Add(testframe2);
            frameslist.Add(testframe3);

            int count = obs.Count;

            obs.AddRange(frameslist);

            Assert.True(called);
            Assert.AreEqual(count + 3 ,obs.Count);
        }

        /// <summary>
        /// Adding null as a range should throw an exception
        /// </summary>
        [Test]
        [ExpectedException("System.ArgumentNullException")]
        public void AddRangeNullTest()
        {
            obs.AddRange(null);
        }

        /// <summary>
        /// Test RemoveRange functionality
        /// </summary>
        [Test]
        public void RemoveRangeTest()
        {
            Frame testframe = MockRepository.GenerateMock<Frame>("");
            Frame testframe2 = MockRepository.GenerateMock<Frame>("");
            Frame testframe3 = MockRepository.GenerateMock<Frame>("");

            List<Frame> frameslist = new List<Frame>();

            frameslist.Add(testframe);
            frameslist.Add(testframe2);
            frameslist.Add(testframe2);
            frameslist.Add(testframe3);

            int count = obs.Count;

            obs.AddRange(frameslist);
            Assert.AreEqual(count + 4, obs.Count);

            List<Frame> frameslist2 = new List<Frame>();
            frameslist2.Add(testframe);
            frameslist2.Add(testframe2);            

            obs.RemoveRange(frameslist2);
            Assert.AreEqual(2, obs.Count);
            Assert.True(called);
        }

        /// <summary>
        /// Test that Removing a null-range throws an exception
        /// </summary>
        [Test]
        [ExpectedException("System.ArgumentNullException")]
        public void RemoveRangeNullTest()
        {
            obs.RemoveRange(null);
        }

        /// <summary>
        /// Test Replace functionality
        /// </summary>
        [Test]
        public void ReplaceTest()
        {
            Frame testframe = MockRepository.GenerateMock<Frame>("");
            Frame testframe2 = MockRepository.GenerateMock<Frame>("");
            Frame testframe3 = MockRepository.GenerateMock<Frame>("");

            List<Frame> frameslist = new List<Frame>();

            frameslist.Add(testframe);
            frameslist.Add(testframe2);
            frameslist.Add(testframe3);

            obs.AddRange(frameslist);

            int range1 = obs.Count;

            obs.Replace(testframe);

            int range2 = obs.Count;

            Assert.AreNotEqual(range1, range2);
            Assert.AreEqual(range1 - 2, range2);
        }

        /// <summary>
        /// Test that replacing a range with null throws an exception
        /// </summary>
        [Test]
        [ExpectedException("System.ArgumentNullException")]
        public void ReplaceRangeNullTest()
        {
            obs.ReplaceRange(null);
        }

        /// <summary>
        /// Test RemoveRange functionality
        /// </summary>
        [Test]
        public void ReplaceRangeTest()
        {
            bool called = false;
            this.obs.CollectionChanged += (o, e) => { called = true; };

            Frame testframe = new Frame(@"\Model\test.jpg");
            Frame testframe2 = new Frame(@"\Model\test.jpg");
            Frame testframe3 = new Frame(@"\Model\test.jpg");
            List<Frame> frameslist = new List<Frame>();
            frameslist.Add(testframe);
            frameslist.Add(testframe2);
            frameslist.Add(testframe3);
            obs.AddRange(frameslist);

            int range1 = obs.Count;

            List<Frame> frameslist2 = new List<Frame>();
            frameslist2.Add(testframe3);
            frameslist2.Add(testframe2);
            frameslist2.Add(testframe);
            frameslist2.Add(testframe2);

            obs.ReplaceRange(frameslist2);

            int range2 = obs.Count;

            Assert.AreNotEqual(range1, range2);
            Assert.AreEqual(range1 + 1, range2);
            Assert.True(called);
        }
    }
}
