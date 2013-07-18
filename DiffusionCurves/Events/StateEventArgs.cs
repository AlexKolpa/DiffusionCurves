using DiffusionCurves.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffusionCurves.Events
{
    /// <summary>
    /// Consists all fields that needs to be returned via an event call.
    /// </summary> 
    public class StateEventArgs : EventArgs
    {
        #region Private fields

        private FramesContainer framescontainer;
        private ProjectState projectstate;

        private string filename;
        private string destination;
        
        private bool active;
        private bool saved;
        
        private List<Frame> frameslist;

        private int _width;
        private int _height;

        public enum WantTo
        {
            Save,
            Unsave,
            Dispose
        };

        private WantTo _wanto;

        #endregion

        /// <summary>
        /// Constructor for StaveEvent event.
        /// </summary>
        public StateEventArgs()
        {
            #region Initialize private fields

            this.filename = "";
            this.destination = "";

            this.active = false;
            this.saved = false;
            
            this.frameslist = new List<Frame>();
            
            this._width = 0;
            this._height = 0;

            this._wanto = WantTo.Save;

            #endregion
        }

        #region Getters and setters for private fields

        public WantTo Wanto
        {
            get { return _wanto; }
            set
            {
                _wanto = value;
            }
        }

        public FramesContainer Framescontainer
        {
            get { return framescontainer; }
            set { framescontainer = value; }
        }

        public ProjectState Projectstate
        {
            get { return projectstate; }
            set { projectstate = value; }
        }

        public string FILENAME
        {
            get { return filename; }
            set { filename = value; }
        }

        public string DESTINATION
        {
            get { return destination; }
            set { destination = value; }
        }

        public bool ACTIVE
        {
            get { return active; }
            set { active = value; }
        }

        public bool SAVED
        {
            get { return saved; }
            set { saved = value; }
        }

        public List<Frame> FRAMESLIST
        {
            get { return frameslist; }
            set { frameslist = value; }
        }

        public int WIDTH
        {
            get { return _width; }
            set { _width = value; }
        }

        public int HEIGHT
        {
            get { return _height; }
            set { _height = value; }
        }

        #endregion
    }
}
