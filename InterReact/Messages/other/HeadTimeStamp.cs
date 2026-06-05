namespace InterReact;

[Message]
public sealed record HeadTimestamp : IHasRequestId
{
    public int RequestId { get; }
    public string HeadTimeStamp { get; }

    internal HeadTimestamp(ResponseReader r)
    {
        RequestId = r.ReadInt();
        HeadTimeStamp = r.ReadString();
    }
}
