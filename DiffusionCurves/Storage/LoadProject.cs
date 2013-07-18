using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiffusionCurves.Model;
using DiffusionCurves.Auxillary;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Windows.Media;
using DiffusionCurves.Events;

namespace DiffusionCurves.Storage
{
    public static class LoadProject
    {
        //Event delegate
        public delegate void OpenProjectEventHandler(object sender, StateEventArgs e);
        //Make event with delegate structure
        public static event OpenProjectEventHandler LoadSaveEvent;
        static String destfile;

        /// <summary>
        /// Gets called when a .dcip needs to be loaded into memory.
        /// </summary>
        public static void Load()
        {
            // Configure open file dialog box
            System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();
            fileDialog.Filter = "DCIP Files|*.dcip";
            fileDialog.Title = "Choose project to open: ";
            System.Windows.Forms.DialogResult result = fileDialog.ShowDialog();

            //Save user filename
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                destfile = fileDialog.FileName;

                BinaryFormatter formatter = new BinaryFormatter();

                StreamReader framesOpen = null, stateOpen = null;
                FramesContainer container = null;
                ProjectState projectState = null;

                string destfilenoext = destfile.Substring(0, destfile.Length - 5);

                try
                {
                    LoadSave.UnZipFolder(destfilenoext);

                    framesOpen = new StreamReader(destfilenoext + @"\data.frames");
                    stateOpen = new StreamReader(destfilenoext + @"\data.state");

                    container = (FramesContainer)formatter.Deserialize(framesOpen.BaseStream);
                    projectState = (ProjectState)formatter.Deserialize(stateOpen.BaseStream);

                    stateOpen.Close();
                    framesOpen.Close();

                    LoadSave.DeleteOriginalFolder(destfilenoext);
                }
                catch (SerializationException e)
                {
                    Prompt("Application error", e.Message);
                    return;
                }

                //Event to fire back
                StateEventArgs stateeventargs = new StateEventArgs();

                stateeventargs.Framescontainer = container;
                stateeventargs.Projectstate = projectState;

                //Return values to caller
                // Check if there are any Subscribers
                if (LoadSaveEvent != null)
                    LoadSaveEvent(null, stateeventargs);
            }
        }

        #region Auxilliary

        /// <summary>
        /// Method to Prompt user with information.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        public static void Prompt(string title, string message)
        {
            System.Windows.MessageBox.Show(message,
                title, System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information);
        }

        #endregion

    }
}
