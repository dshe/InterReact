using NodaTime;
using Stringification;
using System.Reactive.Linq;
namespace CurrentTime;

public class CurrentTimes(ITestOutputHelper output, TestFixture fixture) : CollectionTestBase(output, fixture)
{
    [Fact]
    public async Task CurrentTimeTest()
    {
        Instant time = await Client
            .Service
            .CreateCurrentTimeObservable()
            .Timeout(TimeSpan.FromSeconds(1))
            .FirstAsync();

        Write(time.Stringify());
    }
}
