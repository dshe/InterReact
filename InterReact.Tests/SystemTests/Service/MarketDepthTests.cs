using InterReact;
using InterReact.SystemTests;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.SystemTests.Service
{
    // Depth is not available from the the demo account.
    public class MarketDepthTests : TestCollectionBase
    {
        public MarketDepthTests(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

        [Fact]
        public async Task T01_MarketDepth()
        {
            if (Client.Request.Config.IsDemoAccount)
                return;

            var contract = new Contract { SecurityType = SecurityType.Cash, Symbol = "USD", Currency = "JPY", Exchange = "IDEALPRO" };

            var depth = Client.Services.CreateMarketDepthObservable(contract).OfTypeUnionSource<MarketDepth>();

            await depth.Take(10);
        }

        [Fact]
        public async Task T02_MarketDepthCollections()
        {
            if (Client.Request.Config.IsDemoAccount)
                return;

            var contract = new Contract { SecurityType = SecurityType.Cash, Symbol = "EUR", Currency = "JPY", Exchange = "IDEALPRO" };

            var depth = Client.Services.CreateMarketDepthObservable(contract);

            //var (bidCollection, askCollection) = depth.ToMarketDepthObservableCollections();

            var subscription = depth.Subscribe(m => { }, e =>
             {
                 // observe data and handle any errors
             });

            //depth.Connect();

            await depth.FirstAsync();

            //Assert.True(bidCollection.Any() || askCollection.Any());

            subscription.Dispose();
        }

    }
}
