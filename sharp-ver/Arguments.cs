using sharp_ver.Enums;

namespace sharp_ver
{
    public class Arguments
    {
        public Tier Tier { get; set; }
        public Action Action { get; set; }
        public Version Version { get; set; }
        public bool DisplayHelp { get; set; }
    }
}