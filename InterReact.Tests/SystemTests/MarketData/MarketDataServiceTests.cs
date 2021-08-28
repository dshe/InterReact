using InterReact;
using InterReact.SystemTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.SystemTests.MarketData
{
    public class MarketDataServiceTests : TestCollectionBase
    {
        public MarketDataServiceTests(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

        [Fact]
        public async Task T01_MarketDataTickSnapshot()
        {
            IObservable<ITick> observable = Client
                .Services
                .CreateTickSnapshotObservable(ForexContract1);

            IList<ITick> ticks = await observable.ToList();

            Assert.NotEmpty(ticks);
        }

        [Fact]
        public async Task T02_MarketDataTicks()
        {
            IObservable<ITick> observable = Client
                .Services
                .CreateTickObservable(StockContract1);

            var tick = await observable.FirstAsync();
        }

        [Fact]
        public async Task T03_MarketDataRealtimeVolume()
        {
            IObservable<ITick> observable = Client
                .Services
                .CreateTickObservable(StockContract1, new[] { GenericTickType.RealtimeVolume });

            var task = observable.OfTickType(t => t.RealtimeVolumeTick).FirstAsync();

            var tick = await observable.FirstAsync();
        }

    }
}
