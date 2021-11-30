
namespace InterReact;

public sealed class OrderState
{
    public OrderStatus Status { get; set; } = new OrderStatus();

    /// <summary>
    /// Initial margin requirement for the order.
    /// </summary>
    public string InitialMarginBefore { get; internal set; } = "";

    /// <summary>
    /// Maintenance margin requirement for the order.
    /// Shows the impact the order would have on your initial margin.
    /// </summary>
    public string MaintenanceMarginBefore { get; internal set; } = "";

    /// <summary>
    /// Shows the impact the order would have on your equity with loan value.
    /// </summary>
    public string EquityWithLoanBefore { get; internal set; } = "";
    public string InitMarginChange { get; internal set; } = "";
    public string MaintMarginChange { get; internal set; } = "";
    public string EquityWithLoanChange { get; internal set; } = "";
    public string InitMarginAfter { get; internal set; } = "";
    public string MaintMarginAfter { get; internal set; } = "";
    public string EquityWithLoanAfter { get; internal set; } = "";

    public double? Commission { get; internal set; }
    /// <summary>
    /// Used in conjunction with the maxCommission field, this defines the lowest end of the possible range into which the actual order commission will fall.
    /// </summary>
    public double? MinimumCommission { get; internal set; }
    /// <summary>
    /// Used in conjunction with the minCommission field, this defines the highest end of the possible range into which the actual order commission will fall.
    /// </summary>
    public double? MaximumCommission { get; internal set; }

    public string CommissionCurrency { get; internal set; } = "";

    public string WarningText { get; internal set; } = "";

    public string CompletedTime { get; internal set; } = "";

    public string CompletedStatus { get; internal set; } = "";
}
