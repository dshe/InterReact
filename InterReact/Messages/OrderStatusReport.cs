using InterReact.Core;
using InterReact.Enums;
using InterReact.Interfaces;
using InterReact.StringEnums;

namespace InterReact.Messages
{
    public sealed class OrderStatusReport : IHasOrderId
    {
        /// <summary>
        /// The order Id that was specified previously in the call to PlaceOrder.
        /// </summary>
        public int OrderId { get; }

        public OrderStatus Status { get; }

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
        public string WhyHeld { get; }

        internal OrderStatusReport(ResponseReader c)
        {
            var version = c.RequireVersion(5);
            OrderId = c.Read<int>();
            Status = c.ReadStringEnum<OrderStatus>();
            Filled = c.Read<double>(); // int -> double
            Remaining = c.Read<double>();
            AverageFillPrice = c.Read<double>();
            PermanentId = c.Read<int>();
            ParentId = c.Read<int>();
            LastFillPrice = c.Read<double>();
            ClientId = c.Read<int>();
            WhyHeld = version >= 6 ? c.ReadString() : string.Empty;
        }
    }
}
