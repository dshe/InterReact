namespace InterReact;

[Message]
public sealed record WshMetaData : IHasRequestId
{
    public int RequestId { get; init; }
    public string Data { get; init; } = "";
    internal WshMetaData() { }
    internal WshMetaData(ResponseReader r)
    {
        RequestId = r.ReadInt();
        Data = r.ReadString();
    }
}
