using System;
using System.Reactive.Linq;
using NodaTime;

namespace InterReact
{
    public sealed partial class Services
    {
        public IObservable<Instant> CreateCurrentTimeObservable() =>
            Response
                .ToObservableSingle<CurrentTime>(Request.RequestCurrentTime)
                .Select(m => m.Time)
                .ToShareSource();
    }
}
