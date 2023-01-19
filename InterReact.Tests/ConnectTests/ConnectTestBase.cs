using Microsoft.Extensions.Logging;
using System.Reactive.Linq;

namespace ConnectTests;

public abstract class ConnectTestBase
{
    protected readonly Action<string> Write;
    protected readonly ILoggerFactory LoggerFactory;
    protected readonly ILogger Logger;

    public ConnectTestBase(ITestOutputHelper output)
    {
        Write = output.WriteLine;
        LoggerFactory = new LoggerFactory().AddMXLogger(Write, LogLevel.Debug);
        Logger = LoggerFactory.CreateLogger("InterReact");
    }

    protected async Task TestClient(IInterReactClient client)
    {
        var instant = await client.Service.CurrentTimeObservable;
        Write($"Test received time from CurrentTimeObservable: {instant}");
    }

    protected Contract StockContract1 { get; } = new()
    { 
        SecurityType = SecurityType.Stock, 
        Symbol = "IBM", 
        Currency = "USD", 
        Exchange = "SMART" 
    };
}
