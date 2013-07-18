using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiffusionCurves.Model;
using OpenTK;

namespace DiffusionCurves.Diffusion
{
    /// <summary>
    /// Class for DiffusionPathControl.
    /// </summary>
    public class DiffusionPathControl : DiffusionCurves.Diffusion.IDiffusionPathControl
    {
        #region Private fields

        int lod = 10;

        public int LOD
        {
            get { return lod; }
            set { lod = value; }
        }

        IPathContainer container;

        DiffusionBuffers buffers;

        #endregion

        /// <summary>
        /// Control for diffusion path.
        /// </summary>
        /// <param name="buffers"></param>
        public DiffusionPathControl(DiffusionBuffers buffers)
        {
            this.buffers = buffers;
        }

        /// <summary>
        /// Rebuild curves.
        /// </summary>
        public void Rebuild()
        {
            buffers.Clear();

            if (container != null)
            {
                ConstructInterpolatedPaths();
                ConstructPathPoints();
            }
        }

        /// <summary>
        /// Sets container.
        /// </summary>
        /// <param name="container"></param>
        public void SetContainer(IPathContainer container)
        {
            this.container = container;
            Rebuild();
        }

        /// <summary>
        /// Construct paths on interpolated points.
        /// </summary>
        void ConstructInterpolatedPaths()
        {
            IEnumerator<Path> pathsEnumerator = container.GetPathsEnumerator();
            while (pathsEnumerator.MoveNext())
            {
                if (pathsEnumerator.Current.Count < 2)
                    continue;

                Vector3[] positionArray;
                Vector4[] leftColorArray;
                Vector4[] rightColorArray;
                uint[] indices;

                ConstructInterpolatedPathPoints(pathsEnumerator.Current, out indices, out positionArray, out leftColorArray, out rightColorArray);

                buffers.ListPathPoints.Add(positionArray);
                buffers.ListLeftColorPoints.Add(leftColorArray);
                buffers.ListRightColorPoints.Add(rightColorArray);
                buffers.ListIndices.Add(indices);
            }
        }

        /// <summary>
        /// Construct a single path based on bezier interpolation.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="indices"></param>
        /// <param name="positions"></param>
        /// <param name="leftColors"></param>
        /// <param name="rightColors"></param>
        void ConstructInterpolatedPathPoints(Path path, out uint[] indices, out Vector3[] positions, out Vector4[] leftColors, out Vector4[] rightColors)
        {
            int pointCount = (path.Count - 1) * lod + 3;

            positions = new Vector3[pointCount];
            leftColors = new Vector4[pointCount];
            rightColors = new Vector4[pointCount];
            indices = new uint[pointCount];

            LinkedList<BezierPoint> pathPoints = path.Points;
            LinkedListNode<BezierPoint> currentNode = pathPoints.First;

            uint index = 0;
            BezierPoint current = currentNode.Value;
            BezierPoint next = currentNode.Next.Value;

            Vector2 pos = BezierCurve(current.Position, current.Control2, next.Position, next.Control1, 1 / (float)lod);
            positions[index] = new Vector3(current.Position - pos + current.Position);
            leftColors[index] = current.LeftColor;
            rightColors[index] = current.RightColor;
            indices[index] = index;
            index++;

            for (int j = 0; j < pathPoints.Count - 1; j++)
            {
                current = currentNode.Value;
                next = currentNode.Next.Value;
                for (int i = 0; i < lod; i++)
                {
                    float mu = i / (float)lod;
                    positions[index] = new Vector3(BezierCurve(current.Position, current.Control2, next.Position, next.Control1, mu));
                    leftColors[index] = Vector4.Lerp(current.LeftColor, next.LeftColor, mu);
                    rightColors[index] = Vector4.Lerp(current.RightColor, next.RightColor, mu);

                    indices[index] = index;

                    index++;
                }

                currentNode = currentNode.Next;
            }

            current = currentNode.Previous.Value;
            next = currentNode.Value;

            positions[index] = new Vector3(BezierCurve(current.Position, current.Control2, next.Position, next.Control1, 1f));
            leftColors[index] = Vector4.Lerp(current.LeftColor, next.LeftColor, 1f);
            rightColors[index] = Vector4.Lerp(current.RightColor, next.RightColor, 1f);
            indices[index] = index;
            index++;

            pos = BezierCurve(current.Position, current.Control2, next.Position, next.Control1, 1 - 1 / (float)lod);
            positions[index] = new Vector3(next.Position - pos + next.Position);
            leftColors[index] = current.LeftColor;
            rightColors[index] = current.RightColor;
            indices[index] = index;
            index++;
        }

        /// <summary>
        /// Construct paths on created points.
        /// </summary>
        void ConstructPathPoints()
        {
            int totalSize = container.TotalPointCount;
            Vector3[] pathPoints = new Vector3[totalSize];

            int index = 0;
            IEnumerator<Path> enumerator = container.GetPathsEnumerator();
            while (enumerator.MoveNext())
            {
                LinkedList<BezierPoint>.Enumerator pointEnumerator = enumerator.Current.GetEnumerator();
                while (pointEnumerator.MoveNext())
                {
                    pathPoints[index++] = new Vector3(pointEnumerator.Current.Position);
                }
            }

            buffers.PathPoints = pathPoints;
        }

        /// <summary>
        /// Calculates the bezier curve.
        /// </summary>
        /// <param name="p1">first position</param>
        /// <param name="c1">first control, relative to its position</param>
        /// <param name="p2">second position</param>
        /// <param name="c2">second control, relative to its position</param>
        /// <param name="mu">ratio between 0.0 and 1.0</param>
        /// <returns></returns>
        Vector2 BezierCurve(Vector2 p1, Vector2 c1, Vector2 p2, Vector2 c2, float mu)
        {
            return 3 * mu * (1 - mu) * (1 - mu) * (c1 + p1) + 3 * mu * mu * (1 - mu) * (c2 + p2)
                + (1 - mu) * (1 - mu) * (1 - mu) * p1 + mu * mu * mu * p2;
        }
    }
}
