using System.Reactive.Disposables;
namespace InterReact;

public static partial class Extension
{
    public static IObservable<T> WithTimeout<T>(this IObservable<T> source, TimeSpan? timeSpan, CancellationToken ct = default)
    {
        return Observable.Create<T>(observer =>
        {
            IDisposable subscription = source.Subscribe(observer);

            CancellationTokenSource cts = new();
            if (timeSpan is not null)
                cts.CancelAfter(timeSpan.Value);

            CancellationTokenRegistration cancellationDisposable = ct.Register(() =>
                observer.OnError(new OperationCanceledException("The operation was canceled.")));

            return new CompositeDisposable(subscription, cts, cancellationDisposable);
        });
    }

}
