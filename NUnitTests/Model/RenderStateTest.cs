using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using DiffusionCurves.Model;
using OpenTK;

namespace NUnitTests.Model
{
    /// <summary>
    /// Test class for the RenderState class
    /// </summary>
    [TestFixture]
    class RenderStateTest
    {
        RenderState render;
        Matrix4 m1;
        Matrix4 m2;
        Vector2 v;
        bool called;

        /// <summary>
        /// Set up instance
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            m1 = new Matrix4(134, 245, 4576, 587, 134, 346, 78, 123, 346, 468, 89, 857, 543, 43, 65, 432);
            m2 = new Matrix4(12,13,24,456,67,678,43,46,234,687,345,314,87,87,214,346);

            v = new Vector2(23435, 34652);

            called = false;
            render = new RenderState(m1, m2, 48, v);
            this.render.PropertyChanged += (o, e) => { called = true; };
        }

        /// <summary>
        /// Tear down instance
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            render = null;
            m1 = new Matrix4();
            m2 = new Matrix4();
            v = new Vector2();
            called = false;
        }

        /// <summary>
        /// Test correct Constructor use, as well as correct setters and getters
        /// </summary>
        [Test]
        public void ConstructorGetSetTestCorrect()
        {
            Assert.AreEqual(render.ProjectionMatrix, m1);
            Assert.AreEqual(render.ViewMatrix, m2);
            Assert.AreEqual(render.ZoomLevel, 48);
            Assert.AreEqual(render.Offset, v);
        }

        /// <summary>
        /// Test correct Constructor use, as well as incorrect setters and getters use
        /// </summary>
        [Test]
        public void ConstructorGetSetTestInCorrect()
        {
            v = new Vector2(48, 48);

            Assert.AreNotEqual(render.ProjectionMatrix, m2);
            Assert.AreNotEqual(render.ViewMatrix, m1);
            Assert.AreNotEqual(render.ZoomLevel, v);
            Assert.AreNotEqual(render.Offset, 48);
        }

        /// <summary>
        /// Test framesize functionality
        /// </summary>
        [Test]
        public void SetFrameSizeTest()
        {
            render.FrameSize = new System.Drawing.Size(48,48);

            Assert.AreEqual(render.FrameSize, new System.Drawing.Size(48, 48));
            Assert.True(called);
        }

        /// <summary>
        /// Test ViewMatrix set & get
        /// </summary>
        [Test]
        public void SetViewMatrixTest()
        {
            render.ViewMatrix = m1;

            Assert.AreEqual(render.ViewMatrix, m1);
            Assert.True(called);
        }
    }
}
