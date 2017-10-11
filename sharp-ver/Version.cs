using System.Text.RegularExpressions;

namespace sharp_ver
{
    public class Version
    {
        // Constructors

        public Version() { }

        public Version(int major, int minor, int patch)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
        }

        public Version(string version)
        {
            int major = 0, minor = 0, patch = 0;
            var match = Regex.Match(version, @"((\d+)\.(\d+)\.(\d+))");

            if (match.Success)
            {
                int.TryParse(match.Groups[2].Value, out major);
                int.TryParse(match.Groups[3].Value, out minor);
                int.TryParse(match.Groups[4].Value, out patch);
            }

            Major = major;
            Minor = minor;
            Patch = patch;
        }

        public Version(System.Version version) : this(version.ToString()) { }

        // Properties

        public int Major { get; }

        public int Minor { get; }

        public int Patch { get; }

        public override string ToString() => $"{Major}.{Minor}.{Patch}";

        // 

        public bool Empty => Major == 0 && Minor == 0 && Patch == 0;
    }
}