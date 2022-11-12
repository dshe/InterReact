using StringEnums;

namespace InterReact;

public sealed class OrderOpenClose : StringEnum<OrderOpenClose>
{
    public static OrderOpenClose Undefined { get; } = Create("");
    public static OrderOpenClose Open { get; } = Create("O");
    /// <summary>
    /// Close. Institutional orders only.
    /// </summary>
    public static OrderOpenClose Close { get; } = Create("C");
}
