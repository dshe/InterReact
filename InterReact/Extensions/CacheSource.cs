using System.Globalization;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
namespace InterReact;

public static partial class Extensions
{
    extension<T>(IObservable<T> source)
    {
        /// <summary>
        /// Returns an observable which shares a single subscription to the source observable.
        /// Messages are cached for replay to new subscribers.
        /// Caching begins with the first observer and ends when there are no observers, 
        /// unless maintainSourceSubscription = true, in which case the source observable continues to update the cache.
        /// </summary>
        public IObservable<T> CacheSource(Func<T, string?> keySelector, Func<T, bool>? isEndMessage = null, bool maintainSourceSubscription = false)
        {
            long index = 0;
            Dictionary<string, (T Item, long Index)> cache = [];
            Lock cacheLock = new();
            Subject<T> subject = new();
            IDisposable? sourceSubscription = null;

            return Observable.Create<T>(observer =>
            {
                lock (cacheLock)
                {
                    foreach ((T Item, long Index) value in cache.Values.OrderBy(v => v.Index))
                        observer.OnNext(value.Item);

                    IDisposable subscription = subject.Subscribe(observer);

                    sourceSubscription ??= source.Subscribe(m =>
                    {
                        string? key = keySelector(m);
                        lock (cacheLock)
                        {
                            if (key is not null)
                            {
                                long i = isEndMessage is not null && isEndMessage(m) ? long.MaxValue : index++;
                                if (key.Length == 0)
                                    key = "~" + i.ToString(CultureInfo.InvariantCulture);
                                cache[key] = (m, i);
                            }
                            subject.OnNext(m);
                        }
                        try
                        {
                            //subject.OnNext(m);
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                //subject.OnError(ex);
                            }
                            catch
                            {
                                ; /* swallow to avoid throw */
                            }
                        }
                    }, () =>
                    {
                        // forward completion and (optionally) clear
                        //subject.OnCompleted();
                    });

                    return Disposable.Create(() =>
                    {
                        lock (cacheLock)
                        {
                            subscription.Dispose();

                            // If there are still observers, keep everything running.
                            // If maintainSourceSubscription is true, keep source subscription even if no observers.
                            if (subject == null || subject.HasObservers || maintainSourceSubscription || sourceSubscription == null)
                                return;

                            // No observers and not maintaining source: dispose source and subject and clear cache
                            sourceSubscription.Dispose();
                            sourceSubscription = null;
                            //subject.Dispose();
                            cache.Clear();
                        }
                    });
                }
            });
        }

        // use scheduler
        // use async
        // make task
        //var x = new ReplaySubject<string>();
        // subscribe to source, update cache, and forward to subject
        //source.Scan
        //source.Select()

        public IObservable<T> CacheSource2(Func<T, string?> keySelector, Func<T, bool>? isEndMessage = null, bool maintainSourceSubscription = false)
        {
            long index = 0;
            Dictionary<string, (T Item, long Index)> cache = new();

            Subject<T> subject = new();
            IDisposable? sourceSubscription = null;
            Lock cacheLock = new();


            return Observable.Create<T>(observer =>
            {
                List<T> payload;
                lock (cacheLock)
                {
                    payload = cache.Values.OrderBy(v => v.Index).Select(v => v.Item).ToList();
                }

                foreach (T? value in payload)
                    observer.OnNext(value);
                IDisposable subscription = subject.Subscribe(observer);

                sourceSubscription ??= source.Subscribe(m =>
                {
                    string? key = keySelector(m);
                    lock (cacheLock)
                    {
                        if (key is not null)
                        {
                            long i = isEndMessage is not null && isEndMessage(m) ? long.MaxValue : index++;
                            if (key.Length == 0)
                                key = "~" + i.ToString(CultureInfo.InvariantCulture);
                            cache[key] = (m, i);
                        }
                    }
                    try
                    {
                        subject.OnNext(m);
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            subject.OnError(ex);
                        }
                        catch
                        {
                            ; /* swallow to avoid throw */
                        }
                    }
                }, () =>
                {
                    subject.OnCompleted();
                });

                return Disposable.Create(() =>
                {
                    subscription.Dispose();

                    lock (cacheLock)
                    {
                        if (subject == null || subject.HasObservers || maintainSourceSubscription || sourceSubscription == null)
                            return;

                        sourceSubscription.Dispose();
                        sourceSubscription = null;

                        subject.Dispose();
                        //subject = null;

                        cache.Clear();
                    }
                });
            });
        }
    }
}
