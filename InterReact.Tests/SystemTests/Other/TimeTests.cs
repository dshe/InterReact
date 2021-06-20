using InterReact;
using InterReact.SystemTests;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.SystemTests.Other
{
    public class TimeTests : TestCollectionBase
    {
        public TimeTests(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

        [Fact]
        public async Task TestTime()
        {
            var dt = await Client.Services.CurrentTimeObservable;
            //Assert.Equal(DateTimeZone.Utc, dt.Zone);
            //Assert.InRange((DateTime.UtcNow - dt).Duration().TotalSeconds, 0, 3);
            //Write($"Time: {dt}");

            //dt = dt.ToLocalTime();
            //Write($"Time: {dt} {dt.Kind}");

            //var xx = Client.Response.OfType<CurrentTime>().FirstAsync();
            //Client.Request.RequestCurrentTime();
            //var xxx = await xx;

            ;


        }
    }


}
