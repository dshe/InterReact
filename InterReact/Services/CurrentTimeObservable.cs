using System.Reactive.Linq;
using NodaTime;

namespace InterReact;

public partial class Service
{
    /// <summary>
    /// An observable which emits the current time, then completes.
    /// </summary>
    public IObservable<Instant> CurrentTimeObservable { get; }

    private IObservable<Instant> CreateCurrentTimeObservable() =>
        Response
            .ToObservableSingle<CurrentTime>(Request.RequestCurrentTime)
            .Select(m => m.Time)
            .ShareSource();
}

