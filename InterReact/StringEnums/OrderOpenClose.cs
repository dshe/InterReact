using StringEnums;

namespace InterReact
{
    public sealed class OrderOpenClose : StringEnum<OrderOpenClose>
    {
        public static readonly OrderOpenClose Undefined = Create("");
        public static readonly OrderOpenClose Open = Create("O");
        /// <summary>
        /// Close. Institutional orders only.
        /// </summary>
        public static readonly OrderOpenClose Close = Create("C");
    }

}
