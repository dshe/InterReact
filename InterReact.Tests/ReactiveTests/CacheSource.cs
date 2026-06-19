using System.Collections.Immutable;
using System.Globalization;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using Microsoft.Reactive.Testing;
namespace UnitTests;

public static class Extension
{
    public static IObservable<IImmutableDictionary<string, T>> ToCache<T>(this IObservable<T> source, Func<T, string> keySelector)
    {
        return source
            .Scan(ImmutableDictionary<string, T>.Empty,
            (dict, item) => dict.SetItem(keySelector(item), item))
        .Replay(1)
        .RefCount();
}


public static IObservable<T> CacheTheSource<T>
        (this IObservable<T> source, Func<T, string?> keySelector,
            Func<T, bool>? isEndMessage = null, bool maintainSourceSubscription = false)
    {
        long index = 0;
        Dictionary<string, (T Item, long Index)> cache = [];
        Lock cacheLock = new();
        Subject<T> subject = new();
        IDisposable? sourceSubscription = null;

        return Observable.Create<T>(observer =>
        {
            List<T> values = [];
            lock (cacheLock)
            {
                foreach ((T Item, long Index) value in cache.Values.OrderBy(v => v.Index))
                    values.Add(value.Item);
            }
            foreach (T value in values)
                observer.OnNext(value);

            IDisposable subscription = subject.Subscribe(observer);

            IObserver<T> obsrver = Observer.Create<T>(m =>
            {
                string? key = keySelector(m);
                if (key is not null)
                {
                    long i = isEndMessage is not null && isEndMessage(m) ? long.MaxValue : index++;
                    if (key.Length == 0)
                        key = "~" + i.ToString(CultureInfo.InvariantCulture);
                    lock (cacheLock)
                    {
                        cache[key] = (m, i);
                    }
                }
                subject.OnNext(m);
            }, (e) =>
            {
                // forward completion and (optionally) clear
                //subject.OnCompleted();
            });


            sourceSubscription ??= source.Subscribe(obsrver);


            return Disposable.Create(() =>
            {
                //lock (subject)
                //{

                subscription.Dispose();

                // If there are still observers, keep everything running.
                // If maintainSourceSubscription is true, keep source subscription even if no observers.
                if (subject.HasObservers || maintainSourceSubscription || sourceSubscription == null)
                    return;

                // No observers and not maintaining source: dispose source and subject and clear cache
                sourceSubscription.Dispose();
                sourceSubscription = null;
                //subject.Dispose();
                //subject = null;
                lock (cacheLock)
                {
                    cache.Clear();
                }
            });
        });
    }
}

public class CacheSourceTest(ITestOutputHelper output) : OutputHelperTestBase(output)
{
    [Fact]
    public async Task T03_CacheAsync()
    {
        Subject<string> source = new();
        IObservable<string> observable = source.CacheTheSource(x => x);

        IDisposable subscription = observable.Subscribe(); // start cache

        source.OnNext("1");
        source.OnNext("2");
        source.OnNext("3");
        source.OnNext("2"); // duplicate key

        IList<string> list = await observable.Take(TimeSpan.FromMilliseconds(100)).ToList();
        Assert.Equal(3, list.Count);

        source.OnNext("10");
        list = await observable.Take(TimeSpan.FromMilliseconds(100)).ToList();
        Assert.Equal(4, list.Count);
    }


    [Fact]
    public async Task T04_Cache_TestAsync()
    {
        Subject<string> source = new();
        IObservable<string> observable = source.CacheTheSource(x => x);
        IDisposable subscription = observable.Subscribe(); // start cache

        IList<string> list1 = await observable.Take(TimeSpan.FromMilliseconds(100)).ToList();
        Assert.Equal(0, list1.Count);

        source.OnNext("1");

        IList<string> list2 = await observable.Take(TimeSpan.FromMilliseconds(100)).ToList();
        Assert.Equal(1, list2.Count);

        subscription.Dispose();
        source.Dispose();
    }

    [Fact]
    public async Task T04_Cache_TescAsync()
    {
        Subject<string> source = new();

        //IObservable<Dictionary<string, string>> xx = source.ToCache(x => x).Replay(1).Replay().AutoConnect();

        BehaviorSubject<Dictionary<string, string>> behaviorSubject = new BehaviorSubject<Dictionary<string, string>>(new Dictionary<string, string>());
        //xx.Subscribe(behaviorSubject);

        IObservable<string> observable = source.CacheTheSource(x => x);
        IDisposable subscription = observable.Subscribe(); // start cache

        IList<string> list1 = await observable.Take(TimeSpan.FromMilliseconds(100)).ToList();
        Assert.Equal(0, list1.Count);

        source.OnNext("1");

        IList<string> list2 = await observable.Take(TimeSpan.FromMilliseconds(100)).ToList();
        Assert.Equal(1, list2.Count);

        subscription.Dispose();
        source.Dispose();
    }

}
