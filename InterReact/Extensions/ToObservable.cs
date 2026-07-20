using System.Reactive;
using System.Reactive.Disposables;
namespace InterReact;

public static partial class Xtensions
{
    extension<T>(IObservable<T> source)
    {
        public IObservable<T> ToObservable(Action startRequest)
        {
            return Observable.Create<T>(observer =>
            {
                IDisposable subscription = source
                    .SubscribeSafe(Observer.Create<T>(
                        onNext: m =>
                        {
                            observer.OnNext(m);
                            observer.OnCompleted();
                        },
                        onError: observer.OnError,
                        onCompleted: observer.OnCompleted));

                startRequest();

                return subscription;
            });
        }
    }

    extension<T>(IObservable<object> source)
    {
        public IObservable<T> ToObservable0(Func<ValueTask> startRequest, Func<ValueTask>? stopRequest = null)
        {
            return Observable.Create<T>(async observer =>
            {
                bool? cancelable = null;

                IDisposable subscription = source
                    .OfType<T>()
                    .SubscribeSafe(Observer.Create<T>(
                        onNext: observer.OnNext,
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
                    await startRequest().ConfigureAwait(false);
                cancelable ??= true;

                return Disposable.Create(async () =>
                {
                    if (cancelable is true && stopRequest is not null)
                        await stopRequest().ConfigureAwait(false);
                    subscription.Dispose();
                });
            });
        }

        public IObservable<T> ToObservable(Func<ValueTask> startRequest, Func<ValueTask>? stopRequest = null)
        {
            return Observable.Create<T>(observer =>
            {
                int started = 0;

                IDisposable subscription = source
                    .OfType<T>()
                    .Subscribe(observer);

                Task startTask = Task.Run(async () =>
                {
                    if (Interlocked.Exchange(ref started, 1) == 0)
                        await startRequest().ConfigureAwait(false);
                });

                return Disposable.Create(() =>
                {
                    subscription.Dispose();
                    _ = Task.Run(async () =>
                    {
                        if (stopRequest is not null && Interlocked.Exchange(ref started, 0) == 1)
                        {
                            await stopRequest().ConfigureAwait(false);
                        }
                    });
                });
            });
        }
    }

    extension(IObservable<object> source)
    {
        //Func<ValueTask>
        // For continuous results with RequestId: AccountUpdatesMulti, MarketData
        // For multiple   results with RequestId: ContractDetails, MarketDataSnapshot
        public IObservable<IHasRequestId> ToObservableWithId(Func<int> getRequestId, Func<int, ValueTask> startRequest, Func<int, ValueTask>? stopRequest = null)
        {
            return Observable.Create<IHasRequestId>(async observer =>
            {
                int id = getRequestId();
                bool? cancelable = null;

                IDisposable subscription = source
                    .WithRequestId(id)
                    .SubscribeSafe(Observer.Create<IHasRequestId>(
                        onNext: observer.OnNext,
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
                    await startRequest(id).ConfigureAwait(false);
                cancelable ??= true;

                return Disposable.Create(async () =>
                {
                    if (cancelable is true && stopRequest is not null)
                        await stopRequest(id).ConfigureAwait(false);
                    subscription.Dispose();
                });
            });
        }
    }
}
