using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace DiffusionCurves.Model
{
    /// <summary>
    /// Class for RenderState.
    /// </summary>
    public class RenderState : INotifyPropertyChanged
    {
        #region Private fields

        public virtual event PropertyChangedEventHandler PropertyChanged;

        Matrix4 projectionMatrix;
        Matrix4 viewMatrix;
        float zoomLevel;
        Vector2 offset;
        Size viewSize;
        Size frameSize;
        int zoomSteps;

        #endregion

        #region Getters setters

        public virtual Matrix4 ProjectionMatrix
        {
            get { return projectionMatrix; }
            set
            {
                projectionMatrix = value;
                NotifyPropertyChanged();
            }
        }
        public virtual Matrix4 ViewMatrix
        {
            get { return viewMatrix; }
            set
            {
                if (value != viewMatrix)
                {
                    viewMatrix = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public virtual float ZoomLevel
        {
            get { return zoomLevel; }
            set
            {
                if (value != zoomLevel)
                {
                    zoomLevel = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public virtual Vector2 Offset
        {
            get { return offset; }
            set
            {
                if (value != offset)
                {
                    offset = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public virtual Size ViewSize
        {
            get { return viewSize; }
            set
            {
                if (value != viewSize)
                {
                    viewSize = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public virtual Size FrameSize
        {
            get { return frameSize; }
            set
            {
                if (value != frameSize)
                {
                    frameSize = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public virtual int ZoomSteps
        {
            get { return zoomSteps; }
            set
            {
                if (value != zoomSteps)
                {
                    zoomSteps = value;
                    NotifyPropertyChanged();
                }
            }
        }

        #endregion

        /// <summary>
        /// Sets the default render state.
        /// </summary>
        public RenderState()
        {
            this.projectionMatrix = Matrix4.Identity;
            this.viewMatrix = Matrix4.Identity;
            this.ZoomLevel = 1;
            Offset = new Vector2(0, 0);
            Defaults();
        }

        /// <summary>
        /// Sets to given render state.
        /// </summary>
        /// <param name="projectionMatrix"></param>
        /// <param name="viewMatrix"></param>
        /// <param name="zoomLevel"></param>
        /// <param name="offset"></param>
        public RenderState(Matrix4 projectionMatrix, Matrix4 viewMatrix, int zoomLevel, Vector2 offset)
        {
            this.projectionMatrix = projectionMatrix;
            this.viewMatrix = viewMatrix;
            this.ZoomLevel = zoomLevel;
            this.Offset = offset;
            Defaults();
        }

        /// <summary>
        /// Sets default.
        /// </summary>
        void Defaults()
        {
            this.ViewSize = Size.Empty;
            this.FrameSize = Size.Empty;
            this.ZoomSteps = 10;
        }

        /// <summary>
        /// Notifies classes of changed properties.
        /// </summary>
        /// <param name="propertyName"></param>
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
