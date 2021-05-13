using System.Reactive.Linq;
using System.Threading.Tasks;

using Xunit;
using InterReact.SystemTests;
using Xunit.Abstractions;
using System;

namespace SystemTests.MarketData
{
    public class RealtimeBarTests : BaseTest
    {
        public RealtimeBarTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task T01_RealtimeBarTest()
        {
            var observable = Client.Services.CreateRealtimeBarObservable(Stock1);

            //if (!Client.Status.IsDemoAccount)
            await observable.FirstAsync().Timeout(TimeSpan.FromSeconds(10));
        }

    }
}
