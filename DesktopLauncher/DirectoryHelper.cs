using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLauncher
{
    public static class DirectoryHelper
    {
        public static string[] GetFilesEx(string path, string _searchPattern, SearchOption searchOption)
        {
            string[] searchPatterns = _searchPattern.Split('|');
            List<string> files = new List<string>();
            foreach (var searchPattern in searchPatterns)
            {
                files.AddRange(System.IO.Directory.GetFiles(path, searchPattern, searchOption));
            }

            files.Sort();
            return files.ToArray();
        }
    }
}
