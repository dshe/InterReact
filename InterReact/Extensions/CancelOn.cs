namespace InterReact;

public static partial class Extension
{
    public static IObservable<T> CancelOn<T>(this IObservable<T> source, CancellationToken ct)
    {
        return Observable.Create<T>(obs =>
        {
            return source.Subscribe(
                onNext: m =>
                {
                    if (ct.IsCancellationRequested)
                        obs.OnError(new OperationCanceledException());
                    else
                        obs.OnNext(m);
                },
                onError: ex => obs.OnError(ct.IsCancellationRequested ? new OperationCanceledException() : ex),
                onCompleted: () =>
                {
                    if (ct.IsCancellationRequested)
                        obs.OnError(new OperationCanceledException());
                    else
                        obs.OnCompleted();
                });
        });
    }

}
