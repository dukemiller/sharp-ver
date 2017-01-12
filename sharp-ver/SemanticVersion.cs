using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace sharp_ver
{
    public class SemanticVersion
    {
        private int _major;

        private int _minor;

        private int _patch;

        public int Major
        {
            get { return _major; }
            set
            {
                value = Math.Max(value, 0);
                if (value > _major)
                    Minor = 0;
                _major = value;
            }
        }

        public int Minor
        {
            get { return _minor; }
            set
            {
                value = Math.Max(value, 0);
                if (value > _minor)
                    Patch = 0;
                _minor = value;
            }
        }

        public int Patch
        {
            get { return _patch; }
            set
            {
                value = Math.Max(value, 0);
                _patch = value;
            }
        }

        public SemanticVersion() { }

        public SemanticVersion(string text)
        {
            var split = Regex.Split(text, @"\.");
            Major = int.Parse(split[0]);
            Minor = int.Parse(split[1]);
            if (split[2].All(char.IsNumber))
                Patch = int.Parse(split[2]);
        }

        public void DoCommand(SemanticVersionTier versionTier, SemanticAction action)
        {
            if (versionTier == SemanticVersionTier.Patch)
                if (action == SemanticAction.Increase)
                    Patch++;
                else
                    Patch--;

            else if (versionTier == SemanticVersionTier.Minor)
                if (action == SemanticAction.Increase)
                    Minor++;
                else
                    Minor--;

            else if (versionTier == SemanticVersionTier.Major)
                if (action == SemanticAction.Increase)
                    Major++;
                else
                    Minor--;
        }

        public override string ToString() => $"{Major}.{Minor}.{Patch}";
    }
}