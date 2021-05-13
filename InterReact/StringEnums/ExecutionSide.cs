using StringEnums;

namespace InterReact
{
    public sealed class ExecutionSide : StringEnum<ExecutionSide>
    {
        public static readonly ExecutionSide Undefined = Create("");
        public static readonly ExecutionSide Bought = Create("BOT");
        public static readonly ExecutionSide Sold = Create("SLD");
    }
}
