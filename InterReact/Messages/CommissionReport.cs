using System;
using InterReact.Core;
using InterReact.Interfaces;

namespace InterReact.Messages
{
    /// <summary>
    /// Sent after trades and after calling RequestExecutions.
    /// (OrderId can be determined using the executionId from Execution)
    /// </summary>
    public sealed class CommissionReport : IHasExecutionId, IHasOrderId, IHasRequestId // output
    {
        public string ExecutionId { get; }

        // Execution is set IF an Execution was received with the same ExecutionId.
        public Execution Execution { get; }

        public int OrderId => Execution?.OrderId ?? -1;
        public int RequestId => Execution?.RequestId ?? -1;

        public double Commission { get; }
        public string Currency { get; }
        public double RealizedPnl { get; }
       
        public double Yield { get; }

        /// <summary>
        /// Yet another date format: YYYYMMDD as integer.
        /// </summary>
        public int YieldRedemptionDate { get; }

        internal CommissionReport(ResponseReader c)
        {
            c.IgnoreVersion();
            ExecutionId = c.ReadString();
            Execution = Execution.Executions[ExecutionId];
            Commission = c.Read<double>();
            Currency = c.ReadString();
            RealizedPnl = c.Read<double>();
            Yield = c.Read<double>();
            YieldRedemptionDate = c.Read<int>();
        }
    }
}
