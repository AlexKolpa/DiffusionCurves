using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using System.Text.RegularExpressions;
using System.IO;

namespace DiffusionCurves.Auxillary
{
    /// <summary>
    /// Auxillary class for NewProject that provides filename checks.
    /// </summary>
    public static class ValidateInput
    {
        /// <summary>
        /// Check if the file name is a valid Windows file name.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>bool</returns>
        public static bool ValidFileName(string filename)
        {
            return !(filename.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) != -1);
        }

        /// <summary>
        /// Check if the destination is a valid Windows destination.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>bool</returns>
        public static bool ValidDestination(string destination)
        {
            return !(destination.IndexOfAny(System.IO.Path.GetInvalidPathChars()) != -1);

        }

        /// <summary>
        /// Check if the user provided a file name.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>bool</returns>
        public static bool EmptyFilename(string filename)
        {
            if (filename.Equals("<name or leave empty>") || filename.Equals("") || filename.Equals("<empty>"))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Check if the user provided a destination.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>bool</returns>
        public static bool EmptyDestination(string filename)
        {
            if (filename.Equals(@"\<dest. or leave empty>") || filename.Equals(""))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Combines filename and destination.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="destination"></param>
        /// <returns>string</returns>
        public static string PathString(string filename, string destination)
        {
            return Path.Combine(destination, filename);
        }

        /// <summary>
        /// Check if the file name the user provided is already taken.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="destination"></param>
        /// <returns>bool</returns>
        public static bool FileNameExists(string filename, string destination)
        {
            if (System.IO.File.Exists(PathString(filename, destination) + ".dcip"))
                return true;
            else return false;
        }

        /// <summary>
        /// Checks if the input is correct and converts it to a double.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>Tuple<bool, double></returns>
        public static Tuple<bool, double> ValidateAndConvertToDouble(string input)
        {
            try
            {
                double result = Convert.ToDouble(input);

                if(result < 0)
                    return new Tuple<bool, double>(false, Double.MinValue);
                else
                    return new Tuple<bool, double>(true, result);
            }
            catch
            {
                return new Tuple<bool, double>(false, Double.MinValue);
            }
        }

        /// <summary>
        /// Validate if the input is int and convert it.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Tuple<bool, int> ValidateAndConvertToInt(string input)
        {
            try
            {
                int result = Convert.ToInt32(input);

                if (result < 0)
                    return new Tuple<bool, int>(false, 0);
                else
                    return new Tuple<bool, int>(true, result);
            }
            catch
            {
                return new Tuple<bool, int>(false, 0);
            }
        }
    }
}
