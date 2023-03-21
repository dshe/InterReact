namespace InterReact;

public static partial class Extension
{
    internal static IObservable<IHasRequestId> WithRequestId(this IObservable<object> source, int requestId)
    {
        return source
            .OfType<IHasRequestId>()
            .Where(x => x.RequestId == requestId);
    }

    internal static IObservable<IHasOrderId> WithOrderId(this IObservable<object> source, int orderId)
    {
        return source
            .OfType<IHasOrderId>()
            .Where(x => x.OrderId == orderId);
    }
}
