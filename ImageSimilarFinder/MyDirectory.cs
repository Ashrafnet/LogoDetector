using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;


    public static class MyDirectory
    {

        // Takes same patterns, and executes in parallel
        public static IEnumerable<string> GetFiles(string path, string[] searchPatterns)
        {


            var v = searchPatterns.AsParallel()
        .SelectMany(searchPattern =>
               Directory.EnumerateFiles(path, searchPattern, SearchOption.AllDirectories));


            return v;
        }
    }



