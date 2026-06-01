using Microsoft.Extensions.Logging;
namespace ConnectTests;

public class NoConnect(ITestOutputHelper output) : OutputHelperTestBase(output, LogLevel.Debug)
{
    [Fact]
    public async Task CancelTest()
    {
        CancellationToken ct = new(true);

        Task<IInterReactClient> task = InterReactClient.CreateAsync(options =>
        {
            options.LogFactory = LogFactory;
            options.TwsPortAddresses = [999];
        }, ct);

        OperationCanceledException ex = await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task);
        Write(ex.ToString());
    }

    [Fact]
    public async Task TimeoutTest()
    {
        CancellationTokenSource cts = new(TimeSpan.FromMilliseconds(100));

        Task<IInterReactClient> task = InterReactClient.CreateAsync(options =>
        {
            options.LogFactory = LogFactory;
            options.TwsPortAddresses = [999];
        }, cts.Token);

        OperationCanceledException ex = await Assert.ThrowsAsync<OperationCanceledException>(() => task);
        Write(ex.ToString());
    }

    [Fact]
    public async Task ConnectionRefusedTest()
    {
        Task<IInterReactClient> task = InterReactClient.CreateAsync(options =>
        {
            options.LogFactory = LogFactory;
            options.TwsPortAddresses = [999];
        }, TestContext.Current.CancellationToken);

        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(() => task);
        Write("Exception: " + ex.ToString());
    }
}
