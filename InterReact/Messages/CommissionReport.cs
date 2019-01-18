using System;
using InterReact.Interfaces;

namespace InterReact.Messages
{
    /// <summary>
    /// Sent after trades and after calling RequestExecutions.
    /// (OrderId can be determined using the executionId from Execution)
    /// </summary>
    public sealed class CommissionReport : IHasExecutionId, IHasOrderId, IHasRequestId // output
    {
        public string ExecutionId { get; internal set; }

        // Execution is set IF an Execution was received with the same ExecutionId.
        public Execution Execution { get; internal set; }

        public int OrderId => Execution?.OrderId ?? -1;
        public int RequestId => Execution?.RequestId ?? -1;

        public double Commission { get; internal set; }
        public string Currency { get; internal set; }
        public double RealizedPnl { get; internal set; }
       
        public double Yield { get; internal set; }

        /// <summary>
        /// Yet another date format: YYYYMMDD as integer.
        /// </summary>
        public int YieldRedemptionDate { get; internal set; }
    }
}
