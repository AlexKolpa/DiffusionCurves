using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DiffusionCurves.Import
{
    /// <summary>
    /// This class handles the import of the images from the disk.
    /// </summary>
    public static class ImportImages
    {
        /// <summary>
        /// Gets called when images need to be imported.
        /// </summary>
        /// <param name="returnedtrue">The user clicked OK</param>
        /// <param name="filepaths">Absolute paths of the images</param>
        /// <param name="framestart">Index from where this import starts</param>
        /// <returns></returns>
        public static List<DiffusionCurves.Model.Frame> Import(bool returnedtrue, string[] filepaths, int framestart)
        {
            // Process filename if the user clicked OK.
            if (returnedtrue)
                return BuildFrames(filepaths, framestart);

            //User did not import
            return new List<DiffusionCurves.Model.Frame>();
        }

        /// <summary>
        /// Loads the images into memory and adds them to a list.
        /// </summary>
        /// <param name="images"></param>
        /// <param name="framestart"></param>
        /// <returns></returns>
        public static List<DiffusionCurves.Model.Frame> BuildFrames(string[] images, int framestart)
        {
            DiffusionCurves.Model.Frame frame;
            List<DiffusionCurves.Model.Frame> frameslist = new List<DiffusionCurves.Model.Frame>();

            int index = framestart;

            //Add images to memory
            foreach (String imageName in images)
            {
                //Import images only if path is correct.
                if (CheckPath(imageName))
                {
                    frame = new DiffusionCurves.Model.Frame(imageName);
                    frameslist.Add(frame);
                    index++;
                }
            }

            return frameslist;
        }

        /// <summary>
        /// Checks wether the path is correct.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool CheckPath(string path)
        {
            try
            {
              new System.IO.FileInfo(path);
              return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
