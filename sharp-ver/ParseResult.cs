namespace sharp_ver
{
    public class ParseResult
    {
        public override string ToString() => $"Result(Action={Action}, Tier={VersionTier}, Path={Path}, Successful={Successful})";

        // Success Related
        public bool Successful { get; set; }
        public string ErrorMessage { get; set; }

        // Data
        public string Path { get; set; }
        public SemanticVersionTier VersionTier { get; set; }
        public SemanticAction Action { get; set; }
    }
}