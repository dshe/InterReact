using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
namespace Temp;

public sealed class ObservableTests(ITestOutputHelper output) : OutputHelperTestBase(output)
{
    [Fact]
    public async Task TestObservableEmptyAsync()
    {
        // string? str
        string str = await Observable.Empty<string>().FirstOrDefaultAsync();
        Assert.Null(str);
    }

    [Fact]
    public async Task TestObservableNeverTimeoutAsync()
    {
        // string? str
        string str = await Observable.Never<string>().Take(TimeSpan.FromMilliseconds(1)).FirstOrDefaultAsync();
        Assert.Null(str);
    }

    [Fact]
    public async Task TestTaskCancellationAsync()
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
    public async Task TestListObservableAsync()
    {
        IList<string> list = await Observable.Never<string>().ToList().Take(TimeSpan.FromSeconds(3)).FirstOrDefaultAsync();
        Assert.Null(list);
    }
   

    [Fact]
    public async Task TestTakeAsync()
    {
        int[] sourceArray = [1, 2, 3, 4];
        IObservable<int> o = sourceArray.ToObservable();

        IList<int> x3 = await o.TakeUntil(x => x == 2).ToList(); // inclusive 12
        IList<int> x4 = await o.TakeUntil(x => x != 3).ToList(); // inclusive 1

        IList<int> x1 = await o.TakeWhile(x => x == 1).ToList(); // exclusive 1
        IList<int> x2 = await o.TakeWhile(x => x != 3).ToList(); // exclusive 12
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

    [Fact]
    public async Task CancellationAsync()
    {
        //await Observable.Throw<string>(new IndexOutOfRangeException());
        //await Observable.Return("a").Timeout(TimeSpan.Zero);

        CancellationTokenSource cts = new();
        cts.Cancel();
        //await Observable.Return("a").CancelOn(cts.Token);

        string s = await CreateObservable()
            .FirstAsync()
            .WithTimeout(TimeSpan.FromSeconds(3), cts.Token)
            //.Catch(Observable.Empty<string>())
            .Catch<string, Exception>(ex =>
            {
                return Observable.Return("");
            });
        ;
    }


}
