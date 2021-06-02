using InterReact;
using InterReact.SystemTests;
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
            var ticks = await Client.Services.CreateTickSnapshotObservable(Forex1).ToList();
            Assert.NotEmpty(ticks);
        }

        [Fact]
        public async Task T02_MarketDataTicks()
        {
            var ticks = Client.Services.CreateTickConnectableObservable(Stock1);

            var task = ticks.OfType<TickPrice>().FirstAsync();

            var connection = ticks.Connect();

            var tick = await task;

            connection.Dispose();
        }

        [Fact]
        public async Task T03_MarketDataRealtimeVolume()
        {
            var ticks = Client.Services.CreateTickConnectableObservable(Stock2, new[] { GenericTickType.RealtimeVolume }, true);

            //var task = ticks.Where(t => t.TickType == TickType.RealtimeVolume).FirstAsync();
            var task = ticks.Skip(1).FirstAsync().ToTask();

            var connection = ticks.Connect();

            var tick = await task;
            ;
            connection.Dispose();
        }
    }
}
