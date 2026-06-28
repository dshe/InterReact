using System.Globalization;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
namespace InterReact;

public static partial class Xtensions
{
    /// <summary>
    /// Returns an observable which shares a single subscription to the source observable.
    /// Messages are cached for replay to new subscribers.
    /// Caching begins with the first observer and ends when there are no observers, 
    /// unless maintainSourceSubscription = true, in which case the source observable continues to update the cache.
    /// </summary>
    extension<T>(IObservable<T> source)
    {
        public IObservable<T[]> GetCache(
            Func<T, string> keySelector,
            Func<T, bool>? isEndMessage = null,
            bool maintainSourceSubscription = false)
        {
            long index = 0;
            Dictionary<string, T> cache = [];
            Lock cacheLock = new();
            Subject<T[]>? subject = null;
            IDisposable? sourceSubscription = null;
            // Wait until all messages are received to return result.
            bool waitForEndMessage = (isEndMessage is not null);

            return Observable.Create<T[]>(observer =>
            {
                lock (cacheLock)
                {
                    if (!waitForEndMessage)
                        observer.OnNext(cache.Values.ToArray());

                    subject ??= new Subject<T[]>();
                    IDisposable subscription = subject.Subscribe(observer);

                    sourceSubscription ??= source.Subscribe(m =>
                    {
                        lock (cacheLock)
                        {
                            if (waitForEndMessage && isEndMessage is not null && isEndMessage(m))
                            {
                                waitForEndMessage = false;
                                subject.OnNext(cache.Values.ToArray());
                                return;
                            }
                            string key = keySelector(m);
                            // string: items are cached based on the specified key
                            // "":     every item is cached based on an arbitrary unique key
                            if (key.Length == 0)
                                key = "~" + index++.ToString(CultureInfo.InvariantCulture);
                            cache[key] = m;
                            if (!waitForEndMessage)
                                subject.OnNext(cache.Values.ToArray());
                        }
                    }, subject.OnError, subject.OnCompleted);

                    return Disposable.Create(() =>
                    {
                        lock (cacheLock)
                        {
                            subscription.Dispose();

                            if (subject is null || subject.HasObservers || maintainSourceSubscription || sourceSubscription is null)
                                return;

                            sourceSubscription.Dispose();
                            sourceSubscription = null;

                            subject.Dispose();
                            subject = null;

                            cache.Clear();
                            waitForEndMessage = (isEndMessage is not null);
                        }
                    });
                }
            });
        }

    }
}
