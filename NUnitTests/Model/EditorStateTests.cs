using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiffusionCurves.Diffusion;
using DiffusionCurves.Model;
using NUnit.Framework;
using OpenTK;

namespace NUnitTests.Diffusion
{
    /// <summary>
    /// Test class for the EditorState class
    /// </summary>
    [TestFixture]
    class EditorStateTests
    {
        EditorState es;
        bool called;

        /// <summary>
        /// Set up instance
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            es = new EditorState();
            called = false;
            this.es.PropertyChanged += (o, e) => { called = true; };
        }

        /// <summary>
        /// Tear down instance
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            es = null;
            called = false;
        }

        /// <summary>
        /// EditorState should be initialized in a fully idle state
        /// </summary>        
        [Test]
        public void SetConstructor()
        {
            EditorState state = new EditorState();

            Assert.That(state.CurrentState, Is.EqualTo(EditorStateControl.MouseState.Idle));
            Assert.That(state.ModifierState, Is.EqualTo(EditorStateControl.KeyModifierState.None));

            Assert.IsNull(state.SelectedPath);
            Assert.IsNull(state.SelectedPoint);
        }

        /// <summary>
        /// Get & Set test for SelectedPoint
        /// </summary>
        [Test]
        public void SelectedPointTest()
        {
            BezierPoint point = new BezierPoint(new Vector2());

            es.SelectedPoint = point;

            Assert.AreEqual(point, es.SelectedPoint);
            Assert.True(called);
        }

        /// <summary>
        /// Get & Set test for SelectedPath
        /// </summary>
        [Test]
        public void SelectedPathTest()
        {
            Path path = new Path();

            es.SelectedPath = path;

            Assert.AreEqual(path, es.SelectedPath);
            Assert.True(called);
        }
    }
}
