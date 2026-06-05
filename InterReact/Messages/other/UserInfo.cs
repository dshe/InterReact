namespace InterReact;

[Message]
public sealed record UserInfo : IHasRequestId
{
    public int RequestId { get; }
    public string WhiteBrandingId { get; }

    internal UserInfo(ResponseReader r)
    {
        RequestId = r.ReadInt();
        WhiteBrandingId = r.ReadString();
    }
}
