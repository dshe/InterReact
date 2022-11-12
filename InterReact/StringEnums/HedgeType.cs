using StringEnums;

namespace InterReact;

public sealed class HedgeType : StringEnum<HedgeType>
{
    public static HedgeType Undefined { get; } = Create("");
    public static HedgeType Delta { get; } = Create("D");
    public static HedgeType Beta { get; } = Create("B");
    public static HedgeType Forex { get; } = Create("F");
    public static HedgeType Pair { get; } = Create("P");
}
