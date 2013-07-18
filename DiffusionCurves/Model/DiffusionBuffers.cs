using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace DiffusionCurves.Model
{
    /// <summary>
    /// Class for DiffusionBuffers
    /// </summary>
    public class DiffusionBuffers
    {
        #region PathPoints
        Vector3[] pathPoints;
        List<Vector3[]> listPathPoints;
        List<Vector4[]> listLeftColorPoints;
        List<Vector4[]> listRightColorPoints;
        List<uint[]> listIndices;

        public Vector3[] PathPoints
        {
            get { return pathPoints; }
            set { pathPoints = value; }
        }

        public List<Vector3[]> ListPathPoints
        {
            get { return listPathPoints; }
            set { listPathPoints = value; }
        }

        public List<Vector4[]> ListLeftColorPoints
        {
            get { return listLeftColorPoints; }
            set { listLeftColorPoints = value; }
        }

        public List<Vector4[]> ListRightColorPoints
        {
            get { return listRightColorPoints; }
            set { listRightColorPoints = value; }
        }

        public List<uint[]> ListIndices
        {
            get { return listIndices; }
            set { listIndices = value; }
        }
        #endregion

        /// <summary>
        /// Constructor for DiffusionBuffers.
        /// </summary>
        public DiffusionBuffers()
        {
            this.Clear();
        }

        /// <summary>
        /// Clears all elements.
        /// </summary>
        public void Clear()
        {
            listPathPoints = new List<Vector3[]>();
            listLeftColorPoints = new List<Vector4[]>();
            listRightColorPoints = new List<Vector4[]>();
            listIndices = new List<uint[]>();
            pathPoints = new Vector3[0];
        }
    }
}
