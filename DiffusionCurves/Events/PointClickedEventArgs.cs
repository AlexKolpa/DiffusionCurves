using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DiffusionCurves.Model;

namespace DiffusionCurves.Events
{
    /// <summary>
    /// Class for PointClicked event.
    /// </summary>
    public class PointClickedEventArgs : EventArgs
    {
        public BezierPoint ClickedPoint { get; private set; }
        public MouseButtons ButtonsPressed { get; private set; }

        /// <summary>
        /// Constructor for Pointclick event.
        /// </summary>
        /// <param name="clickedPoint"></param>
        /// <param name="buttonsPressed"></param>
        public PointClickedEventArgs(BezierPoint clickedPoint, MouseButtons buttonsPressed)
        {
            ClickedPoint = clickedPoint;
            ButtonsPressed = buttonsPressed;
        }
    }
}
