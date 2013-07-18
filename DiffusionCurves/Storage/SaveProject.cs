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

namespace DiffusionCurves.Storage
{
    /// <summary>
    /// This class saves the given projectstate:
    /// - Images;
    /// - Curves.
    /// </summary>
    public static class SaveProject
    {
        /// <summary>
        /// Gets called when a project needs to be saved to the disk.
        /// </summary>
        /// <param name="givenprojectstate"></param>
        /// <returns>void</returns>
        public static void Save(ProjectState givenprojectstate, FramesContainer framescontainer)
        {
            string pathstring = ValidateInput.PathString(givenprojectstate.FileName, givenprojectstate.Destination);
            
            LoadSave.UnZipFolder(pathstring);

            File.Delete(pathstring + ".dcip");

            WriteData(pathstring, givenprojectstate, framescontainer);

            LoadSave.ZipFolderRenameToDCIP(pathstring);

            LoadSave.DeleteOriginalFolder(pathstring);
        }

        #region Auxilliary methods

        /// <summary>
        ///// Write data to file.
        /// </summary>
        /// <param name="pathstring"></param>
        /// <param name="givenprojectstate"></param>
        /// <param name="framescontainer"></param>
        public static void WriteData(string pathstring, ProjectState givenprojectstate, FramesContainer framescontainer)
        {
            StreamWriter frames = new StreamWriter(pathstring + @"\data.frames");
            StreamWriter state = new StreamWriter(pathstring + @"\data.state");

            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(frames.BaseStream, framescontainer);
                formatter.Serialize(state.BaseStream, givenprojectstate);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                throw;
            }

            frames.Close();
            state.Close();
        }

        #endregion
    }
}
