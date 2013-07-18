using System;
using DiffusionCurves.Model;
namespace DiffusionCurves.Diffusion
{
    /// <summary>
    /// Interface for EditorStateControl.
    /// </summary>
    public interface IEditorStateControl
    {
        void DeleteSelectedPoint();
        bool DragStateItem(OpenTK.Vector2 newPosition);
        void SetIdle();
        void UpdateState(OpenTK.Vector2 position);
        void SetPathsContainer(IPathContainer container);
    }
}
