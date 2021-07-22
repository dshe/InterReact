using StringEnums;

namespace InterReact
{
    public sealed class TradeAction : StringEnum<TradeAction>
    {
        public static TradeAction Undefined { get; } = Create("");

        public static TradeAction Buy { get; } = Create("BUY");
        public static TradeAction Sell { get; } = Create("SELL");

        /// <summary>
        /// SSHORT is only supported for institutional account configured with Long/Short account segments or clearing with a separate account.
        /// </summary>
        public static TradeAction SellShort { get; } = Create("SSHORT");

        /// <summary>
        /// Allows orders to be marked as exempt from SEC Rule 201.
        /// </summary>
        //public static TradeAction SellShortExemptFromSecRule201 { get; } = Create("SSHORTX");
    }
}