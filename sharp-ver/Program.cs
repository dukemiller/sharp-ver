using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using sharp_ver.Enums;
using Action = sharp_ver.Enums.Action;

namespace sharp_ver
{
    public static class Program
    {
        private static string ExecutingDirectory => Directory.GetCurrentDirectory();

        private static readonly Regex HelpPattern = 
            new Regex(@"--help|/\?|-h", RegexOptions.IgnoreCase);

        private static readonly Regex SubtractPattern = 
            new Regex(@"^(?:s|red|dec|-)", RegexOptions.IgnoreCase);

        private static readonly Regex AddPattern = 
            new Regex(@"^(?:a(?:dd)?|inc|\+)", RegexOptions.IgnoreCase);

        private static readonly Regex AssemblyInfoPattern =
            new Regex(@"((?:\d+\.){2}(?:\d+))(?:\.?)", RegexOptions.Multiline);

        // 

        public static void Main(string[] args)
        {
            if (args.Length == 0)
                NoArgument();

            else
            {
                Result<Arguments> result;

                switch (args.Length)
                {
                    case 1:
                        result = SingleArgument(args[0]);
                        break;
                    case 2:
                        result = DoubleArgument(args[0], args[1]);
                        break;
                    default:
                        Console.WriteLine("Invalid number of arguments.");
                        return;
                }

                EnactAction(result);
            }
        }

        // 

        public static void NoArgument()
        {
            var path = RetrieveAssemblyInfoPath();
            if (path.Successful)
            {
                var version = RetrieveVersion(path.Value);
                if (version.Successful)
                    Console.WriteLine(version.Value);
                else
                    Console.WriteLine(version.ErrorMessage);
            }
            else
                Console.WriteLine(path.ErrorMessage);
        }

        public static Result<Arguments> SingleArgument(string argument)
        {
            if (HelpPattern.IsMatch(argument))
                return new Result<Arguments> { Value = new Arguments { DisplayHelp = true } };

            var tier = ValueOrDefault<Tier>(argument);

            if (tier == null)
            {
                var version = new Version(argument);
                return version.Empty
                    ? new Result<Arguments> { ErrorMessage = "Incorrect first argument given.", Successful = false }
                    : new Result<Arguments> { Value = new Arguments { Action = Action.Set, Version = version } };
            }

            // Default action: Increase
            return new Result<Arguments> { Value = new Arguments { Tier = tier.Value, Action = Action.Increase } };

        }

        public static Result<Arguments> DoubleArgument(string argument1, string argument2)
        {
            var tier = ValueOrDefault<Tier>(argument1);

            if (tier == null)
            {
                return new Result<Arguments>
                {
                    ErrorMessage = "Incorrect first argument given.",
                    Successful = false
                };
            }

            Action action;

            if (AddPattern.IsMatch(argument2))
                action = Action.Increase;
            else if (SubtractPattern.IsMatch(argument2))
                action = Action.Decrease;
            else
                return new Result<Arguments> { Successful = false, ErrorMessage = "Incorrect second argument given." };

            return new Result<Arguments> { Value = new Arguments { Action = action, Tier = tier.Value } };
        }

        // 

        public static T? ValueOrDefault<T>(string value, T? @default = null) where T : struct
        {
            return Enum.TryParse(value, true, out T result) ? result : @default;
        }

        public static void ChangeVersion(Version current, Version newest, string path)
        {
            var text = File.ReadAllLines(path);
            var ct = current.ToString();
            var nt = newest.ToString();
            for (var i = 0; i < text.Length; i++)
                if (!text[i].StartsWith("//"))
                    text[i] = text[i].Replace(ct, nt);
            File.WriteAllLines(path, text);
        }

        public static Version CreateNewVersion(Version version, Action action, Tier tier)
        {
            int Applier(int number) => action == Action.Increase
                ? number + 1
                : action == Action.Decrease
                    ? Math.Max(number - 1, 0)
                    : number;

            switch (tier)
            {
                case Tier.Patch:
                    return new Version(version.Major, version.Minor, Applier(version.Patch));
                case Tier.Minor:
                    return new Version(version.Major, Applier(version.Minor), 0);
                case Tier.Major:
                    return new Version(Applier(version.Major), 0, 0);
                default:
                    return version;
            }
        }

        public static Result<string> RetrieveAssemblyInfoPath()
        {
            // Path
            if (!Directory.Exists(ExecutingDirectory))
                return new Result<string>
                {
                    Successful = false,
                    ErrorMessage = "This is somehow called in a directory that does not exist."
                };

            var sln = Directory.GetFiles(ExecutingDirectory).FirstOrDefault(f => f.EndsWith(".sln"));
            if (sln == null)
                return new Result<string>
                {
                    Successful = false,
                    ErrorMessage = "Current executing folder is not a solution directory."
                };


            var folder = sln.Split(Path.DirectorySeparatorChar).Last().Split('.').Reverse().Skip(1).First();
            var path = Path.GetFullPath(Path.Combine(ExecutingDirectory, folder, "Properties", "AssemblyInfo.cs"));
            if (!File.Exists(path))
                return new Result<string>
                {
                    Successful = false,
                    ErrorMessage = "The path to AssemblyInfo.cs is not clear."
                };

            return new Result<string> { Value = path };

        }

        public static Result<Version> RetrieveVersion(string path)
        {
            var match = AssemblyInfoPattern.Match(File.ReadAllText(path));
            return match.Success
                ? new Result<Version> { Value = new Version(match.Groups[1].Value) }
                : new Result<Version> { Successful = false, ErrorMessage = "Version could not be found." };
        }

        private static void AddToGit()
        {
            var info = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = "add */AssemblyInfo.cs",
                UseShellExecute = false,
                WorkingDirectory = ExecutingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            var process = new Process
            {
                StartInfo = info
            };

            process.Start();
        }
        
        private static void EnactAction(Result<Arguments> parsedArgumentResult)
        {
            if (parsedArgumentResult.Successful)
            {
                if (parsedArgumentResult.Value.DisplayHelp)
                    DisplayHelp();

                else
                {
                    var path = RetrieveAssemblyInfoPath();
                    if (path.Successful)
                    {

                        var current = RetrieveVersion(path.Value);
                        if (current.Successful)
                        {
                            var newest = parsedArgumentResult.Value.Action == Action.Set
                                ? parsedArgumentResult.Value.Version
                                : CreateNewVersion(current.Value, parsedArgumentResult.Value.Action,
                                    parsedArgumentResult.Value.Tier);
                            ChangeVersion(current.Value, newest, path.Value);
                            AddToGit();
                            Console.WriteLine($"{current.Value} -> {newest}");
                        }

                        else
                            Console.WriteLine(current.ErrorMessage);
                    }

                    else
                        Console.WriteLine(path.ErrorMessage);
                }
            }

            else
                Console.WriteLine(parsedArgumentResult.ErrorMessage);
        }

        private static void DisplayHelp()
        {
            Console.WriteLine("Modify the version in AssemblyInfo.cs in different ways.\n");
            Console.WriteLine("-- Usage 1: sv\n" +
                              "Display the current version.\n"
            );
            Console.WriteLine("-- Usage 2: sv {tier} {action}\n" +
                              "{tier}     = patch | minor | major     -> the semvar tier to change\n" +
                              "{action}   = increase | decrease       -> the action to perform on the given tier\n\n"+
                              "Apply {action} on {tier}. If no {action} is given, by default use increase.\n"
            );
            Console.WriteLine("-- Usage 3: sv {version}\n" +
                              "{version}  = (x.y.z)                   -> a numeric version\n\n" +
                              "Change the current version to the given {version}."
            );
        }
    }
}