using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiffusionCurves.Model;
using OpenTK;

namespace DiffusionCurves.Diffusion
{
    /// <summary>
    /// Class for RenderStateControl.
    /// </summary>
    public class RenderStateControl : DiffusionCurves.Diffusion.IRenderStateControl
    {
        #region Private fields

        public const int BoundarySize = 10;

        RenderState renderState;

        #endregion

        /// <summary>
        /// Constructor for RenderStateControl.
        /// </summary>
        /// <param name="renderState"></param>
        public RenderStateControl(RenderState renderState)
        {
            this.renderState = renderState;
            this.renderState.PropertyChanged += renderState_PropertyChanged;
        }

        /// <summary>
        /// Handler for when property gets changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void renderState_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ZoomLevel":
                case "Offset":
                case "FrameSize":
                case "ViewSize":
                    RecalculateProjectionMatrix();
                    break;
                default:
                    break;
            }
        }
        
        /// <summary>
        /// Sets the new size of the viewport in the renderstate
        /// </summary>
        /// <param name="width">Width of the viewport, in pixels</param>
        /// <param name="height"></param>
        public void SetViewport(Size viewportSize)
        {
            renderState.ViewSize = viewportSize;
            ZoomInternal(0);
            PanInternal(new Vector2());
        }

        /// <summary>
        /// Moves the editor, scaled by the zoom level
        /// </summary>
        public void Pan(Vector2 offset)
        {
            PanInternal(offset);
            ZoomInternal(0);
        }

        private void PanInternal(Vector2 offset)
        {
            AdjustBounds(ref offset);
            renderState.Offset += offset;
        }

        /// <summary>
        /// resets the delta position to keep it within the bounds of the editor
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        void AdjustBounds(ref Vector2 offset)
        {
            Vector2 newPos = offset + renderState.Offset;
            float zoomedBoundary = BoundarySize * renderState.ZoomLevel;

            if (newPos.X - renderState.FrameSize.Width / 2 > zoomedBoundary)
            {
                offset.X = zoomedBoundary - renderState.Offset.X + renderState.FrameSize.Width / 2;
            }
            else if (newPos.X + renderState.FrameSize.Width / 2 < -zoomedBoundary)
            {
                offset.X = -zoomedBoundary - renderState.Offset.X - renderState.FrameSize.Width / 2;
            }

            if (newPos.Y - renderState.FrameSize.Height / 2 > zoomedBoundary)
            {
                offset.Y = zoomedBoundary - renderState.Offset.Y + renderState.FrameSize.Height / 2;
            }
            else if (newPos.Y + renderState.FrameSize.Height / 2 < -zoomedBoundary)
            {
                offset.Y = -zoomedBoundary - renderState.Offset.Y - renderState.FrameSize.Height / 2;
            }            
        }

        /// <summary>
        /// Zooms in by a certain scale, based on the amount of steps defined and the maximum and minimum zoom size
        /// </summary>
        /// <param name="zoomDelta">The number of steps the zoom level increases</param>
        public void Zoom(int zoomDelta)
        {
            ZoomInternal(zoomDelta);
            PanInternal(new Vector2());
        }

        void ZoomInternal(int zoomDelta)
        {
            int zoomMargin = 200;
            float minZoom = Math.Min(1f, Math.Min(renderState.ViewSize.Width / (float)(renderState.FrameSize.Width + zoomMargin), renderState.ViewSize.Height / (float)(renderState.FrameSize.Height + zoomMargin)));
            float maxZoom = 5;

            renderState.ZoomLevel += (zoomDelta / (float)renderState.ZoomSteps * (maxZoom - minZoom));
            if (renderState.ZoomLevel < minZoom)
                renderState.ZoomLevel = minZoom;
            else if (renderState.ZoomLevel > maxZoom)
                renderState.ZoomLevel = maxZoom;
        }

        /// <summary>
        /// Recalculates the projection matrix based on the render state.
        /// </summary>
        private void RecalculateProjectionMatrix()
        {
            int viewWidthOffset = (int)(renderState.ViewSize.Width / (2f * renderState.ZoomLevel));
            int viewHeightOffset = (int)(renderState.ViewSize.Height / (2f * renderState.ZoomLevel));

            int width = renderState.ViewSize.Width;
            int height = renderState.ViewSize.Height;
            int screenDiagonal = (int)(Math.Sqrt(width * width + height * height) * 1 / renderState.ZoomLevel);

            renderState.ProjectionMatrix = Matrix4.CreateOrthographicOffCenter(
                renderState.Offset.X - viewWidthOffset,
                renderState.Offset.X + viewWidthOffset,
                renderState.Offset.Y - viewHeightOffset,
                renderState.Offset.Y + viewHeightOffset, 
                -2,
                screenDiagonal * 6f);
        }

        /// <summary>
        /// Converts a screenspace vector position to a position in worldspace, to make it scale- and pan-invariant.
        /// </summary>
        /// <param name="screenPosition">A position, in screenspace</param>
        /// <returns>A new position, in worldspace</returns>
        public Vector2 ToWorldSpace(Vector2 screenPosition)
        {
            int viewWidthOffset = (int)(renderState.ViewSize.Width / (2f * renderState.ZoomLevel));
            int viewHeightOffset = (int)(renderState.ViewSize.Height / (2f * renderState.ZoomLevel));
            return new Vector2(renderState.Offset.X - viewWidthOffset + screenPosition.X / renderState.ZoomLevel,
                renderState.Offset.Y - viewHeightOffset + screenPosition.Y / renderState.ZoomLevel);
        }

        /// <summary>
        /// Resets the renderstate
        /// </summary>
        /// <param name="frameWidth">Width of the frame, in pixels</param>
        /// <param name="frameHeight">Height of the frame, in pixels</param>
        public void Reset()
        {
            renderState.Offset = new Vector2();
            renderState.ZoomLevel = 1;
        }

        /// <summary>
        /// Sets the size of the project's frame.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetFrameSize(Size frameSize)
        {
            renderState.FrameSize = frameSize;
        }
    }
}
