namespace InterReact;

[Message]
public sealed record TickAttribBidAsk
{
    public bool BidPastLow { get; init; }
    public bool AskPastHigh { get; init; }
    internal TickAttribBidAsk() { }
    internal TickAttribBidAsk(int value)
    {
        BitMask mask = new(value);
        BidPastLow = mask[0];
        AskPastHigh = mask[1];
    }
}

[Message]
public sealed record TickAttribLast
{
    public bool PastLimit { get; init; }
    public bool Unreported { get; init; }
    internal TickAttribLast() { }
    internal TickAttribLast(int value)
    {
        BitMask mask = new(value);
        PastLimit = mask[0];
        Unreported = mask[1];
    }
}
