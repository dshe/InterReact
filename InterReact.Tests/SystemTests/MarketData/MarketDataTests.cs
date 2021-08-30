using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.SystemTests.MarketData
{
    public class MarketDataTests : TestCollectionBase
    {
        public MarketDataTests(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

        private async Task<List<IHasRequestId>> MakeRequest(Contract contract)
        {
            int id = Client.Request.GetNextId();

            List<IHasRequestId> messages = new();

            var subscription = Client
                .Response
                .OfType<IHasRequestId>()
                .Where(x => x.RequestId == id)
                .Subscribe(m => messages.Add(m));

            Client.Request.RequestMarketDataType(MarketDataType.Delayed);

            Client.Request.RequestMarketData(id, contract, isSnapshot: false);

            await Task.Delay(2000);

            subscription.Dispose();

            return messages;
        }

        [Fact]
        public async Task TestTicks()
        {
            Contract contract = new()
                { SecurityType = SecurityType.Stock, Symbol = "IBM", Currency = "USD", Exchange = "SMART" };

            List<IHasRequestId> messages = await MakeRequest(contract);

            Assert.NotEmpty(messages);
            Alert alert = messages.OfType<Alert>().Single();
            Write(alert.Message);
            Assert.False(alert.IsFatal);
            Assert.Equal(messages.Count() - 1, messages.OfType<Tick>().Count());
        }

        [Fact]
        public async Task TestTicksInvalid()
        {
            Contract contract = new()
                { SecurityType = SecurityType.Stock, Symbol = "InvalidSymbol", Currency = "USD", Exchange = "SMART" };

            List<IHasRequestId> messages = await MakeRequest(contract);

            var m = messages.Single();

            Alert alert = messages.OfType<Alert>().Single();
            Assert.True(alert.IsFatal);
            Write(alert.Message);
        }
    }
}
