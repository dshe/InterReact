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
    public string AccountName { get; }
    public Contract Contract { get; }
    public decimal Position { get;}
    public double MarketPrice { get; }
    public double MarketValue { get; }
    public double AverageCost { get; }
    public double UnrealizedPnl { get; }
    public double RealizedPnl { get; }
    internal PortfolioValue(ResponseReader r)
    {
        r.RequireMessageVersion(8);
        Contract = new()
        {
            ContractId = r.ReadInt(),
            Symbol = r.ReadString(),
            SecurityType = r.ReadStringEnum<SecurityType>(),
            LastTradeDateOrContractMonth = r.ReadString(),
            Strike = r.ReadDouble(),
            Right = r.ReadStringEnum<OptionRightType>(),
            Multiplier = r.ReadString(),
            PrimaryExchange = r.ReadString(), // note
            Currency = r.ReadString(),
            LocalSymbol = r.ReadString(),
            TradingClass = r.ReadString()
        };
        Position = r.ReadDecimal();
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
