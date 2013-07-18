using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Rhino.Mocks;
using DiffusionCurves;
using OpenTK;
using DiffusionCurves.Diffusion;
using System.Diagnostics;
using DiffusionCurves.Model;
using System.ComponentModel;

namespace NUnitTests
{
    [TestFixture]
    class EditorStateControlTests
    {
        EditorStateControl inputstateControl;
        EditorState editorState;
        RenderState renderState;
        ProjectState projectState;
        IPathContainer container;
        
        [SetUp]
        public void InputStateSetup()
        {
            editorState = new EditorState();
            renderState = new RenderState();
            projectState = MockRepository.GenerateMock<ProjectState>();
            container = MockRepository.GenerateMock<IPathContainer>();            
            inputstateControl = new EditorStateControl(editorState, projectState, renderState);
            inputstateControl.SetPathsContainer(container);
        }

        /// <summary>
        /// Constructing the inputstate should set a new 
        /// </summary>
        [Test]
        public void TestInputStateConstructor()
        {
            Assert.That(editorState.CurrentState, Is.EqualTo(EditorStateControl.MouseState.Idle));
            Assert.That(editorState.ModifierState, Is.EqualTo(EditorStateControl.KeyModifierState.None));

            Assert.IsNull(editorState.SelectedPoint);
            Assert.IsNotNull(editorState.SelectedPath);
            Assert.True(editorState.SelectedPath.IsEmpty());
        }

        /// <summary>
        /// Tests the update state without any modifiers and a single empty path (start state)
        /// </summary>
        [Test]
        public void TestEmptyUpdateState()
        {
            container.Stub(x => x.GetPathsEnumerator()).Return(null).WhenCalled(x => x.ReturnValue = new List<Path>() { new Path() }.GetEnumerator());
            

            Vector2 vector = new Vector2(200, 150);
            inputstateControl.UpdateState(vector);

            Assert.That(editorState.SelectedPoint, Is.EqualTo(new BezierPoint(vector)));
            Assert.That(editorState.CurrentState, Is.EqualTo(EditorStateControl.MouseState.NewPointDragging));
            Assert.That(editorState.ModifierState, Is.EqualTo(EditorStateControl.KeyModifierState.None));
            Assert.False(editorState.SelectedPath.IsEmpty());
        }

        /// <summary>
        /// Clicking on a existing point should switch the state to drag mode
        /// </summary>
        [Test]
        public void TestSinglePointUpdateState()
        {
            Path simplePath = new Path();
            Vector2 position = new Vector2(200, 150);
            BezierPoint createdPoint = simplePath.AddPointLast(position).Value;

            container.Stub(x => x.GetPathsEnumerator()).Return(null).WhenCalled(x => x.ReturnValue = new List<Path>() { simplePath }.GetEnumerator());        

            inputstateControl.UpdateState(position);

            Assert.That(editorState.SelectedPoint, Is.EqualTo(createdPoint));
            Assert.That(editorState.SelectedPath, Is.EqualTo(simplePath));
            Assert.That(editorState.CurrentState, Is.EqualTo(EditorStateControl.MouseState.PointDragging));
        }

        /// <summary>
        /// clicking controlpoint1 with control active goes to control dragging mode
        /// </summary>
        [Test]
        public void TestControl1PointUpdateState()
        {
            Path simplePath = new Path();
            Vector2 position = new Vector2(200, 150);
            BezierPoint createdPoint = simplePath.AddPointLast(position).Value;

            editorState.SelectedPoint = createdPoint;
            editorState.SelectedPath = simplePath;
            editorState.CurrentState = EditorStateControl.MouseState.Idle;
            editorState.ModifierState = EditorStateControl.KeyModifierState.ControlDragging;

            container.Stub(x => x.GetPathsEnumerator()).Return(null).WhenCalled(x => x.ReturnValue = new List<Path>() { simplePath }.GetEnumerator());    

            inputstateControl.UpdateState(position);

            Assert.That(editorState.SelectedPoint, Is.EqualTo(createdPoint));
            Assert.That(editorState.SelectedPath, Is.EqualTo(simplePath));
            Assert.That(editorState.CurrentState, Is.EqualTo(EditorStateControl.MouseState.Control1Dragging));
        }

        /// <summary>
        /// clicking controlpoint2 with control active goes to control dragging mode
        /// </summary>
        [Test]
        public void TestControl2PointUpdateState()
        {
            Path simplePath = new Path();
            Vector2 position = new Vector2(200, 150);
            BezierPoint createdPoint = simplePath.AddPointLast(position).Value;
            Vector2 controlPosition = new Vector2(50, 0);
            createdPoint.Control2 = controlPosition;

            editorState.SelectedPoint = createdPoint;
            editorState.SelectedPath = simplePath;
            editorState.CurrentState = EditorStateControl.MouseState.Idle;
            editorState.ModifierState = EditorStateControl.KeyModifierState.ControlDragging;

            container.Stub(x => x.GetPathsEnumerator()).Return(null).WhenCalled(x => x.ReturnValue = new List<Path>() { simplePath }.GetEnumerator());

            inputstateControl.UpdateState(position + controlPosition);

            Assert.That(editorState.SelectedPoint, Is.EqualTo(createdPoint));
            Assert.That(editorState.SelectedPath, Is.EqualTo(simplePath));
            Assert.That(editorState.CurrentState, Is.EqualTo(EditorStateControl.MouseState.Control2Dragging));            
        }

        /// <summary>
        /// Dragging an existing point should move it to the new position
        /// </summary>
        [Test]
        public void TestSinglePointDragState()
        {
            Vector2 position = new Vector2(200, 150);
            BezierPoint createdPoint = new BezierPoint(position);

            editorState.SelectedPoint = createdPoint;
            editorState.CurrentState = EditorStateControl.MouseState.PointDragging;
            editorState.ModifierState = EditorStateControl.KeyModifierState.None;

            Vector2 newPosition = new Vector2(200, 160);
            Assert.True(inputstateControl.DragStateItem(newPosition));

            Assert.That(editorState.SelectedPoint, Is.EqualTo(createdPoint));
            Assert.That(createdPoint.Position, Is.EqualTo(newPosition));
            Assert.That(editorState.CurrentState, Is.EqualTo(EditorStateControl.MouseState.PointDragging));
            Assert.That(editorState.ModifierState, Is.EqualTo(EditorStateControl.KeyModifierState.None));
        }

        /// <summary>
        /// Dragging an existing point with control1 state should move control point 1
        /// </summary>
        [Test]
        public void TestControl1PointDragState()
        {
            Vector2 position = new Vector2(200, 150);
            BezierPoint createdPoint = new BezierPoint(position);

            editorState.SelectedPoint = createdPoint;
            editorState.CurrentState = EditorStateControl.MouseState.Control1Dragging;

            Vector2 newPosition = new Vector2(200, 160);
            Assert.True(inputstateControl.DragStateItem(newPosition));

            Assert.That(editorState.SelectedPoint, Is.EqualTo(createdPoint));
            Assert.That(createdPoint.Control1, Is.EqualTo(newPosition - position));
            Assert.That(editorState.CurrentState, Is.EqualTo(EditorStateControl.MouseState.Control1Dragging));
        }

        /// <summary>
        /// Dragging an existing point with control2 state should move control point 2
        /// </summary>
        [Test]
        public void TestControl2PointDragState()
        {
            Vector2 position = new Vector2(200, 150);
            BezierPoint createdPoint = new BezierPoint(position);

            editorState.SelectedPoint = createdPoint;
            editorState.CurrentState = EditorStateControl.MouseState.Control2Dragging;

            Vector2 newPosition = new Vector2(200, 160);
            Assert.True(inputstateControl.DragStateItem(newPosition));

            Assert.That(editorState.SelectedPoint, Is.EqualTo(createdPoint));
            Assert.That(createdPoint.Control2, Is.EqualTo(newPosition - position));
            Assert.That(editorState.CurrentState, Is.EqualTo(EditorStateControl.MouseState.Control2Dragging));
        }

        /// <summary>
        /// Dragging with new point should move both control points
        /// </summary>
        [Test]
        public void TestNewPointDragState()
        {
            Vector2 position = new Vector2(200, 150);
            BezierPoint createdPoint = new BezierPoint(position);

            editorState.SelectedPoint = createdPoint;
            editorState.CurrentState = EditorStateControl.MouseState.NewPointDragging;

            Vector2 newPosition = new Vector2(200, 160);
            Assert.True(inputstateControl.DragStateItem(newPosition));

            Assert.That(editorState.SelectedPoint, Is.EqualTo(createdPoint));
            Assert.That(createdPoint.Control1, Is.EqualTo(newPosition - position));
            Assert.That(createdPoint.Control2, Is.EqualTo(-(newPosition - position)));            
            Assert.That(editorState.CurrentState, Is.EqualTo(EditorStateControl.MouseState.NewPointDragging));
        }

        /// <summary>
        /// Moving mouse when idle does nothing
        /// </summary>
        [Test]
        public void TestIdleMouseDragState()
        {
            Vector2 position = new Vector2(200, 150);
            BezierPoint createdPoint = new BezierPoint(position);

            editorState.SelectedPoint = createdPoint;
            editorState.CurrentState = EditorStateControl.MouseState.Idle;

            Vector2 newPosition = new Vector2(200, 160);
            Assert.False(inputstateControl.DragStateItem(newPosition));

            Assert.That(editorState.SelectedPoint, Is.EqualTo(createdPoint));
            Assert.That(editorState.CurrentState, Is.EqualTo(EditorStateControl.MouseState.Idle));
        }

        /// <summary>
        /// Dragging mouse when SelectedPoint is null should do nothing
        /// </summary>
        [Test]
        public void TestNullSelectedPointDragState()
        {
            editorState.SelectedPoint = null;

            Vector2 newPosition = new Vector2(200, 160);
            Assert.False(inputstateControl.DragStateItem(newPosition));
            Assert.IsNull(editorState.SelectedPoint);
        }

        /// <summary>
        /// When clicked outside the range of an existing point, a new point should be created and set as selected point
        /// </summary>
        [Test]
        public void TestPointOutsideOfRange()
        {
            Path simplePath = new Path();
            Vector2 position = new Vector2(200, 150);
            BezierPoint createdPoint = simplePath.AddPointLast(position).Value;

            editorState.SelectedPoint = createdPoint;
            editorState.SelectedPath = simplePath;
            editorState.CurrentState = EditorStateControl.MouseState.Idle;
            editorState.ModifierState = EditorStateControl.KeyModifierState.None;

            container.Stub(x => x.GetPathsEnumerator()).Return(null).WhenCalled(x => x.ReturnValue = new List<Path>() { simplePath }.GetEnumerator());

            Vector2 newPosition = new Vector2(200, 161);
            inputstateControl.UpdateState(newPosition);

            Assert.That(editorState.SelectedPoint, Is.EqualTo(new BezierPoint(newPosition)));
            Assert.That(editorState.CurrentState, Is.EqualTo(EditorStateControl.MouseState.NewPointDragging));
            Assert.That(editorState.SelectedPath, Is.EqualTo(simplePath));
        }

        /// <summary>
        /// When current point is not at the end of the path, a new path should be created with a new point
        /// </summary>
        [Test]
        public void TestPointOutsideOfRangeWithNewPath()
        {
            Path simplePath = new Path();
            Vector2 position = new Vector2(200, 150);
            simplePath.AddPointLast(new Vector2(200, 100));
            BezierPoint createdPoint = simplePath.AddPointLast(position).Value;
            simplePath.AddPointLast(new Vector2(200, 200));

            editorState.SelectedPoint = createdPoint;
            editorState.SelectedPath = simplePath;
            editorState.CurrentState = EditorStateControl.MouseState.Idle;
            editorState.ModifierState = EditorStateControl.KeyModifierState.None;

            container.Stub(x => x.GetPathsEnumerator()).Return(null).WhenCalled(x => x.ReturnValue = new List<Path>() { simplePath }.GetEnumerator());

            Vector2 newPosition = new Vector2(250, 150);
            inputstateControl.UpdateState(newPosition);

            Assert.That(editorState.SelectedPoint, Is.EqualTo(new BezierPoint(newPosition)));
            Assert.That(editorState.CurrentState, Is.EqualTo(EditorStateControl.MouseState.NewPointDragging));

            container.AssertWasCalled(x => x.AddPath(Arg<Path>.Is.Anything), opt => opt.Repeat.Times(2));
            Assert.That(editorState.SelectedPath, Is.Not.EqualTo(simplePath));
        }

        /// <summary>
        /// Adding a new path when the current path isn't empty 
        /// </summary>
        [Test]
        public void TestAddNewPath()
        {
            Path simplePath = editorState.SelectedPath;
            Vector2 position = new Vector2(200, 150);
            BezierPoint createdPoint = simplePath.AddPointLast(position).Value;

            editorState.SelectedPoint = createdPoint;
            editorState.SelectedPath = simplePath;
            editorState.CurrentState = EditorStateControl.MouseState.Idle;
            editorState.ModifierState = EditorStateControl.KeyModifierState.NewPath;

            container.Stub(x => x.GetPathsEnumerator()).Return(null).WhenCalled(x => x.ReturnValue = new List<Path>() { simplePath }.GetEnumerator());
            container.Stub(x => x.GetLastPath()).Return(simplePath);

            Vector2 newPosition = new Vector2(250, 150);
            inputstateControl.UpdateState(newPosition);

            Assert.That(editorState.SelectedPoint, Is.EqualTo(new BezierPoint(newPosition)));
            Assert.That(editorState.CurrentState, Is.EqualTo(EditorStateControl.MouseState.NewPointDragging));

            container.AssertWasCalled(x => x.AddPath(Arg<Path>.Is.Anything), opt => opt.Repeat.Times(2));
            Assert.That(editorState.SelectedPath, Is.Not.EqualTo(simplePath));
        }

        //Test that no useless paths get created

        /// <summary>
        /// When last path is empty, even with newpath active, 
        /// no new path should be created and the point should be added to the selected path
        /// </summary>
        [Test]
        public void TestAddNewPathWithEmptyPath()
        {
            editorState.CurrentState = EditorStateControl.MouseState.Idle;
            editorState.ModifierState = EditorStateControl.KeyModifierState.NewPath;

            container.Stub(x => x.GetPathsEnumerator()).Return(null).WhenCalled(x => x.ReturnValue = new List<Path>() { editorState.SelectedPath }.GetEnumerator());
            container.Stub(x => x.GetLastPath()).Return(null).WhenCalled(x => x.ReturnValue = editorState.SelectedPath);
            container.Stub(x => x.Count).Return(1);

            Debug.WriteLine("Test Empty Path:");
            Vector2 position = new Vector2(200, 150);
            inputstateControl.UpdateState(position);

            Assert.That(editorState.SelectedPoint, Is.EqualTo(new BezierPoint(position)));
            Assert.That(editorState.CurrentState, Is.EqualTo(EditorStateControl.MouseState.NewPointDragging));
            Assert.False(editorState.SelectedPath.IsEmpty());
            container.AssertWasCalled(x => x.AddPath(Arg<Path>.Is.Anything), opt => opt.Repeat.Times(1));
        }

        /// <summary>
        /// When a point is deleted, it should be removed from the path, and as the selected point
        /// </summary>
        [Test]
        public void TestDeletePoint()
        {
            Path selectedPath = new Path();
            Vector2 position = new Vector2(200, 150);
            BezierPoint selectedPoint = selectedPath.AddPointLast(position).Value;

            editorState.SelectedPath = selectedPath;
            editorState.SelectedPoint = selectedPoint;

            inputstateControl.DeleteSelectedPoint();

            Assert.IsNotNull(editorState.SelectedPath);
            Assert.False(editorState.SelectedPath.Contains(selectedPoint));
            Assert.IsNull(editorState.SelectedPoint);
        }

        /// <summary>
        /// Setting the state to idle should change to mousestate to idle
        /// </summary>
        [Test]
        public void TestSetIdle()
        {
            editorState.CurrentState = EditorStateControl.MouseState.PointDragging;
            editorState.ModifierState = EditorStateControl.KeyModifierState.NewPath;

            inputstateControl.SetIdle();

            Assert.That(editorState.CurrentState, Is.EqualTo(EditorStateControl.MouseState.Idle));
            Assert.That(editorState.ModifierState, Is.EqualTo(EditorStateControl.KeyModifierState.None));
        }

        /// <summary>
        /// If an empty container gets set as the active container, the selected point should be cleared,
        /// and a new path should be added and set as the selected path.
        /// </summary>
        [Test]
        public void TestSetPathContainerNewPath()
        {
            IPathContainer container = MockRepository.GenerateMock<IPathContainer>();
            container.Stub(x => x.Count).Return(0);

            inputstateControl.SetPathsContainer(container);

            Assert.IsNull(editorState.SelectedPoint);
            Assert.IsNotNull(editorState.SelectedPath);
            container.AssertWasCalled(x => x.AddPath(Arg<Path>.Is.Anything));
        }

        /// <summary>
        /// If a non-empty container gets set as the active container, the selected point should be cleared, 
        /// and the selected path should be the last path in the container.
        /// </summary>
        [Test]
        public void TestSetPathContainerExistingPath()
        {
            IPathContainer container = MockRepository.GenerateMock<IPathContainer>();
            Path simplePath = new Path();
            container.Stub(x => x.Count).Return(1);
            container.Stub(x => x.GetLastPath()).Return(simplePath);

            inputstateControl.SetPathsContainer(container);

            Assert.IsNull(editorState.SelectedPoint);
            Assert.IsNotNull(editorState.SelectedPath);
            Assert.That(editorState.SelectedPath, Is.EqualTo(simplePath));
            container.AssertWasNotCalled(x => x.AddPath(Arg<Path>.Is.Anything));
        }

        /// <summary>
        /// event handling for projectstate should change the selected path
        /// </summary>
        [Test]
        public void TestProjectStateChangedNewPath()
        {
            container.Stub(x => x.Count).Return(0);
            
            projectState.Raise(x => x.PropertyChanged += null, new object[] { projectState, new PropertyChangedEventArgs("ActiveLayerIndex") });

            Assert.IsNull(editorState.SelectedPoint);
            container.AssertWasCalled(x => x.AddPath(Arg<Path>.Is.Anything));
            Assert.True(editorState.SelectedPath.IsEmpty());
        }

        /// <summary>
        /// event handling for other properties should be ignored
        /// </summary>
        [Test]
        public void TestProjectStateNotChanged()
        {
            container.Stub(x => x.Count).Return(0);

            projectState.Raise(x => x.PropertyChanged += null, new object[] { projectState, new PropertyChangedEventArgs("OtherProperty") });

            container.AssertWasCalled(x => x.AddPath(Arg<Path>.Is.Anything), options => options.Repeat.Once());
        }

    }
}
