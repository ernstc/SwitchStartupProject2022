using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SwitchStartupProject
{
    public static class Paths
    {
        /// <summary>
        /// Convert an absolute file path to a relative file path relative to a given folder path
        /// </summary>
        /// <remarks>
        /// Slightly adapted source code from http://stackoverflow.com/questions/703281/getting-path-relative-to-the-current-working-directory/703290#703290
        /// </remarks>
        public static string GetPathRelativeTo(string filespec, string folder)
        {
            var pathUri = new Uri(filespec);
            // Folders must end in a slash
            if (!folder.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                folder += Path.DirectorySeparatorChar;
            }
            var folderUri = new Uri(folder);
            return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
        }
    }
}
