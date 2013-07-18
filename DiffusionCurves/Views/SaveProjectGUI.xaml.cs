using DiffusionCurves.Auxillary;
using DiffusionCurves.Events;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for SaveProject.xaml
    /// </summary>
    public partial class SaveProjectGUI : Window
    {
        #region Private fields

        private DiffusionCurves.Model.ProjectState givenprojectstate;
        DiffusionCurves.Model.FramesContainer framescontainer;

        //Event delegate
        public delegate void SaveProjectEventHandler(object sender, StateEventArgs e);
        //Make event with delegate structure
        public event SaveProjectEventHandler ProjectSaved;

        #endregion

        /// <summary>
        /// Constructor of the dialog.
        /// </summary>
        public SaveProjectGUI(DiffusionCurves.Model.ProjectState givenprojectstate, DiffusionCurves.Model.FramesContainer framescontainer)
        {
            InitializeComponent();

            this.givenprojectstate = givenprojectstate;

            this.framescontainer = framescontainer;
        }

        /// <summary>
        /// Handles if browse gets clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Browse_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            System.Windows.Forms.FolderBrowserDialog browseDialog = new System.Windows.Forms.FolderBrowserDialog();

            //Show button to create new folder
            browseDialog.ShowNewFolderButton = true;

            // Show open file dialog box
            System.Windows.Forms.DialogResult result = browseDialog.ShowDialog();

            //Save user filename
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                Destination_textbox.Foreground = Brushes.White;         //Set textcolor
                Destination_textbox.Text = browseDialog.SelectedPath;   //Set into textbox
            }
        }

        /// <summary>
        /// Handles if cancel gets clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Handles if create project button gets clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SaveProject_Click(object sender, RoutedEventArgs e)
        {
            //Get data from GUI
            string destination = Destination_textbox.Text;
            string filename = Filename_textbox.Text;

            //Event to fire back
            StateEventArgs stateeventargs = new StateEventArgs();

            //Path 1: Empty project
            if (ValidateInput.EmptyFilename(filename) && ValidateInput.EmptyDestination(destination))
            {
                Prompt("Empty fields", "You must input both the filename and the destination");
                return;
            }
            //Path 2:
            else if (ValidateInput.EmptyFilename(filename) && !ValidateInput.EmptyDestination(destination))
            {
                Prompt("No file name entered.", "No filename given, please enter a file name or empty destination.");
                return;
            }
            //Path 3: 
            else if (!ValidateInput.EmptyFilename(filename) && ValidateInput.EmptyDestination(destination))
            {
                Prompt("No destination entered.", "No destination is given, please enter a destination or empty the filename.");
                return;
            }
            //Path 4: Named and destined project
            else
            {
                //Path 4_1 
                if (!ValidateInput.ValidFileName(filename) && !ValidateInput.ValidDestination(destination))
                {
                    Prompt("Invalid file name and destination", "Invalid file name and destination, choose another file name and destination.");
                    return;
                }
                //Path 4_2
                else if (!ValidateInput.ValidFileName(filename) && ValidateInput.ValidDestination(destination))
                {
                    Prompt("Invalid file name", "Invalid file name, choose another file name.");
                    return;
                }
                //Path 4_3
                else if (ValidateInput.ValidFileName(filename) && !ValidateInput.ValidDestination(destination))
                {
                    Prompt("Invalid destination", "Invalid destination, choose another destination.");
                    return;
                }
                //Path 4_4
                else if (ValidateInput.FileNameExists(filename, destination))
                {
                    Prompt("File already exists", "The file already exists for this destination, choose another file name.");
                    return;
                }
                //Right Path
                else
                {
                    //Save project
                    try
                    {
                        //update givenprojectstate
                        this.givenprojectstate.Destination = destination;
                        this.givenprojectstate.FileName = filename;
                        this.givenprojectstate.Active = true;
                        this.givenprojectstate.Saved = true;

                        //create and save
                        DiffusionCurves.Storage.CreateProject.Create(this.givenprojectstate.FileName, this.givenprojectstate.Destination);
                        DiffusionCurves.Storage.SaveProject.Save(this.givenprojectstate, this.framescontainer);

                        //Set FILENAME and Path in mainwindow
                        stateeventargs.DESTINATION = destination;
                        stateeventargs.FILENAME = filename;

                        //Set ACTIVE and SAVED in mainwindow
                        stateeventargs.ACTIVE = true;
                        stateeventargs.SAVED = true;

                        //Return values to caller
                        if (ProjectSaved != null)
                            ProjectSaved(this, stateeventargs);

                        //Dispose NewProject dialog
                        this.Close();

                        return;
                    }
                    catch (Exception ex)
                    {
                        Prompt("Application error", ex.Message);
                        return;
                    }
                }
            }
        }

        #region Auxillary methods

        /// <summary>
        /// Method to Prompt user with information.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        public void Prompt(string title, string message)
        {
            System.Windows.MessageBox.Show(message,
                title, System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information);
        }

        #endregion

        #region GUI events

        /// <summary>
        /// Change background and text on keyboardfocus: filename.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Filename_textbox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (Filename_textbox.Text.Equals("<name or leave empty>"))
            {
                Filename_textbox.Foreground = Brushes.White;
                Filename_textbox.Text = string.Empty;
            }
        }

        /// <summary>
        /// Change background and text on keyboardfocus: destination.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Destination_textbox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (Destination_textbox.Text.Equals(@"\<dest. or leave empty>"))
            {
                Destination_textbox.Foreground = Brushes.White;
                Destination_textbox.Text = @"\";
            }
        }

        #endregion GUI events
    }
}
