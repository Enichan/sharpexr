using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EXRViewer {
    static class CommandLineArguments {
        /// <summary>
        /// Parses command line arguments.
        /// </summary>
        /// <param name="arguments">String containing the arguments. Uses Environment.CommandLine if null.</param>
        /// <returns>List of arguments.</returns>
        public static List<string> Parse(string arguments = null) {
            Regex regex;

            if (arguments == null) {
                arguments = Environment.CommandLine;
            }
            List<string> args = new List<string>();

            regex = new Regex("\"[^\"]*\"+|[^\\s]+");

            foreach (Match match in regex.Matches(arguments)) {
                if (!String.IsNullOrWhiteSpace(match.Value)) {
                    string s = match.Value.Trim();
                    if (s.StartsWith("\""))
                        s = s.Substring(1);
                    if (s.EndsWith("\""))
                        s = s.Substring(0, s.Length - 1);
                    args.Add(s);
                }
            }

            args.RemoveAt(0);
            return args;
        }
    }
}
