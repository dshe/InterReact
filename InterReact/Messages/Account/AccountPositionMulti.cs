namespace InterReact;

[Message]
public sealed record AccountPositionMulti : IHasRequestId
{
    public int RequestId { get; init; }
    public bool IsEndMessage { get; init; }
    public string Account { get; init; } = "";
    public string ModelCode { get; init; } = "";
    public Contract Contract { get; } = new();
    public decimal Position { get; init; }
    public double AverageCost { get; init; }
    internal AccountPositionMulti() { }
    internal AccountPositionMulti(ResponseReader r, bool isEndMessage)
    {
        r.IgnoreMessageVersion();
        RequestId = r.ReadInt();
        IsEndMessage = isEndMessage;
        if (isEndMessage)
            return;
        Account = r.ReadString();
        Contract.Read(r, includePrimaryExchange: false);
        Position = r.ReadDecimal();
        AverageCost = r.ReadDouble();
        ModelCode = r.ReadString();
    }
}
