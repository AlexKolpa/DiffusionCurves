using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffusionCurves.Events
{
    public class InterpolationArgs : EventArgs
    {
        public bool Handled { get; set; }
    }
}
