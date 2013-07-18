using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.IO;
using System.Security.Permissions;
using System.Security;
using System.Reflection;
using System.Security.Principal;
using System.Security.AccessControl;
using DiffusionCurves.Auxillary;

namespace DiffusionCurves.Storage
{
    /// <summary>
    /// CreateProject is a static class that creates:
    /// - Folder;
    /// - Inits necessary files in the folder;
    /// - Compresses it;
    /// - Changes extension to .dcip.
    /// </summary>
    public static class CreateProject
    {
        /// <summary>
        /// Gets called when a project needs to be created.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="destination"></param>
        /// <returns>bool: went well/bad</returns>
        public static void Create(string filename, string destination)
        {
            string pathstring = ValidateInput.PathString(filename, destination);

            CreateFolder(pathstring);

            DiffusionCurves.Auxillary.LoadSave.ZipFolderRenameToDCIP(pathstring);

            DiffusionCurves.Auxillary.LoadSave.DeleteOriginalFolder(pathstring);
        }

        #region Auxillary methods

        /// <summary>
        /// Creates the directory to write to.
        /// </summary>
        /// <param name="pathstring"></param>
        public static void CreateFolder(string pathstring)
        {
            System.IO.Directory.CreateDirectory(pathstring);
        }

        #endregion
    }
}
