using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiffusionCurves.Diffusion;
using DiffusionCurves.Model;
using NUnit.Framework;
using Rhino.Mocks;
using OpenTK;

namespace NUnitTests.Diffusion
{
    /// <summary>
    /// Test class for the PathContainer class
    /// </summary>
    [TestFixture]
    class PathContainerTests
    {
        PathContainer container;

        /// <summary>
        /// Set up instance
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            container = new PathContainer();
        }

        /// <summary>
        /// Tear down instance
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            container = null;
        }

        /// <summary>
        /// Test constructor functionality
        /// </summary>
        [Test]
        public void SimpleConstructorTest()
        {
            Assert.NotNull(container);
            Assert.That(this.container.Count, Is.EqualTo(0));
        }

        /// <summary>
        /// Test AddPath functionality
        /// </summary>
        [Test]
        public void AddPathTest()
        {
            Path path = MockRepository.GenerateMock<Path>();
            container.AddPath(path);

            Assert.That(container.Count, Is.EqualTo(1));
            Assert.That(container.GetLastPath(), Is.EqualTo(path));
        }

        /// <summary>
        /// Adding a null path should throw an exeption
        /// </summary>
        [Test]
        [ExpectedException("System.ArgumentNullException")]
        public void AddNullPathTest()
        {
            container.AddPath(null);
        }

        /// <summary>
        /// Test RemovePath functionality
        /// </summary>
        [Test]
        public void RemovePathTest()
        {
            Path path = new Path();
            container.AddPath(path);

            Assert.That(container.Count, Is.EqualTo(1));
            Assert.True(container.RemovePath(path));
            Assert.That(container.Count, Is.EqualTo(0));
        }

        /// <summary>
        /// Removing a null-path returns false
        /// </summary>
        [Test]
        public void RemoveNullPathTest()
        {
            Assert.False(this.container.RemovePath(null));
        }

        /// <summary>
        /// Test Clone functionality
        /// </summary>
        [Test]
        public void CloneTest()
        {
            IPathContainer container1 = new PathContainer();
            Path dummyPath1 = MockRepository.GenerateMock<Path>();            
            Path dummyPath2 = MockRepository.GenerateMock<Path>();
            dummyPath1.Stub(x => x.Equals(Arg<Path>.Is.Anything)).Return(true);
            dummyPath2.Stub(x => x.Equals(Arg<Path>.Is.Anything)).Return(true);
            container1.AddPath(dummyPath1);
            container1.AddPath(dummyPath2);

            IPathContainer container2 = container1.Clone();

            Assert.True(container1.Equals(container2));
            Assert.False(container1 == container2);
        }

        /// <summary>
        /// Test Clear functionality
        /// </summary>
        [Test]
        public void ClearTest()
        {
            PathContainer container = new PathContainer();
            Path dummyPath1 = MockRepository.GenerateMock<Path>();
            Path dummyPath2 = MockRepository.GenerateMock<Path>();
            container.AddPath(dummyPath1);
            container.AddPath(dummyPath2);

            container.Clear();

            Assert.AreEqual(0, container.Count);

            Assert.AreEqual(0, container.TotalPointCount);
        }

        /// <summary>
        /// Test Equals functionality
        /// </summary>
        [Test]
        public void EqualsTest() 
        {
            PathContainer container2 = new PathContainer();

            Assert.True(container.Equals(container));
            Assert.True(container.Equals(container2));

            Path dummyPath1 = MockRepository.GenerateMock<Path>();
            container2.AddPath(dummyPath1);

            Assert.False(container.Equals(container2));
            Assert.True(container2.Equals(container2));
        }

        /// <summary>
        /// Test TotalPoints functionality
        /// </summary>
        [Test]
        public void TotalPointsTest()
        {
            /*BezierPoint point = new BezierPoint(new Vector2());
            BezierPoint point2 = new BezierPoint(new Vector2());
            BezierPoint point3 = new BezierPoint(new Vector2());*/

            Path path = new Path();
            path.AddPointFirst(new Vector2());
            path.AddPointFirst(new Vector2());
            path.AddPointFirst(new Vector2());

            Path path2 = new Path();
            path.AddPointLast(new Vector2());
            path.AddPointLast(new Vector2());

            container.AddPath(path);
            container.AddPath(path2);

            Assert.AreEqual(5, container.TotalPointCount);
            Assert.AreEqual(2, container.Count);
        }

        [Test]
        public void EqualsFalseTest()
        {
            PathContainer container = new PathContainer();

            Assert.False(container.Equals(148));
            Assert.False(container.Equals("diffusion"));
            Assert.False(container.Equals(null));
        }

        [Test]
        public void SetGetPaths()
        {
            PathContainer container = new PathContainer();
            List<Path> paths = new List<Path>();
            Path path1 = new Path();
            Path path2 = new Path();
            Path path3 = new Path();
            paths.Add(path1);
            paths.Add(path2);
            paths.Add(path3);

            container.Paths = paths;

            Assert.AreEqual(container.Paths, paths);
        }
    }
}
