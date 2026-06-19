namespace InterReact;

[Message]
public sealed record WshEventDataReceived : IHasRequestId // output
{
    public int RequestId { get; init; }
    public string Data { get; init; } = "";
    internal WshEventDataReceived() { }
    internal WshEventDataReceived(ResponseReader r)
    {
        RequestId = r.ReadInt();
        Data = r.ReadString();
    }
}
