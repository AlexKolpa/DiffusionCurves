using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DiffusionCurves.Views
{
    /// <summary>
    /// Interaction logic for TimeLinePanel.xaml
    /// </summary>
    public abstract partial class TimeLinePanel : UserControl
    {
        public event RoutedPropertyChangedEventHandler<double> ValueChanged;
        
        /// <summary>
        /// For when the value has been changed.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnValueChanged(RoutedPropertyChangedEventArgs<double> e)
        {
            if (ValueChanged != null)
                ValueChanged(this, e);
        }
    }
}
