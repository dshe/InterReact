namespace InterReact;

public sealed class AccountPositionsMulti : IHasRequestId
{
    public int RequestId { get; }
    public bool IsEndMessage { get; }
    public string Account { get; }
    public string ModelCode { get; }
    public Contract Contract { get; }
    public decimal Position { get; }
    public double AverageCost { get; }
    internal AccountPositionsMulti(ResponseReader r, bool isEndMessage)
    {
        r.IgnoreMessageVersion();
        RequestId = r.ReadInt();
        IsEndMessage = isEndMessage;
        if (isEndMessage)
        {
            Account = "";
            ModelCode = "";
            Contract = new();
            return;
        }
        Account = r.ReadString();
        Contract = new(r, includePrimaryExchange: false);
        Position = r.ReadDecimal();
        AverageCost = r.ReadDouble();
        ModelCode = r.ReadString();
    }
}
