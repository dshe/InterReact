namespace Other;

public class NoConnect : ConnectTestBase
{
    public NoConnect(ITestOutputHelper output) : base(output) { }

    [Fact]
    public async Task CancelTest()
    {
        CancellationToken ct = new(true);

        var task = new InterReactClientConnector()
            .WithPort(999)
            .WithLogger(Logger)
            .ConnectAsync(ct);

        OperationCanceledException ex = await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await task);
        Write(ex.ToString());
    }

    [Fact]
    public async Task TimeoutTest()
    {
        CancellationTokenSource cts = new(TimeSpan.FromMilliseconds(100));
 
        var task = new InterReactClientConnector()
            .WithPort(999)
            .WithLogger(Logger)
            .ConnectAsync(cts.Token);

        OperationCanceledException ex = await Assert.ThrowsAsync<OperationCanceledException>(async () => await task);
        Write(ex.ToString());
    }

    [Fact]
    public async Task ConnectionRefusedTest()
    {
        var task = new InterReactClientConnector()
            .WithLogger(Logger)
            .WithPort(999)
            .ConnectAsync();

        ArgumentException ex = await Assert.ThrowsAsync<ArgumentException>(async () => await task);
        Write("Exception: " + ex.ToString());
    }
}
