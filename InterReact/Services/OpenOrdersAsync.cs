using System.Reactive.Threading.Tasks;

namespace InterReact;

public partial class Service
{
    /// <summary>
    /// Returns open orders and order status reports.
    /// https://interactivebrokers.github.io/tws-api/open_orders.html
    /// </summary>
    public async Task<IList<object>> GetOpenOrdersAsync(OpenOrdersRequestType type = OpenOrdersRequestType.AllOpenOrders, CancellationToken ct = default)
    {
        Task<IList<object>> task = Response
            .Where(m => m is OpenOrder or OrderStatusReport or OrderBound or OpenOrderEnd)
            .TakeWhile(m => m is not OpenOrderEnd) // exclusive
            .ToList()
            .ToTask(ct);

        switch (type)
        {
            case OpenOrdersRequestType.OpenOrders:
                Request.RequestOpenOrders();
                break;
            case OpenOrdersRequestType.AllOpenOrders:
                Request.RequestAllOpenOrders();
                break;
            case OpenOrdersRequestType.AutoOpenOrders:
                Request.RequestAutoOpenOrders(false);
                break;
            case OpenOrdersRequestType.AutoOpenOrdersWithBind:
                Request.RequestAutoOpenOrders(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        } ;

        IList<object> list = await task.ConfigureAwait(false);

        return list;
    }
}

