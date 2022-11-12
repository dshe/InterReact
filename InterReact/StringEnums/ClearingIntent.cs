using StringEnums;

namespace InterReact;

public sealed class ClearingIntent : StringEnum<ClearingIntent>
{
    public static ClearingIntent Default { get; } = Create("");
    public static ClearingIntent Ib { get; } = Create("IB");
    public static ClearingIntent Away { get; } = Create("Away");
    public static ClearingIntent PostTradeAllocation { get; } = Create("PTA");
}
