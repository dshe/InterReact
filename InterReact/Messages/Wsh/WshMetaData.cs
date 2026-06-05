namespace InterReact;

[Message]
public sealed record WshMetaData : IHasRequestId
{
    public int RequestId { get; }
    public string Data { get; }
    internal WshMetaData() => Data = "";
    internal WshMetaData(ResponseReader r)
    {
        RequestId = r.ReadInt();
        Data = r.ReadString();
    }
}
