using NodaTime;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace Other;

public class Time : TestCollectionBase
{
    public Time(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact(Skip ="TimeTest may interfere with TimeAsyncTest")]
    public async Task TimeTest()
    {
        Task<Instant> task = Client
            .Response
            .OfType<CurrentTime>()
            .Select(m => m.Time)
            .FirstAsync()
            .ToTask();

        Client.Request.RequestCurrentTime();

        Instant dt = await task;

        Write($"Time: {dt}.");
    }

    [Fact]
    public async Task TimeAsyncTest()
    {
        Instant dt = await Client
            .Service
            .GetCurrentTimeAsync()
            .WaitAsync(TimeSpan.FromSeconds(1));

        Write($"Time: {dt}.");
    }
}
