using StringEnums;

namespace InterReact
{
    public sealed class TradeAction : StringEnum<TradeAction>
    {
        public static readonly TradeAction Undefined = Create("");

        public static readonly TradeAction Buy = Create("BUY");
        public static readonly TradeAction Sell = Create("SELL");

        /// <summary>
        /// SSHORT is only supported for institutional account configured with Long/Short account segments or clearing with a separate account.
        /// </summary>
        public static readonly TradeAction SellShort = Create("SSHORT");

        /// <summary>
        /// Allows orders to be marked as exempt from SEC Rule 201.
        /// </summary>
        //public static readonly TradeAction SellShortExemptFromSecRule201 = Create("SSHORTX");
    }
}