﻿namespace InterReact;

public sealed class AccountPosition
{
    public string Account { get; }
    public Contract Contract { get; }
    public decimal Position { get; }
    public double AverageCost { get; }
    internal AccountPosition(ResponseReader r)
    {
        r.RequireMessageVersion(3);
        Account = r.ReadString();
        Contract = new(r, includePrimaryExchange: false);
        Position = r.ReadDecimal();
        AverageCost = r.ReadDouble();
    }
}

public sealed class AccountPositionEnd
{
    internal AccountPositionEnd(ResponseReader r) => r.IgnoreMessageVersion();
}
