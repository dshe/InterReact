namespace InterReact;

public sealed class AccountUpdateMulti : IHasRequestId
{
    public int RequestId { get; }
    public bool IsEndMessage { get; }
    public string Account { get; }
    public string ModelCode { get; }
    public string Key { get; }
    public string Currency { get; }
    public string Value { get; }
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
