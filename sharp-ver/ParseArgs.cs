using System.Collections.Generic;
using System.Linq;

namespace sharp_ver
{
    public class ParseArgs
    {
        public ParseArgs(IEnumerable<string> args)
        {
            var argList = args.Select(a => a.ToLower()).ToList();
            Tier = argList[0];
            Action = argList.Count > 1 ? argList[1] : "add";
        }

        public string Tier { get; set; }
        public string Action { get; set; }
    }
}