using System.Reactive.Linq;

namespace InterReact;

public partial class Service
{
    /// <summary>
    /// Creates an observable which, upon subscription, returns open orders and order status reports, then completes.
    /// Multiple simultaneous observers among different OpenOrderRequestTypes is not supported.
    /// https://interactivebrokers.github.io/tws-api/open_orders.html
    /// </summary>
    public IObservable<object> CreateOpenOrdersObservable(OpenOrdersRequestType type = OpenOrdersRequestType.AllOpenOrders)
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
            .Where(m => m is OpenOrder ||
                        m is OrderStatusReport ||
                        m is OrderBound ||
                        m is OpenOrderEnd)
            .ToObservableMultiple<object, OpenOrderEnd>(start)
            .ShareSource();
    }
}

