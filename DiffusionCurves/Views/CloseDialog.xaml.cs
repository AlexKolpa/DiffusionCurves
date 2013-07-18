using DiffusionCurves.Events;
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
using System.Windows.Shapes;

namespace DiffusionCurves.Views
{
    /// <summary>
    /// Interaction logic for CloseDialog.xaml
    /// </summary>
    public partial class CloseDialog : Window
    {
        //Event delegate
        public delegate void CloseProjectEventHandler(object sender, StateEventArgs e);
        //Make event with delegate structure
        public event CloseProjectEventHandler ProjectClosed;

        /// <summary>
        /// Constructor for CloseDialog.
        /// </summary>
        public CloseDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handler for cancel button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            //Event to fire back
            StateEventArgs stateeventargs = new StateEventArgs();
            stateeventargs.Wanto = StateEventArgs.WantTo.Dispose;

            //Return values to caller
            if (ProjectClosed != null)
                ProjectClosed(this, stateeventargs);

            this.Close();
        }

        /// <summary>
        /// Handler for save button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            //Event to fire back
            StateEventArgs stateeventargs = new StateEventArgs();
            stateeventargs.Wanto = StateEventArgs.WantTo.Save;

            //Return values to caller
            if (ProjectClosed != null)
                ProjectClosed(this, stateeventargs);

            this.Close();
        }

        /// <summary>
        /// Handler for Don't Save button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DontSave_Click(object sender, RoutedEventArgs e)
        {
            //Event to fire back
            StateEventArgs stateeventargs = new StateEventArgs();
            stateeventargs.Wanto = StateEventArgs.WantTo.Unsave;

            //Return values to caller
            if (ProjectClosed != null)
                ProjectClosed(this, stateeventargs);

            this.Close();
        }
    }
}
