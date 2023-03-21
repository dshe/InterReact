using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace Utility;

public sealed class FirstOrDefaultTest
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
    public async Task TestTaskCancellaion()
    {
        CancellationTokenSource cts = new();
        Task<string?> task = Observable.Never<string>().FirstOrDefaultAsync().ToTask(cts.Token);
        cts.Cancel();
        await Assert.ThrowsAsync<TaskCanceledException>(async() => await task);
    }

    [Fact]
    public async Task TestTaskCancellaionWaitAsync()
    {
        CancellationTokenSource cts = new();
        Task<string?> task = Observable.Never<string>().FirstOrDefaultAsync().ToTask();
        cts.Cancel();
        await Assert.ThrowsAsync<TaskCanceledException>(async () => await task.WaitAsync(cts.Token));
    }


}