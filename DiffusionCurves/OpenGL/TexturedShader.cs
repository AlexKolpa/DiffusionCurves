using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace DiffusionCurves.OpenGL
{
    public class TexturedShader : BasicShader
    {
        int texCoordHandle;
        int projectionMatrixLocation, modelviewMatrixLocation;

        public TexturedShader(String vertexPath, String fragmentPath)
            : base(vertexPath, null, fragmentPath)
        {
        }
        
        public void BindTexture(ref int textureId, TextureUnit textureUnit, string UniformName)
        {
            GL.ActiveTexture(textureUnit);
            GL.BindTexture(TextureTarget.Texture2D, textureId);
            GL.Uniform1(GL.GetUniformLocation(ShaderProgramHandle, UniformName), textureUnit - TextureUnit.Texture0);
        }

        protected override void QueryUniformLocations()
        {
            projectionMatrixLocation = GL.GetUniformLocation(ShaderProgramHandle, "projection_matrix");
            modelviewMatrixLocation = GL.GetUniformLocation(ShaderProgramHandle, "modelview_matrix");            
        }

        public void SetModelviewMatrix(Matrix4 matrix)
        {
            GL.UniformMatrix4(modelviewMatrixLocation, false, ref matrix);
        }

        public void SetProjectionMatrix(Matrix4 matrix)
        {
            GL.UniformMatrix4(projectionMatrixLocation, false, ref matrix);
        }

        protected override void BindAttributes(int programHandle)
        {
            base.BindAttributes(programHandle);
            GL.BindAttribLocation(programHandle, 1, "tex_coord");
        }

        protected override void GenerateBuffers()
        {
            base.GenerateBuffers();
            GL.GenBuffers(1, out texCoordHandle);
        }

        public override void EnableAttributes()
        {
            base.EnableAttributes();
            GL.BindBuffer(BufferTarget.ArrayBuffer, texCoordHandle);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);            
        }

        public override void DisableAttributes()
        {
            base.DisableAttributes();
            GL.BindBuffer(BufferTarget.ArrayBuffer, texCoordHandle);
            GL.DisableVertexAttribArray(1);            
        }

        public void SetTextureCoordinates(Vector2[] texCoords)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, texCoordHandle);

            GL.BufferData<Vector2>(BufferTarget.ArrayBuffer,
                new IntPtr(texCoords.Length * Vector2.SizeInBytes),
                texCoords, BufferUsageHint.StaticDraw);
        }
    }
}
