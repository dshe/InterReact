using System.Reactive.Disposables;
using System.Reactive.Subjects;
namespace InterReact;

public static partial class Extension
{
    /// <summary>
    /// Returns an observable which shares a single subscription to the source observable.
    /// Messages are cached for replay to new subscribers.
    /// Caching begins with the first observer and ends when there are no observers.
    /// </summary>
    internal static IObservable<T> CacheSource<T>
        (this IObservable<T> source, Func<T, string> keySelector, Func<T, bool> isEndMessage)
    {
        long index = 0;
        Dictionary<string, (T Item, long Index)> cache = [];

        Subject<T>? subject = null;
        IDisposable sourceSubscription = Disposable.Empty;

        return Observable.Create<T>(observer =>
        {
            lock (cache)
            {
                subject ??= new Subject<T>();

                foreach ((T Item, long Index) value in cache.Values.OrderBy(v => v.Index))
                    observer.OnNext(value.Item);

                IDisposable subscription = subject.Subscribe(observer);

                if (Equals(sourceSubscription, Disposable.Empty))
                {
                    sourceSubscription = source.Subscribe(m =>
                    {
                        lock (cache)
                        {
                            if (Equals(sourceSubscription, Disposable.Empty))
                                return;
                            // return the end message last so that the observer may complete.
                            long i = isEndMessage(m) ? long.MaxValue : index++;
                            string key = keySelector(m);
                            if (key.Length != 0)
                                cache[key] = (m, i);
                        }
                        subject.OnNext(m);
                    },
                    e => subject.OnError(e),
                    subject.OnCompleted);
                }

                return Disposable.Create(() =>
                {
                    lock (cache)
                    {
                        subscription.Dispose();
                        if (subject.HasObservers)
                            return;
                        sourceSubscription.Dispose();
                        sourceSubscription = Disposable.Empty;
                        cache.Clear();
                        subject.Dispose();
                        subject = null;
                    }
                });
            }
        });
    }

    /// <summary>
    /// Returns an observable which shares a single subscription to the source observable.
    /// Messages are cached for replay to new subscribers.
    /// Caching begins with the first observer and ends when there are no observers.
    /// </summary>
    internal static IObservable<T> CacheSource2<T>
        (this IObservable<T> source, Func<T, string> keySelector)
    {
        long index = 0;
        Dictionary<string, (T Item, long Index)> cache = [];

        return Observable.Create<T>(observer =>
        {
            lock (cache)
            {
                foreach ((T Item, long Index) value in cache.Values.OrderBy(v => v.Index))
                    observer.OnNext(value.Item);

                return source.Subscribe(x =>
                {
                    lock (cache)
                    {
                        string key = keySelector(x);
                        if (key.Length != 0)
                            cache[key] = (x, index++);
                    }
                    observer.OnNext(x);
                },
                e => observer.OnError(e),
                observer.OnCompleted);
            }
        });
    }

}
