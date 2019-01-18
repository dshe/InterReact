using InterReact.Enums;
using InterReact.Interfaces;
using InterReact.StringEnums;
using Stringification;

namespace InterReact.Messages
{
    public sealed class Execution : IHasRequestId, IHasOrderId, IHasExecutionId
    {
        /// <summary>
        /// RequestId will be -1 if this object sent due to an execution rather than a request for executions.
        /// </summary>
        public int RequestId { get; internal set; } = int.MaxValue; // StringifyAlways

        public int OrderId { get; internal set; }

        /// <summary>
        /// Unique order execution id.
        /// </summary>
        public string ExecutionId { get; internal set; }

        /// <summary>
        /// The Id of the client which placed the order.
        /// Note: TWS orders have a fixed client id of "0."
        /// </summary>
        public int ClientId { get; internal set; }

        /// <summary>
        /// The order execution time.
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// Cusomer account number.
        /// </summary>
        public string Account { get; set; }

        public string Exchange { get; internal set; }
        public ExecutionSide Side { get; internal set; } = ExecutionSide.Undefined;

        /// <summary>
        /// The number of shares filled.
        /// </summary>
        public double Shares { get; internal set; }

        /// <summary>
        /// The order execution price, not including commissions.
        /// </summary>
        public double Price { get; internal set; }

        /// <summary>
        /// The TWS id used to identify orders, which remains the same over TWS sessions.
        /// </summary>
        public int PermanentId { get; internal set; }

        /// <summary>
        /// Identifies the position as one to be liquidated last should the need arise.
        /// </summary>
        public int Liquidation { get; internal set; }
        public double CumulativeQuantity { get; internal set; }

        /// <summary>
        /// The average price, which includes commissions.
        /// </summary>
        public double AveragePrice { get; internal set; }
        public string OrderReference { get; internal set; }

        /// <summary>
        /// IncludeAll the Economic Value Rule name and the respective optional argument. The two Values should be separated by a colon. For example, aussieBond:YearsToExpiration=3. When the optional argument is not present, the first value will be followed by a colon.
        /// </summary>
        public string EconomicValueRule { get; internal set; }

        /// <summary>
        /// Tells you approximately how much the market value of a contract would change if the price were to change by 1. It cannot be used to get market value by multiplying the price by the approximate multiplier.
        /// </summary>
        public double EconomicValueMultiplier { get; internal set; }

        public Contract Contract { get; } = new Contract();
    }

    public sealed class ExecutionEnd : IHasRequestId
    {
        public int RequestId { get; internal set; }
    }

}
