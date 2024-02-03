namespace InterReact;

public sealed class AccountValue
{
    public string AccountName { get; }
    public string Key { get; }
    public string Value { get; }
    public string Currency { get; }
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
    public decimal Position { get; }
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
            SecurityType = r.ReadString(),
            LastTradeDateOrContractMonth = r.ReadString(),
            Strike = r.ReadDouble(),
            Right = r.ReadString(),
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
    public string Time { get; }
    internal AccountUpdateTime(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        Time = r.ReadString();
    }
}

public sealed class AccountUpdateEnd
{
    public string AccountName { get; }
    internal AccountUpdateEnd(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        AccountName = r.ReadString();
    }
}
