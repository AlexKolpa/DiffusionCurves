using DiffusionCurves.Diffusion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DiffusionCurves.Model
{
    /// <summary>
    /// Model class for a frame.
    /// </summary>
    [Serializable]
    public class Frame
    {
        #region Private fields of Frame class

        private string imageurl;
        private IPathContainer curves;
        
        #endregion

        /// <summary>
        /// Constructor for Frame.
        /// </summary>
        /// <param name="imageurl"></param>
        public Frame(string imageurl)
        {
            #region Initialize private fields
                        
            this.imageurl = imageurl;
            this.curves = new PathContainer();

            #endregion
        }

        #region Getters and setters

        public string ImageUrl
        {
            get { return imageurl; }
            set { imageurl = value; }
        }

        public IPathContainer Curves
        {
            get { return curves; }
            set { curves = value; }
        }

        #endregion
    }
}
