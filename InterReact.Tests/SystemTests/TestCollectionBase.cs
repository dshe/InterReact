using Microsoft.Extensions.Logging;

namespace SystemTests;
// CollectionDefinition - tests do not run in parallel, which keeps logging relevent!

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
    // This class has no code, and is never created.
    // Its purpose is simply to be the place to apply [CollectionDefinition]
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
    }

    protected Contract StockContract1 { get; } = new()
    { 
        SecurityType = SecurityType.Stock, 
        Symbol = "IBM", 
        Currency = "USD", 
        Exchange = "SMART" 
    };

    protected Contract ForexContract1 { get; } = new()
    { 
        SecurityType = SecurityType.Cash, 
        Symbol = "EUR", 
        Currency = "USD", 
        Exchange = "IDEALPRO" 
    };
}
