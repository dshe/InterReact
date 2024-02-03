namespace InterReact;

public static class OrderStatus
{
    public const string Unknown = "";

    /// <summary>
    /// Indicates that an order has been transmitted, but there has been no confirmation that the order has been accepted by the order destination.
    /// This order status is not sent by TWS and should be explicitly set by the API developer when an order is submitted.
    /// </summary>
    public const string PendingSubmit = "PendingSubmit";

    /// <summary>
    /// Indicates that there has been a request to cancel the order
    /// but no cancellation confirmation has been received from the order destination.
    /// An execution may occur while while the cancellation request is pending.
    /// This order status is not sent by TWS and should be explicitly set by the API developer when an order is canceled.
    /// </summary>
    public const string PendingCancel = "PendingCancel";

    /// <summary>
    /// Indicates that a simulated order type has been accepted by the IB system and
    /// that this order has yet to be elected. The order is held in the IB system
    /// (and the status remains DARK BLUE) until the election criteria are met.
    /// At that time the order is transmitted to the order destination as specified
    /// </summary>
    public const string PreSubmitted = "PreSubmitted";

    /// <summary>
    /// Indicates that the order has been accepted at the order destination and is working.
    /// </summary>
    public const string Submitted = "Submitted";

    /// <summary>
    /// Indicates that the balance of the order has been confirmed canceled by the IB system.
    /// This could occur unexpectedly when IB or the destination has rejected your order.
    /// </summary>
    public const string Cancelled = "Cancelled";

    /// <summary>
    /// The order has been completely filled.
    /// </summary>
    public const string Filled = "Filled";

    public const string Inactive = "Inactive";

    public const string PartiallyFilled = "PartiallyFilled";

    public const string ApiPending = "ApiPending";

    /// <summary>
    /// Api Cancelled.
    /// </summary>
    public const string ApiCancelled = "ApiCancelled";
}
