using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.SystemTests.MarketData
{
    public class MarketDataRequestTests : TestCollectionBase
    {
        public MarketDataRequestTests(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

        private async Task<IList<IHasRequestId>> MakeRequest(Contract contract)
        {
            int id = Client.Request.GetNextId();

            var task = Client
                .Response
                .OfType<IHasRequestId>()
                .Where(x => x.RequestId == id)
                .TakeUntil(x => x is SnapshotEndTick || (x is Alert alert && alert.IsFatal))
                .Where(x => x is not SnapshotEndTick)
                .ToList()
                .ToTask(); // start task

            Client.Request.RequestMarketDataType(MarketDataType.Delayed);

            Client.Request.RequestMarketData(id, contract, isSnapshot: true);

            IList<IHasRequestId> messages = await task;

            return messages;
        }

        [Fact]
        public async Task MarketDataTickSnapshot()
        {
             Contract contract = new()
               { SecurityType = SecurityType.Stock, Symbol = "IBM", Currency = "USD", Exchange = "SMART" };

            IList<IHasRequestId> messages = await MakeRequest(contract);

            Assert.True(messages.Count > 1);

            Assert.Equal(1, messages.OfType<Alert>().Count());
            Assert.Equal(messages.Count() - 1, messages.OfType<Tick>().Count());
        }

        [Fact]
        public async Task MarketDataTickInvalidSnapshot()
        {
            Contract contract = new()
                { SecurityType = SecurityType.Stock, Symbol = "InvalidSymbol", Currency = "USD", Exchange = "SMART" };

            IList<IHasRequestId> messages = await MakeRequest(contract);

            var message = messages.Single();
            Assert.IsType<Alert>(message);
        }
    }
}
