namespace sharp_ver
{
    public class ParseResult
    {
        public bool Successful { get; set; }
        public string Path { get; set; }
        public SemanticVersionTier VersionTier { get; set; }
        public SemanticAction Action { get; set; }
        public override string ToString() => $"Result(Action={Action}, Tier={VersionTier}, Path={Path}, Successful={Successful})";
    }
}