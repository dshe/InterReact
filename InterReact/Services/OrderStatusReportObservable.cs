using System.Reactive.Linq;

namespace InterReact;

public partial class Service
{
    /// <summary>
    /// An observable which emits OrderStatusReport messages on Order Status
    /// changes. OrderStatusReports begin to be sent for an Order after they 
    /// are sent to TWS and transmitted.
    /// </summary>
    public IObservable<object> OrderStatusReportObservable { get; }

    private IObservable<object> CreateOrderStatusReportObservable() =>
        Response
            .Where(x => x is OrderStatusReport || x is OrderStatusReportEnd)
            .CacheSource(GetOrderStatusReportCacheKey)
            .ShareSource();

     private static string GetOrderStatusReportCacheKey(object o)
    {
        return o switch
        {
            OrderStatusReport r => $"{r.OrderId}+{r.Status}",
            OrderStatusReportEnd _ => "OrderStatusReportEnd",
            _ => throw new ArgumentException($"Unhandled type: {o.GetType()}.")
        };
    }
}

