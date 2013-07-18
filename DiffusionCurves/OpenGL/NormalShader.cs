using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace DiffusionCurves.OpenGL
{
    public class NormalShader : BasicShader
    {
        int  leftColorVBOHandle, rightColorVBOHandle;
        int projectionMatrixLocation, modelviewMatrixLocation, screenDiagonalLocation, ratioLocation;

        public NormalShader(String vertexPath, String geometryPath, String fragmentPath)
            : base(vertexPath, geometryPath, fragmentPath)
        {
        }

        protected override void QueryUniformLocations()
        {
            projectionMatrixLocation = GL.GetUniformLocation(ShaderProgramHandle, "projection_matrix");
            modelviewMatrixLocation = GL.GetUniformLocation(ShaderProgramHandle, "modelview_matrix");
            screenDiagonalLocation = GL.GetUniformLocation(ShaderProgramHandle, "screen_diagonal");
            ratioLocation = GL.GetUniformLocation(ShaderProgramHandle, "ratio");
        }

        public void SetModelviewMatrix(Matrix4 matrix)
        {
            GL.UniformMatrix4(modelviewMatrixLocation, false, ref matrix);
        }

        public void SetProjectionMatrix(Matrix4 matrix)
        {
            GL.UniformMatrix4(projectionMatrixLocation, false, ref matrix);
        }

        public void SetScreenDiagonal(int screenDiagonal)
        {
            GL.Uniform1(screenDiagonalLocation, screenDiagonal);
        }

        public void SetRatio(float ratio)
        {
            GL.Uniform1(ratioLocation, ratio);
        }

        protected override void BindAttributes(int programHandle)
        {
            base.BindAttributes(programHandle);
            GL.BindAttribLocation(programHandle, 1, "vertex_normal");
            GL.BindAttribLocation(programHandle, 2, "left_color");
            GL.BindAttribLocation(programHandle, 3, "right_color");
        }

        protected override void GenerateBuffers()
        {
            base.GenerateBuffers();
            GL.GenBuffers(1, out leftColorVBOHandle);
            GL.GenBuffers(1, out rightColorVBOHandle);
        }

        public override void EnableAttributes()
        {
            base.EnableAttributes();

            GL.BindBuffer(BufferTarget.ArrayBuffer, leftColorVBOHandle);
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 4, VertexAttribPointerType.Float, false, Vector4.SizeInBytes, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, rightColorVBOHandle);
            GL.EnableVertexAttribArray(3);
            GL.VertexAttribPointer(3, 4, VertexAttribPointerType.Float, false, Vector4.SizeInBytes, 0);            

        }

        public override void DisableAttributes()
        {
            base.DisableAttributes();

            GL.BindBuffer(BufferTarget.ArrayBuffer, leftColorVBOHandle);
            GL.DisableVertexAttribArray(2);

            GL.BindBuffer(BufferTarget.ArrayBuffer, rightColorVBOHandle);
            GL.DisableVertexAttribArray(3);
        }

        public void SetLeftColorData(Vector4[] leftColorData)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, leftColorVBOHandle);

            GL.BufferData<Vector4>(BufferTarget.ArrayBuffer,
                new IntPtr(leftColorData.Length * Vector4.SizeInBytes),
                leftColorData, BufferUsageHint.StaticDraw);
        }

        public void SetRightColorData(Vector4[] rightColorData)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, rightColorVBOHandle);

            GL.BufferData<Vector4>(BufferTarget.ArrayBuffer,
                new IntPtr(rightColorData.Length * Vector4.SizeInBytes),
                rightColorData, BufferUsageHint.StaticDraw);
        }
    }
}
