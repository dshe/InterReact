namespace InterReact;

public static partial class Extension
{
    internal static IObservable<IHasRequestId> WithRequestId(this IObservable<object> source, int requestId) =>
        source
            .OfType<IHasRequestId>()
            .Where(m => m.RequestId == requestId);

    internal static IObservable<IHasOrderId> WithOrderId(this IObservable<object> source, int orderId) =>
        source
            .OfType<IHasOrderId>()
            .Where(m => m.OrderId == orderId);

    public static IObservable<T> TakeWhileInclusive<T>(this IObservable<T> source, Func<T, bool> predicate) =>
        Observable.Create<T>(o =>
            source.Subscribe(m =>
            {
                o.OnNext(m);
                if (!predicate(m))
                    o.OnCompleted();
            }
        ));

    public static IObservable<T[]> Accumulate<T, TEnd>(this IObservable<object> source)
    {
        return Observable.Create <T[]>(observer =>
        {
            List<T> list = [];
            return source.Subscribe(onNext: o =>
            {
                if (o is T t)
                {
                    list.Add(t);
                    return;
                }
                if (o is TEnd)
                {
                    observer.OnNext([.. list]);
                    list.Clear();
                    return;
                }
                throw new InvalidOperationException($"Invalid type: {o.GetType().Name}.");
            }, 
            onError: observer.OnError,
            onCompleted: observer.OnCompleted);
        });
    }

}
