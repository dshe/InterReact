namespace InterReact;

[Message]
public sealed record NewsBulletin
{
    public int MessageId { get; init; }

    public NewsBulletinType Type { get; init; }

    public string Message { get; init; } = "";

    /// <summary>
    /// The exchange from which this message originated.
    /// </summary>
    public string Origin { get; init; } = "";
    internal NewsBulletin() { }
    internal NewsBulletin(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        MessageId = r.ReadInt();
        Type = r.ReadEnum<NewsBulletinType>();
        Message = r.ReadString();
        Origin = r.ReadString();
    }
}
