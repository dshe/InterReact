using System.Reactive;

namespace InterReact;

public static partial class Extension
{
    internal static IObservable<IHasRequestId> ToObservableMultipleWithRequestId<TEnd>(
        this IObservable<object> source, Func<int> getRequestId, Action<int> startRequest)
            where TEnd : IHasRequestId
    {
        return Observable.Create<IHasRequestId>(observer =>
        {
            int requestId = getRequestId();

            IDisposable subscription = source
                .OfType<IHasRequestId>() // IMPORTANT!
                .Where(m => m.RequestId == requestId)
                .SubscribeSafe(Observer.Create<IHasRequestId>(
                    onNext: m =>
                    {
                        if (m is TEnd)
                        {
                            observer.OnCompleted();
                            return;
                        }
                        observer.OnNext(m);
                        if (m is AlertMessage alert && alert.IsFatal) // IMPORTANT!
                            observer.OnCompleted();
                    },
                    onError: observer.OnError,
                    onCompleted: observer.OnCompleted));

            startRequest(requestId);

            return subscription;
        });
    }
}
