using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace InterReact;

public partial class Service
{    
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

