﻿using Microsoft.Extensions.Logging;

namespace Other;

public class NoConnect : ConnectTestBase
{
    public NoConnect(ITestOutputHelper output) : base(output, LogLevel.Debug) { }

    [Fact]
    public async Task CancelTest()
    {
        CancellationToken ct = new(true);

        Task<IInterReactClient> task = InterReactClient.ConnectAsync(options =>
        {
            options.LogFactory = LogFactory;
            options.TwsPortAddress = "999";
        }, null, ct);

        OperationCanceledException ex = await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task);
        Write(ex.ToString());
    }

    [Fact]
    public async Task TimeoutTest()
    {
        CancellationTokenSource cts = new(TimeSpan.FromMilliseconds(100));

        Task<IInterReactClient> task = InterReactClient.ConnectAsync(options =>
        {
            options.LogFactory = LogFactory;
            options.TwsPortAddress = "999";
        }, null, cts.Token);

        OperationCanceledException ex = await Assert.ThrowsAsync<OperationCanceledException>(() => task);
        Write(ex.ToString());
    }

    [Fact]
    public async Task ConnectionRefusedTest()
    {
        Task<IInterReactClient> task = InterReactClient.ConnectAsync(options =>
        {
            options.LogFactory = LogFactory;
            options.TwsPortAddress = "999";
        });

        ArgumentException ex = await Assert.ThrowsAsync<ArgumentException>(() => task);
        Write("Exception: " + ex.ToString());
    }
}
