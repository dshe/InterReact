namespace InterReact
{
    public sealed class ScannerParameters
    {
        public string Parameters { get; }
        internal ScannerParameters(ResponseReader c)
        {
            c.IgnoreVersion();
            Parameters = c.ReadString();
        }
    }
}
