using InterReact.Interfaces;
using InterReact.StringEnums;

namespace InterReact.Messages
{
    public sealed class OpenOrder : IHasOrderId
    {
        public Order Order { get; } = new Order();
        public int OrderId => Order.OrderId;

        public Contract Contract { get; } = new Contract();

        public OrderStatus Status { get; internal set; } = OrderStatus.Unknown;

        /// <summary>
        /// Initial margin requirement for the order.
        /// </summary>
        public string InitialMargin { get; internal set; }

        /// <summary>
        /// Maintenance margin requirement for the order.
        /// Shows the impact the order would have on your initial margin.
        /// </summary>
        public string MaintenanceMargin { get; internal set; }

        /// <summary>
        /// Shows the impact the order would have on your equity with loan value.
        /// </summary>
        public string EquityWithLoan { get; internal set; }

        public double? Commission { get; internal set; }
        /// <summary>
        /// Used in conjunction with the maxCommission field, this defines the lowest end of the possible range into which the actual order commission will fall.
        /// </summary>
        public double? MinimumCommission { get; internal set; }
        /// <summary>
        /// Used in conjunction with the minCommission field, this defines the highest end of the possible range into which the actual order commission will fall.
        /// </summary>
        public double? MaximumCommission { get; internal set; }

        public string CommissionCurrency { get; internal set; }

        public string WarningText { get; internal set; }

    }

    public sealed class OpenOrderEnd { }

}