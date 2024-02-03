using System.Reactive;

namespace InterReact;

public static partial class Extension
{
    internal static IObservable<IHasRequestId> ToObservableMultipleWithId<TEnd>(
        this IObservable<object> source, Func<int> getRequestId, Action<int> startRequest)
            where TEnd : IHasRequestId
    {
        return Observable.Create<IHasRequestId>(observer =>
        {
            int id = getRequestId();

            IDisposable subscription = source
                .WithRequestId(id) // IMPORTANT!
                .SubscribeSafe(Observer.Create<IHasRequestId>(
                    onNext: m =>
                    {
                        if (m is TEnd)
                            observer.OnCompleted();
                        else
                            observer.OnNext(m);
                    },
                    onError: observer.OnError,
                    onCompleted: observer.OnCompleted));

            startRequest(id);

            return subscription;
        });
    }
}
