namespace InterReact;

public sealed class WshMetaData : IHasRequestId
{
    public int RequestId { get; }
    public string Data { get; }
    internal WshMetaData(ResponseReader r)
    {
        RequestId = r.ReadInt();
        Data = r.ReadString();
    }
}
