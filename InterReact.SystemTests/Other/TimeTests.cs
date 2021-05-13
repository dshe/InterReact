using System.Reactive.Linq;
using System.Threading.Tasks;
using InterReact;
using InterReact.SystemTests;
using Xunit;
using Xunit.Abstractions;

namespace SystemTests.Other
{
    public class TimeTests : BaseTest
    {
        public TimeTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task TestTime()
        {
            var dt = await Client.Services.CreateCurrentTimeObservable();
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
