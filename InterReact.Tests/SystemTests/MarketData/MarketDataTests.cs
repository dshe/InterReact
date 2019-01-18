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

namespace InterReact.Tests.SystemTests.MarketData
{
    public class MarketDataTests : BaseSystemTest
    {
        public MarketDataTests(SystemTestFixture fixture, ITestOutputHelper output) : base(fixture, output) { }

        [Fact]
        public async Task T01_MarketDataTickSnapshot()
        {
            var ticks = await Client.Services.TickSnapshotObservable(Stock1).ToList();
            Assert.NotEmpty(ticks);
        }

        [Fact]
        public async Task T02_MarketDataTicks()
        {
            var ticks = Client.Services.TickConnectableObservable(Stock2);

            var connection = ticks.Connect();

            await ticks.OfType<TickPrice>().FirstAsync();
            await ticks.OfType<TickSize>().FirstAsync();
            await ticks.OfType<TickTime>().FirstAsync();

            connection.Dispose();
        }

        [Fact]
        public async Task T03_MarketDataRealtimeVolume()
        {
            var ticks = Client.Services.TickConnectableObservable(Stock2, new[] { GenericTickType.RealtimeVolume }, true);

            var connection = ticks.Connect();

            await ticks.Where(t => t.TickType == TickType.RealtimeVolume).FirstAsync();

            connection.Dispose();
        }

    }
}
