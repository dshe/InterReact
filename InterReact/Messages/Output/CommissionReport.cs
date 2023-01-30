using Microsoft.Extensions.Logging;

namespace InterReact;

/// <summary>
/// Sent after trades and also after calling RequestExecutions().
/// </summary>
public sealed class CommissionReport : IHasExecutionId, IHasOrderId
{
    public string ExecutionId { get; }
    /// OrderId is determined using the executionId from Execution.
    public int OrderId { get; }
    public double Commission { get; }
    public string Currency { get; }
    public double RealizedPnl { get; }
    public double Yield { get; }
    // Yet another date format: YYYYMMDD as integer.
    public int YieldRedemptionDate { get; }
    internal CommissionReport(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        ExecutionId = r.ReadString();
        Commission = r.ReadDouble();
        Currency = r.ReadString();
        RealizedPnl = r.ReadDouble();
        Yield = r.ReadDouble();
        YieldRedemptionDate = r.ReadInt();

        if (Execution.ExecutionIds.TryGetValue(ExecutionId, out int orderId))
            OrderId = orderId;
        else
           r.Logger.LogWarning("CommissionReport: could not determine OrderId from Executions.");
    }
}
