namespace InterReact;

public sealed class HeadTimestamp : IHasRequestId // output
{
    public int RequestId { get; }
    public string HeadTimeStamp { get; } = "";

    internal HeadTimestamp() { }

    internal HeadTimestamp(ResponseReader r)
    {
        RequestId = r.ReadInt();
        HeadTimeStamp = r.ReadString();
    }
}
