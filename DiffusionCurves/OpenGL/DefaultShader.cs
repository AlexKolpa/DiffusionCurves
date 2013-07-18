using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace DiffusionCurves.OpenGL
{
    public class DefaultShader : BasicShader
    {
        int projectionMatrixLocation, modelviewMatrixLocation, colorLocation;

        public DefaultShader(string vertexShaderPath, string fragmentShaderPath)
            : base(vertexShaderPath, null, fragmentShaderPath)
        {
        }

        protected override void QueryUniformLocations()
        {
            projectionMatrixLocation = GL.GetUniformLocation(ShaderProgramHandle, "projection_matrix");
            modelviewMatrixLocation = GL.GetUniformLocation(ShaderProgramHandle, "modelview_matrix");
            colorLocation = GL.GetUniformLocation(ShaderProgramHandle, "pixel_color");
        }

        public void SetVertexColor(Color color)
        {
            GL.Uniform4(colorLocation, color);
        }

        public void SetModelviewMatrix(Matrix4 matrix)
        {
            GL.UniformMatrix4(modelviewMatrixLocation, false, ref matrix);
        }

        public void SetProjectionMatrix(Matrix4 matrix)
        {
            GL.UniformMatrix4(projectionMatrixLocation, false, ref matrix);
        }
    }
}
