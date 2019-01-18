using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using InterReact.Tests.Utility;
using NodaTime;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.Tests.SystemTests.Other
{
    public class TimeTests : BaseSystemTest
    {
        public TimeTests(SystemTestFixture fixture, ITestOutputHelper output) : base(fixture, output) { }

        [Fact]
        public async Task TestTime()
        {
            var dt = await Client.Services.CurrentTimeObservable;
            //Assert.Equal(DateTimeZone.Utc, dt.Zone);
            //Assert.InRange((DateTime.UtcNow - dt).Duration().TotalSeconds, 0, 3);
            Write($"Time: {dt}");

            //dt = dt.ToLocalTime();
            //Write($"Time: {dt} {dt.Kind}");
        }
    }


}
