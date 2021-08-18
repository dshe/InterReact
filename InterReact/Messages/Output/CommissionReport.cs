namespace InterReact
{
    /// <summary>
    /// Sent after trades and after calling RequestExecutions.
    /// (OrderId can be determined using the executionId from Execution)
    /// </summary>
    public sealed class CommissionReport : IHasRequestId, IHasOrderId, IHasExecutionId
    {
        public int RequestId { get; } = -1;
        public int OrderId { get; } = -1;
        // Execution is set IF an Execution was received with the same ExecutionId.
        public Execution? Execution { get; } // (not part of IB Api)

        public string ExecutionId { get; }
        public double Commission { get; }
        public string Currency { get; }
        public double RealizedPnl { get; }
        public double Yield { get; }
        // Yet another date format: YYYYMMDD as integer.
        public int YieldRedemptionDate { get; }

        internal CommissionReport(ResponseReader r)
        {
            r.IgnoreVersion();
            ExecutionId = r.ReadString();
            Commission = r.ReadDouble();
            Currency = r.ReadString();
            RealizedPnl = r.ReadDouble();
            Yield = r.ReadDouble();
            YieldRedemptionDate = r.ReadInt();

            if (Execution.Executions.TryGetValue(ExecutionId, out Execution? execution))
            {
                Execution = execution;
                OrderId = execution.OrderId;
                RequestId = execution.RequestId;
            }
        }
    }
}
