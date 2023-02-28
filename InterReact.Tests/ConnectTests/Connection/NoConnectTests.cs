using Microsoft.Extensions.Logging;

namespace Other;

public class NoConnect : ConnectTestBase
{
    public NoConnect(ITestOutputHelper output) : base(output, LogLevel.Debug) { }

    [Fact]
    public async Task CancelTest()
    {
        CancellationToken ct = new(true);

        Task<IInterReactClient> task = new InterReactClientConnector()
            .WithPort(999)
            .WithLoggerFactory(LogFactory)
            .ConnectAsync(ct);

        OperationCanceledException ex = await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await task);
        Write(ex.ToString());
    }

    [Fact]
    public async Task TimeoutTest()
    {
        CancellationTokenSource cts = new(TimeSpan.FromMilliseconds(100));

        Task<IInterReactClient> task = new InterReactClientConnector()
            .WithPort(999)
            .WithLoggerFactory(LogFactory)
            .ConnectAsync(cts.Token);

        OperationCanceledException ex = await Assert.ThrowsAsync<OperationCanceledException>(async () => await task);
        Write(ex.ToString());
    }

    [Fact]
    public async Task ConnectionRefusedTest()
    {
        Task<IInterReactClient> task = new InterReactClientConnector()
            .WithLoggerFactory(LogFactory)
            .WithPort(999)
            .ConnectAsync();

        ArgumentException ex = await Assert.ThrowsAsync<ArgumentException>(async () => await task);
        Write("Exception: " + ex.ToString());
    }
}
