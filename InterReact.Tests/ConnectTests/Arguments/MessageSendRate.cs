using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Args;

public class MessageSendRate(ITestOutputHelper output) : ConnectTestBase(output, LogLevel.Debug)
{
    [Fact]
    public async Task SendRateTest()
    {
        IInterReactClient client = await InterReactClient.ConnectAsync(options => 
            options.LogFactory = LogFactory);

        int count = 0;
        long start = Stopwatch.GetTimestamp();
        while (Stopwatch.GetTimestamp() - start < Stopwatch.Frequency)
        {
            client.Request.RequestGlobalCancel(); // arbitrary request
            count++;
        }
        Write($"message send rate: {count:0} messages/second.");
        Assert.InRange(count, 30, 80);

        await client.DisposeAsync();
    }
}
