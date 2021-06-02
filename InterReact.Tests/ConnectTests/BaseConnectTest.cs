using Microsoft.Extensions.Logging;
using Stringification;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.ConnectTests
{
    [Trait("Category", "ConnectTests")]
    public abstract class BaseConnectTest
    {
        protected readonly Action<string> Write;
        protected readonly ILoggerFactory LoggerFactory;
        protected readonly ILogger Logger;

        public BaseConnectTest(ITestOutputHelper output)
        {
            Write = output.WriteLine;
            LoggerFactory = new LoggerFactory().AddMXLogger(Write);
            Logger = LoggerFactory.CreateLogger("InterReact");
        }

        protected async Task TestClient(IInterReactClient client)
        {
            client.Response.StringifyItems().Subscribe(s => Write($"response: {s}"));
            var instant = await client.Services.CreateCurrentTimeObservable();
            Write($"Instant: {instant}");
            await Task.Delay(500);
        }
    }
}
