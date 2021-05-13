using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Microsoft.Extensions.Logging;
using MXLogger;

namespace InterReact.SystemTests
{
    public abstract class BaseTest : IAsyncLifetime
    {
        private static readonly Task<IInterReactClient> ClientTask;
        protected static IInterReactClient Client;
        protected readonly Action<string> Write;
        protected readonly ILogger Logger;
        protected int Id;
#pragma warning disable CS8618 // Non-nullable field is uninitialized.
        static BaseTest()
#pragma warning restore CS8618 // Non-nullable field is uninitialized.
        {
            ClientTask = new InterReactClientBuilder().BuildAsync();
            AppDomain.CurrentDomain.DomainUnload += async (sender, e) =>
            {
                if (Client != null)
                    await Client.DisposeAsync().ConfigureAwait(false); ;
            };
        }

        protected BaseTest(ITestOutputHelper output)
        {
            Write = output.WriteLine;
            Logger = new LoggerFactory()
                .AddMXLogger(Write)
                .CreateLogger("Test");
        }

        public async Task InitializeAsync()
        {
            if (Client == null)
                Client = await ClientTask.ConfigureAwait(false);
            //if (Client.Config.IsDemoAccount)
            //    Client.Request.RequestMarketDataType(MarketDataType.Delayed);
            Id = Client.Request.GetNextId();
            Write($"BaseTest: NextId = {Id}.");
            //subscription = Client.Response.Spy(Logger).Subscribe(Responses.Add);
        }

        public Task DisposeAsync() => Task.CompletedTask;

        protected readonly Contract Stock1 =
            new Contract { SecurityType = SecurityType.Stock, Symbol = "IBM", Currency = "USD", Exchange = "SMART" };

        protected readonly Contract Stock2 =
            new Contract { SecurityType = SecurityType.Stock, Symbol = "AAPL", Currency = "USD", Exchange = "SMART" };

        protected readonly Contract Forex1 =
            new Contract { SecurityType = SecurityType.Cash, Symbol = "EUR", Currency = "USD", Exchange = "IDEALPRO" };
    }
}
