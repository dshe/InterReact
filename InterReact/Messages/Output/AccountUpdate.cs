using NodaTime.Text;

namespace InterReact;

public sealed class AccountValue
{
    public string Key { get; }
    public string Value { get; }
    public string Currency { get; }
    public string AccountName { get; }
    internal AccountValue(ResponseReader r)
    {
        int version = r.GetMessageVersion();
        Key = r.ReadString();
        Value = r.ReadString();
        Currency = r.ReadString();
        AccountName = version >= 2 ? r.ReadString() : "";
    }
}

public sealed class PortfolioValue
{
    public string AccountName { get; init; }
    public Contract Contract { get; }
    public double Position { get; init; }
    public double MarketPrice { get; init; }
    public double MarketValue { get; init; }
    public double AverageCost { get; init; }
    public double UnrealizedPnl { get; init; }
    public double RealizedPnl { get; init; }
    internal PortfolioValue(ResponseReader r)
    {
        r.RequireMessageVersion(8);
        Contract = new(r);
        Position = r.ReadDouble();
        MarketPrice = r.ReadDouble();
        MarketValue = r.ReadDouble();
        AverageCost = r.ReadDouble();
        UnrealizedPnl = r.ReadDouble();
        RealizedPnl = r.ReadDouble();
        AccountName = r.ReadString();
    }
}

public sealed class AccountUpdateTime
{
    private static readonly LocalTimePattern TimePattern = LocalTimePattern.CreateWithInvariantCulture("HH:mm");
    public LocalTime Time { get; }
    internal AccountUpdateTime(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        Time = r.ReadLocalTime(TimePattern);
    }
}

/// <summary>
/// This signals the end of update values for a particular account, not the end of the observable.
/// </summary>
public sealed class AccountUpdateEnd
{
    public string AccountName { get; }
    internal AccountUpdateEnd(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        AccountName = r.ReadString();
    }
}
