namespace InterReact;

[Message]
public sealed record AccountPosition
{
    public bool IsEndMessage { get; init; }
    public string Account { get; init; } = "";
    public Contract Contract { get; } = new();
    public decimal Position { get; init; }
    public double AverageCost { get; init;  }
    internal AccountPosition() { }
    internal AccountPosition(ResponseReader r, bool isEndMessage)
    {
        IsEndMessage = isEndMessage;
        if (isEndMessage)
        {
            r.IgnoreMessageVersion();
            return;
        }
        r.RequireMessageVersion(3);
        Account = r.ReadString();
        Contract.Read(r, includePrimaryExchange: false);
        Position = r.ReadDecimal();
        AverageCost = r.ReadDouble();
    }
}

