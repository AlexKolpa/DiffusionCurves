using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using DiffusionCurves.Diffusion;
using DiffusionCurves.Events;
using DiffusionCurves.Model;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace DiffusionCurves.Diffusion
{
    /// <summary>
    /// An eventhandler and container for the GLControl
    /// </summary>
    public class GLControlHost : System.Windows.Forms.Integration.WindowsFormsHost
    {
        #region Private fields

        public event EventHandler<PointClickedEventArgs> PointClicked;

        EditorState editorState;
        IEditorStateControl editorStateControl;
        IDiffusionRenderer renderer;
        GLControl glControl;
        IDiffusionPathControl bufferControl;
        IRenderStateControl renderControl;

        Vector2 oldMousePos;

        bool loaded = false;

        /// <summary>
        /// Gets the status of OpenGL in the GLControl
        /// </summary>
        public bool GLLoaded
        {
            get { return loaded; }
        }

        #endregion

        /// <summary>
        /// Constructs a GLControlHost object, which handles the GLControl events and passes them to the other objects
        /// </summary>
        /// <param name="control"></param>
        /// <param name="state"></param>
        /// <param name="input"></param>
        /// <param name="renderer"></param>
        public GLControlHost(GLControl control, 
            EditorState state, 
            IEditorStateControl input, 
            IDiffusionRenderer renderer,
            IRenderStateControl renderControl,
            IDiffusionPathControl bufferControl)
        {            
            this.glControl = control;
            this.editorState = state;
            this.editorStateControl = input;
            this.renderer = renderer;
            this.renderControl = renderControl;
            this.bufferControl = bufferControl;

            renderer.PropertyChanged += renderer_PropertyChanged;            
            glControl.Load += glControl_Load;
            glControl.Load += renderer.glControl_Load;
            glControl.Paint += glControl_Paint;
            glControl.Resize += glControl_Resize;
            glControl.MouseDown += glControl_MouseDown;
            glControl.MouseUp += glControl_MouseUp;
            glControl.MouseMove += glControl_MouseMove;
            glControl.MouseWheel += glControl_MouseWheel;
            glControl.KeyDown += glControl_KeyDown;
            glControl.KeyUp += glControl_KeyUp;
        }
        
        /// <summary>
        /// Fired when the mouse is scrolled while the GLControl has focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void glControl_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Delta != 0)
            {
                int delta = e.Delta / 120;
                renderControl.Zoom(delta);
                glControl.Invalidate();
            }
        }
                
        /// <summary>
        /// Fired when a key is released when the GLControl has focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void glControl_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            editorState.ModifierState = EditorStateControl.KeyModifierState.None;            
        }

        /// <summary>
        /// Fired when a key is pressed when the GLControl has focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void glControl_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyData == Keys.Back)
            {
                editorStateControl.DeleteSelectedPoint();

                bufferControl.Rebuild();
                glControl.Invalidate();
            }
            else if ((e.KeyData & Keys.Control) != 0)
            {
                editorState.ModifierState = EditorStateControl.KeyModifierState.ControlDragging;
            }
            else if ((e.KeyData & Keys.Shift) != 0)
            {
                editorState.ModifierState = EditorStateControl.KeyModifierState.NewPath;
            }
        }

        /// <summary>
        /// Fired when the GLControl finishes loading OpenGL functionality
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void glControl_Load(object sender, EventArgs e)
        {
            loaded = true;
        }

        Vector2 AdjustBoundsToView(Vector2 screenPosition)
        {
            if (screenPosition.X < 0)
                screenPosition.X = 0;
            else if (screenPosition.X >= glControl.Width)
                screenPosition.X = glControl.Width - 1;

            if (screenPosition.Y < 0)
                screenPosition.Y = 0;
            else if (screenPosition.Y >= glControl.Height)
                screenPosition.Y = glControl.Height - 1;

            return screenPosition;
        }

        /// <summary>
        /// Mouse moved event for the OpenGL window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void glControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Vector2 mousePos = renderControl.ToWorldSpace(AdjustBoundsToView(new Vector2(e.X, glControl.Height - e.Y)));

            if (e.Button == MouseButtons.Left)
            {
                if (editorStateControl.DragStateItem(mousePos))
                    bufferControl.Rebuild();
                glControl.Invalidate();
            }            

            if (e.Button == MouseButtons.Middle)
            {
                Vector2 diff = mousePos - oldMousePos;
                renderControl.Pan(diff);
                oldMousePos = mousePos;
                glControl.Invalidate();
            }
        }

        /// <summary>
        /// Mouse up event for the OpenGL window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void glControl_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            editorStateControl.SetIdle();
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
            oldMousePos = new Vector2(-1, -1);
        }

        /// <summary>
        /// Mouse down event for the OpenGL window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void glControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Vector2 worldMousePos = renderControl.ToWorldSpace(AdjustBoundsToView(new Vector2(e.X, glControl.Height - e.Y)));
            
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                editorStateControl.UpdateState(worldMousePos);

                if(editorState.CurrentState == EditorStateControl.MouseState.PointDragging)
                    if (PointClicked != null)
                        PointClicked(this, new PointClickedEventArgs(editorState.SelectedPoint, e.Button));
            }

            if (e.Button == MouseButtons.Middle)
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.SizeAll;
                oldMousePos = worldMousePos;
            }

            bufferControl.Rebuild();
            glControl.Invalidate();
            
        }

        /// <summary>
        /// OpenGL viewport needs to be reset when the window gets resized
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void glControl_Resize(object sender, EventArgs e)
        {
            renderControl.SetViewport(glControl.Size);
        }

        /// <summary>
        /// event for painting the view when the control is invalidated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void glControl_Paint(object sender, PaintEventArgs e)
        {
            if (!loaded)
                return;

            renderer.Draw();
            glControl.SwapBuffers();
        }

        /// <summary>
        /// Requests the GLControl to be redrawn, when external factors changed
        /// </summary>
        public void RequestPaint()
        {
            glControl.Invalidate();
        }

        /// <summary>
        /// event when important properties of the diffusion renderer change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void renderer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "DisplayDiffusion":
                case "Iterations":
                    glControl.Invalidate();
                    break;
                default:
                    break;
            }
        }
        
        /// <summary>
        /// Set a new bitmap and paths for the editor.
        /// </summary>
        /// <param name="bitmap">Bitmap for the new frame, can be null if no image is required</param>
        /// <param name="container">The container for the new frame</param>
        public void SetNewFrame(Bitmap bitmap, IPathContainer container)
        {
            if(container == null)
                throw new ArgumentNullException("PathsContainer cannot be null");

            editorStateControl.SetPathsContainer(container);
            editorStateControl.SetIdle();

            bufferControl.SetContainer(container);

            renderer.SetFrame(bitmap, container);
            
            glControl.Invalidate();
        }

        /// <summary>
        /// Sets the framesize of eacht project.
        /// </summary>
        /// <param name="frameSize"></param>
        public void SetProjectFrameSize(Size frameSize)
        {
            renderControl.SetFrameSize(frameSize);
        }
    }
}
