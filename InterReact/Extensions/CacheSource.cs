using System.Collections;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
namespace InterReact;

public static partial class Extension
{
    /// <summary>
    /// Returns an observable which shares a single subscription to the source observable.
    /// Messages are cached for replay to new subscribers.
    /// Caching begins with the first observer and ends when there are no observers, 
    /// unless maintainSourceSubscription = true, in which case the source observable continues to update the cache.
    /// </summary>
    public static IObservable<T> CacheSource<T>
        (this IObservable<T> source, Func<T, string?> keySelector,
            Func<T, bool>? isEndMessage = null, bool maintainSourceSubscription = false)
    {
        long index = 0;
        Dictionary<string, (T Item, long Index)> cache = new();
        object cacheLock = new();

        ISubject<T>? subject = null;
        Subject<T>? innerSubject = null;
        IDisposable? sourceSubscription = null;

        return Observable.Create<T>(observer =>
        {
            List<T> payload = new();
            lock (cacheLock)
            {
                foreach ((T Item, long Index) value in cache.Values.OrderBy(v => v.Index))
                    payload.Add(value.Item);
            }
            foreach (var value in payload)
                observer.OnNext(value);

            // create a synchronized wrapper around a concrete Subject<T> we can dispose later
            lock (cacheLock)
            {
                innerSubject ??= new Subject<T>();
                subject ??= Subject.Synchronize(innerSubject);
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
                    // forward completion and (optionally) clear
                    subject.OnCompleted();
                });

                return Disposable.Create(() =>
                {
                    lock (cacheLock)
                    {
                        subscription.Dispose();

                        // If there are still observers, keep everything running.
                        // If maintainSourceSubscription is true, keep source subscription even if no observers.
                        if (innerSubject == null || innerSubject.HasObservers || maintainSourceSubscription || sourceSubscription == null)
                            return;

                        // No observers and not maintaining source: dispose source and subject and clear cache
                        sourceSubscription.Dispose();
                        sourceSubscription = null;

                        innerSubject.Dispose();
                        innerSubject = null;
                        subject = null;

                        cache.Clear();
                    }
                });
            }
        });
    }

    /*
    public static IObservable<T[]> CacheSource3<T>(IObservable<T> source)
    {
        return source.Scan(
            new Dictionary<string, T>(),
            (dict, item) =>
            {
                var key = "ee";
                dict[key] = item; // Add/update

                return dict.Select(x => x.Value).Select(x => x).ToArray();
            });
    }
    */

    public static IObservable<T> CacheSource2<T>
        (this IObservable<T> source, Func<T, string?> keySelector,
            Func<T, bool>? isEndMessage = null, bool maintainSourceSubscription = false)
    {
        long index = 0;
        Dictionary<string, (T Item, long Index)> cache = new();

        ISubject<T>? subject = null;
        Subject<T>? innerSubject = null;
        IDisposable? sourceSubscription = null;
        object cacheLock = new();

        // use scheduler
        // use async
        // make task

        //var x = new ReplaySubject<string>();

        // suvscribe to source, update cache, and forward to subject
        //source.Scan
        //source.Select()





        return Observable.Create<T>(observer =>
        {
            List<T> payload;
            IDisposable subscription;

            lock (cacheLock)
            {
                payload = cache.Values.OrderBy(v => v.Index).Select(v => v.Item).ToList();
                innerSubject ??= new Subject<T>();
                subject ??= Subject.Synchronize(innerSubject);
                subscription = subject.Subscribe(observer);
            }

            try
            {
                foreach (var value in payload)
                    observer.OnNext(value);
            }
            catch (Exception ex)
            {
                try
                {
                    subject.OnError(ex);
                }
                catch
                {
                    ;
                }
            }

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


                    if (innerSubject == null || innerSubject.HasObservers || maintainSourceSubscription || sourceSubscription == null)
                        return;

                    sourceSubscription.Dispose();
                    sourceSubscription = null;

                    innerSubject.Dispose();
                    innerSubject = null;
                    subject = null;

                    cache.Clear();
                }
            });
        });
    }
}
