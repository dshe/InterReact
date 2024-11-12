using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
namespace Utility;

public sealed class ObservableTests(ITestOutputHelper output) : UnitTestBase(output)
{
    [Fact]
    public async Task TestObservableEmpty()
    {
        // string? str
        string str = await Observable.Empty<string>().FirstOrDefaultAsync();
        Assert.Null(str);
    }

    [Fact]
    public async Task TestObservableNeverTimeout()
    {
        // string? str
        string str = await Observable.Never<string>().Take(TimeSpan.FromMilliseconds(1)).FirstOrDefaultAsync();
        Assert.Null(str);
    }

    [Fact]
    public async Task TestTaskCancellation()
    {
        CancellationTokenSource cts = new();
        Task<string?> task = Observable.Never<string>().FirstOrDefaultAsync().ToTask(cts.Token);
        cts.Cancel();
        await Assert.ThrowsAsync<TaskCanceledException>(() => task);
    }

    [Fact]
    public async Task TestTaskCancellationWaitAsync()
    {
        CancellationTokenSource cts = new();
        Task<string?> task = Observable.Never<string>().FirstOrDefaultAsync().ToTask();
        cts.Cancel();
        await Assert.ThrowsAsync<TaskCanceledException>(() => task.WaitAsync(cts.Token));
    }

    [Fact]
    public async Task TestListObservable()
    {
        IList<string> list = await Observable.Never<string>().ToList().Take(TimeSpan.FromSeconds(3)).FirstOrDefaultAsync();
        Assert.Null(list);
    }
   

    [Fact]
    public async Task TestTake()
    {
        int[] sourceArray = [1, 2, 3, 4];
        IObservable<int> o = sourceArray.ToObservable();

        IList<int> x3 = await o.TakeUntil(x => x == 2).ToList(); // inclusive 12
        IList<int> x4 = await o.TakeUntil(x => x != 3).ToList(); // inclusive 1

        IList<int> x1 = await o.TakeWhile(x => x == 1).ToList(); // exclusive 1
        IList<int> x2 = await o.TakeWhile(x => x != 3).ToList(); // exclusive 12

        IList<int> x11 = await o.TakeWhileInclusive(x => x == 1).ToList(); // inclusive 12
        IList<int> x22 = await o.TakeWhileInclusive(x => x != 3).ToList(); // inclusive 123
    }

    [Fact]
    public void TestCatchErrorHandlingObservable()
    {
        IObservable<string> o = Observable.Create<string>(observer =>
        {
            observer.OnNext("a");
            observer.OnError(new InvalidOperationException("error message"));
            return Disposable.Empty;
        });

        var xx = o.Catch<string, Exception>(ex =>
        {
            Console.WriteLine(ex.Message);
            return Observable.Empty<string>(); // Completes
        });

        static IObservable<string> ErrorFunction(Exception ex)
        {
            Console.WriteLine(ex.Message);
            return Observable.Empty<string>(); // Completes
        }
        var xy = o.Catch<string, Exception>(ErrorFunction);
    }

    private IObservable<string> CreateObservable()
    {
        return Observable.Create<string>(async (obs, ct) =>
        {
            Write("Subscribing");
            await Task.Delay(500, ct);
            obs.OnNext("a");
            obs.OnCompleted();
        });
    }

    //private void Some(IReadOnlyList<string> argx)
    //private void Some(IReadOnlyCollection<string> argx)
    private void Some(IList<string> argx)
    //private void Some(string[] arg)
    //private void Some(IEnumerable<string> arg)
    {

    }

    [Fact]
    public async Task Cancellation()
    {
        var arrayx = new[] { "a", "b", "c" };
        IEnumerable<string> enumx = new[] { "a", "b", "c" };
        List<string> list = arrayx.ToList();
        IList<string> list2 = arrayx.ToList();

        //Some(enumx);
        Some(arrayx);
        Some(["xx"]);
        Some(list);
        Some(list2);



        //await Observable.Throw<string>(new IndexOutOfRangeException());

        //await Observable.Return("a").Timeout(TimeSpan.Zero);

        CancellationTokenSource cts = new();
        cts.Cancel();
        //await Observable.Return("a").CancelOn(cts.Token);

        string s = await CreateObservable()
            .FirstAsync()
            .CancelOn(cts.Token)
            .Timeout(TimeSpan.FromSeconds(3))
            //.Catch(Observable.Empty<string>())
            .Catch<string, Exception>(ex =>
            {
                return Observable.Return("");
            });
        ;
    }


}
