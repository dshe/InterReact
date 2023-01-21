using System.Diagnostics;

namespace Args;

public class MessageSendRate : ConnectTestBase
{
    public MessageSendRate(ITestOutputHelper output) : base(output) { }

    [Fact]
    public async Task SendRateDefaultTest()
    {
        int count = 0;
        IInterReactClient client = await new InterReactClientConnector()
            .WithLogger(Logger)
            .ConnectAsync();
        await Task.Delay(100);

        long start = Stopwatch.GetTimestamp();
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
        int count = 0;
        var client = await new InterReactClientConnector()
            .WithLogger(Logger)
            .WithMaxRequestsPerSecond(100)
            .ConnectAsync();
        await Task.Delay(100);

        long start = Stopwatch.GetTimestamp();
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
