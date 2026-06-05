namespace InterReact;

[Message]
public sealed record PnL : IHasRequestId
{
    public int RequestId { get; init; }
    public double DailyPnL { get; init; }
    public double UnrealizedPnL { get; init; }
    public double RealizedPnL { get; init; }
    internal PnL() { }
    internal PnL(ResponseReader r)
    {
        RequestId = r.ReadInt();
        DailyPnL = r.ReadDouble();
        UnrealizedPnL = r.ReadDouble();
        RealizedPnL = r.ReadDouble();
    }
}

[Message]
public sealed record PnLSingle : IHasRequestId
{
    public int RequestId { get; init; }
    public decimal Pos { get; init; }
    public double DailyPnL { get; init; }
    public double UnrealizedPnL { get; init; }
    public double RealizedPnL { get; init; }
    public double Value { get; init; }
    internal PnLSingle() { }
    internal PnLSingle(ResponseReader r)
    {
        RequestId = r.ReadInt();
        Pos = r.ReadDecimal();
        DailyPnL = r.ReadDouble();
        UnrealizedPnL = r.ReadDouble();
        RealizedPnL = r.ReadDouble();
        Value = r.ReadDouble();
    }
}
