using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiffusionCurves.Model;
using Emgu.CV;
using Emgu.CV.Structure;
using OpenTK;

namespace DiffusionCurves.Interpolation
{
    /// <summary>
    /// Class for PyramidOpticalFlowInterpolator.
    /// </summary>
    class PyramidOpticalFlowInterpolator : Interpolator
    {
        /// <summary>
        /// An interpolater which uses the iterative Lucas-Kanade method in pyramids, where each point in the path container is treated as an image feature
        /// </summary>
        /// <param name="renderState">The render state needed to determine the image position of each BezierPoint</param>
        public PyramidOpticalFlowInterpolator(RenderState renderState)
            :base(renderState)
        {
        }

        /// <summary>
        /// Interpolates the path using the iterative Lucas-Kanade method for each point in the path as a feature
        /// </summary>
        /// <param name="previousPath">The path of the previous frame</param>
        /// <param name="nextPath">The empty path of the to be interpolated frame</param>
        /// <param name="previousImage">The previous image</param>
        /// <param name="nextImage">The next image to interpolate to</param>
        /// <returns>A value between 0 and 1 indicating the accuracy of the interpolation</returns>
        protected override double Interpolate(Model.IPathContainer previousPath, Model.IPathContainer nextPath, Image<Bgr, Byte> previousImage, Image<Bgr, Byte> nextImage)
        {
            List<PointF> featureList = new List<PointF>();
            List<Path> activePaths = new List<Path>();

            for(int i = 0; i < previousPath.LayerIndices[previousPath.ActivePathsLayer].Count; i++)
            {
                activePaths.Add(previousPath.Paths[previousPath.LayerIndices[previousPath.ActivePathsLayer][i]]);
            }

            IEnumerator<Path> pathEnumerator = activePaths.GetEnumerator();
            while (pathEnumerator.MoveNext())
            {       
                LinkedList<BezierPoint>.Enumerator pointEnumerator = pathEnumerator.Current.GetEnumerator();
                while (pointEnumerator.MoveNext())
                {
                    featureList.Add(GetImagePoint(pointEnumerator.Current));
                }
            }

            Image<Gray, Byte> prevGray = previousImage.Convert<Gray, Byte>();
            Image<Gray, Byte> nextGray = nextImage.Convert<Gray, Byte>();
            PointF[] featureArray = featureList.ToArray();
            PointF[] newFeatures;
            byte[] errors;
            float[] trackErrors;
            Emgu.CV.Structure.MCvTermCriteria criteria = new Emgu.CV.Structure.MCvTermCriteria(10);
            
            OpticalFlow.PyrLK(prevGray, nextGray, featureArray, new Size(10, 10), 5, criteria, out newFeatures, out errors, out trackErrors);

            IPathContainer tempPathContainer = previousPath.Clone();
            IEnumerator<Path> tempPathEnumerator = tempPathContainer.GetPathsEnumerator();
            int index = 0;
            for (int j = 0; j < previousPath.Count; j++)
            {
                if (previousPath.LayerIndices[previousPath.ActivePathsLayer].Contains(j))
                {
                    LinkedList<BezierPoint>.Enumerator pointEnumerator = tempPathContainer.Paths[j].GetEnumerator();
                    while (pointEnumerator.MoveNext())
                    {
                        Translate(pointEnumerator.Current, featureArray[index], newFeatures[index++]);
                    }
                }
            }            

            IEnumerator<Path> nextPathEnumerator = tempPathContainer.GetPathsEnumerator();
            while (nextPathEnumerator.MoveNext())
            {
                nextPath.AddPath(nextPathEnumerator.Current);
            }

            nextPath.ActivePathsLayer = tempPathContainer.ActivePathsLayer;
            nextPath.LayerIndices = tempPathContainer.LayerIndices;

            double error = DetermineError(previousPath, nextPath, errors);
            return error;
        }

        /// <summary>
        /// Determines the error.
        /// </summary>
        /// <param name="oldPaths"></param>
        /// <param name="newPaths"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        double DetermineError(IPathContainer oldPaths, IPathContainer newPaths, byte[] errors)
        {
            int sum = 0;
            foreach (byte b in errors)
            {
                sum += b;
            }
            if (sum == 0)
                return double.MaxValue;

            double totalError = 0;
            List<Path> oldPathsList = oldPaths.Paths;
            List<Path> newPathsList = newPaths.Paths;
            for (int i = 0; i < oldPathsList.Count; i++)
            {
                totalError += DeterminePathError(oldPathsList[i], newPathsList[i]);
            }

            return totalError;
        }

        /// <summary>
        /// Determines the path error.
        /// </summary>
        /// <param name="oldPath"></param>
        /// <param name="newPath"></param>
        /// <returns></returns>
        double DeterminePathError(Path oldPath, Path newPath)
        {
            if (oldPath.Count == 0)
                return 0;

            double[] distances = new double[oldPath.Count];
            LinkedList<BezierPoint> oldPathList = oldPath.Points;
            LinkedList<BezierPoint> newPathList = newPath.Points;
            for (int i = 0; i < oldPath.Count; i++)
            {
                distances[i] = (double)(newPathList.ElementAt(i).Position- oldPathList.ElementAt(i).Position).Length;
            }

            Array.Sort(distances);

            int size = distances.Length;
            int mid = size / 2;
            double median = (size % 2 != 0) ? (double)distances[mid] : ((double)distances[mid] + (double)distances[mid - 1]) / 2;

            double rootMeanSquared = Math.Sqrt(SquaredAbsDiff(distances, median) / distances.Length);

            return rootMeanSquared;
        }

        /// <summary>
        /// Squeares the absolute difference.
        /// </summary>
        /// <param name="distances"></param>
        /// <param name="median"></param>
        /// <returns></returns>
        double SquaredAbsDiff(double[] distances, double median)
        {
            double squaredSumDiff = 0;
            for (int i = 0; i < distances.Length; i++)
            {
                double diff = Math.Abs(distances[i] - median);
                squaredSumDiff += (diff * diff);
            }

            return squaredSumDiff;
        }

        /// <summary>
        /// Translates the bezierpoint based on the old and new bitmap positions
        /// </summary>
        /// <param name="point">The BezierPoint that needs to be translated</param>
        /// <param name="oldPos">The image position of the BezierPoint</param>
        /// <param name="newPos">The new image position retreived from the Optical Flow algorithm</param>
        void Translate(BezierPoint point, PointF oldPos, PointF newPos)
        {
            float scale = Math.Max(bitmapSize.Width / (float)renderState.FrameSize.Width, bitmapSize.Height / (float)renderState.FrameSize.Height);
            Vector2 motion = new Vector2(newPos.X, newPos.Y) - new Vector2(oldPos.X, oldPos.Y);
            motion.Y = -motion.Y;
            motion /= scale;

            point.Position += motion;
        }
    }
}
