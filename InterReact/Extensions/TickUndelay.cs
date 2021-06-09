using System;
using System.Reactive.Linq;

namespace InterReact
{
    public static partial class Extensions
    {
        public static IObservable<Tick> Undelay(this IObservable<Tick> source) =>
            source.Do(tick => tick.Undelay());
    }
}
