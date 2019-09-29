using System;
using System.IO;
using Image.Base;
using OperatingSystemCore;
using System.Collections.Generic;
using System.Linq;

namespace iorg
{
    public class Program
    {
        static void Main(string[] args)
        {
            #region check for proper calling
            if (args.Length < 1 || args.Length > 4)
            {
                Console.WriteLine($"usage: [filter=<filter>] [remove=<removeString>] [from=<sourceDirectory>] [to=<destDirectory>]");
                Console.WriteLine("examples:");
                Console.WriteLine("\tiorg filter=*.png from=. to=.");
                Console.WriteLine("\tiorg filter=*.jpeg from=. to=..");
                return;
            }
            #endregion
            #region get all options, named and unnamed params
            var options = from _ in args where _[0] == '-' select _;
            var namedVars = from _ in args where _.Contains("=") select _;
            var unnamedVar = from _ in args where !(_[0] == '-' || _.Contains("=")) select _;
            #endregion
            var p = new Dictionary<string, string>();
            #region set named parameters to dictionary
            foreach (var elem in namedVars)
            {
                var list = elem.Split('=');
                p.Add(list[0], list[1]);
            }
            #endregion
            #region set all params for the rename call
            if (!p.ContainsKey("to") || string.IsNullOrEmpty(p["to"]) || p["to"] == ".")
                p["to"] = Directory.GetCurrentDirectory();
            if (!p.ContainsKey("from") || string.IsNullOrEmpty(p["from"]) || p["from"] == ".")
                p["from"] = Directory.GetCurrentDirectory();
            if (!p.ContainsKey("filter") || string.IsNullOrEmpty(p["filter"]))
                p["filter"] = "*.jpg";
            if (!p.ContainsKey("remove") || string.IsNullOrEmpty(p["remove"]))
                p["remove"] = "";
            #endregion
            var count = ImageTreeFile.MoveToTree(
                    fromDir: p["from"], 
                    toDirRoot: p["to"], 
                    filter: p["filter"], 
                    remove: p["remove"]); 
            Console.WriteLine($"{count} files moved.");
        }
    }
}
