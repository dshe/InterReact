using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.UnitTests.Experimental;

public static class ExtensionX
{
    // This is inadequate because after timeout the task keeps running!
    // Need to pass a cancellation token to cancel the task.
    public static async Task<TResult> TimeoutAfter<TResult>(this Task<TResult> task, TimeSpan timeout)
    {

        using CancellationTokenSource timeoutCancellationTokenSource = new();

        Task completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
        if (completedTask == task)
        {
            timeoutCancellationTokenSource.Cancel();
            return await task;  // Very important in order to propagate exceptions
        }
        else
        {
            throw new TimeoutException("The operation has timed out.");
        }
    }
}

public class AsyncTimeout : UnitTestsBase
{
    public AsyncTimeout(ITestOutputHelper output) : base(output) { }

    public async Task<string> GetString()
    {
        for (int i = 0; i < 20; i++)
        {
            Logger.LogInformation("wait " + i);
            await Task.Delay(500);
        }
        return "finished";
    }

    [Fact]
    public async Task Test_Async_Timeout()
    {
        try
        {
            await GetString().TimeoutAfter(TimeSpan.FromMilliseconds(1000));
            Logger.LogInformation("complete");
        }
        catch (Exception e)
        {
            Logger.LogInformation(e.Message);
        }

        await Task.Delay(4000);
        ;
    }
}
