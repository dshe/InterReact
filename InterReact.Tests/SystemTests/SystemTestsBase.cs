using Microsoft.Extensions.Logging;

namespace SystemTests;

public abstract class SystemTestsBase : IAsyncLifetime, IDisposable
{
    internal static readonly DynamicLogger DynamicLogger = new();
    private static readonly Task<IInterReactClient> ClientTask;
    protected static IInterReactClient Client => ClientTask.Result;
    protected readonly Action<string> Write;
    protected readonly ILogger Logger;

    static SystemTestsBase()
    {
        ClientTask = new InterReactClientConnector().ConnectAsync();
 
        AppDomain.CurrentDomain.DomainUnload += async (sender, e) =>
        {
            if (!ClientTask.IsCompletedSuccessfully)
                return;
            await Client.DisposeAsync().ConfigureAwait(false);
        };
    }

    protected SystemTestsBase(ITestOutputHelper output)
    {
        Write = output.WriteLine;
  
        Logger = new LoggerFactory()
            .AddMXLogger(Write, LogLevel.Debug)
            .CreateLogger("Test");
 
        DynamicLogger.Add(Logger);
    }

    public async Task InitializeAsync()
    {
        await ClientTask.ConfigureAwait(false);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
            DynamicLogger.Remove(Logger);
    }
}
