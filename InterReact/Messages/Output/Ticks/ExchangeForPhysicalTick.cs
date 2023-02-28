namespace InterReact;

public sealed class ExchangeForPhysicalTick : ITick
{
    public int RequestId { get; }
    public TickType TickType { get; } = TickType.Undefined;
    public double BasisPoints { get; }
    public string FormattedBasisPoints { get; }
    public double ImpliedFuturesPrice { get; }
    public int HoldDays { get; }
    public string FutureLastTradeDate { get; }
    public double DividendImpact { get; }
    public double DividendsToLastTradeDate { get; }
    internal ExchangeForPhysicalTick(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        RequestId = r.ReadInt();
        TickType = r.ReadEnum<TickType>();
        BasisPoints = r.ReadDouble();
        FormattedBasisPoints = r.ReadString();
        ImpliedFuturesPrice = r.ReadDouble();
        HoldDays = r.ReadInt();
        FutureLastTradeDate = r.ReadString();
        DividendImpact = r.ReadDouble();
        DividendsToLastTradeDate = r.ReadDouble();
    }
}
