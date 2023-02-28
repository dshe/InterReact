using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Args;

public class MessageSendRate : ConnectTestBase
{
    public MessageSendRate(ITestOutputHelper output) : base(output, LogLevel.Debug) { }

    [Fact]
    public async Task SendRateTest()
    {
        int count = 0;
        IInterReactClient client = await new InterReactClientConnector()
            .WithLoggerFactory(LogFactory)
            .ConnectAsync();

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
