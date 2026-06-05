namespace InterReact;

[Message]
public sealed record WshEventDataReceived : IHasRequestId // output
{
    public int RequestId { get; }
    public string Data { get; }

    internal WshEventDataReceived(ResponseReader r)
    {
        RequestId = r.ReadInt();
        Data = r.ReadString();
    }
}
