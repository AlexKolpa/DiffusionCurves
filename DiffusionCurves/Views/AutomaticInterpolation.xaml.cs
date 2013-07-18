using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DiffusionCurves.Interpolation;

namespace DiffusionCurves.Views
{
    /// <summary>
    /// Interaction logic for AutomaticInterpolation.xaml
    /// </summary>
    partial class AutomaticInterpolation : Window
    {
        public event EventHandler<AutoInterpolationEventArgs> StartInterpolation;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AutomaticInterpolation()
        {
            InitializeComponent();            
            this.errorValue.PreviewTextInput += errorValue_PreviewTextInput;
        }

        /// <summary>
        /// Error event handler for automatic interpolation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void errorValue_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            short val;
            bool isShort = Int16.TryParse(errorValue.Text + e.Text, out val);
            e.Handled = !isShort;

            if (isShort)
            {
                if (val < 6)
                {
                    errorMessage.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    errorMessage.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Cancels action, disposes window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Starts the automatic interpolation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

            if (StartInterpolation != null)
                StartInterpolation(this, new AutoInterpolationEventArgs(double.Parse(this.errorValue.Text)));            
        }
    }
}
