using System;
namespace DiffusionCurves.OpenGL
{
    public interface IShader
    {
        void DisableAttributes();
        void DrawElements(OpenTK.Graphics.OpenGL.BeginMode mode);
        void DrawArray(OpenTK.Graphics.OpenGL.BeginMode mode);
        void EnableAttributes();
        void SetIndices(uint[] indices);
        void SetVertexPositions(OpenTK.Vector3[] vertexData);
        int ShaderProgramHandle { get; }
        void CreateProgram();
    }
}
