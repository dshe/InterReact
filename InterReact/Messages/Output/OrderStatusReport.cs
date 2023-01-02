namespace InterReact;

public sealed class OrderStatusReport : IHasOrderId
{
    /// <summary>
    /// The order Id that was specified previously in the call to PlaceOrder.
    /// </summary>
    public int OrderId { get; }

    public OrderStatus Status { get; } = OrderStatus.Unknown;

    /// <summary>
    /// Specifies the number of shares that have been executed.
    /// </summary>
    public double Filled { get; }

    /// <summary>
    /// Specifies the number of shares still outstanding.
    /// </summary>
    public double Remaining { get; }

    /// <summary>
    /// The average price of the shares that have been executed.
    /// This parameter is valid only if the filled parameter value
    /// is greater than zero. Otherwise, the price parameter will be zero.
    /// </summary>
    public double AverageFillPrice { get; }

    /// <summary>
    /// The TWS id used to identify orders. Remains the same over TWS sessions.
    /// </summary>
    public int PermanentId { get; }

    /// <summary>
    /// The order ID of the parent order, used for bracket and auto trailing stop orders.
    /// </summary>
    public int ParentId { get; }

    /// <summary>
    /// The last price of the shares that have been executed. This parameter is valid
    /// only if the filled parameter value is greater than zero. Otherwise, the price parameter will be zero.
    /// </summary>
    public double LastFillPrice { get; }

    /// <summary>
    /// The Id of the client (or TWS) that placed the order.
    /// The TWS orders have a fixed ClientId and orderId of 0 that distinguishes them from API orders.
    /// </summary>
    public int ClientId { get; }

    /// <summary>
    /// This field is used to identify an order held when TWS is trying to locate shares for a short sell.
    /// The value used to indicate this is 'locate'.
    /// </summary>
    public string WhyHeld { get; } = "";

    public double? MktCapPrice { get; }

    internal OrderStatusReport() { }

    internal OrderStatusReport(ResponseReader r)
    {
        if (!r.Connector.SupportsServerVersion(ServerVersion.MARKET_CAP_PRICE))
            r.IgnoreVersion();

        OrderId = r.ReadInt();
        Status = r.ReadStringEnum<OrderStatus>();
        Filled = r.ReadDouble();
        Remaining = r.ReadDouble();
        AverageFillPrice = r.ReadDouble();
        PermanentId = r.ReadInt();
        ParentId = r.ReadInt();
        LastFillPrice = r.ReadDouble();
        ClientId = r.ReadInt();
        WhyHeld = r.ReadString();

        if (r.Connector.SupportsServerVersion(ServerVersion.MARKET_CAP_PRICE))
            MktCapPrice = r.ReadDouble();
    }
}
public sealed class OrderStatusReportEnd: IHasRequestId
{
    public int RequestId { get; }
    internal OrderStatusReportEnd(ResponseReader r)
    {
        r.IgnoreVersion();
        RequestId = r.ReadInt();
    }
}
