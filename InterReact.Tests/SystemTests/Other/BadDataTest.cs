using InterReact;
using InterReact.SystemTests;
using Stringification;
using System;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.SystemTests.Other
{
    public class BadDataTest : TestCollectionBase
    {
        public BadDataTest(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

        [Fact]
        public async Task TestBadRequest()
        {
            var id = Client.Request.GetNextId();

            var alert = Client.Response
                .OfType<IHasRequestId>()
                .Where(r => r.RequestId == id)
                .FirstAsync()
                .Timeout(TimeSpan.FromSeconds(5))
                .ToTask();

            Client.Request.RequestMarketData(id, new Contract());

            var x = await alert;
            Assert.IsType<Alert>(x);
        }

        [Fact]
        public async Task TestBadResponse()
        {
            var requestId = int.MaxValue; // this value will trigger a receive parse error

            var response = Client.Response;

            Client.Request.RequestMarketData(requestId, StockContract1);

            await Assert.ThrowsAnyAsync<InvalidDataException>(async () => await response);
        }
    }
}
