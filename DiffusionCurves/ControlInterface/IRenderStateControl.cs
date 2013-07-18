using System;
using System.Drawing;
using OpenTK;
namespace DiffusionCurves.Diffusion
{
    /// <summary>
    /// Interface for RenderStateControl.
    /// </summary>
    public interface IRenderStateControl
    {
        void Pan(Vector2 offset);
        void Reset();
        void SetFrameSize(Size frameSize);
        void SetViewport(Size viewportSize);
        void Zoom(int zoomDelta);
        OpenTK.Vector2 ToWorldSpace(OpenTK.Vector2 screenPosition);
    }
}
