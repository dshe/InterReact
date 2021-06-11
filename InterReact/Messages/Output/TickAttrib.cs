namespace InterReact
{
    public sealed class TickAttrib
    {
        public bool CanAutoExecute { get; }
        public bool PastLimit { get; }
        public bool PreOpen { get; }
        public bool Unreported { get; }
        public bool BidPastLow { get; }
        public bool AskPastHigh { get; }

        internal TickAttrib(ResponseReader? r = null)
        {
            if (r == null)
                return;
            int value = r.ReadInt();
            CanAutoExecute = value == 1;
            if (!r.Config.SupportsServerVersion(ServerVersion.PastLimit))
                return;
            BitMask mask = new(value);
            CanAutoExecute = mask[0];
            PastLimit = mask[1];
            if (r.Config.SupportsServerVersion(ServerVersion.PreOpenBidAsk))
                PreOpen = mask[2];
        }
    }

    public sealed class TickAttribBidAsk
    {
        public bool BidPastLow { get; }
        public bool AskPastHigh { get; }

        public TickAttribBidAsk(int value)
        {
            BitMask mask = new(value);
            BidPastLow = mask[0];
            AskPastHigh = mask[1];
        }
    }

    public sealed class TickAttribLast
    {
        public bool PastLimit { get; }
        public bool Unreported { get; }

        public TickAttribLast(int value)
        {
            BitMask mask = new(value);
            PastLimit = mask[0];
            Unreported = mask[1];
        }

    }
}
