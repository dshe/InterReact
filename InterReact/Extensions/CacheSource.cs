using System.Globalization;
using System.Reactive.Disposables;
namespace InterReact;

public static partial class Xtensions
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
            List<IObserver<T>> observers = [];
            Lock gate = new();
            IDisposable? sourceSubscription = null;

            void ForAllOnNext(T m)
            {
                string? key = keySelector(m);
                if (key is not null)
                {
                    long i = isEndMessage is not null && isEndMessage(m) ? long.MaxValue : index++;
                    if (key.Length == 0)
                        key = "~" + i.ToString(CultureInfo.InvariantCulture);
                    cache[key] = (m, i);
                }
                foreach (IObserver<T> obs in observers.ToArray())
                {
                    try
                    {
                        obs.OnNext(m);
                    }
                    catch (NullReferenceException)
                    {
                        // avoids race conditioon
                    }
                }
            }
            void ForAllOnError(Exception ex)
            {
                foreach (IObserver<T> obs in observers.ToArray())
                    try
                    {
                        obs.OnError(ex);
                    }
                    catch { }
            }
            void ForAllOnCompleted()
            {
                foreach (IObserver<T> obs in observers.ToArray())
                    try
                    {
                        obs.OnCompleted();
                    }
                    catch { }
            }

            return Observable.Create<T>(observer =>
            {
            lock (gate)
            {
                foreach (T item in cache.Values.OrderBy(v => v.Index).Select(v => v.Item).ToArray())
                    observer.OnNext(item);
                observers.Add(observer);

                sourceSubscription ??= source.Subscribe(
                    m =>
                    {
                        lock (gate)
                            ForAllOnNext(m);
                    },
                    ex => ForAllOnError(ex),
                    ForAllOnCompleted);

                    return Disposable.Create(() =>
                    {
                        lock (gate)
                        {
                            observers.Remove(observer);
                            if (observers.Count > 0 || maintainSourceSubscription)
                                return;
                            sourceSubscription.Dispose();
                            sourceSubscription = null;
                            cache.Clear();
                        }
                    });
                }
            });
        }
    }
}
