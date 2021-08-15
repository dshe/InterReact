using InterReact.SystemTests;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.SystemTests.Service
{
    public class RealtimeBarTests : TestCollectionBase
    {
        public RealtimeBarTests(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

        [Fact]
        public async Task T01_RealtimeBarTest()
        {
            var observable = Client.Services.CreateRealtimeBarsObservable(StockContract1);

            await observable.FirstAsync().Timeout(TimeSpan.FromSeconds(10));
        }

    }
}
