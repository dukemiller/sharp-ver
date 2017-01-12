using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace sharp_ver
{
    public static class Program
    {
        /// <summary>
        ///     The directory where this program is being called from
        /// </summary>
        private static string ExecutingDirectory => Directory.GetCurrentDirectory();

        /// <summary>
        ///     The pattern in assemblyinfo that lists the version
        /// </summary>
        private const string Pattern = @"^\[assembly: Assembly.+\(""(([0-9]\.){2}[0-9](?=\.?))";

        /// <summary>
        ///     The pattern for finding words that represent reducing from a number
        /// </summary>
        private const string SubtractPattern = @"^s|^red|^dec";

        /// <summary>
        ///     The pattern for finding words that represent adding to a number
        /// </summary>
        private const string AddPattern = @"^a|^inc";

        /// <summary>
        ///     Verify and validate the given arguments
        /// </summary>
        private static ParseResult VerifyArgs(ParseArgs args)
        {
            // The result is only successful if it gets to the end
            var result = new ParseResult {Successful = false};

            // Path
            if (!Directory.Exists(ExecutingDirectory))
            {
                result.ErrorMessage = "This is somehow called in a directory that does not exist.";
                return result;
            }
            var sln = Directory.GetFiles(ExecutingDirectory).FirstOrDefault(f => f.EndsWith(".sln"));
            if (sln == null)
            {
                result.ErrorMessage = "Current executing folder is not a solution directory.";
                return result;
            }
            var folder = sln.Split(Path.DirectorySeparatorChar).Last().Split('.').Reverse().Skip(1).First();
            var path = Path.GetFullPath(Path.Combine(ExecutingDirectory, folder, "Properties", "AssemblyInfo.cs"));
            if (!File.Exists(path))
            {
                result.ErrorMessage = "The path to AssemblyInfo.cs is not clear.";
                return result;
            }
            result.Path = path;

            // Tier
            if (args.Tier.Equals("minor"))
                result.VersionTier = SemanticVersionTier.Minor;
            else if (args.Tier.Equals("patch"))
                result.VersionTier = SemanticVersionTier.Patch;
            else if (args.Tier.Equals("major"))
                result.VersionTier = SemanticVersionTier.Major;
            else
            {
                result.ErrorMessage = "Incorrect input for argument \"version-tier\".";
                return result;
            }

            // Action
            if (Regex.IsMatch(args.Action, AddPattern))
                result.Action = SemanticAction.Increase;
            else if (Regex.IsMatch(args.Action, SubtractPattern))
                result.Action = SemanticAction.Decrease;
            else
            {
                result.ErrorMessage = "Incorrect input for argument \"action\".";
                return result;
            }

            result.Successful = true;
            return result;
        }

        /// <summary>
        ///     Exit the console application, printing out the given message
        /// </summary>
        private static void ExitMessage(string message)
        {
            Console.WriteLine(message);
            System.Environment.Exit(1);
        }

        /// <summary>
        ///     Do a 'git add */AssemblyInfo.cs' on the given path
        /// </summary>
        private static void Git(string path)
        {
            var info = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = "add */AssemblyInfo.cs",
                UseShellExecute = false,
                WorkingDirectory = path,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            var process = new Process
            {
                StartInfo = info
            };

            process.Start();
        }

        public static void Main(string[] args)
        {
            const string argExample = "Arguments: version-tier [action]\n" +
                                      "--version-tier  = patch | minor | major           ->  the semvar version tier to update\n" +
                                      "--action        = add/increase | reduce/decrease  ->  the action to perform on that tier\n" +
                                      "                  [by default will add]";
            var notCorrectArgs = $"Incorrect number of arguments. {argExample}";

            // get args

            if (args.Length == 0)
                ExitMessage(argExample);

            else if (args.Length > 2)
                ExitMessage(notCorrectArgs);

            var result = VerifyArgs(new ParseArgs(args));

            if (!result.Successful)
                ExitMessage(result.ErrorMessage);

            // Find and replace the lines

            var lines = File.ReadAllLines(result.Path).Select(line =>
            {
                var find = Regex.Matches(line, Pattern).Cast<Match>().ToList();
                if (find.Count > 0)
                {
                    var group = find.First().Groups.Cast<Group>().Skip(1).First().ToString();
                    var version = new SemanticVersion(group);
                    version.DoCommand(result.VersionTier, result.Action);
                    line = line.Replace(group, version.ToString());
                }
                return line;
            });

            // Write and add to git

            File.WriteAllLines(result.Path, lines);
            Git(ExecutingDirectory);
        }
    }
}
