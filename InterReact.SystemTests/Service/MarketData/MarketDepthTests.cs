using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using InterReact;
using InterReact.SystemTests;
using Xunit;
using Xunit.Abstractions;
using InterReact.Extensions;

namespace SystemTests.MarketData
{
    // Depth is not available from the the demo account.
    public class MarketDepthTests : BaseTest
    {
        public MarketDepthTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task T01_MarketDepth()
        {
            if (Client.Config.IsDemoAccount)
                return;

            var contract = new Contract { SecurityType = SecurityType.Cash, Symbol = "USD", Currency = "JPY", Exchange = "IDEALPRO" };

            var depth = Client.Services.CreateMarketDepthObservable(contract);

            await depth.Take(10);
        }

        [Fact]
        public async Task T02_MarketDepthCollections()
        {
            if (Client.Config.IsDemoAccount)
                return;

            var contract = new Contract { SecurityType = SecurityType.Cash, Symbol = "EUR", Currency = "JPY", Exchange = "IDEALPRO" };

            var depth = Client.Services.CreateMarketDepthObservable(contract);

            var (bidCollection, askCollection) = depth.ToMarketDepthObservableCollections();

            var subscription = depth.Subscribe(m => { }, e =>
             {
                // observe data and handle any errors
            });

            depth.Connect();

            await depth.FirstAsync();

            Assert.True(bidCollection.Any() || askCollection.Any());

            subscription.Dispose();
        }

    }
}
