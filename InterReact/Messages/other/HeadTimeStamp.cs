namespace InterReact;

[Message]
public sealed record HeadTimestamp : IHasRequestId
{
    public int RequestId { get; init; }
    public string HeadTimeStamp { get; init; } = "";
    internal HeadTimestamp() { }
    internal HeadTimestamp(ResponseReader r)
    {
        RequestId = r.ReadInt();
        HeadTimeStamp = r.ReadString();
    }
}
