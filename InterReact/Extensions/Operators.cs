namespace InterReact;

public static partial class Xtensions
{
    extension(IObservable<object> source)
    {
        internal IObservable<IHasRequestId> WithRequestId(int requestId) =>
            source.OfType<IHasRequestId>().Where(m => m.RequestId == requestId);

        internal IObservable<IHasOrderId> WithOrderId(int orderId) =>
            source.OfType<IHasOrderId>().Where(m => m.OrderId == orderId);

        public IObservable<T> OfTypeOnly<T>() =>
            Observable.Create<T>(observer =>
            {
                return source.Subscribe(
                    m =>
                    {
                        if (m is T t)
                            observer.OnNext(t);
                        else if (m is Alert a)
                            observer.OnError(a.ToAlertException());
                        else
                            observer.OnError(new InvalidOperationException($"Unexpected type: {m.GetType().Name}."));
                    },
                    observer.OnError,
                    observer.OnCompleted);
            });
    }
    extension<T>(IObservable<T> source)
    {
        // Timeout(TimeSpan) does not accept Timespan.MaxValue or Timeout.InfiniteTimeSpan, so use null.
        internal IObservable<T> TimeoutIf(TimeSpan? timeout) =>
            timeout.HasValue ? source.Timeout(timeout.Value) : source;

        public IObservable<T> ThrowOnAlert() =>
            source.SelectMany(m =>
            {
                if (m is Alert a)
                    return Observable.Throw<T>(a.ToAlertException());
                return Observable.Return(m); // Continues the stream normally
            });
    }
}
