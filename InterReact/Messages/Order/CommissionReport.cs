namespace InterReact;

/// <summary>
/// Sent after trades and also after calling RequestExecutions().
/// </summary>
[Message]
public sealed record CommissionReport : IHasExecutionId, IHasOrderId
{
    public string ExecutionId { get; init; } = "";
    public double Commission { get; init; }
    public string Currency { get; init; } = "";
    public double RealizedPnl { get; init; }
    public double Yield { get; init; }
    // Yet another date format: YYYYMMDD as integer.
    public int YieldRedemptionDate { get; init; }
    public int OrderId { get; init; } // OrderId may be set below
    internal CommissionReport() { }
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
