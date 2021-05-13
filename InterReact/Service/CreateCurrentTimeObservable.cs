using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using InterReact.Extensions;

using NodaTime;

namespace InterReact
{
    public sealed partial class Services
    {
        public IObservable<Instant> CreateCurrentTimeObservable() =>
            Response
            .ToObservable<CurrentTime>(Request.RequestCurrentTime)
            .Select(m => m.Time)
            .ToShareSource();
    }
}
