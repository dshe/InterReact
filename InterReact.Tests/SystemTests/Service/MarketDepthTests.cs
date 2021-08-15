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
            //if (Client.Request.Config.IsDemoAccount)
            //    return;

            //Client.Request.RequestMarketDepth(7, ForexContract1);
            Client.Request.RequestContractDetails(7, ForexContract1);

            /*
            IObservable<Union<MarketDepth, Alert>> observable = Client
                .Services
                .CreateMarketDepthObservable(ForexContract1);

            observable.Subscribe();
            */

            await Task.Delay(3000);
            //var depth = await observable.Take(TimeSpan.FromSeconds(10)).ToList();
            ;

        }

        [Fact]
        public async Task T02_MarketDepthCollections()
        {
            if (Client.Request.Config.IsDemoAccount)
                return;

            var depth = Client.Services.CreateMarketDepthObservable(ForexContract1);

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
