using InterReact.Messages;
using System;
using System.Reactive.Linq;

namespace InterReact.Extensions
{
    public static class TickExtensions
    {
        public static IObservable<T> Undelay<T>(this IObservable<T> source) where T : Tick =>
            source.Do(x => x.Undelay());
    }
}
