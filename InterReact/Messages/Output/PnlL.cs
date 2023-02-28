namespace InterReact;

public sealed class PnL : IHasRequestId
{
    public int RequestId { get; }
    public double DailyPnL { get; }
    public double UnrealizedPnL { get; }
    public double RealizedPnL { get; }

    internal PnL(ResponseReader r)
    {
        RequestId = r.ReadInt();
        DailyPnL = r.ReadDouble();
        UnrealizedPnL = r.ReadDouble();
        RealizedPnL = r.ReadDouble();
    }
}

public sealed class PnLSingle : IHasRequestId
{
    public int RequestId { get; }
    public decimal Pos { get; }
    public double DailyPnL { get; }
    public double UnrealizedPnL { get; }
    public double RealizedPnL { get; }
    public double Value { get; }

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
