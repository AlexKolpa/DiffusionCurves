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
    /// Abstract class for Interpolator.
    /// </summary>
    public abstract class Interpolator
    {
        #region Private fields

        protected RenderState renderState;
        protected Size bitmapSize = Size.Empty;

        #endregion

        /// <summary>
        /// Constructor for Interpolation.
        /// </summary>
        /// <param name="renderState"></param>
        public Interpolator(RenderState renderState)
        {
            this.renderState = renderState;
        }

        /// <summary>
        /// Gets the position of the image.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        protected Vector2 GetImagePosition(BezierPoint point)
        {
            float scale = Math.Max(bitmapSize.Width / (float)renderState.FrameSize.Width, bitmapSize.Height / (float)renderState.FrameSize.Height);
            int imageXOffset = (int)(bitmapSize.Width / (2 * scale));
            int imageYOffset = (int)(bitmapSize.Height / (2 * scale));
            Vector2 offset = new Vector2(imageXOffset, imageYOffset);

            Vector2 scaledPosition = (point.Position + offset) * scale;
            scaledPosition.Y = bitmapSize.Height - scaledPosition.Y;

            return scaledPosition;
        }

        /// <summary>
        /// Gets the point of the image.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        protected Point GetImagePoint(BezierPoint point)
        {
            Vector2 pos = GetImagePosition(point);
            return new Point((int)pos.X, (int)pos.Y);
        }

        /// <summary>
        /// Checks if it is inside the bitmap.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        protected bool IsInsideBitmap(Vector2 position)
        {
            return IsInsideBitmap(new Point((int)position.X, (int)position.Y));
        }

        /// <summary>
        /// Checks if it is inside the bitmap.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        protected bool IsInsideBitmap(Point position)
        {
            return position.X >= 0 && 
                position.Y >= 0 &&
                position.X < bitmapSize.Width &&
                position.Y < bitmapSize.Height;
        }

        /// <summary>
        /// Interpolates the paths.
        /// </summary>
        /// <param name="previousPath"></param>
        /// <param name="nextPath"></param>
        /// <param name="previousImage"></param>
        /// <param name="nextImage"></param>
        /// <returns></returns>
        public double InterpolatePaths(IPathContainer previousPath, IPathContainer nextPath, Image<Bgr, Byte> previousImage, Image<Bgr, Byte> nextImage)
        {
            bitmapSize = previousImage.Size;

            return Interpolate(previousPath, nextPath, previousImage, nextImage);
        }

        /// <summary>
        /// Interpolates on give paths.
        /// </summary>
        /// <param name="previousPath"></param>
        /// <param name="nextPath"></param>
        /// <param name="previousImage"></param>
        /// <param name="nextImage"></param>
        /// <returns></returns>
        protected abstract double Interpolate(IPathContainer previousPath, IPathContainer nextPath, Image<Bgr, Byte> previousImage, Image<Bgr, Byte> nextImage);
        
    }
}
