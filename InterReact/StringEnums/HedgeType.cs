using StringEnums;

namespace InterReact.StringEnums
{
    public sealed class HedgeType : StringEnum<HedgeType>
    {
        public static readonly HedgeType Undefined = Create("");
        public static readonly HedgeType Delta = Create("D");
        public static readonly HedgeType Beta = Create("B");
        public static readonly HedgeType Forex = Create("F");
        public static readonly HedgeType Pair = Create("P");
    }
}