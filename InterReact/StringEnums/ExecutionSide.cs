using StringEnums;

namespace InterReact;

public sealed class ExecutionSide : StringEnum<ExecutionSide>
{
    public static ExecutionSide Undefined { get; } = Create("");
    public static ExecutionSide Bought { get; } = Create("BOT");
    public static ExecutionSide Sold { get; } = Create("SLD");
}
