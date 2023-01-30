using Microsoft.Extensions.Logging;

namespace SystemTests;

public class TestFixture : IAsyncLifetime
{
    internal readonly DynamicLogger DynamicLogger = new();
    internal IInterReactClient? Client;
    public TestFixture() { }
    public async Task InitializeAsync()
    {
        Client = await new InterReactClientConnector()
            .WithLogger(DynamicLogger)
            .ConnectAsync()
            .ConfigureAwait(false);
    }
    public async Task DisposeAsync()
    {
        if (Client is not null)
            await Client.DisposeAsync().ConfigureAwait(false);
    }
}

[CollectionDefinition("Test Collection")]
public class TestCollection : ICollectionFixture<TestFixture>
{
    // CollectionDefinition - tests do not run in parallel, which keeps logging relevent.
    // This class has no code, and is never created.
    // Its purpose is simply as a placeholder to apply [CollectionDefinition]
    // and all the ICollectionFixture<> interfaces.
}

[Collection("Test Collection")]
public class TestCollectionBase
{
    protected readonly Action<string?> Write;
    protected readonly ILogger Logger;
    protected readonly IInterReactClient Client;

    public TestCollectionBase(ITestOutputHelper output, TestFixture fixture)
    {
        Write = (s) => output.WriteLine(s + "\n");

        Logger = new LoggerFactory().AddMXLogger(Write, LogLevel.Debug).CreateLogger("Test");
        fixture.DynamicLogger.Add(Logger, true);

        Client = fixture.Client ?? throw new Exception("Client");

        // Tests should run with the demo account since orders are submitted.
        // The demo account does not have data subscriptions, so use delayed data.
        // Note that delayed data produces delayed tick types: 
        // TickType.BidPrice => TickType.DelayedBidPrice.
        Client.Request.RequestMarketDataType(MarketDataType.Delayed);
    }
}
