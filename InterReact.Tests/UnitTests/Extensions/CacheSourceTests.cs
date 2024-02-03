using Microsoft.Reactive.Testing;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;

namespace Extension;

public class CacheSourceTests : ReactiveUnitTestBase
{
    public CacheSourceTests(ITestOutputHelper output) : base(output) { }

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

        Recorded<Notification<string>>[] expected1 = new[] { OnNext(100, "one") };
        Assert.Equal(expected1.ToList(), observer1.Messages);

        Recorded<Notification<string>>[] expected2 = new[] { OnNext(150, "one") };
        Assert.Equal(expected2.ToList(), observer2.Messages);

        testScheduler.Start();
        Assert.Equal(3, observer1.Messages.Count);
        Assert.Equal(3, observer2.Messages.Count);
    }

    [Fact]
    public async Task T01_Empty()
    {
        IObservable<string> observable = Observable.Empty<string>().CacheSource(x => x); // completes
        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(() => observable.ToTask());
        Assert.Equal("Sequence contains no elements.", ex.Message);
    }

    [Fact]
    public async Task T03_Cache()
    {
        Subject<string> source = new();
        IObservable<string> observable = source.CacheSource(x => x);

        observable.Subscribe(); // start cache

        source.OnNext("1");
        source.OnNext("2");
        source.OnNext("3");
        source.OnNext("2"); // duplicate key

        string first = await observable.FirstAsync();

        IList<string> list1 = await observable.Take(1).ToList();

        IList<string> list = await observable.Take(TimeSpan.FromMilliseconds(10)).ToList();
        Assert.Equal(3, list.Count);

        source.OnNext("10");
        list = await observable.Take(TimeSpan.FromMilliseconds(10)).ToList();
        Assert.Equal(4, list.Count);
    }

    [Fact]
    public void T04_Throw_In_OnNext()
    {
        Subject<string> source = new();
        IObservable<string> observable = source.CacheSource(x => x);

        observable.Subscribe(x =>
            throw new BarrierPostPhaseException("some exception"));

        Assert.Throws<BarrierPostPhaseException>(() => source.OnNext("message"));
    }
}
