using System.Collections.Generic;
using System.Linq;

namespace sharp_ver
{
    public class ParseArgs
    {
        public ParseArgs(IEnumerable<string> args)
        {
            var argList = args.Select(a => a.ToLower()).ToList();
            Path = argList[0];
            Tier = argList[1];
            Action = argList[2];
        }

        public string Path { get; set; }
        public string Tier { get; set; }
        public string Action { get; set; }
    }
}