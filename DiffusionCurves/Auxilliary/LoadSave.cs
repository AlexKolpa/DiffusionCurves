using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffusionCurves.Auxillary
{
    public static class LoadSave
    {
        /// <summary>
        /// Zips the created folder and changes the extenstion to .dcip.
        /// </summary>
        /// <param name="pathstring"></param>
        public static void ZipFolderRenameToDCIP(string pathstring)
        {
            ZipFile.CreateFromDirectory(@pathstring, @pathstring + ".dcip", CompressionLevel.NoCompression, false);
        }

        /// <summary>
        /// Deletes original folder.
        /// </summary>
        /// <param name="pathstring"></param>
        public static void DeleteOriginalFolder(string pathstring)
        {
            System.IO.Directory.Delete(@pathstring, true);
        }

        /// <summary>
        /// Unzip input folder.
        /// </summary>
        /// <param name="pathstring"></param>
        public static void UnZipFolder(string pathstring)
        {
            ZipFile.ExtractToDirectory(pathstring + ".dcip", pathstring);
        }
    }
}
