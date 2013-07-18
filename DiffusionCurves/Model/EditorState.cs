using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DiffusionCurves.Events;

namespace DiffusionCurves.Model
{
    /// <summary>
    /// Defines EditorState class
    /// </summary>
    public class EditorState
    {
        #region Private fields

        public event EventHandler<PropertyChangedEventArgs> PropertyChanged;

        private BezierPoint selectedPoint;
        private Path selectedPath;
        private bool allowSmoothUpdate;
        
        #endregion

        #region Getters setters

        public bool AllowSmoothUpdate
        {
            get { return allowSmoothUpdate; }
            set { allowSmoothUpdate = value; }
        }

        public BezierPoint SelectedPoint
        {
            get
            {
                return selectedPoint;
            }
            set
            {
                if (value != selectedPoint)
                {
                    if(selectedPoint != null)
                        selectedPoint.PropertyChanged -= selectedPoint_PropertyChanged;

                    selectedPoint = value;

                    if (selectedPoint != null)
                        selectedPoint.PropertyChanged += selectedPoint_PropertyChanged;

                    NotifyStateChanged();
                }
            }
        }

        public Path SelectedPath
        {
            get
            {
                return selectedPath;
            }
            set
            {
                if (value != selectedPath)
                {
                    selectedPath = value;
                    NotifyStateChanged();
                }
            }
        }

        public DiffusionCurves.Diffusion.EditorStateControl.MouseState CurrentState { get; set; }
        public DiffusionCurves.Diffusion.EditorStateControl.KeyModifierState ModifierState { get; set; }

        #endregion

        /// <summary>
        /// Handler for when a point is selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void selectedPoint_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyStateChanged(e.PropertyName);
        }
        
        /// <summary>
        /// Notifies other classes of changed property.
        /// </summary>
        /// <param name="propertyName"></param>
        private void NotifyStateChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
