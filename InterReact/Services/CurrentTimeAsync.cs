using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace InterReact;

public partial class Service
{
    /// <summary>
    /// Returns the current time from TWS.
    /// Concurrent calls are not supported. 
    /// </summary>
    public async Task<Instant> GetCurrentTimeAsync()
    {
        Task<CurrentTime> task = Response
            .OfType<CurrentTime>()
            .FirstAsync()
            .ToTask();
        
        Request.RequestCurrentTime();

        CurrentTime currentTime = await task.ConfigureAwait(false);

        Instant time = currentTime.Time;

        return time; 
    }
}

