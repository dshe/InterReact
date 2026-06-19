namespace InterReact;

[Message]
public sealed record UserInfo : IHasRequestId
{
    public int RequestId { get; init; }
    public string WhiteBrandingId { get; init; } = "";
    internal UserInfo() { }
    internal UserInfo(ResponseReader r)
    {
        RequestId = r.ReadInt();
        WhiteBrandingId = r.ReadString();
    }
}
