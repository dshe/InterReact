using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace InterReact
{
    public static partial class Extensions
    {
        /// <summary>
        /// Returns a ConnectableObservable which shares a single subscription to the source observable.
        /// Messages are cached for replay to new subscribers.
        /// Call Connect() to subscribe to the source.
        /// Call Dispose() on the value returned from Connect() to disconnect from the source, release all subscriptions, and clear the cache.
        /// Used by AccountUpdates, MarketData (Ticks).
        /// </summary>
        internal static CacheSource<T, TKey> ToCacheSource<T, TKey>(
            this IObservable<T> source, Func<T, TKey> keySelector, bool sort = false) where TKey : notnull =>
                new(source, keySelector, sort);
    }

    internal sealed class CacheSource<T, TKey> : ObservableBase<T>, IConnectableObservable<T> where TKey:notnull
    {
        private readonly Dictionary<TKey, T> cache = new();
        private Subject<T> subject = new();
        private readonly IObservable<T> source;
        private IDisposable? connection;
        private readonly Func<T, TKey> keySelector;
        private readonly bool sort;

        internal CacheSource(IObservable<T> source, Func<T, TKey> keySelector, bool sort)
        {
            this.source = source;
            this.keySelector = keySelector;
            this.sort = sort;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer)
        {
            lock (cache)
            {
                foreach (var value in sort ? cache.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value) : cache.Values)
                    observer.OnNext(value);
                return subject.Subscribe(observer);
            }
        }

        public IDisposable Connect()
        {
            lock (cache)
            {
                if (connection == null)
                {
                    connection = source
                        .Subscribe(
                            onNext: item =>
                            {
                                lock (cache)
                                {
                                    cache[keySelector(item)] = item;
                                    subject.OnNext(item);
                                }
                            },
                            onError: subject.OnError,
                            onCompleted: subject.OnCompleted);
                }

                return Disposable.Create(() =>
                {
                    lock (cache)
                    {
                        connection?.Dispose();
                        connection = null;
                        subject.Dispose(); // unsubscribe subscribers, if any
                        subject = new Subject<T>();
                        cache.Clear();
                    }
                });
            }
        }
    }
}
