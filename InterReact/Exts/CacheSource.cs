using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace InterReact;

public static partial class Ext
{
    /// <summary>
    /// Returns an observable which shares a single subscription to the source observable.
    /// Messages are cached for replay to new subscribers.
    /// Caching begins with the first observer and ends when there are no observers.
    /// </summary>
    internal static IObservable<T> CacheSource<T>
        (this IObservable<T> source, Func<T, string> keySelector)
    {
        long index = 0;
        Dictionary<string, (T Item, long Index)> cache = new();

        Subject<T>? subject = null;
        IDisposable sourceSubscription = Disposable.Empty;

        return Observable.Create<T>(observer =>
        {
            lock (cache)
            {
                subject ??= new();

                foreach (var value in cache.Values.OrderBy(v => v.Index))
                    observer.OnNext(value.Item);

                IDisposable subscription = subject.Subscribe(observer);

                if (sourceSubscription == Disposable.Empty)
                {
                    sourceSubscription = source.Subscribe(x =>
                    {
                        lock (cache)
                        {
                            if (sourceSubscription == Disposable.Empty)
                                return;
                            string key = keySelector(x);
                            if (key.Length != 0)
                            {
                                if (cache.TryGetValue(key, out var item))
                                    item.Item = x;
                                else
                                    cache.Add(key, (x, index++));
                            }
                        }
                        subject.OnNext(x);
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
}
