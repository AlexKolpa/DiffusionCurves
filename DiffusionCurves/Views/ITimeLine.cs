using System;
using System.Collections.Generic;
using System.Windows.Controls;
namespace DiffusionCurves.Views
{
    /// <summary>
    /// Interface for the timeline.
    /// </summary>
    public interface ITimeLine
    {
        void AddFrame(global::DiffusionCurves.Model.Frame frame);
        int CURSORINDEX { get; set; }
        void InitializeComponent();
    }
}
