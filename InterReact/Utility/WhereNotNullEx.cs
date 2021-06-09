using System;
using System.Reactive.Linq;

namespace InterReact
{
    public static partial class Extensions
    {
        public static IObservable<T> WhereNotNull<T>(this IObservable<T?> source) where T : struct =>
            source
                .Where(x => x.HasValue)
                .Select(x => x!.Value);

        public static IObservable<T> WhereNotNull<T>(this IObservable<T?> source) where T : class =>
            source
                .Where(x => x != null)!;
    }
}
