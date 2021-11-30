namespace InterReact;

public sealed class ScannerParameters
{
    public string Parameters { get; } = "";

    internal ScannerParameters() { }

    internal ScannerParameters(ResponseReader r)
    {
        r.IgnoreVersion();
        Parameters = r.ReadString();
    }
}
