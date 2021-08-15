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

/*
new TickPrice();
new TickSize();
new TickString();
new TickRealtimeVolume(); // from TickString
new TickTime(); // from TickString
new TickGeneric();
new TickHalted(); // from TickGeneric
new TickExchangeForPhysical(); // not tested
*/

namespace InterReact.SystemTests.Service
{
    public class MarketDataTests : TestCollectionBase
    {
        public MarketDataTests(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

        [Fact]
        public async Task T01_MarketDataTickSnapshot()
        {
            IObservable<Union<Tick, Alert>> observable = Client
                .Services
                .CreateTickSnapshotObservable(ForexContract1);

            IList<Union<Tick, Alert>> ticks = await observable.ToList();

            Assert.NotEmpty(ticks);
        }

        [Fact]
        public async Task T02_MarketDataTicks()
        {
            IObservable<Union<Tick, Alert>> observable = Client
                .Services
                .CreateTickObservable(StockContract1);

            var tick = await observable.FirstAsync();
        }

        [Fact]
        public async Task T03_MarketDataRealtimeVolume()
        {
            IObservable<Union<Tick, Alert>> observable = Client
                .Services
                .CreateTickObservable(StockContract1, new[] { GenericTickType.RealtimeVolume });

            var task = observable.OfType<Tick>().Where(t => t.TickType == TickType.RealtimeVolume).FirstAsync();

            var tick = await observable.FirstAsync();
        }

    }
}
