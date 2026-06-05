namespace InterReact;

[Message]
public sealed record AccountPosition
{
    public bool IsEndMessage { get; init; }
    public string Account { get; init; } = "";
    public Contract Contract { get; init; }
    public decimal Position { get; init; }
    public double AverageCost { get; init;  }
    internal AccountPosition() => Contract = new();
    internal AccountPosition(ResponseReader r, bool isEndMessage)
    {
        if (isEndMessage)
        {
            r.IgnoreMessageVersion();
            IsEndMessage = true;
            Contract = new();
            return;
        }
        r.RequireMessageVersion(3);
        Account = r.ReadString();
        Contract = new(r, includePrimaryExchange: false);
        Position = r.ReadDecimal();
        AverageCost = r.ReadDouble();
    }
}
