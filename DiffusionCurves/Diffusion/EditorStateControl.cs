using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using OpenTK;
using System.Diagnostics;
using System.ComponentModel;
using DiffusionCurves.Diffusion;
using DiffusionCurves.Model;
using DiffusionCurves.Events;
using System.Runtime.CompilerServices;

namespace DiffusionCurves.Diffusion
{
    /// <summary>
    /// Class for EditorStateControl.
    /// </summary>
    public class EditorStateControl : DiffusionCurves.Diffusion.IEditorStateControl
    {
        #region Private fields

        IPathContainer pathsContainer;
        EditorState editorState;
        ProjectState projectState;
        RenderState renderState;

        float clickRange = 10f;

        private float ClickRange
        {
            get { return clickRange / renderState.ZoomLevel; }
        }
        #endregion

        public enum KeyModifierState
        {
            None,
            ControlDragging,
            NewPath            
        }

        public enum MouseState
        {
            Idle,
            Control1Dragging,
            Control2Dragging,
            NewPointDragging,
            PointDragging            
        };
        
        /// <summary>
        /// Constructor for EditorStateControl.
        /// </summary>
        /// <param name="editorState"></param>
        /// <param name="projectState"></param>
        public EditorStateControl(EditorState editorState, ProjectState projectState, RenderState renderState)
        {
            this.editorState = editorState;
            this.renderState = renderState;
            this.projectState = projectState;
            this.projectState.PropertyChanged += projectState_PropertyChanged;

            editorState.CurrentState = MouseState.Idle;
            editorState.ModifierState = KeyModifierState.None;
        }
        
        /// <summary>
        /// Sets the selected path for the newly created.
        /// </summary>
        void SetNewSelectedPath()
        {
            Path lastPath = null;

            if(pathsContainer.Count > 0)
                lastPath = pathsContainer.GetLastPath();

            if (lastPath != null && lastPath.IsEmpty())
            {
                editorState.SelectedPath = lastPath;
            }
            else
            {
                Path newPath = new Path();
                editorState.SelectedPath = newPath;
                pathsContainer.AddPath(newPath);
            }
        }

        /// <summary>
        /// Deletes the selected point.
        /// </summary>
        public void DeleteSelectedPoint()
        {
            if (projectState.EditorState != ProjectState.ProjectEditorState.Color)
            {
                editorState.SelectedPoint = editorState.SelectedPath.RemovePoint(editorState.SelectedPoint);

                Path lastPath = null;
                if (pathsContainer.Count > 0)
                    lastPath = pathsContainer.GetLastPath();

                if (editorState.SelectedPath.IsEmpty() && lastPath != editorState.SelectedPath)
                {
                    pathsContainer.RemovePath(editorState.SelectedPath);
                    SetNewSelectedPath();
                }
            }
        }

        /// <summary>
        /// Updates the state of the editor.
        /// </summary>
        /// <param name="position"></param>
        public void UpdateState(Vector2 position)
        {
            BezierPoint clickedPoint = GetPointWithinRange(position, ClickRange);
            if (clickedPoint != null)
            {
                editorState.SelectedPoint = clickedPoint;
                IEnumerator<Path> pathsEnumerator = pathsContainer.GetPathsEnumerator();
                while (pathsEnumerator.MoveNext())
                {
                    if (pathsEnumerator.Current.Contains(clickedPoint))
                    {
                        editorState.SelectedPath = pathsEnumerator.Current;
                        break;
                    }
                }
                editorState.CurrentState = MouseState.PointDragging;
            }

            if (projectState.EditorState != ProjectState.ProjectEditorState.Color)
            {
                if (editorState.ModifierState == KeyModifierState.ControlDragging)
                {
                    editorState.CurrentState = GetControlWithinRange(position, ClickRange);
                }
                else if (clickedPoint == null)
                {
                    editorState.SelectedPoint = AddPoint(position);
                    editorState.CurrentState = MouseState.NewPointDragging;
                }
            }
        }

        /// <summary>
        /// moves part of the selected point based on the state of the editor
        /// </summary>
        /// <param name="newPosition">The new position of the mouse filename</param>
        /// <returns></returns>
        public bool DragStateItem(Vector2 newPosition)
        {            
            if (editorState.SelectedPoint != null && projectState.EditorState != ProjectState.ProjectEditorState.Color)
            {
                switch (editorState.CurrentState)
                {
                    case MouseState.Control1Dragging:
                        editorState.SelectedPoint.Control1 = newPosition - editorState.SelectedPoint.Position;
                        break;
                    case MouseState.Control2Dragging:
                        editorState.SelectedPoint.Control2 = newPosition - editorState.SelectedPoint.Position;
                        break;
                    case MouseState.PointDragging:
                        editorState.SelectedPoint.Position = newPosition;
                        break;
                    case MouseState.NewPointDragging:
                        editorState.SelectedPoint.Control1 = newPosition - editorState.SelectedPoint.Position;
                        editorState.SelectedPoint.Control2 = -(newPosition - editorState.SelectedPoint.Position);
                        break;
                    default:
                        return false;

                }

                return true;
            }

            return false;
        }
        /// <summary>
        /// Sets Editor state to idle.
        /// </summary>
        public void SetIdle()
        {
            editorState.CurrentState = MouseState.Idle;
            editorState.ModifierState = KeyModifierState.None;
        }

        /// <summary>
        /// Adds point to diffusion curves.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        BezierPoint AddPoint(Vector2 position)
        {
            if (editorState.ModifierState == KeyModifierState.NewPath)
            {
                SetNewSelectedPath();
            }

            LinkedListNode<BezierPoint> createdPoint = null;

            BezierPoint colorCopy = null;

            if (editorState.SelectedPath.GetLastPoint() == editorState.SelectedPoint || editorState.SelectedPath.IsEmpty())
            {
                createdPoint = editorState.SelectedPath.AddPointLast(position);
                if(createdPoint.Previous != null)
                    colorCopy = createdPoint.Previous.Value;
            }
            else if (editorState.SelectedPath.GetFirstPoint() == editorState.SelectedPoint)
            {
                createdPoint = editorState.SelectedPath.AddPointFirst(position);
                if(createdPoint.Next != null)
                    colorCopy = createdPoint.Next.Value;
            }
            else
            {
                SetNewSelectedPath();

                createdPoint = editorState.SelectedPath.AddPointLast(position);
            }

            if (colorCopy != null)
            {
                createdPoint.Value.LeftColor = colorCopy.LeftColor;
                createdPoint.Value.RightColor = colorCopy.RightColor;
            }

            if (editorState.AllowSmoothUpdate)
                UpdatePoint(createdPoint);

            return createdPoint.Value;
        }

        /// <summary>
        /// Gets the control of a point.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        MouseState GetControlWithinRange(Vector2 position, float range)
        {   
            MouseState result = MouseState.Idle;

            if (editorState.SelectedPoint == null)
                return result;

            position -= editorState.SelectedPoint.Position;

            if ((editorState.SelectedPoint.Control1 - position).Length < range)
            {
                result = MouseState.Control1Dragging;
            }
            else if ((editorState.SelectedPoint.Control2 - position).Length < range)
            {
                result = MouseState.Control2Dragging;
            }

            return result;
        }
         
        /// <summary>
        /// Gets points within the range of given position.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        BezierPoint GetPointWithinRange(Vector2 position, float range)
        {
            BezierPoint result = null;

            if (editorState.SelectedPoint != null)
            {
                if ((editorState.SelectedPoint.Position - position).Length < range)
                    return editorState.SelectedPoint;
            }

            IEnumerator<Path> pathEnumerator = pathsContainer.GetPathsEnumerator();
            while (pathEnumerator.MoveNext())
            {
                BezierPoint pathPoint = pathEnumerator.Current.GetClosestPoint(position);

                if (pathPoint != null)
                {
                    if ((pathPoint.Position - position).Length < range)
                        return pathPoint;
                }
            }

            return result;
        }

        void UpdatePoint(LinkedListNode<BezierPoint> point)
        {   
            if(editorState.SelectedPath.Count > 2)
            {
                BezierPoint center = null;
                BezierPoint back = null;
                BezierPoint front = point.Value;
                if (point.Next == null)
                {
                    center = point.Previous.Value;
                    back = point.Previous.Previous.Value;
                }
                else if (point.Previous == null)
                {
                    back = front;
                    center = point.Next.Value;
                    front = point.Next.Next.Value;
                }
                else
                    return;

                Vector2[] controlpoints = GetControlsPoints(back.Position, center.Position, front.Position);

                center.Control1 = controlpoints[0];
                center.Control2 = controlpoints[1];
            }
        }

        /// <summary>
        /// Generates controlpoints for point p1
        /// </summary>
        /// <param name="p0">The point previous to p1</param>
        /// <param name="p1">The point to generate control points for</param>
        /// <param name="p2">The point next to p1</param>
        /// <param name="tension">tightness of the curve</param>
        /// <returns>a set of two control points for p1</returns>
        public static Vector2[] GetControlsPoints(Vector2 p0, Vector2 p1, Vector2 p2, float tension = 0.5f)
        {
            // get length of lines [p0-p1] and [p1-p2]
            float d01 = (p0 - p1).Length;
            float d12 = (p1 - p2).Length;
            // calculate scaling factors as fractions of total
            float sa = tension * d01 / (d01 + d12);
            float sb = tension * d12 / (d01 + d12);
            // left control point
            float c1x = p1.X - sa * (p2.X - p0.X);
            float c1y = p1.Y - sa * (p2.Y - p0.Y);
            // right control point
            float c2x = p1.X + sb * (p2.X - p0.X);
            float c2y = p1.Y + sb * (p2.Y - p0.Y);
            // return control points
            return new Vector2[] { new Vector2(c1x, c1y) - p1, new Vector2(c2x, c2y) - p1 };
        }
        
        /// <summary>
        /// Event for handling changes in the project state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void projectState_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ActiveLayerIndex":
                    SetNewSelectedPath();
                    editorState.SelectedPoint = null;                    
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Sets the paths in the container.
        /// </summary>
        /// <param name="container"></param>
        public void SetPathsContainer(IPathContainer container)
        {
            if (container != null)
            {
                pathsContainer = container;
                editorState.SelectedPoint = null;

                if (pathsContainer.Count == 0)
                {
                    SetNewSelectedPath();
                }
                else
                {
                    editorState.SelectedPath = pathsContainer.GetLastPath();
                }
            }
        }
    }
}
