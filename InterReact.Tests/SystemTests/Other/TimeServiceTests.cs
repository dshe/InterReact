using NodaTime;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace Other;

public class Time : TestCollectionBase
{
    public Time(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact]
    public async Task TimeTest()
    {
        // delay to ensure both time tests do not run at the same tine
        await Task.Delay(1000);

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
    public async Task TimeServiceTest()
    {
        // delay to ensure both time tests do not run at the same tine
        await Task.Delay(1000);

        Instant dt = await Client.Service.CurrentTimeObservable;

        Write($"Time: {dt}.");
    }
}
