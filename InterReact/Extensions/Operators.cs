namespace InterReact;

public static partial class Extensions
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

}
