namespace Other;

public class NoConnect : ConnectTestBase
{
    public NoConnect(ITestOutputHelper output) : base(output) { }

    [Fact]
    public async Task CancelTest()
    {
        var ct = new CancellationToken(true);

        var task = new InterReactClientConnector()
            .WithPort(999)
            .WithLogger(Logger)
            .ConnectAsync(ct);

        var ex = await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await task);
        Write(ex.ToString());
    }

    [Fact]
    public async Task TimeoutTest()
    {
        var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));
 
        var task = new InterReactClientConnector()
            .WithPort(999)
            .WithLogger(Logger)
            .ConnectAsync(cts.Token);
 
        var ex = await Assert.ThrowsAsync<OperationCanceledException>(async () => await task);
        Write(ex.ToString());
    }

    [Fact]
    public async Task ConnectionRefusedTest()
    {
        var task = new InterReactClientConnector()
            .WithLogger(Logger)
            .WithPort(999)
            .ConnectAsync();
 
        var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await task);
        Write("Exception: " + ex.ToString());
    }
}
