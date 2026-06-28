using Microsoft.Reactive.Testing;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
namespace UnitTests;

public class CacheSourceTests(ITestOutputHelper output) : OutputHelperTestBase(output)
{
    [Fact]
    public void T00_Test()
    {
        TestScheduler testScheduler = new();

        ITestableObserver<string> observer1 = testScheduler.CreateObserver<string>();
        ITestableObserver<string> observer2 = testScheduler.CreateObserver<string>();

        ITestableObservable<string> source = testScheduler.CreateHotObservable(
            OnNext(100, "one"),
            OnNext(200, "two"),
            OnNext(300, "three"));

        IObservable<string> observable = source.CacheSource(x => x);

        observable.Subscribe(observer1);
        testScheduler.AdvanceBy(150);
        observable.Subscribe(observer2);

        Recorded<Notification<string>>[] expected1 = [OnNext(100, "one")];
        Assert.Equal([.. expected1], observer1.Messages);

        Recorded<Notification<string>>[] expected2 = [OnNext(150, "one")];
        Assert.Equal([.. expected2], observer2.Messages);

        testScheduler.Start();
        Assert.Equal(3, observer1.Messages.Count);
        Assert.Equal(3, observer2.Messages.Count);
    }

    [Fact]
    public async Task T03_CacheAsync()
    {
        Subject<string> source = new();
        IObservable<string> observable = source.CacheSource(x => x);

        IDisposable subscription = observable.Subscribe(); // start cache

        source.OnNext("1");
        source.OnNext("2");
        source.OnNext("3");
        source.OnNext("2"); // duplicate key

        string first = await observable.FirstAsync();

        IList<string> list1 = await observable.Take(1).ToList();

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
        IObservable<string> observable = source.CacheSource(x => x);
        IDisposable subscription = observable.Subscribe(); // start cache

        source.OnNext("1");

        IList<string> list1 = await observable.Take(TimeSpan.FromMilliseconds(1000)).ToList();
        Assert.Equal(1, list1.Count);

        source.OnNext("2");

        IList<string> list2 = await observable.Take(TimeSpan.FromMilliseconds(100)).ToList();
        Assert.Equal(2, list2.Count);

        subscription.Dispose();
        source.Dispose();
    }

    [Fact]
    public async Task T01_EmptyAsync()
    {
        IObservable<string> observable1 = Observable.Empty<string>();

        var ex1 = await Assert.ThrowsAsync<InvalidOperationException>(async () => await observable1);
        Assert.Equivalent("Sequence contains no elements.", ex1.Message);

        var ex2 = await Assert.ThrowsAsync<InvalidOperationException>(async () => await observable1.CacheSource(x => x));
        Assert.Equivalent("Sequence contains no elements.", ex2.Message);
    }

    [Fact]
    public async Task T01_NeverAsync()
    {
        IObservable<string> observable1 = Observable.Never<string>().Timeout(TimeSpan.FromMilliseconds(100));
        await Assert.ThrowsAsync<TimeoutException>(async () => await observable1);
        await Assert.ThrowsAsync<TimeoutException>(async () => await observable1.CacheSource(x => x));
    }

    [Fact]
    public void T04_Throw_In_OnNext()
    {
        Subject<string> source1 = new();
        IObservable<string> observable1 = source1;
        observable1.Subscribe(x =>
            throw new BarrierPostPhaseException("some exception"));
        Assert.Throws<BarrierPostPhaseException>(() => source1.OnNext("message"));

        Subject<string> source2 = new();
        IObservable<string> observable2 = source2.CacheSource(x => x);
        observable2.Subscribe(x =>
            throw new BarrierPostPhaseException("some exception"));

        Assert.Throws<BarrierPostPhaseException>(() => source2.OnNext("message"));
    }
}
