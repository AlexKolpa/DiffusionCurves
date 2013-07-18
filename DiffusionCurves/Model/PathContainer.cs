using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DiffusionCurves.Model
{
    /// <summary>
    /// Class for PathContainer.
    /// </summary>
    [Serializable]
    public class PathContainer : IPathContainer
    {
        List<Path> paths;
        int activeLayer = 0;
        List<int>[] layeredPathIndices;
        
        /// <summary>
        /// Constructor for PathContainer.
        /// </summary>
        public PathContainer()
        {
            paths = new List<Path>();
            layeredPathIndices = new List<int>[3];
            layeredPathIndices[0] = new List<int>();
            layeredPathIndices[1] = new List<int>();
            layeredPathIndices[2] = new List<int>();
        }

        public List<Path> Paths
        {
            get { return paths; }
            set { paths = value; }
        }

        /// <summary>
        /// Gets an enumerator of paths.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Path> GetPathsEnumerator()
        {
            return paths.GetEnumerator();
        }

        public int Count
        {
            get { return paths.Count; }
        }
        
        public int ActivePathsLayer
        {
            get
            {
                return activeLayer;
            }
            set
            {
                activeLayer = value;
            }
        }

        public List<int>[] LayerIndices
        {
            get { return layeredPathIndices; }
            set { layeredPathIndices = value; }
        }

        /// <summary>
        /// Adds a path to existing paths.
        /// </summary>
        /// <param name="path"></param>
        public void AddPath(Path path)
        {
            if (path == null) 
                throw new ArgumentNullException();

            paths.Add(path);
            layeredPathIndices[activeLayer].Add(paths.Count - 1);
        }

        /// <summary>
        /// Gets the last path.
        /// </summary>
        /// <returns></returns>
        public Path GetLastPath()
        {            
            return paths.ElementAt(Count - 1);
        }

        /// <summary>
        /// Remove path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool RemovePath(Path path)
        {
            int index = paths.IndexOf(path);
            layeredPathIndices[activeLayer].Remove(index);
            return paths.Remove(path);
        }
        
        /// <summary>
        /// Clones paths.
        /// </summary>
        /// <returns></returns>
        public IPathContainer Clone()
        {
            PathContainer newContainer = new PathContainer();
            IEnumerator<Path> pathsEnumerator = GetPathsEnumerator();
            while (pathsEnumerator.MoveNext())
            {
                newContainer.AddPath(pathsEnumerator.Current.Clone());
            }

            newContainer.layeredPathIndices = new List<int>[3];
            newContainer.layeredPathIndices[0] = new List<int>(layeredPathIndices[0]);
            newContainer.layeredPathIndices[1] = new List<int>(layeredPathIndices[1]);
            newContainer.layeredPathIndices[2] = new List<int>(layeredPathIndices[2]);
            newContainer.activeLayer = activeLayer;

            return newContainer;
        }

        /// <summary>
        /// Checks for equality of paths.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            if (!(obj is PathContainer))
                return false;

            PathContainer containerObj = obj as PathContainer;

            bool layerIndicesEqual = true;
            layerIndicesEqual &= Enumerable.SequenceEqual(this.layeredPathIndices[0], containerObj.layeredPathIndices[0]);
            layerIndicesEqual &= Enumerable.SequenceEqual(this.layeredPathIndices[1], containerObj.layeredPathIndices[1]);
            layerIndicesEqual &= Enumerable.SequenceEqual(this.layeredPathIndices[2], containerObj.layeredPathIndices[2]);

            return Enumerable.SequenceEqual(this.Paths, containerObj.Paths) &&
                this.ActivePathsLayer == containerObj.ActivePathsLayer && layerIndicesEqual;
                
        }

        /// <summary>
        /// Clears the path.
        /// </summary>
        public void Clear()
        {
            paths.Clear();
        }

        /// <summary>
        /// Counts all points in the class.
        /// </summary>
        public int TotalPointCount
        {
            get
            {
                int total = 0;
                IEnumerator<Path> pathsEnumerator = GetPathsEnumerator();
                while (pathsEnumerator.MoveNext())
                {
                    total += pathsEnumerator.Current.Count;
                }

                return total;
            }
        }
    }
}
