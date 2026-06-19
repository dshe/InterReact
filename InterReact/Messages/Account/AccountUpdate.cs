namespace InterReact;

[Message]
public sealed record AccountValue
{
    public string Account { get; init; } = "";
    public string Key { get; init; } = "";
    public string Value { get; init; } = "0";
    public string Currency { get; init; } = "";
    internal AccountValue() { }
    internal AccountValue(ResponseReader r)
    {
        r.RequireMessageVersion(2);
        Key = r.ReadString();
        Value = r.ReadString();
        Currency = r.ReadString();
        Account = r.ReadString();
    }
}

[Message]
public sealed record PortfolioValue
{
    public string Account { get; init; } = "";
    public Contract Contract { get; init; } = new();
    public decimal Position { get; init; }
    public double MarketPrice { get; init; }
    public double MarketValue { get; init; }
    public double AverageCost { get; init; }
    public double UnrealizedPnl { get; init; }
    public double RealizedPnl { get; init; }
    internal PortfolioValue() { }
    internal PortfolioValue(ResponseReader r)
    {
        r.RequireMessageVersion(8);
        Contract.Read(r, includeExchange: false);
        Position = r.ReadDecimal();
        MarketPrice = r.ReadDouble();
        MarketValue = r.ReadDouble();
        AverageCost = r.ReadDouble();
        UnrealizedPnl = r.ReadDouble();
        RealizedPnl = r.ReadDouble();
        Account = r.ReadString();
    }
}

[Message]
public sealed record AccountUpdateTime
{
    public string Time { get; init; } = "";
    internal AccountUpdateTime() { }
    internal AccountUpdateTime(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        Time = r.ReadString();
    }
}

[Message]
public sealed record AccountUpdateEnd
{
    public string Account { get; init; } = "";
    internal AccountUpdateEnd() { }
    internal AccountUpdateEnd(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        Account = r.ReadString();
    }
}
