using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace InterReact
{
    public static partial class Extensions
    {
        /// <summary>
        /// Returns an observable which shares a single subscription to the source observable.
        /// Messages are cached for replay to new subscribers.
        /// Caching begins with the first observer and ends when there are no observers.
        /// </summary>
        internal static IObservable<T> ShareSourceCache<T, TKey>
            (this IObservable<T> source, Func<T, TKey?> keySelector)
                where TKey : notnull
        {
            long index = 0;
            Dictionary<TKey, (T Item, long Index)> cache = new();

            IObservable<T> cachedSource = source.Do(m =>
            {
                TKey? key = keySelector(m);
                if (key == null)
                    return;
                lock (cache)
                {
                    if (cache.TryGetValue(key, out var item))
                        item.Item = m;
                    else
                        cache.Add(key, (m, index++));
                }
            }).Finally(() =>
            {
                lock (cache)
                {
                    cache.Clear();
                }
            }).Publish().RefCount();

            return Observable.Defer(() =>
            {
                lock (cache)
                {
                    return cache
                        .Values
                        .OrderBy(v => v.Index)
                        .Select(v => v.Item)
                        .ToList()
                        .ToObservable();
                }
            }).Concat(cachedSource);
        }
    }
}
