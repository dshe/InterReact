using Microsoft.Extensions.Logging;
using System.Diagnostics;
namespace ConnectTests;

public class MessageSendRate(ITestOutputHelper output) : OutputHelperTestBase(output, LogLevel.Debug)
{
    [Fact]
    public async Task SendRateTestAsync()
    {
        IInterReactClient client = await InterReactClient.CreateAsync(options => 
            options.LogFactory = LogFactory, TestContext.Current.CancellationToken);

        int count = 0;
        long start = Stopwatch.GetTimestamp();
        while (Stopwatch.GetTimestamp() - start < Stopwatch.Frequency)
        {
            await client.Request.SetServerLogLevelAsync(LogEntryLevel.Warning); // arbitrary request
            count++;
        }
        Write($"message send rate: {count:0} messages/second.");
        Assert.InRange(count, 30, 80);

        await client.DisposeAsync();
    }
}
