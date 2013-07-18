using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace DiffusionCurves
{
    class BezierCurveShader
    {
        int indicesLength = 0;

        int modelviewMatrixLocation,
            projectionMatrixLocation,
            colorLocation,            
            positionVboHandle,            
            indicesVboHandle;

        int shaderProgramHandle;

        public int ShaderProgramHandle
        {
            get { return shaderProgramHandle; }
        }       

        Matrix4 projectionMatrix, modelviewMatrix;

        public BezierCurveShader(
            String vertexShaderLocation, 
            String fragmentShaderLocation)
        {
            int vertexShaderHandle = CreateShader(ShaderType.VertexShader, vertexShaderLocation);
            int fragmentShaderHandle = CreateShader(ShaderType.FragmentShader, fragmentShaderLocation);

            shaderProgramHandle = CreateProgram(
                new int[] { vertexShaderHandle, fragmentShaderHandle });

            QueryUniformLocations();
            GenerateBuffers();            
            LoadAttributes();
        }

        private void GenerateBuffers()
        {
            GL.GenBuffers(1, out positionVboHandle);
            GL.GenBuffers(1, out indicesVboHandle);
        }

        void BindAttributes(int handle)
        {
            GL.BindAttribLocation(handle, 0, "vertex_position");        
        }

        void LoadAttributes()
        {            
            GL.BindBuffer(BufferTarget.ArrayBuffer, positionVboHandle);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);   
        }

        int CreateShader(ShaderType type, String shaderLocation)
        {
            int handle = GL.CreateShader(type);
            String source;
            LoadShaderSource(shaderLocation, out source);
            GL.ShaderSource(handle, source);
            GL.CompileShader(handle);
            return handle;
        }

        private void LoadShaderSource(string location, out string output)
        {
            StreamReader reader = new StreamReader(location);
            output = reader.ReadToEnd();
            reader.Close();
        }

        int CreateProgram(int[] handles)
        {
            int programHandle = GL.CreateProgram();

            BindAttributes(programHandle);

            foreach(int handle in handles)
                GL.AttachShader(programHandle, handle);
            
            GL.LinkProgram(programHandle);            

            string programInfoLog;
            GL.GetProgramInfoLog(shaderProgramHandle, out programInfoLog);
            Debug.WriteLine(programInfoLog);

            foreach (int handle in handles)
                GL.DetachShader(programHandle, handle);

            return programHandle;
        }

        private void QueryUniformLocations()
        {
            projectionMatrixLocation = GL.GetUniformLocation(shaderProgramHandle, "projection_matrix");
            modelviewMatrixLocation = GL.GetUniformLocation(shaderProgramHandle, "modelview_matrix");
            colorLocation = GL.GetUniformLocation(shaderProgramHandle, "color");
        }

        public void SetModelviewMatrix(Matrix4 matrix)
        {
            modelviewMatrix = matrix;
            GL.UniformMatrix4(modelviewMatrixLocation, false, ref modelviewMatrix);
        }

        public void SetProjectionMatrix(Matrix4 matrix)
        {
            projectionMatrix = matrix;
            GL.UniformMatrix4(projectionMatrixLocation, false, ref projectionMatrix);
        }

        public void SetColor(Color color)
        {
            GL.Uniform4(colorLocation, color);
        }

        public void Draw(BeginMode mode)
        {
            if (indicesLength < 2)
                return;

            GL.DrawElements(mode, indicesLength,
                DrawElementsType.UnsignedInt, IntPtr.Zero);
        }

        public void SetVertexPositions(Vector3[] vertexData)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, positionVboHandle);

            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer,
                new IntPtr(vertexData.Length * Vector3.SizeInBytes),
                vertexData, BufferUsageHint.StaticDraw);
        }

        public void SetIndices(uint[] indices)
        {            
            indicesLength = indices.Length;
            
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indicesVboHandle);
            GL.BufferData<uint>(BufferTarget.ElementArrayBuffer,
                new IntPtr(indices.Length * Vector3.SizeInBytes),
                indices, BufferUsageHint.StaticDraw);
        }
    }
}
