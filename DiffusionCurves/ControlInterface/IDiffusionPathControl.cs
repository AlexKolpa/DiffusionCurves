using System;
namespace DiffusionCurves.Diffusion
{
    /// <summary>
    /// Interface for DiffusionPathControl.
    /// </summary>
    public interface IDiffusionPathControl
    {
        int LOD { get; set; }
        void Rebuild();
        void SetContainer(DiffusionCurves.Model.IPathContainer container);
    }
}
