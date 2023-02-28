namespace InterReact;

public static partial class Extension
{
    internal static IObservable<object> WithRequestId(this IObservable<object> source, int id)
    {
        return source
            .OfType<IHasRequestId>()
            .Where(x => x.RequestId == id);
    }
    internal static IObservable<object> WithOrderId(this IObservable<object> source, int id)
    {
        return source
            .OfType<IHasOrderId>()
            .Where(x => x.OrderId == id);
    }
}
