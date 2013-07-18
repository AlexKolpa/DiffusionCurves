using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffusionCurves.Interpolation
{
    /// <summary>
    /// Class for AutoInterpolation
    /// </summary>
    public class AutoInterpolationEventArgs : EventArgs
    {
        private double errorValue;

        public double ErrorValue
        {
            get { return errorValue; }
        }

        /// <summary>
        /// Constructor for AutoInterpolation.
        /// </summary>
        /// <param name="errorValue"></param>
        public AutoInterpolationEventArgs(double errorValue)
        {
            this.errorValue = errorValue;
        }
    }
}
