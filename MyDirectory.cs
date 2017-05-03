using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace LogoDetector
{
    public static class MyDirectory
    {   // Regex version
        public static IEnumerable<string> GetFiles(string path,
                            string searchPatternExpression = "",
                            SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            Regex reSearchPattern = new Regex(searchPatternExpression, RegexOptions.IgnoreCase);
            return Directory.EnumerateFiles(path, "*", searchOption)
                            .Where(file =>
                                     reSearchPattern.IsMatch(Path.GetExtension(file)));
        }

        // Takes same patterns, and executes in parallel
        public static IEnumerable<string> GetFiles(string path,
                            string[] searchPatterns, Dictionary<string, float> previuseLogs,
                            SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            Alphaleonis.Win32.Filesystem.DirectoryEnumerationOptions options = Alphaleonis.Win32.Filesystem.DirectoryEnumerationOptions.ContinueOnException | Alphaleonis.Win32.Filesystem.DirectoryEnumerationOptions.Files;
            if (searchOption == SearchOption.AllDirectories)
                options = Alphaleonis.Win32.Filesystem.DirectoryEnumerationOptions.ContinueOnException | Alphaleonis.Win32.Filesystem.DirectoryEnumerationOptions.Files | Alphaleonis.Win32.Filesystem.DirectoryEnumerationOptions.Recursive;

            var v = searchPatterns.AsParallel()
                     .SelectMany(searchPattern =>
                            Alphaleonis.Win32.Filesystem.Directory.EnumerateFiles
                            (path, searchPattern, options)).Where(x => !previuseLogs.ContainsKey(x));


            /*  var v = searchPatterns.AsParallel()
         .SelectMany(searchPattern =>
                Directory.EnumerateFiles(path, searchPattern, SearchOption.AllDirectories ).Where(x => !previuseLogs.ContainsKey(x)));
                */

            return v;
        }
    }


}
