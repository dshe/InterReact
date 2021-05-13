using System;
using System.Threading.Tasks;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using MXLogger;
using Stringification;

namespace InterReact.ConnectTests
{
    public abstract class BaseTest
    {
        protected readonly Action<string> Write;
        protected readonly ILoggerFactory LoggerFactory;
        protected readonly ILogger Logger;

        public BaseTest(ITestOutputHelper output)
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
