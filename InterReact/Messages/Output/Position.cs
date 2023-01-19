namespace InterReact;

public sealed class Position
{
    public string Account { get; } = "";
    public Contract Contract { get; } = new();
    public double Quantity { get; }
    public double AverageCost { get; }

    internal Position() { }

    internal Position(ResponseReader r)
    {
        r.RequireMessageVersion(3);
        Account = r.ReadString();
        Contract.ContractId = r.ReadInt();
        Contract.Symbol = r.ReadString();
        Contract.SecurityType = r.ReadStringEnum<SecurityType>();
        Contract.LastTradeDateOrContractMonth = r.ReadString();
        Contract.Strike = r.ReadDouble();
        Contract.Right = r.ReadStringEnum<OptionRightType>();
        Contract.Multiplier = r.ReadString();
        Contract.Exchange = r.ReadString();
        Contract.Currency = r.ReadString();
        Contract.LocalSymbol = r.ReadString();
        Contract.TradingClass = r.ReadString();
        Quantity = r.ReadDouble();
        AverageCost = r.ReadDouble();
    }
}

public sealed class PositionEnd
{
    internal PositionEnd(ResponseReader r) => r.IgnoreMessageVersion();
}
