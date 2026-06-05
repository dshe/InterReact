namespace InterReact;

[Message]
public sealed record AccountPositionsMulti : IHasRequestId
{
    public int RequestId { get; init; }
    public bool IsEndMessage { get; init; }
    public string Account { get; init; } = "";
    public string ModelCode { get; init; } = "";
    public Contract Contract { get; init; }
    public decimal Position { get; init; }
    public double AverageCost { get; init; }
    internal AccountPositionsMulti() => Contract = new();
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
