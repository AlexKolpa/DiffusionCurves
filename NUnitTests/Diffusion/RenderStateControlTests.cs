using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiffusionCurves.Diffusion;
using DiffusionCurves.Model;
using NUnit.Framework;
using OpenTK;
using Rhino.Mocks;

namespace NUnitTests.Diffusion
{
    /// <summary>
    /// Test class for the RenderStateControl class
    /// </summary>
    [TestFixture]
    class RenderStateControlTests
    {
        RenderStateControl renderStateControl;
        RenderState renderState;

        /// <summary>
        /// Set up instance
        /// </summary>
        [SetUp]
        public void RenderStateControlSetUp()
        {
            renderState = MockRepository.GenerateMock<RenderState>();
            renderStateControl = new RenderStateControl(renderState);

            renderState.Stub(x => x.Offset).PropertyBehavior();
            renderState.Stub(x => x.FrameSize).PropertyBehavior();
            renderState.Stub(x => x.ZoomLevel).PropertyBehavior();
            renderState.Stub(x => x.ViewSize).PropertyBehavior();
            renderState.Stub(x => x.ProjectionMatrix).PropertyBehavior();
            renderState.Stub(x => x.ZoomSteps).PropertyBehavior();            
        }

        /// <summary>
        /// Test setter & getter for viewport
        /// </summary>
        [Test]
        public void TestSetViewPort()
        {
            renderState.FrameSize = new Size(300, 400);
            renderState.ZoomLevel = 1f;
            renderState.ZoomSteps = 10;
            renderState.Offset = new Vector2();
            Size viewportSize = new Size(400, 500);

            renderStateControl.SetViewport(viewportSize);
            Assert.AreEqual(renderState.ViewSize, viewportSize);            
            Assert.AreEqual(renderState.ZoomLevel, 1f);
        }
        
        /// <summary>
        /// Pan should be able to move around inside the boundaries at any zoomlevel
        /// </summary>
        [Test]
        public void TestPanInsideBoundaries1()
        {
            Vector2 offset = new OpenTK.Vector2();
            Size frameSize = new Size(500, 500);
            float zoomLevel = 1f;
            Size viewSize = new Size(700, 700);
            renderState.Offset = offset;
            renderState.FrameSize = frameSize;
            renderState.ZoomLevel = zoomLevel;
            renderState.ViewSize = viewSize;
            renderState.ZoomSteps = 10;

            Vector2 delta = new Vector2(50, 50);

            renderStateControl.Pan(delta);

            Assert.That(renderState.Offset, Is.EqualTo(delta));
        }

        /// <summary>
        /// Should still move around when pan moves directly against the edge
        /// </summary>
        [Test]
        public void TestPanInsideBoundaries2()
        {
            Vector2 offset = new OpenTK.Vector2(200, 200);
            Size frameSize = new Size(500, 500);
            float zoomLevel = 1f;
            Size viewSize = new Size(700, 700);
            renderState.Offset = offset;
            renderState.FrameSize = frameSize;
            renderState.ZoomLevel = zoomLevel;
            renderState.ViewSize = viewSize;
            renderState.ZoomSteps = 10;
            
            Vector2 boundary = new Vector2(RenderStateControl.BoundarySize, RenderStateControl.BoundarySize);
            Vector2 delta = new Vector2(50 , 50) + boundary;

            renderStateControl.Pan(delta);
                        
            Assert.AreEqual(renderState.Offset, offset + delta);
        }

        /// <summary>
        /// Should still move around when pan moves directly against the edge
        /// </summary>
        [Test]
        public void TestPanOutSideBoundaries()
        {
            Vector2 offset = new OpenTK.Vector2(250, 250);
            Size frameSize = new Size(500, 500);
            float zoomLevel = 1f;
            Size viewSize = new Size(700, 700);            
            renderState.Offset = offset;
            renderState.FrameSize = frameSize;
            renderState.ZoomLevel = zoomLevel;
            renderState.ViewSize = viewSize;
            renderState.ZoomSteps = 10;

            Vector2 boundary = new Vector2(RenderStateControl.BoundarySize, RenderStateControl.BoundarySize);
            Vector2 delta = new Vector2(50, 50) + boundary;

            renderStateControl.Pan(delta);

            Assert.AreEqual(renderState.Offset, offset + boundary);
        }

        /// <summary>
        /// Zoom shouldn't affect panning when inside boundaries
        /// </summary>
        [Test]
        public void TestPanWithZoomInsideBoundaries1()
        {
            Vector2 offset = new OpenTK.Vector2();
            Size frameSize = new Size(500, 500);
            float zoomLevel = 2f;
            Size viewSize = new Size(700, 700);
            renderState.Offset = offset;
            renderState.FrameSize = frameSize;
            renderState.ZoomLevel = zoomLevel;
            renderState.ViewSize = viewSize;
            renderState.ZoomSteps = 10;

            Vector2 delta = new Vector2(50, 50);

            renderStateControl.Pan(delta);

            Assert.AreEqual(renderState.Offset, delta);
        }

        /// <summary>
        /// Zoom should allow for more boundary movement
        /// </summary>
        [Test]
        public void TestPanWithZoomInsideBoundaries2()
        {
            Vector2 offset = new OpenTK.Vector2(200, 200);
            Size frameSize = new Size(500, 500);
            float zoomLevel = 2f;
            Size viewSize = new Size(700, 700);
            renderState.Offset = offset;
            renderState.FrameSize = frameSize;
            renderState.ZoomLevel = zoomLevel;
            renderState.ViewSize = viewSize;
            renderState.ZoomSteps = 10;

            Vector2 boundary = new Vector2(RenderStateControl.BoundarySize, RenderStateControl.BoundarySize) * zoomLevel;
            Vector2 delta = new Vector2(50, 50) + boundary;

            renderStateControl.Pan(delta);

            Assert.AreEqual(renderState.Offset, offset + delta);
        }

        /// <summary>
        /// ... but shouldn't allow for too much movement
        /// </summary>
        [Test]
        public void TestPanWithZoomOutsideBoundaries()
        {
            Vector2 offset = new OpenTK.Vector2(-250, -250);
            Size frameSize = new Size(500, 500);
            float zoomLevel = 2f;
            Size viewSize = new Size(700, 700);
            renderState.Offset = offset;
            renderState.FrameSize = frameSize;
            renderState.ZoomLevel = zoomLevel;
            renderState.ViewSize = viewSize;
            renderState.ZoomSteps = 10;

            Vector2 boundary = new Vector2(-RenderStateControl.BoundarySize, -RenderStateControl.BoundarySize) * zoomLevel;
            Vector2 delta = new Vector2(-50, -50) + boundary;

            renderStateControl.Pan(delta);

            Assert.AreEqual(renderState.Offset, offset + boundary);
        }

        /// <summary>
        /// Should zoom in by a portion of the total zoom range
        /// </summary>
        [Test]
        public void ZoomIn()
        {
            Size frameSize = new Size(500, 500);
            float zoomLevel = 1f;
            Size viewSize = new Size(700, 700);
            int zoomSteps = 10;
            renderState.Offset = new Vector2();
            renderState.FrameSize = frameSize;
            renderState.ZoomLevel = zoomLevel;
            renderState.ViewSize = viewSize;
            renderState.ZoomSteps = zoomSteps;

            renderStateControl.Zoom(2);

            Assert.AreEqual(renderState.ZoomLevel, zoomLevel + 2 / 10f);
        }

        /// <summary>
        /// Should zoom out by a portion of the total zoom range
        /// </summary>
        [Test]
        public void ZoomOut()
        {
            Size frameSize = new Size(500, 500);
            float zoomLevel = 2f;
            Size viewSize = new Size(700, 700);
            int zoomSteps = 10;
            renderState.Offset = new Vector2();
            renderState.FrameSize = frameSize;
            renderState.ZoomLevel = zoomLevel;
            renderState.ViewSize = viewSize;
            renderState.ZoomSteps = zoomSteps;

            renderStateControl.Zoom(-2);

            Assert.AreEqual(renderState.ZoomLevel, zoomLevel - 2 / 10f);
        }

        /// <summary>
        /// Zooming out beyond zoom range should cap zoom at minimum zoom
        /// </summary>
        [Test]
        public void ZoomInBeyondLimit()
        {
            Size frameSize = new Size(500, 500);
            float zoomLevel = 2f;
            Size viewSize = new Size(700, 700);
            int zoomSteps = 10;
            renderState.Offset = new Vector2();
            renderState.FrameSize = frameSize;
            renderState.ZoomLevel = zoomLevel;
            renderState.ViewSize = viewSize;
            renderState.ZoomSteps = zoomSteps;

            renderStateControl.Zoom(2);

            Assert.AreEqual(renderState.ZoomLevel, zoomLevel);
        }

        /// <summary>
        /// Zooming in beyond zoom range should cap zoom at maximum zoom
        /// </summary>
        [Test]
        public void ZoomOutBeyondLimit()
        {
            Size frameSize = new Size(500, 500);
            float zoomLevel = 1f;
            Size viewSize = new Size(700, 700);
            int zoomSteps = 10;
            renderState.Offset = new Vector2();
            renderState.FrameSize = frameSize;
            renderState.ZoomLevel = zoomLevel;
            renderState.ViewSize = viewSize;
            renderState.ZoomSteps = zoomSteps;

            renderStateControl.Zoom(-2);

            Assert.AreEqual(renderState.ZoomLevel, zoomLevel);
        }

        //empty check matrix
        Matrix4 test = new Matrix4(new Vector4(), new Vector4(), new Vector4(), new Vector4());

        void ProjectionPresets()
        {            
            Vector2 offset = new OpenTK.Vector2(-250, -250);
            Size frameSize = new Size(500, 500);
            float zoomLevel = 2f;
            Size viewSize = new Size(700, 700);
            renderState.Offset = offset;
            renderState.FrameSize = frameSize;
            renderState.ZoomLevel = zoomLevel;
            renderState.ViewSize = viewSize;
            renderState.ZoomSteps = 10;

            renderState.ProjectionMatrix = test;
        }

        /// <summary>
        /// Projection matrix should change when zoomlevel has been updated
        /// </summary>
        [Test]
        public void TestProjectionMatrixChangedZoomLevel()
        {
            ProjectionPresets();

            renderState.Raise(x => x.PropertyChanged += null, new object[] { renderState, new PropertyChangedEventArgs("ZoomLevel") });

            Assert.AreNotEqual(renderState.ProjectionMatrix, test);
        }

        /// <summary>
        /// Projection matrix should change when offset has been updated
        /// </summary>
        [Test]
        public void TestProjectionMatrixChangedOffset()
        {
            ProjectionPresets();

            renderState.Raise(x => x.PropertyChanged += null, new object[] { renderState, new PropertyChangedEventArgs("Offset") });

            Assert.AreNotEqual(renderState.ProjectionMatrix, test);
        }

        /// <summary>
        /// Projection matrix should change when viewsize has been updated
        /// </summary>
        [Test]
        public void TestProjectionMatrixChangedViewSize()
        {
            ProjectionPresets();

            renderState.Raise(x => x.PropertyChanged += null, new object[] { renderState, new PropertyChangedEventArgs("ViewSize") });

            Assert.AreNotEqual(renderState.ProjectionMatrix, test);
        }

        /// <summary>
        /// Projection matrix should change when viewsize has been updated
        /// </summary>
        [Test]
        public void TestProjectionMatrixChangedFrameSize()
        {
            ProjectionPresets();

            renderState.Raise(x => x.PropertyChanged += null, new object[] { renderState, new PropertyChangedEventArgs("FrameSize") });

            Assert.AreNotEqual(renderState.ProjectionMatrix, test);
        }

        /// <summary>
        /// It should not change when a parameter not related to it has been updated (like itself)
        /// </summary>
        [Test]
        public void TestProjectionNotMatrixChanged()
        {
            ProjectionPresets();

            renderState.Raise(x => x.PropertyChanged += null, new object[] { renderState, new PropertyChangedEventArgs("ProjectionMatrix") });

            Assert.AreEqual(renderState.ProjectionMatrix, test);
        }

        /// <summary>
        /// Resetting should reset the view, but not the size (they should change according to other input)
        /// </summary>
        [Test]
        public void TestReset()
        {
            Vector2 offset = new OpenTK.Vector2(-250, -250);
            float zoomLevel = 2f;
            renderState.Offset = offset;
            renderState.ZoomLevel = zoomLevel;

            renderStateControl.Reset();

            Assert.AreEqual(renderState.ZoomLevel, 1f);
            Assert.AreEqual(renderState.Offset, new Vector2());
        }

        /// <summary>
        /// Should change the 
        /// </summary>
        [Test]
        public void TestSetFrameSize()
        {
            Size frameSize = new Size(500, 500);
            renderStateControl.SetFrameSize(frameSize);

            Assert.AreEqual(renderState.FrameSize, frameSize);
        }

        /// <summary>
        /// Center should be (0,0)
        /// </summary>
        [Test]
        public void ToWorldSpaceTestOrigin()
        {
            renderState.ViewSize = new Size(500, 500);
            renderState.Offset = new Vector2();
            renderState.ZoomLevel = 1f;

            Vector2 worldPos = renderStateControl.ToWorldSpace(new Vector2(250, 250));

            Assert.AreEqual(worldPos, new Vector2());
        }

        /// <summary>
        /// Other places should return positions accordingly
        /// </summary>
        [Test]
        public void ToWorldSpaceTestIdentity()
        {
            renderState.ViewSize = new Size(500, 500);
            renderState.Offset = new Vector2(125, 125);
            renderState.ZoomLevel = 2f;

            Vector2 worldPos = renderStateControl.ToWorldSpace(new Vector2(0, 0));

            Assert.AreEqual(worldPos, new Vector2());
        }
    }
}
