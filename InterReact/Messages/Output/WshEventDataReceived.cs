namespace InterReact;

public sealed class WshEventDataReceived : IHasRequestId // output
{
    public int RequestId { get; }
    public string Data { get; }

    internal WshEventDataReceived(ResponseReader reader)
    {
        RequestId = reader.ReadInt();
        Data = reader.ReadString();
    }
}
