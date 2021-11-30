using Microsoft.Extensions.Logging;
using Stringification;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.ConnectTests;

[Trait("Category", "ConnectTests")]
public abstract class ConnectTestsBase
{
    protected readonly Action<string> Write;
    protected readonly ILoggerFactory LoggerFactory;
    protected readonly ILogger Logger;

    public ConnectTestsBase(ITestOutputHelper output)
    {
        Write = output.WriteLine;
        LoggerFactory = new LoggerFactory().AddMXLogger(Write, LogLevel.Debug);
        Logger = LoggerFactory.CreateLogger("InterReact");
    }

    protected async Task TestClient(IInterReactClient client)
    {
        client.Response.Select(x => x.Stringify()).Subscribe(s => Write($"response: {s}"));
        var instant = await client.Services.CurrentTimeObservable;
        Write($"Test calling CurrentTimeObservable: {instant}");
        await Task.Delay(500);
    }
}
