using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace Utility;

public sealed class ObservableTests
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
        // IList<string>? list
        IList<string> list = await Observable.Never<string>().ToList().Take(TimeSpan.FromSeconds(3)).FirstOrDefaultAsync();
        Assert.Null(list);
    }

    [Fact]
    public async Task TestTake()
    {
        IObservable<int> o = new int[] { 1, 2, 3, 4 }.ToObservable();

        IList<int> x3 = await o.TakeUntil(x => x == 2).ToList(); // inclusive 12
        IList<int> x4 = await o.TakeUntil(x => x != 3).ToList(); // inclusive 1

        IList<int> x1 = await o.TakeWhile(x => x == 1).ToList(); // exclusive 1
        IList<int> x2 = await o.TakeWhile(x => x != 3).ToList(); // exclusive 12

        IList<int> x11 = await o.TakeWhileInclusive(x => x == 1).ToList(); // inclusive 12
        IList<int> x22 = await o.TakeWhileInclusive(x => x != 3).ToList(); // inclusive 123
        ;
    }

    [Fact]
    public async Task TestAsyncVersusObservable()
    {
        string result1 = await Task
            .FromResult("a")
            .WaitAsync(CancellationToken.None);

        var result2 = new [] { new AlertMessage() }
            .ToObservable()
            //.OfType<IHasRequestId>()
            .ThrowAlertMessage() // modify exception handling
            //.OfType<ContractDetails>()
            //.Select(x => x) // manipulation
            //.Timeout(TimeSpan.FromSeconds(3)) // timeout
            .ToTask(CancellationToken.None);  // cancellation

        await Assert.ThrowsAsync<AlertException>(() => result2);
    }

    /*
    public class ObservableCollectionx<T> : INotifyCollectionChanged, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        //
    }
    */
}

// Value Object, mmutable type
public sealed class Name
{
    public string Value { get; }
    private Name(string value) => Value = value;
    public static Name Create(string name)
    {
        // validation
        return new Name(name);
    }
}

