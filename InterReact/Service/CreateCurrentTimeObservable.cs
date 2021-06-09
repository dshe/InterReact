using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
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
