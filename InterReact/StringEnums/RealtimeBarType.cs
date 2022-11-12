using StringEnums;

namespace InterReact;

public sealed class RealtimeBarType : StringEnum<RealtimeBarType>
{
    public static RealtimeBarType Trades { get; } = Create("TRADES");
    public static RealtimeBarType Bid { get; } = Create("BID");
    public static RealtimeBarType Ask { get; } = Create("ASK");
    public static RealtimeBarType MidPoint { get; } = Create("MIDPOINT");
}


