using StringEnums;

namespace InterReact
{
    public sealed class TimeInForce : StringEnum<TimeInForce>
    {
        /// <summary>
        /// Order valid for the current day. This is the default;
        /// </summary>
        public static readonly TimeInForce Day = Create("DAY");

        /// <summary>
        /// Good Till Cancelled.
        /// </summary>
        public static readonly TimeInForce GoodUntilCancelled = Create("GTC");

        /// <summary>
        /// You can set the time in force for MARKET or LIMIT orders as IOC.
        /// This dictates that any portion of the order not executed immediately after it becomes available on the market will be cancelled.
        /// </summary>
        public static readonly TimeInForce ImmediateOrCancel = Create("IOC");

        public static readonly TimeInForce GoodUntilDate = Create("GTD");

        public static readonly TimeInForce LimitOrMarketOnOpen = Create("OPG");

        /// <summary>
        /// Setting FOK as the time in force dictates that the entire order must execute immediately or be canceled.
        /// </summary>
        public static readonly TimeInForce FillOrKill = Create("FOK");

        public static readonly TimeInForce DayUntilCancelled = Create("DTC");
    }

}