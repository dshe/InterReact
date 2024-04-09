namespace InterReact;

public sealed class AccountPositionMulti : IHasRequestId
{
    public int RequestId { get; }
    public string Account { get; }
    public Contract Contract { get; }
    public decimal Position { get; }
    public double AvgCost { get; }
    public string ModelCode { get; }
    internal AccountPositionMulti(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        RequestId = r.ReadInt();
        Account = r.ReadString();
        Contract = new(r, includePrimaryExchange: false);
        Position = r.ReadDecimal();
        AvgCost = r.ReadDouble();
        ModelCode = r.ReadString();
    }
}

public sealed class AccountPositionMultiEnd : IHasRequestId
{
    public int RequestId { get; }
    internal AccountPositionMultiEnd(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        RequestId = r.ReadInt();
    }
}
