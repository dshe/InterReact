using Microsoft.Extensions.Logging;

namespace ConnectTests;

public abstract class ConnectTestBase
{
    protected readonly Action<string> Write;
    protected readonly ILoggerFactory LoggerFactory;
    protected readonly ILogger Logger;

    public ConnectTestBase(ITestOutputHelper output)
    {
        Write = output.WriteLine;
        
        LoggerFactory = new LoggerFactory()
            .AddMXLogger(Write, LogLevel.Debug);
        
        Logger = LoggerFactory
            .CreateLogger("InterReact");
    }

    protected async Task TestClient(IInterReactClient client)
    {
        Instant instant = await client
            .Service
            .GetCurrentTimeAsync()
            .WaitAsync(TimeSpan.FromSeconds(6));

        Write($"Test received time from CurrentTimeObservable: {instant}");
    }

}
