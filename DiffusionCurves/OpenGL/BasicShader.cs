using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace DiffusionCurves.OpenGL
{
    public abstract class BasicShader : IShader
    {   
        int shaderProgramHandle;
                
        /// <summary>
        /// The handle referring to the OpenGL shader program object.
        /// </summary>
        public int ShaderProgramHandle
        {
            get { return shaderProgramHandle; }
        }

        int vertexPositionVBOHandle, indicesVBOHandle;
        int indicesLength, verticesLength;

        String vertexPath, geometryPath, fragmentPath;

        /// <summary>
        /// The constructor for a default shader.
        /// </summary>
        /// <param name="vertexPath">Path to the vertex shader source</param>
        /// <param name="geometryPath">Path to the geometry shader source, can be null</param>
        /// <param name="fragmentPath">Path to the fragment shader source</param>
        public BasicShader(String vertexPath, String geometryPath, String fragmentPath)
        {
            this.vertexPath = vertexPath;
            this.geometryPath = geometryPath;
            this.fragmentPath = fragmentPath;
        }
        
        /// <summary>
        /// Constructs program. Should only be called when OpenGL has been loaded.
        /// </summary>
        public void CreateProgram()
        {
            int vertexShaderHandle = CreateShader(ShaderType.VertexShader, vertexPath);
            int geometryShaderHandle = -1;
            if (geometryPath != null)
                geometryShaderHandle = CreateShader(ShaderType.GeometryShaderExt, geometryPath);
            int fragmentShaderHandle = CreateShader(ShaderType.FragmentShader, fragmentPath);

            int[] shaders = new int[geometryShaderHandle == -1 ? 2 : 3];
            shaders[0] = vertexShaderHandle;
            shaders[1] = fragmentShaderHandle;

            if (shaders.Length == 3)
            {
                shaders[1] = geometryShaderHandle;
                shaders[2] = fragmentShaderHandle;
            }
            shaderProgramHandle = CreateProgram(shaders);

            QueryUniformLocations();
            GenerateBuffers();      
        }

        /// <summary>
        /// Constructs a single shader object.
        /// </summary>
        /// <param name="type">The type of the shader object</param>
        /// <param name="shaderLocation">The path where the shader source is stored</param>
        /// <returns></returns>
        int CreateShader(ShaderType type, String shaderLocation)
        {
            int handle = GL.CreateShader(type);
            String source;
            LoadShaderSource(shaderLocation, out source);
            GL.ShaderSource(handle, source);
            GL.CompileShader(handle);
            return handle;
        }

        /// <summary>
        /// Loads the source from the path.
        /// </summary>
        /// <param name="location">Path pointing to the source of the shader</param>
        /// <param name="output">The shader source as output</param>
        private void LoadShaderSource(string location, out string output)
        {
            StreamReader reader = new StreamReader(location);
            output = reader.ReadToEnd();
            reader.Close();
        }

        /// <summary>
        /// Creates the shader program object with the shader objects as input
        /// </summary>
        /// <param name="handles">Array of integer containing the handles of the OpenGL shader objects</param>
        /// <returns></returns>
        int CreateProgram(int[] handles)
        {
            int programHandle = GL.CreateProgram();

            foreach (int handle in handles)
                GL.AttachShader(programHandle, handle);

            BindAttributes(programHandle);

            GL.LinkProgram(programHandle);

            Debug.WriteLine(GL.GetProgramInfoLog(programHandle));

            foreach (int handle in handles)
                GL.DetachShader(programHandle, handle);

            return programHandle;
        }

        /// <summary>
        /// Querying the uniforms in the shader program should be done in this function
        /// </summary>
        protected abstract void QueryUniformLocations();

        /// <summary>
        /// Bind the default vertex attributes to an index
        /// </summary>
        /// <param name="programHandle">the program handle of the to be linked shader program object</param>
        protected virtual void BindAttributes(int programHandle)
        {
            GL.BindAttribLocation(programHandle, 0, "vertex_position");
        }

        /// <summary>
        /// Disable the vertex attributes
        /// </summary>
        public virtual void DisableAttributes()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexPositionVBOHandle);
            GL.DisableVertexAttribArray(0);
        }

        /// <summary>
        /// Generate the buffers for the vertex attributes
        /// </summary>
        protected virtual void GenerateBuffers()
        {
            GL.GenBuffers(1, out vertexPositionVBOHandle);
            GL.GenBuffers(1, out indicesVBOHandle);
        }

        /// <summary>
        /// Enable the vertex attributes
        /// </summary>
        public virtual void EnableAttributes()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexPositionVBOHandle);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indicesVBOHandle);
        }

        /// <summary>
        /// Draws the elements stored in the Vertex Buffer Objects (VBO)
        /// </summary>
        /// <param name="mode">The mode by which these vertices are drawn</param>
        public virtual void DrawElements(OpenTK.Graphics.OpenGL.BeginMode mode)
        {            
            GL.DrawElements(mode, indicesLength,
                DrawElementsType.UnsignedInt, IntPtr.Zero);
        }

        /// <summary>
        /// Draws the elements in the VBO without the use of the index buffer
        /// </summary>
        /// <param name="mode">The mode by which these vertices are drawn</param>
        public virtual void DrawArray(OpenTK.Graphics.OpenGL.BeginMode mode)
        {
            GL.DrawArrays(mode, 0, verticesLength);
        }

        /// <summary>
        /// Sets the indices of the  Vertex Buffer Object
        /// </summary>
        /// <param name="indices">an array of indices pointing to the corresponding vertices</param>
        public void SetIndices(uint[] indices)
        {
            indicesLength = indices.Length;

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indicesVBOHandle);
            GL.BufferData<uint>(BufferTarget.ElementArrayBuffer,
                new IntPtr(indices.Length * sizeof(uint)),
                indices, BufferUsageHint.StaticDraw);
        }

        /// <summary>
        /// Sets the positions of the Vertex Buffer Object
        /// </summary>
        /// <param name="vertexData">an array of position of the vertices</param>
        public void SetVertexPositions(OpenTK.Vector3[] vertexData)
        {
            verticesLength = vertexData.Length;

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexPositionVBOHandle);

            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer,
                new IntPtr(vertexData.Length * Vector3.SizeInBytes),
                vertexData, BufferUsageHint.StaticDraw);
        }
    }
}
