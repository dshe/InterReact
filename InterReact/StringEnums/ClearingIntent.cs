using StringEnums;

namespace InterReact
{
    public sealed class ClearingIntent : StringEnum<ClearingIntent>
    {
        public static readonly ClearingIntent Default = Create("");
        public static readonly ClearingIntent Ib = Create("IB");
        public static readonly ClearingIntent Away = Create("Away");
        public static readonly ClearingIntent PostTradeAllocation = Create("PTA");
    }
}
