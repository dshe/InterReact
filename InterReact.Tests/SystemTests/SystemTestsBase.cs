﻿using Microsoft.Extensions.Logging;

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
        Logger = new LoggerFactory().AddMXLogger(Write, LogLevel.Debug).CreateLogger("Test");
        DynamicLogger.Add(Logger);
    }

    public async Task InitializeAsync()
    {
        await ClientTask.ConfigureAwait(false);
        //if (Client.Config.IsDemoAccount)
        //    Client.Request.RequestMarketDataType(MarketDataType.Delayed);
        //subscription = Client.Response.Spy(Logger).Subscribe(Responses.Add);
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


    protected readonly Contract Stock1 =
        new() { SecurityType = SecurityType.Stock, Symbol = "IBM", Currency = "USD", Exchange = "SMART" };

    protected readonly Contract Stock2 =
        new() { SecurityType = SecurityType.Stock, Symbol = "AAPL", Currency = "USD", Exchange = "SMART" };

    protected readonly Contract Forex1 =
        new() { SecurityType = SecurityType.Cash, Symbol = "EUR", Currency = "USD", Exchange = "IDEALPRO" };
}
