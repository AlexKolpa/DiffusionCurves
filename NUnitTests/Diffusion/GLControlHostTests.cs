using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using DiffusionCurves.Diffusion;
using DiffusionCurves.Model;
using NUnit.Framework;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

[assembly: RequiresSTA]
namespace NUnitTests.Diffusion
{
    /// <summary>
    /// Test class for the GLCotrolHost class
    /// </summary>
    [TestFixture, RequiresSTA]
    class GLControlHostTests
    {
        IDiffusionRenderer renderer;
        IEditorStateControl stateControl;
        EditorState state;
        ProjectState projectState;
        GLControl glControl;
        GLControlHost host;
        IRenderStateControl renderControl;
        IDiffusionPathControl bufferControl;

        /// <summary>
        /// Set up instance
        /// </summary>
        [SetUp, STAThread]
        public void TestSetup()
        {
            state = MockRepository.GenerateMock<EditorState>();
            projectState = MockRepository.GenerateMock<ProjectState>();
            renderer = MockRepository.GenerateStub<IDiffusionRenderer>();
            stateControl = MockRepository.GenerateMock<IEditorStateControl>();
            glControl = MockRepository.GenerateMock<GLControl>();
            renderControl = MockRepository.GenerateMock<IRenderStateControl>();
            bufferControl = MockRepository.GenerateMock<IDiffusionPathControl>();

            host = new GLControlHost(glControl, state, stateControl, renderer, renderControl, bufferControl);
        }

        /// <summary>
        /// Load should set the object as loaded and call the diffusionrenderer to load as well
        /// </summary>
        [Test, STAThread]
        public void TestLoad()
        {
            glControl.Stub(x => x.Width).Return(100);
            glControl.Stub(x => x.Height).Return(100);
            glControl.Raise(x => x.Load += null, new object[] { glControl, EventArgs.Empty });

            Assert.True(host.GLLoaded);
            renderer.AssertWasCalled(x => x.glControl_Load(Arg<object>.Is.Equal(glControl), Arg<EventArgs>.Is.Anything));
        }

        /// <summary>
        /// Renderer should resize when the host gets resized
        /// </summary>
        [Test, STAThread]
        public void TestResize()
        {
            glControl.Stub(x => x.Width).Return(100);
            glControl.Stub(x => x.Height).Return(100);
            glControl.Raise(x => x.Resize += null, new object[] { glControl, EventArgs.Empty });

            renderControl.AssertWasCalled(x => x.SetViewport(Arg<Size>.Is.Anything));
        }

        /// <summary>
        /// State should be set to idle when mouse is up
        /// </summary>
        [Test, STAThread]
        public void TestMouseUp()
        {
            MouseEventArgs emptyMouseEvent = new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0);
            glControl.Raise(x => x.MouseUp += null, new object[] { glControl, emptyMouseEvent });

            stateControl.AssertWasCalled(x => x.SetIdle());
        }

        /// <summary>
        /// When mouse is down, state should be updated, and the view should be redrawn
        /// </summary>
        [Test, STAThread]
        public void TestMouseDown()
        {
            MouseEventArgs emptyMouseEvent = new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0);
            glControl.Raise(x => x.MouseDown += null, glControl, emptyMouseEvent);

            stateControl.AssertWasCalled(x => x.UpdateState(new Vector2(0, 0)));
            glControl.AssertWasCalled(x => x.Invalidate());
        }

        /// <summary>
        /// When moving mouse with dragging, the view should redraw
        /// </summary>
        [Test, STAThread]
        public void TestMouseMoveWithDrag()
        {
            MouseEventArgs emptyMouseEvent = new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0);
            stateControl.Stub(x => x.DragStateItem(Arg<Vector2>.Is.Anything)).Return(true);
            glControl.Raise(x => x.MouseMove += null, glControl, emptyMouseEvent);

            stateControl.AssertWasCalled(x => x.DragStateItem(Arg<Vector2>.Is.Anything));
            glControl.AssertWasCalled(x => x.Invalidate());
        }

        /// <summary>
        /// When moving mouse with dragging, the view should not redraw
        /// </summary>
        [Test, STAThread]
        public void TestMouseMoveWithoutDrag()
        {
            MouseEventArgs emptyMouseEvent = new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0);
            stateControl.Stub(x => x.DragStateItem(Arg<Vector2>.Is.Anything)).Return(false);
            glControl.Raise(x => x.MouseMove += null, glControl, emptyMouseEvent);

            stateControl.AssertWasNotCalled(x => x.DragStateItem(Arg<Vector2>.Is.Anything));
            glControl.AssertWasNotCalled(x => x.Invalidate());
        }

        /// <summary>
        /// Pressing back key should delete selected point
        /// </summary>
        [Test, STAThread]
        public void TestKeyDownBack()
        {
            KeyEventArgs args = new KeyEventArgs(Keys.Back);

            glControl.Raise(x => x.KeyDown += null, host, args);

            stateControl.AssertWasCalled(x => x.DeleteSelectedPoint());

            glControl.AssertWasCalled(x => x.Invalidate());
        }

        /// <summary>
        /// Pressing left control should set modifier state to control dragging
        /// </summary>
        [Test, STAThread]
        public void TestKeyDownControlLeft()
        {
            KeyEventArgs args = new KeyEventArgs(Keys.Control | Keys.Left);

            glControl.Raise(x => x.KeyDown += null, host, args);

            Assert.That(state.ModifierState, Is.EqualTo(EditorStateControl.KeyModifierState.ControlDragging));
        }

        /// <summary>
        /// Pressing right control should set modifier state to control dragging
        /// </summary>
        [Test, STAThread]
        public void TestKeyDownControlRight()
        {
            KeyEventArgs args = new KeyEventArgs(Keys.Control | Keys.Left);

            glControl.Raise(x => x.KeyDown += null, host, args);

            Assert.That(state.ModifierState, Is.EqualTo(EditorStateControl.KeyModifierState.ControlDragging));
        }

        /// <summary>
        /// Pressing left shift should set modifier state to new path
        /// </summary>
        [Test, STAThread]
        public void TestKeyDownShiftLeft()
        {
            KeyEventArgs args = new KeyEventArgs(Keys.Shift | Keys.Left);

            glControl.Raise(x => x.KeyDown += null, host, args);

            Assert.That(state.ModifierState, Is.EqualTo(EditorStateControl.KeyModifierState.NewPath));
        }

        /// <summary>
        /// Pressing right shift should set modifier state to new path
        /// </summary>
        [Test, STAThread]
        public void TestKeyDownShiftRight()
        {
            KeyEventArgs args = new KeyEventArgs(Keys.Shift | Keys.Left);

            glControl.Raise(x => x.KeyDown += null, host, args);

            Assert.That(state.ModifierState, Is.EqualTo(EditorStateControl.KeyModifierState.NewPath));
        }

        /// <summary>
        /// Pressing other keys should have no effect on the state
        /// </summary>
        [Test, STAThread]
        public void TestOtherKey()
        {
            KeyEventArgs args = new KeyEventArgs(Keys.Q);

            glControl.Raise(x => x.KeyDown += null, host, args);

            Assert.That(state.ModifierState, Is.EqualTo(EditorStateControl.KeyModifierState.None));
            Assert.That(state.CurrentState, Is.EqualTo(EditorStateControl.MouseState.Idle));
        }

        /// <summary>
        /// Letting a key go should have no effect on the state
        /// </summary>
        [Test, STAThread]
        public void TestKeyUp()
        {
            state.ModifierState = EditorStateControl.KeyModifierState.NewPath;

            glControl.Raise(x => x.KeyUp += null, host, null);

            Assert.That(state.ModifierState, Is.EqualTo(EditorStateControl.KeyModifierState.None));
        }

        /// <summary>
        /// When the control is loaded, the view can draw
        /// </summary>
        [Test, STAThread]
        public void TestpaintWithLoad()
        {
            glControl.Stub(x => x.Width).Return(100);
            glControl.Stub(x => x.Height).Return(100);
            glControl.Raise(x => x.Load += null, new object[] { glControl, EventArgs.Empty });

            glControl.Raise(x => x.Paint += null, glControl, null);

            renderer.AssertWasCalled(x => x.Draw());
            glControl.AssertWasCalled(x => x.SwapBuffers());
        }

        /// <summary>
        /// View shouldn't draw if the control hasn't been loaded, this causes OpenGL errors
        /// </summary>
        [Test, STAThread]
        public void TestpaintWithoutLoad()
        {
            glControl.Raise(x => x.Paint += null, glControl, null);

            renderer.AssertWasNotCalled(x => x.Draw());
            glControl.AssertWasNotCalled(x => x.SwapBuffers());
        }

        /// <summary>
        /// Setting a new frame should reset the state, and set the arguments as active in EditorStateControl and DiffusionRenderer
        /// </summary>
        [Test, STAThread]
        public void TestSetNewFrame()
        {
            Bitmap bitmap = MockRepository.GenerateMock<Bitmap>();
            IPathContainer container = MockRepository.GenerateMock<IPathContainer>();

            host.SetNewFrame(bitmap, container);

            stateControl.AssertWasCalled(x => x.SetPathsContainer(container));
            stateControl.AssertWasCalled(x => x.SetIdle());

            renderer.AssertWasCalled(x => x.SetFrame(bitmap, container));

            glControl.AssertWasCalled(x => x.Invalidate());
        }

        /// <summary>
        /// Passing null for pathscontainer should throw an error
        /// </summary>
        [Test, STAThread]
        public void TestSetNewFrameNullContainer()
        {
            Bitmap bitmap = MockRepository.GenerateMock<Bitmap>();
            IPathContainer container = null;

            Assert.Throws<ArgumentNullException>(() => host.SetNewFrame(bitmap, container));
        }
    }
}
