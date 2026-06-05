namespace InterReact;

[Message]
public sealed record AccountUpdateMulti : IHasRequestId
{
    public int RequestId { get; init; }
    public bool IsEndMessage { get; init; }
    public string Account { get; init; } = "";
    public string ModelCode { get; init; } = "";
    public string Key { get; init; } = "";
    public string Currency { get; init; } = "";
    public string Value { get; init; } = "";
    internal AccountUpdateMulti() { }
    internal AccountUpdateMulti(ResponseReader r, bool isEndMessage)
    {
        r.IgnoreMessageVersion();
        RequestId = r.ReadInt();
        IsEndMessage = isEndMessage;
        if (isEndMessage)
        {
            Account = "";
            ModelCode = "";
            Key = "";
            Value = "";
            Currency = "";
            return;
        }
        Account = r.ReadString();
        ModelCode = r.ReadString();
        Key = r.ReadString();
        Value = r.ReadString();
        Currency = r.ReadString();
    }
}
