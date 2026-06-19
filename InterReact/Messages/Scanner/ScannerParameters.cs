namespace InterReact;

[Message]
public sealed class ScannerParameters
{
    public string Parameters { get; init; } = "";
    internal ScannerParameters() { }
    internal ScannerParameters(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        Parameters = r.ReadString();
    }
}
