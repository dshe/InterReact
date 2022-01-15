using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
namespace InterReact.SystemTests;
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
        if (Client != null)
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
[Trait("Category", "SystemTests")]
public class TestCollectionBase
{
    protected readonly Action<string?> Write;
    protected readonly ILogger Logger;
    protected readonly IInterReactClient Client;

    public TestCollectionBase(ITestOutputHelper output, TestFixture fixture)
    {
        Write = output.WriteLine;

        Logger = new LoggerFactory().AddMXLogger(Write, LogLevel.Debug).CreateLogger("Test");
        fixture.DynamicLogger.Add(Logger, true);

        Client = fixture.Client ?? throw new Exception("Client");
    }

    protected readonly Contract StockContract1 = new()
    { SecurityType = SecurityType.Stock, Symbol = "IBM", Currency = "USD", Exchange = "SMART" };

    protected readonly Contract ForexContract1 = new()
    { SecurityType = SecurityType.Cash, Symbol = "EUR", Currency = "USD", Exchange = "IDEALPRO" };
}
