using System;
using System.ComponentModel;
using System.Drawing;
using DiffusionCurves.Model;
namespace DiffusionCurves.Diffusion
{
    /// <summary>
    /// Interface for DiffusionRenderer.
    /// </summary>
    public interface IDiffusionRenderer
    {
        event PropertyChangedEventHandler PropertyChanged;
        int Iterations { get; set; }
        bool DisplayDiffusion { get; set; }
        DiffusionBuffers Buffers { get; set; }
        RenderState RenderState { get; set; }
        void Draw();
        void glControl_Load(object sender, EventArgs e);
        void SetFrame(System.Drawing.Bitmap bitmap, IPathContainer container);
        void SetViewport(Size size);        
        int DrawDiffusion();
    }
}
