using InterReact.SystemTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.SystemTests.MarketData
{
    public class MarketDataServiceTests : TestCollectionBase
    {
        public MarketDataServiceTests(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

        private async Task<IList<ITick>> MakeRequest(Contract contract)
        {
            Client.Request.RequestMarketDataType(MarketDataType.Delayed);

            IList<ITick> ticks = await Client
                .Services
                .CreateTickObservable(contract)
                .Take(TimeSpan.FromSeconds(2))
                .ToList();

            return ticks;
        }

        [Fact]
        public async Task TestTicks()
        {
            Contract contract = new()
                { SecurityType = SecurityType.Stock, Symbol = "IBM", Currency = "USD", Exchange = "SMART" };

            IList<ITick> ticks = await MakeRequest(contract);

            Assert.NotEmpty(ticks);
        }

        [Fact]
        public async Task TestTicksInvalid()
        {
            Contract contract = new()
                { SecurityType = SecurityType.Stock, Symbol = "InvalidSymbol", Currency = "USD", Exchange = "SMART" };

            var alertException = await Assert.ThrowsAsync<AlertException>(async () => await MakeRequest(contract));

            Write(alertException.Message);
        }
    }
}
