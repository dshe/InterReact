namespace InterReact;

public sealed class TickAttrib
{
    public bool CanAutoExecute { get; }
    public bool PastLimit { get; }
    public bool PreOpen { get; }
    public bool Unreported { get; }
    public bool BidPastLow { get; }
    public bool AskPastHigh { get; }

    internal TickAttrib() { }

    internal TickAttrib(ResponseReader? r = null)
    {
        if (r == null)
            return;
        int value = r.ReadInt();
        CanAutoExecute = value == 1;
        if (!r.Builder.SupportsServerVersion(ServerVersion.PAST_LIMIT))
            return;
        BitMask mask = new(value);
        CanAutoExecute = mask[0];
        PastLimit = mask[1];
        if (r.Builder.SupportsServerVersion(ServerVersion.PRE_OPEN_BID_ASK))
            PreOpen = mask[2];
    }
}

public sealed class TickAttribBidAsk
{
    public bool BidPastLow { get; internal set; }
    public bool AskPastHigh { get; internal set; }

    internal TickAttribBidAsk() { }

    internal void Set(int value)
    {
        BitMask mask = new(value);
        BidPastLow = mask[0];
        AskPastHigh = mask[1];
    }
}

public sealed class TickAttribLast
{
    public bool PastLimit { get; internal set; }
    public bool Unreported { get; internal set; }

    internal TickAttribLast() { }

    internal void Set(int value)
    {
        BitMask mask = new(value);
        PastLimit = mask[0];
        Unreported = mask[1];
    }

}
