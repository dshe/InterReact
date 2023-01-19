using System.Diagnostics;
using System.Net;
using System.Reactive.Linq;

namespace Args;

public class MessageSendRate : ConnectTestBase
{
    public MessageSendRate(ITestOutputHelper output) : base(output) { }

    [Fact]
    public async Task SendRateDefaultTest()
    {
        var count = 0;
        var client = await new InterReactClientConnector().WithLogger(Logger).ConnectAsync();
        await Task.Delay(100);

        var start = Stopwatch.GetTimestamp();
        while (Stopwatch.GetTimestamp() - start < Stopwatch.Frequency)
        {
            client.Request.RequestGlobalCancel();
            count++;
        }
        Write($"message send rate: {count:0} messages/second.");
        Assert.InRange(count, 10, 100);

        await client.DisposeAsync();
    }

    [Fact]
    public async Task SendRateChangeTest()
    {
        var count = 0;
        var client = await new InterReactClientConnector().WithLogger(Logger).WithMaxRequestsPerSecond(100).ConnectAsync();
        await Task.Delay(100);

        var start = Stopwatch.GetTimestamp();
        while (Stopwatch.GetTimestamp() - start < Stopwatch.Frequency)
        {
            client.Request.RequestGlobalCancel();
            count++;
        }

        Write($"message send rate: {count:0} messages/second.");
        Assert.InRange(count, 0, 110);

        await client.DisposeAsync();
    }
}
