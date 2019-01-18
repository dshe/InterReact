using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using InterReact.Messages;
using Xunit;
using InterReact.Tests.Utility;
using Xunit.Abstractions;
using System;

namespace InterReact.Tests.SystemTests.MarketData
{
    public class RealtimeBarTests : BaseSystemTest
    {
        public RealtimeBarTests(SystemTestFixture fixture, ITestOutputHelper output) : base(fixture, output) { }

        [Fact]
        public async Task T01_RealtimeBarTest()
        {
            var observable = Client.Services.RealtimeBarObservable(Stock1);

            //if (!Client.Status.IsDemoAccount)
                await observable.FirstAsync().Timeout(TimeSpan.FromSeconds(10));
        }

    }
}
