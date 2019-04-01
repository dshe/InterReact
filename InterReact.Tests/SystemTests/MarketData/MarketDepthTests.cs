using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using InterReact.Interfaces;
using InterReact.Messages;
using InterReact.Enums;
using InterReact.Tests.Utility;
using Xunit;
using Xunit.Abstractions;
using System.Reactive.Threading.Tasks;
using Stringification;
using System.Threading;
using InterReact.Extensions;
using InterReact.Utility;
using InterReact.StringEnums;

namespace InterReact.Tests.SystemTests.MarketData
{
    // Depth is not available from the the demo account.
    public class MarketDepthTests : BaseSystemTest
    {
        public MarketDepthTests(SystemTestFixture fixture, ITestOutputHelper output) : base(fixture, output) { }

        [Fact]
        public async Task T01_MarketDepth()
        {
            if (Client.Config.IsDemoAccount())
                return;

            var contract = new Contract { SecurityType = SecurityType.Cash, Symbol = "USD", Currency = "JPY", Exchange = "IDEALPRO" };

            var depth = Client.Services.MarketDepthObservable(contract);

            await depth.Take(10);
        }

        [Fact]
        public async Task T02_MarketDepthCollections()
        {
            if (Client.Config.IsDemoAccount())
                return;

            var contract = new Contract { SecurityType = SecurityType.Cash, Symbol = "EUR", Currency = "JPY", Exchange = "IDEALPRO" };

            var depth = Client.Services.MarketDepthObservable(contract);

            var (bidCollection, askCollection) = depth.ToMarketDepthObservableCollections();

            var subscription = depth.Subscribe(m => {}, e =>
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
