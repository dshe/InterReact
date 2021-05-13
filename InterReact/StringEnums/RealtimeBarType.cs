using StringEnums;

namespace InterReact
{
    public sealed class RealtimeBarType : StringEnum<RealtimeBarType>
    {
        public static readonly RealtimeBarType Trades = Create("TRADES");
        public static readonly RealtimeBarType Bid = Create("BID");
        public static readonly RealtimeBarType Ask = Create("ASK");
        public static readonly RealtimeBarType MidPoint = Create("MIDPOINT");
    }
}
