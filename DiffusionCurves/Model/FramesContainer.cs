using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DiffusionCurves.Model
{
    [Serializable]
    public class FramesContainer : INotifyPropertyChanged
    {
        #region Private fields

        [field: NonSerialized]
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        
        ObservableRangeCollection<Model.Frame> framesList;
        int cursorIndex;

        #endregion

        #region Getters setters

        public ObservableRangeCollection<Model.Frame> FramesList
        {
            get { return framesList; }
        }

        #endregion

        /// <summary>
        /// Updates the cursor index:
        /// - Checks if it is not out of bounds;
        /// - Calls GUI.
        /// </summary>
        public int CursorIndex
        {
            get { return cursorIndex; }
            set
            {
                if (value > Count || value < 0)
                    throw new System.IndexOutOfRangeException();
                else
                {
                    cursorIndex = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Keeps count of the number of elements in frameslist.
        /// </summary>
        public int Count
        {
            get { return framesList.Count; }
        }

        /// <summary>
        /// Constructor for FramesContainer
        /// </summary>
        public FramesContainer()
        {
            framesList = new ObservableRangeCollection<Frame>();
            framesList.CollectionChanged += framesList_CollectionChanged;
        }

        /// <summary>
        /// Adds frame to frames container.
        /// </summary>
        /// <param name="frame"></param>
        public void AddFrame(Model.Frame frame)
        {
            framesList.Add(frame);
        }

        /// <summary>
        /// Appends frames container.
        /// </summary>
        /// <param name="frameCollection"></param>
        public void AddRange(IEnumerable<Model.Frame> frameCollection)
        {
            framesList.AddRange(frameCollection);
        }

        /// <summary>
        /// Removes current frame.
        /// </summary>
        public void RemoveCurrentFrame()
        {
            framesList.RemoveAt(CursorIndex);
        }

        /// <summary>
        /// Clears frames container.
        /// </summary>
        public void Clear()
        {
            framesList.Clear();            
        }

        /// <summary>
        /// Copies frames container.
        /// </summary>
        /// <param name="container"></param>
        public void Copy(FramesContainer container)
        {
            framesList.Clear();
            framesList.AddRange(container.FramesList);
            CursorIndex = container.CursorIndex;
        }

        /// <summary>
        /// Handler for when the collection in frameslist has been changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void framesList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
                CollectionChanged(this, e);
        }

        /// <summary>
        /// Notifies other classes on change of property.
        /// </summary>
        /// <param name="propertyName"></param>
        void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
