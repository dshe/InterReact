using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace Other;

public class Time : TestCollectionBase
{
    public Time(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact(Skip = "TimeAsyncTest may not succeed just after startup (which itself returns a Time message)")]
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
}
