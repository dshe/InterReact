using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.SystemTests
{
    public abstract class TestBase : IAsyncLifetime, IDisposable
    {
        internal static readonly DynamicLogger DynamicLogger = new();
        private static readonly Task<IInterReactClient> ClientTask;
        protected static IInterReactClient Client => ClientTask.Result;
        protected readonly Action<string> Write;
        protected readonly ILogger Logger;
        protected int Id;

        static TestBase()
        {
            ClientTask = InterReactClientBuilder.Create().BuildAsync();
            AppDomain.CurrentDomain.DomainUnload += async (sender, e) =>
            {
                if (!ClientTask.IsCompletedSuccessfully)
                    return;
                await Client.DisposeAsync().ConfigureAwait(false);
            };
        }

        protected TestBase(ITestOutputHelper output)
        {
            Write = output.WriteLine;
            Logger = new LoggerFactory().AddMXLogger(Write).CreateLogger("Test");
            DynamicLogger.Add(Logger);
        }

        public async Task InitializeAsync()
        {
            await ClientTask.ConfigureAwait(false);
            //if (Client.Config.IsDemoAccount)
            //    Client.Request.RequestMarketDataType(MarketDataType.Delayed);
            Id = Client.Request.GetNextId();
            Write($"BaseTest: NextId = {Id}.");
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
}
