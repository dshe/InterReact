using StringEnums;

namespace InterReact;

public sealed class OrderStatus : StringEnum<OrderStatus>
{
    public static OrderStatus Unknown { get; } = Create("");

    /// <summary>
    /// Indicates that an order has been transmitted, but there has been no confirmation that the order has been accepted by the order destination.
    /// This order status is not sent by TWS and should be explicitly set by the API developer when an order is submitted.
    /// </summary>
    public static OrderStatus PendingSubmit { get; } = Create("PendingSubmit");

    /// <summary>
    /// Indicates that there has been a request to cancel the order
    /// but no cancellation confirmation has been received from the order destination.
    /// An execution may occur while while the cancellation request is pending.
    /// This order status is not sent by TWS and should be explicitly set by the API developer when an order is canceled.
    /// </summary>
    public static OrderStatus PendingCancel { get; } = Create("PendingCancel");

    /// <summary>
    /// Indicates that a simulated order type has been accepted by the IB system and
    /// that this order has yet to be elected. The order is held in the IB system
    /// (and the status remains DARK BLUE) until the election criteria are met.
    /// At that time the order is transmitted to the order destination as specified
    /// </summary>
    public static OrderStatus PreSubmitted { get; } = Create("PreSubmitted");

    /// <summary>
    /// Indicates that the order has been accepted at the order destination and is working.
    /// </summary>
    public static OrderStatus Submitted { get; } = Create("Submitted");

    /// <summary>
    /// Indicates that the balance of the order has been confirmed canceled by the IB system.
    /// This could occur unexpectedly when IB or the destination has rejected your order.
    /// </summary>
    public static OrderStatus Cancelled { get; } = Create("Cancelled");

    /// <summary>
    /// The order has been completely filled.
    /// </summary>
    public static OrderStatus Filled { get; } = Create("Filled");

    public static OrderStatus Inactive { get; } = Create("Inactive");

    public static OrderStatus PartiallyFilled { get; } = Create("PartiallyFilled");

    public static OrderStatus ApiPending { get; } = Create("ApiPending");

    /// <summary>
    /// Api Cancelled.
    /// </summary>
    public static OrderStatus ApiCancelled { get; } = Create("ApiCancelled");
}
