using System;
using System.Reactive.Linq;

namespace InterReact
{
    public sealed partial class Services
    {
        /// <summary>
        /// Creates an observable which, upon subscription, returns open orders and order status reports, then completes.
        /// Multiple simultaneous observers among different OpenOrderRequestTypes is not supported.
        /// </summary>
        public IObservable<IOrder> CreateOpenOrdersObservable(OpenOrdersRequestType type = OpenOrdersRequestType.AllOpenOrders)
        {
            Action start = type switch
            {
                OpenOrdersRequestType.OpenOrders => Request.RequestOpenOrders,
                OpenOrdersRequestType.AllOpenOrders => Request.RequestAllOpenOrders,
                OpenOrdersRequestType.AutoOpenOrders => () => Request.RequestAutoOpenOrders(false),
                OpenOrdersRequestType.AutoOpenOrdersWithBind => () => Request.RequestAutoOpenOrders(true),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
            };
            return Response
                .Where(m => m is OpenOrder || m is OrderStatusReport || m is OpenOrderEnd)
                .ToObservableMultiple<IOrder, OpenOrderEnd>(start)
                .ToShareSource();
        }
    }
    public static partial class Extensions
    {
        public static IObservable<OpenOrder> OfTypeOpenOrder(this IObservable<IOrder> source) => source.OfType<OpenOrder>();
        public static IObservable<OrderStatusReport> OfTypeOrderStatusReport(this IObservable<IOrder> source) => source.OfType<OrderStatusReport>();
    }
}

