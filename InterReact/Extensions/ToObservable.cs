using System.Reactive;
using System.Reactive.Disposables;
namespace InterReact;

public static partial class Extension
{
    // For continuous results with RequestId: AccountUpdatesMulti, AccountPositionsMulti, MarketData
    // For multiple   results with RequestId: ContractDetails, MarketDataSnapshot
    // IF T is IHasRequestId, then AlertMessages are not sent to OnError
    internal static IObservable<T> ToObservableWithId<T>(
        this IObservable<object> source, 
        Func<int> getRequestId,
        Action<int> startRequest, Action<int>? stopRequest = null,
        Func<IHasRequestId, bool>? isEndMessage = null)
    {
        return Observable.Create<T>(observer =>
        {
            int id = getRequestId();
            bool? cancelable = null;

            IDisposable subscription = source
                .WithRequestId(id)
                .SubscribeSafe(Observer.Create<IHasRequestId>(
                    onNext: m =>
                    {
                        if (isEndMessage!= null && isEndMessage(m))
                        {
                            cancelable = false;
                            observer.OnCompleted();
                        }
                        else if (m is AlertMessage alert && alert.IsFatal)
                        {
                            cancelable = false;
                            observer.OnError(alert.ToAlertException());
                        }
                        else if (m is T t)
                            observer.OnNext(t);
                        else
                        {
                            cancelable = false;
                            observer.OnError(new InvalidOperationException($"Unexpected message type: {m.GetType().Name}."));
                        }
                    },
                    onError: e =>
                    {
                        cancelable = false;
                        observer.OnError(e);
                    },
                    onCompleted: () =>
                    {
                        cancelable = false;
                        observer.OnCompleted();
                    }));

            if (cancelable is null)
                startRequest(id);
            cancelable ??= true;

            return Disposable.Create(() =>
            {
                if (cancelable is true && stopRequest is not null)
                    stopRequest(id);
                subscription.Dispose();
            });
        });
    }


}
