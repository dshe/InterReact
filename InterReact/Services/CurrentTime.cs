using System.Reactive.Threading.Tasks;

namespace InterReact;

public partial class Service
{
    public async Task<Instant> GetCurrentTimeAsync(CancellationToken ct = default)
    {
        Task<Instant> task = Response
            .OfType<CurrentTime>()
            .Select(time =>time.Seconds)
            .Select(seconds => Instant.FromUnixTimeSeconds(seconds))
            .FirstAsync()
            .ToTask(ct);

        Request.RequestCurrentTime();

        return await task.ConfigureAwait(false);
    }
}