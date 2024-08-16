namespace InterReact;

public sealed class NewsBulletin
{
    public int MessageId { get; }

    public NewsBulletinType Type { get; }

    public string Message { get; }

    /// <summary>
    /// The exchange from which this message originated.
    /// </summary>
    public string Origin { get; }

    internal NewsBulletin(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        MessageId = r.ReadInt();
        Type = r.ReadEnum<NewsBulletinType>();
        Message = r.ReadString();
        Origin = r.ReadString();
    }
}
