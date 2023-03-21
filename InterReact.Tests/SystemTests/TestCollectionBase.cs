using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.Utilities;

namespace SystemTests;

public sealed class TestFixture : IAsyncLifetime
{
    internal readonly SharedWriter SharedWriter = new();
    internal IInterReactClient? Client;

    public async Task InitializeAsync()
    {
        ILoggerFactory loggerFactory = LoggerFactory
            .Create(builder => builder
                .AddMXLogger(SharedWriter.Write)
                .SetMinimumLevel(LogLevel.Debug));

        Client = await new InterReactClientConnector()
            .WithLoggerFactory(loggerFactory)
            .ConnectAsync()
            .ConfigureAwait(false);

        // Tests should run with the demo account since orders are submitted.
        // The demo account does not have data subscriptions, so use delayed data.
        // Note that delayed data produces delayed tick types: 
        // TickType.BidPrice => TickType.DelayedBidPrice.
        await Task.Delay(500);
        Client.Request.RequestMarketDataType(MarketDataType.Delayed);
        await Task.Delay(500);
    }

    public async Task DisposeAsync()
    {
        if (Client is not null)
            await Client.DisposeAsync().ConfigureAwait(false);
    }
}

[CollectionDefinition("Test Collection")]
public sealed class TestCollection : ICollectionFixture<TestFixture>
{
    // CollectionDefinition - tests do not run in parallel, which keeps logging relevent.
    // This class has no code, and is never created.
    // Its purpose is simply as a placeholder to apply [CollectionDefinition]
    // and all the ICollectionFixture<> interfaces.
}

[Collection("Test Collection")]
public abstract class TestCollectionBase : IDisposable
{
    private bool disposed;
    private readonly Action RemoveWriter;
    protected readonly Action<string> Write;
    protected readonly IInterReactClient Client;

    protected TestCollectionBase(ITestOutputHelper output, TestFixture fixture)
    {
        fixture.SharedWriter.Add(output.WriteLine);
        RemoveWriter = () => fixture.SharedWriter.Remove(output.WriteLine);
        Write = (s) => output.WriteLine(s + "\r\n");

        Client = fixture.Client ?? throw new NullReferenceException("Client");
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposed)
            return;
        disposed = true;

        if (disposing)
        {
            // dispose managed objects
            RemoveWriter();
        }
        // free unmanaged resources and override finalizer
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
