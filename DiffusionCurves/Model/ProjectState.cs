using DiffusionCurves.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DiffusionCurves.Model
{
    /// <summary>
    /// This class describes project state model.
    /// </summary>
    [Serializable]
    public class ProjectState : INotifyPropertyChanged
    {
        #region Private fields of ProjectState class

        //Event to fire back to mainwindow
        [field: NonSerialized]
        public virtual event PropertyChangedEventHandler PropertyChanged;

        public enum ProjectEditorState
        {
            Create,
            Color,
            Neutral
        };

        //Flags
        private bool _active;
        private bool _saved;

        //DESTINATION + Name
        private string _destination;
        private string _filename;

        //Buttons
        private ProjectEditorState _editorState;

        //Layers
        private int _activelayerindex;
        
        //Widht height
        private int _width;
        private int _height;

        #endregion

        /// <summary>
        /// Constructor for ProjectState.
        /// </summary>
        public ProjectState()
        {
            #region Initialize private fields

            //Flags
            this._saved = false;
            this._active = false;

            //DESTINATION + Name
            this._destination = string.Empty;
            this._filename = "<empty>";

            //Buttons
            this._editorState = ProjectEditorState.Neutral;

            //Width height
            this._width = 0;
            this._height = 0;

            this._activelayerindex = 0;

            #endregion
        }

        #region Getters and setters

        public ProjectEditorState EditorState
        {
            get { return _editorState; }
            set { 
                _editorState = value;
                Saved = false;
                NotifyPropertyChanged();
            }
        }

        public bool Active
        {
            get { return _active; }
            set { 
                _active = value;
            }
        }

        /// <summary>
        /// Updates saved and calls GUI.
        /// </summary>
        public bool Saved
        {
            get { return _saved; }
            set
            {
                _saved = value;
                NotifyPropertyChanged();                
            }
        }

        public string Destination
        {
            get { return _destination; }
            set {
                _destination = value;
            }
        }

        public string FileName
        {
            get { return _filename; }
            set { 
                _filename = value;
            }
        }

        public int Width
        {
            get { return _width; }
            set { 
                _width = value;
            }
        }

        public int Height
        {
            get { return _height; }
            set { 
                _height = value;
            }
        }

        /// <summary>
        /// The index for the layers.
        /// </summary>
        public virtual int ActiveLayerIndex
        {
            get { return _activelayerindex; }
            set { 
                _activelayerindex = value;
                Saved = false;
                NotifyPropertyChanged();    
            }
        }

        #endregion

        /// <summary>
        /// Copies a given state to the state in the memory.
        /// </summary>
        /// <param name="state"></param>
        public void Copy(ProjectState state)
        {
            this.EditorState = state.EditorState;
            this.Active = state.Active;
            this.Saved = state.Saved;
            this.Destination = state.Destination;
            this.FileName = state.FileName;
            this.Width = state.Width;
            this.Height = state.Height;
            this.ActiveLayerIndex = state.ActiveLayerIndex;
        }

        #region Update the GUI if called

        /// <summary>
        /// Handles the GUI call from the setters via an event.
        /// </summary>      
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
