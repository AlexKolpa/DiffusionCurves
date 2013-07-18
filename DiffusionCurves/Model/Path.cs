using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Diagnostics;
using System.Xml.Serialization;

namespace DiffusionCurves.Model
{
    /// <summary>
    /// Class for Path.
    /// </summary>
    [Serializable]
    public class Path
    {
        #region Private fields

        LinkedList<BezierPoint> curves;

        #endregion

        #region Getters setters

        public int Count
        {
            get { return curves.Count; }
        }

        #endregion

        /// <summary>
        /// Constructor for path.
        /// </summary>
        public Path()
        {
            curves = new LinkedList<BezierPoint>();
        }

        /// <summary>
        /// Adds point to the end of the list.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public LinkedListNode<BezierPoint> AddPointLast(Vector2 point)
        {
            BezierPoint bezierPoint = new BezierPoint(point);
            return curves.AddLast(bezierPoint);
        }

        /// <summary>
        /// Adds point to the beginning of the list.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public LinkedListNode<BezierPoint> AddPointFirst(Vector2 point)
        {
            BezierPoint bezierPoint = new BezierPoint(point);
            return curves.AddFirst(bezierPoint); 
        }

        /// <summary>
        /// Adds point.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="controlPoint1"></param>
        /// <param name="controlPoint2"></param>
        public void AddPoint(Vector2 point, Vector2 controlPoint1, Vector2 controlPoint2)
        {
            curves.AddLast(new BezierPoint(point, controlPoint1, controlPoint2));
        }

        /// <summary>
        /// Removes the given bezier point.
        /// </summary>
        /// <param name="point"></param>
        public BezierPoint RemovePoint(BezierPoint point)
        {
            LinkedListNode<BezierPoint> current = curves.Find(point);
            BezierPoint neighbor = null;
            if (current != null)
            {
                if (current.Previous != null)
                    neighbor = current.Previous.Value;
                else if (current.Next != null)
                    neighbor = current.Next.Value;
                curves.Remove(point);
            }

            return neighbor;
        }
        
        public LinkedList<BezierPoint> Points
        {
            get { return curves; }
            set { curves = value; }
        }

        /// <summary>
        /// Gets the last point.
        /// </summary>
        /// <returns></returns>
        public BezierPoint GetLastPoint()
        {
            if (curves.Count == 0)
                return null;

            return curves.Last.Value;
        }

        /// <summary>
        /// Gets the first point.
        /// </summary>
        /// <returns></returns>
        public BezierPoint GetFirstPoint()
        {
            if (curves.Count == 0)
                return null;

            return curves.First.Value;
        }

        /// <summary>
        /// Removes the last point.
        /// </summary>
        public void RemoveLastPoint()
        {
            if (curves.Count == 0)
                return;

            curves.RemoveLast();
        }

        /// <summary>
        /// Gets an enumerator of the list of curves.
        /// </summary>
        /// <returns></returns>
        public LinkedList<BezierPoint>.Enumerator GetEnumerator()
        {
            return curves.GetEnumerator();
        }
        
        /// <summary>
        /// Checks if the list already contains the bezierpoint.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Contains(BezierPoint point)
        {
            return curves.Contains(point);
        }

        /// <summary>
        /// Checks if ther is not curve.
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return curves.Count == 0;
        }

        /// <summary>
        /// Get closest position point.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public BezierPoint GetClosestPoint(Vector2 position)
        {
            BezierPoint closestPoint = null;

            float currentDistance = float.MaxValue;

            LinkedList<BezierPoint>.Enumerator curveEnumerator = GetEnumerator();

            while (curveEnumerator.MoveNext())
            {
                float distance = (curveEnumerator.Current.Position - position).Length;
                if (distance < currentDistance)
                {
                    currentDistance = distance;
                    closestPoint = curveEnumerator.Current;
                }
            }

            return closestPoint;
        }
        
        /// <summary>
        /// Checks for equality of path.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            if (!(obj is Path))
                return false;

            Path pathObj = obj as Path;

            return Enumerable.SequenceEqual<BezierPoint>(this.Points, pathObj.Points);
        }

        /// <summary>
        /// Clones the path.
        /// </summary>
        /// <returns></returns>
        public Path Clone()
        {
            Path newPath = new Path();

            LinkedList<BezierPoint>.Enumerator pathEnumerator = GetEnumerator();
            LinkedList<BezierPoint> newCurves = newPath.Points;
            while (pathEnumerator.MoveNext())
            {
                newCurves.AddLast(pathEnumerator.Current.Clone());
            }

            return newPath;
        }
    }
}
