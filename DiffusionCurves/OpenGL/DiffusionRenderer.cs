using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using DiffusionCurves.Diffusion;
using DiffusionCurves.Model;
using DiffusionCurves.Views;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace DiffusionCurves.OpenGL
{
    /// <summary>
    /// DiffusionRenderer manages all OpenGL calls and draws everything 
    /// to the given viewport, which is defined in the GLControl.
    /// </summary>
    public class DiffusionRenderer : DiffusionCurves.Diffusion.IDiffusionRenderer
    {
        public event PropertyChangedEventHandler PropertyChanged;

        IPathContainer container;

        int iterations = 20;

        /// <summary>
        /// The number of iterations for the diffusion.
        /// </summary>
        public int Iterations
        {
            get { return iterations; }
            set
            {
                if (iterations != value)
                {
                    iterations = value;
                    NotifyPropertyChanged();
                }
            }
        }

        bool drawDiffusion = true;

        /// <summary>
        /// Disables diffusion altogether
        /// </summary>
        public bool DisplayDiffusion
        {
            get { return drawDiffusion; }
            set
            {
                if (drawDiffusion != value)
                {
                    drawDiffusion = value;
                    NotifyPropertyChanged();
                }
            }
        }

        int bitmapWidth, bitmapHeight;

        System.Drawing.Color controlLineColor = System.Drawing.Color.Black;

        float controlLineWidth = 1f;

        EditorState currentState;
        RenderState renderState;
        DiffusionBuffers buffers;

        public DiffusionBuffers Buffers
        {
            get { return buffers; }
            set { buffers = value; }
        }

        public RenderState RenderState
        {
            get { return renderState; }
            set { 
                renderState = value;
                renderState_PropertyChanged(this, new PropertyChangedEventArgs("ProjectionMatrix"));
                renderState_PropertyChanged(this, new PropertyChangedEventArgs("ViewSize"));
            }
        }

        int backgroundTextureID;

        int FBOHandle;
        int colorTexturePing, colorTexturePong, depthTexture;

        DefaultShader pointShader;
        TexturedShader imageShader;
        TexturedShader diffuseShader;
        NormalShader lineShader;
        NormalShader endPointShader;

        bool loaded = false;
        Bitmap imageToLoad = null;

        /// <summary>
        /// Constructs the DiffusionRenderer to draw the editor's current frame. DiffusionRenderer
        /// can only start drawing when OpenGL has finished loading.
        /// </summary>
        /// <param name="editorState">Used as a reference to draw the currently selected point</param>
        /// <param name="pathsContainer">Used as a reference to draw all the paths in the current frame</param>
        public DiffusionRenderer(RenderState renderState, 
            EditorState currentState, DiffusionBuffers buffers,
            DefaultShader pointShader, TexturedShader imageShader, TexturedShader diffuseShader, 
            NormalShader lineShader, NormalShader endPointShader)
        {
            this.renderState = renderState;            
            this.currentState = currentState;
            this.buffers = buffers;
            this.pointShader = pointShader;
            this.imageShader = imageShader;
            this.diffuseShader = diffuseShader;
            this.lineShader = lineShader;
            this.endPointShader = endPointShader;

            this.renderState.PropertyChanged += renderState_PropertyChanged;
        }

        #region events
        void renderState_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ProjectionMatrix":
                    GL.MatrixMode(MatrixMode.Projection);
                    Matrix4 projectionMatrix = renderState.ProjectionMatrix;
                    GL.LoadMatrix(ref projectionMatrix);

                    GL.UseProgram(pointShader.ShaderProgramHandle);
                    pointShader.SetProjectionMatrix(projectionMatrix);
                    pointShader.SetModelviewMatrix(Matrix4.Identity);

                    GL.UseProgram(imageShader.ShaderProgramHandle);
                    imageShader.SetProjectionMatrix(projectionMatrix);
                    imageShader.SetModelviewMatrix(Matrix4.Identity);

                    GL.UseProgram(lineShader.ShaderProgramHandle);
                    lineShader.SetProjectionMatrix(projectionMatrix);
                    lineShader.SetModelviewMatrix(Matrix4.Identity);

                    GL.UseProgram(endPointShader.ShaderProgramHandle);
                    endPointShader.SetProjectionMatrix(projectionMatrix);
                    endPointShader.SetModelviewMatrix(Matrix4.Identity);
                    break;
                case "ViewSize":
                    SetViewport(renderState.ViewSize);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Event for setting several constants when OpenGL finishes loading
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void glControl_Load(object sender, EventArgs e)
        {
            loaded = true;

            GL.ClearColor(OpenTK.Graphics.Color4.Gray);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Texture2D);

            pointShader.CreateProgram();
            imageShader.CreateProgram();
            lineShader.CreateProgram();
            endPointShader.CreateProgram();
            diffuseShader.CreateProgram();

            GL.UseProgram(diffuseShader.ShaderProgramHandle);
            diffuseShader.SetProjectionMatrix(Matrix4.Identity);
            diffuseShader.SetModelviewMatrix(Matrix4.Identity);

            GenerateFrameBuffer();

            if (imageToLoad != null)
                SetBitmap(imageToLoad);
            imageToLoad = null;
        }
        #endregion

        /// <summary>
        /// Generates the frame buffer for the diffusion
        /// </summary>
        void GenerateFrameBuffer()
        {
            GL.GenFramebuffers(1, out FBOHandle);
            GenerateFrameTextures();
        }

        /// <summary>
        /// Generates the textures that will be attached to the frame buffer
        /// </summary>
        void GenerateFrameTextures()
        {
            GenerateTexture(out colorTexturePing, PixelInternalFormat.Rgba8, OpenTK.Graphics.OpenGL.PixelFormat.Rgba);
            GenerateTexture(out colorTexturePong, PixelInternalFormat.Rgba8, OpenTK.Graphics.OpenGL.PixelFormat.Rgba);
            GenerateTexture(out depthTexture, PixelInternalFormat.DepthComponent, OpenTK.Graphics.OpenGL.PixelFormat.DepthComponent);    
            
        }

        /// <summary>
        /// Generates a single texture for the frame buffer
        /// </summary>
        /// <param name="textureID">handle to the OpenGL texture object</param>
        /// <param name="internalFormat">Format and size of the internal bit storage</param>
        /// <param name="format">Format of the internal storage</param>
        void GenerateTexture(out int textureID, PixelInternalFormat internalFormat, OpenTK.Graphics.OpenGL.PixelFormat format)
        {
            GL.GenTextures(1, out textureID);
            GL.BindTexture(TextureTarget.Texture2D, textureID);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
            GL.TexImage2D(TextureTarget.Texture2D, 0, internalFormat, renderState.ViewSize.Width, renderState.ViewSize.Height, 0, format, PixelType.UnsignedByte, IntPtr.Zero);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
        
        /// <summary>
        /// Sets an orthographic projection with the given width and height as the viewport
        /// </summary>
        public void SetViewport(Size size)
        {
            GL.Viewport(0, 0, size.Width, size.Height);

            GL.DeleteTexture(colorTexturePing);
            GL.DeleteTexture(colorTexturePong);
            GL.DeleteTexture(depthTexture);

            GenerateFrameTextures();
        }

        /// <summary>
        /// The main draw call to draw the current frame
        /// </summary>
        public void Draw()
        {
            GL.ClearColor(Color.Gray);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            GL.Disable(EnableCap.DepthTest);
            DrawBackground();
            DrawImage();

            if (drawDiffusion)
            {
                GL.Enable(EnableCap.DepthTest);
                int tex = DrawDiffusion();
                if (GL.IsTexture(tex))
                {
                    GL.UseProgram(imageShader.ShaderProgramHandle);
                    imageShader.SetProjectionMatrix(Matrix4.Identity);
                    imageShader.EnableAttributes();
                    imageShader.BindTexture(ref tex, TextureUnit.Texture0, "background");

                    DrawTexturedQuad(-1, 1, 1, -1, imageShader);
                    imageShader.DisableAttributes();
                    imageShader.SetProjectionMatrix(renderState.ProjectionMatrix);
                }
                GL.Disable(EnableCap.DepthTest);
            }

            DrawBezierLines();
            DrawControlLines();
            DrawPoints();            
            GL.Enable(EnableCap.DepthTest);

            if (GL.GetError() != ErrorCode.NoError)
                Debug.WriteLine(GL.GetError().ToString());
        }

        /// <summary>
        /// Draws the actual diffusion to the buffer, and overlays a full screen texture on the viewport.
        /// </summary>
        public int DrawDiffusion()
        {
            if (buffers.ListPathPoints.Count == 0)
                return -1;

            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, FBOHandle);
            GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext, TextureTarget.Texture2D, colorTexturePing, 0);
            GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachmentExt, TextureTarget.Texture2D, depthTexture, 0);

            CheckFrameBufferStatus();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            DrawBezierCurves();
            DrawEndPoints();

            GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachmentExt, TextureTarget.Texture2D, 0, 0);

            int currentColorTexture = colorTexturePing;
            int currentColorBuffer = colorTexturePong;

            GL.UseProgram(diffuseShader.ShaderProgramHandle);
            diffuseShader.EnableAttributes();
            diffuseShader.BindTexture(ref depthTexture, TextureUnit.Texture1, "depth_texture");
            int width = renderState.ViewSize.Width;
            int height = renderState.ViewSize.Height;
            float scale = (float)Math.Sqrt(width * width + height * height);
            GL.Uniform1(GL.GetUniformLocation(diffuseShader.ShaderProgramHandle, "xScale"), (scale-1f) / width);
            GL.Uniform1(GL.GetUniformLocation(diffuseShader.ShaderProgramHandle, "yScale"), (scale-1f) / height);   
            for (int i = 0; i < iterations; i++)
            {
                GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext, TextureTarget.Texture2D, currentColorBuffer, 0);
                CheckFrameBufferStatus();

                GL.ClearColor(Color.Black);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                GL.Uniform1(GL.GetUniformLocation(diffuseShader.ShaderProgramHandle, "iteration"), i + 1);                
                diffuseShader.BindTexture(ref currentColorTexture, TextureUnit.Texture0, "color_texture");                
                DrawTexturedQuad(-1, 1, 1, -1, diffuseShader);

                int tempColorTexture = currentColorTexture;
                currentColorTexture = currentColorBuffer;
                currentColorBuffer = tempColorTexture;
            }
            diffuseShader.DisableAttributes();

            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);

            return currentColorTexture;
        }

        /// <summary>
        /// Draws the bitmap that has been set for the given frame, 
        /// or nothing when no bitmap was set
        /// </summary>
        void DrawBackground()
        {
            int xOffset = renderState.FrameSize.Width / 2;
            int yOffset = renderState.FrameSize.Height / 2;

            Vector3[] pointsArray = new Vector3[]
            {
                new Vector3(-xOffset, -yOffset, 0),
                new Vector3(xOffset, -yOffset, 0),
                new Vector3(xOffset, yOffset, 0),
                new Vector3(-xOffset, yOffset, 0)
            };

            uint[] indices = new uint[]
            {
                0, 1, 2, 0, 2, 3
            };

            GL.UseProgram(pointShader.ShaderProgramHandle);                   
                        
            pointShader.EnableAttributes();
            pointShader.SetVertexColor(Color.White);
            pointShader.SetVertexPositions(pointsArray);
            pointShader.SetIndices(indices);     
            pointShader.DrawElements(BeginMode.Triangles);
            pointShader.DisableAttributes();
        }

        /// <summary>
        /// Draws the background image
        /// </summary>
        void DrawImage()
        {
            if (!GL.IsTexture(backgroundTextureID))
                return;
            
            float scale = Math.Max(bitmapWidth / (float)renderState.FrameSize.Width, bitmapHeight / (float)renderState.FrameSize.Width);

            int xOffset = (int)(bitmapWidth / (2 * scale));
            int yOffset = (int)(bitmapHeight / (2 * scale));
            
            GL.UseProgram(imageShader.ShaderProgramHandle);
            imageShader.EnableAttributes();
            imageShader.BindTexture(ref backgroundTextureID, TextureUnit.Texture0, "background");
            DrawTexturedQuad(-xOffset, xOffset, -yOffset, yOffset, imageShader);
            imageShader.DisableAttributes();

        }

        /// <summary>
        /// Help function to draw a single, texture quad on the screen
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        /// <param name="shader"></param>
        void DrawTexturedQuad(float left, float right, float top, float bottom, TexturedShader shader)
        {
            Vector2[] texCoords = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1)
            };

            Vector3[] positions = new Vector3[]
            {
                new Vector3(left, bottom, 0),
                new Vector3(right, bottom, 0),
                new Vector3(right, top, 0),
                new Vector3(left, top, 0),
            };

            uint[] indices = new uint[]
            {
                0, 1, 2, 0, 2, 3
            };

            shader.SetIndices(indices);
            shader.SetVertexPositions(positions);
            shader.SetTextureCoordinates(texCoords);
            shader.DrawElements(BeginMode.Triangles);
        }

        /// <summary>
        /// Draws all the geometry shaded curves of the paths in the container.
        /// </summary>
        void DrawBezierCurves()
        {
            GL.UseProgram(lineShader.ShaderProgramHandle);
            int width = renderState.ViewSize.Width;
            int height = renderState.ViewSize.Height;
            lineShader.SetScreenDiagonal((int)Math.Sqrt(width * width + height * height));
            lineShader.SetRatio(width / (float)height);

            lineShader.EnableAttributes();
            for(int i = 0; i < buffers.ListPathPoints.Count; i++)
            {                
                lineShader.SetIndices(buffers.ListIndices[i]);
                lineShader.SetVertexPositions(buffers.ListPathPoints[i]);
                lineShader.SetLeftColorData(buffers.ListLeftColorPoints[i]);
                lineShader.SetRightColorData(buffers.ListRightColorPoints[i]);
                lineShader.DrawElements(BeginMode.LineStripAdjacency);                
            }
            lineShader.DisableAttributes();
        }

        /// <summary>
        /// Draws the end points of each path
        /// </summary>
        void DrawEndPoints()
        {
            GL.UseProgram(endPointShader.ShaderProgramHandle);
            int width = renderState.ViewSize.Width;
            int height = renderState.ViewSize.Height;
            endPointShader.SetScreenDiagonal((int)Math.Sqrt(width * width + height * height));
            endPointShader.SetRatio(width / (float)height);

            endPointShader.EnableAttributes();
            for (int i = 0; i < buffers.ListPathPoints.Count; i++)
            {
                uint[] indices = new uint[]
                {
                    0, 1, 2, 2
                };

                endPointShader.SetIndices(indices);
                endPointShader.SetVertexPositions(buffers.ListPathPoints[i]);
                endPointShader.SetLeftColorData(buffers.ListLeftColorPoints[i]);
                endPointShader.SetRightColorData(buffers.ListRightColorPoints[i]);
                endPointShader.DrawElements(BeginMode.LinesAdjacency);

                int count = buffers.ListPathPoints[i].Length;

                indices = new uint[]
                {
                    (uint)count - 1, (uint)count - 2, (uint)count - 3, (uint)count - 3
                };

                endPointShader.SetIndices(indices);
                endPointShader.SetLeftColorData(buffers.ListRightColorPoints[i]);
                endPointShader.SetRightColorData(buffers.ListLeftColorPoints[i]);
                endPointShader.DrawElements(BeginMode.LinesAdjacency);
            }

            endPointShader.DisableAttributes();
        }

        /// <summary>
        /// Draw the lines of the bezier curves.
        /// </summary>
        void DrawBezierLines()
        {
            GL.LineWidth(4f);
            GL.UseProgram(pointShader.ShaderProgramHandle);
            pointShader.EnableAttributes();
            pointShader.SetVertexColor(Color.Black);
            
            for (int i = 0; i < buffers.ListPathPoints.Count; i++)
            {  
                DrawBezierLines(i);                 
            }

            GL.LineWidth(2f);

            for (int i = 0; i < buffers.ListPathPoints.Count; i++)
            {
                if(container.LayerIndices[container.ActivePathsLayer].Contains(i))
                    pointShader.SetVertexColor(Color.White);
                else
                    pointShader.SetVertexColor(Color.Gray);
                DrawBezierLines(i);
            }

            lineShader.DisableAttributes();
        }

        /// <summary>
        /// Draws the lines for a single path
        /// </summary>
        /// <param name="index"></param>
        void DrawBezierLines(int index)
        {
            uint[] withoutEndPoints = new uint[buffers.ListIndices[index].Length];
            buffers.ListIndices[index].CopyTo(withoutEndPoints, 0);
            withoutEndPoints[0] = withoutEndPoints[1];
            withoutEndPoints[withoutEndPoints.Length - 1] = withoutEndPoints[withoutEndPoints.Length - 2];

            pointShader.SetIndices(withoutEndPoints);
            pointShader.SetVertexPositions(buffers.ListPathPoints[index]);
            pointShader.DrawElements(BeginMode.LineStrip);
        }

        /// <summary>
        /// Draws all the points with an overlay to make for easier viewing.
        /// </summary>
        void DrawPoints()
        {
            GL.UseProgram(pointShader.ShaderProgramHandle);

            GL.PointSize(8f);
            DrawPoints(Color.Black, Color.Black);

            GL.PointSize(6f);
            DrawPoints(Color.Red, Color.Yellow);

        }

        /// <summary>
        /// Draws the actual points with given color.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="selectedColor"></param>
        void DrawPoints(Color color, Color selectedColor)
        {
            pointShader.SetVertexColor(color);

            pointShader.EnableAttributes();

            pointShader.SetVertexPositions(buffers.PathPoints);
            pointShader.DrawArray(BeginMode.Points);
            
            pointShader.DisableAttributes();            
            pointShader.SetVertexColor(selectedColor);

            GL.Begin(BeginMode.Points);
            BezierPoint currentPoint = currentState.SelectedPoint;
            if (currentPoint != null)
            {
                GL.Color3(selectedColor);
                GL.Vertex2(currentPoint.Position);

                GL.Color3(selectedColor);
                GL.Vertex2(currentPoint.Position + currentPoint.Control1);
                GL.Vertex2(currentPoint.Position + currentPoint.Control2);
            }
            GL.End();
        }

        /// <summary>
        /// Draws the control lines of the currently selected point
        /// </summary>
        void DrawControlLines()
        {
            BezierPoint currentPoint = currentState.SelectedPoint;
            if (currentPoint == null)
                return;
            Vector3[] points = new Vector3[]{
                new Vector3(currentPoint.Position),
                new Vector3(currentPoint.Position + currentPoint.Control1),
                new Vector3(currentPoint.Position),
                new Vector3(currentPoint.Position + currentPoint.Control2)
            };

            uint[] indices = new uint[] { 0, 1, 2, 3 };

            GL.UseProgram(pointShader.ShaderProgramHandle);

            pointShader.EnableAttributes();

            pointShader.SetVertexPositions(points);
            pointShader.SetIndices(indices);

            pointShader.SetVertexColor(Color.Black);
            GL.LineWidth(controlLineWidth + 2);
            pointShader.DrawElements(BeginMode.Lines);

            pointShader.SetVertexColor(Color.Yellow);
            GL.LineWidth(controlLineWidth);
            pointShader.DrawElements(BeginMode.Lines);

            pointShader.DisableAttributes();
        }

        /// <summary>
        /// Sets the bitmap for the new frame
        /// </summary>
        /// <param name="bitmap">the new bitmap, can be null if no bitmap is required for this frame</param>
        public void SetFrame(Bitmap bitmap, IPathContainer container)
        {
            this.container = container;

            if (bitmap == null)
                backgroundTextureID = -1;
            else
            {
                if (!loaded)
                {
                    imageToLoad = bitmap;
                }
                else
                {
                    SetBitmap(bitmap);
                }
            }
        }

        /// <summary>
        /// Set the background image for the current frame
        /// </summary>
        /// <param name="bitmap"></param>
        void SetBitmap(Bitmap bitmap)
        {            
            CreateTexture(ref backgroundTextureID, bitmap);

            bitmapWidth = bitmap.Width;
            bitmapHeight = bitmap.Height;
        }

        /// <summary>
        /// Helper function to construct an OpenGL texture
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="bitmap"></param>
        private void CreateTexture(ref int texture, Bitmap bitmap)
        {
            // load texture 
            if (!GL.IsTexture(texture))
                GL.GenTextures(1, out texture);

            //Still required else TexImage2D will be applyed on the last bound texture
            GL.BindTexture(TextureTarget.Texture2D, texture);

            BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
            ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
            OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            bitmap.UnlockBits(data);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        }

        /// <summary>
        /// Helper function to make sure the frame buffer has been constructed properly
        /// </summary>
        void CheckFrameBufferStatus()
        {
            FramebufferErrorCode error = GL.Ext.CheckFramebufferStatus(FramebufferTarget.FramebufferExt);

            switch (error)
            {
                case FramebufferErrorCode.FramebufferCompleteExt:
                    {
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteAttachmentExt:
                    {
                        Console.WriteLine("FBO: One or more attachment points are not framebuffer attachment complete. This could mean there’s no texture attached or the format isn’t renderable. For color textures this means the base format must be RGB or RGBA and for depth textures it must be a DEPTH_COMPONENT format. Other causes of this error are that the width or height is zero or the z-offset is out of range in case of render to volume.");
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteMissingAttachmentExt:
                    {
                        Console.WriteLine("FBO: There are no attachments.");
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteDimensionsExt:
                    {
                        Console.WriteLine("FBO: Attachments are of different size. All attachments must have the same width and height.");
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteFormatsExt:
                    {
                        Console.WriteLine("FBO: The color attachments have different format. All color attachments must have the same format.");
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteDrawBufferExt:
                    {
                        Console.WriteLine("FBO: An attachment point referenced by GL.DrawBuffers() doesn’t have an attachment.");
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteReadBufferExt:
                    {
                        Console.WriteLine("FBO: The attachment point referenced by GL.ReadBuffers() doesn’t have an attachment.");
                        break;
                    }
                case FramebufferErrorCode.FramebufferUnsupportedExt:
                    {
                        Console.WriteLine("FBO: This particular FBO configuration is not supported by the implementation.");
                        break;
                    }
                default:
                    {
                        Console.WriteLine("FBO: Status unknown. (yes, this is really bad.)");
                        break;
                    }
            }
        }

        /// <summary>
        /// Event for changes in the diffusion renderer
        /// </summary>
        /// <param name="propertyName"></param>
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
