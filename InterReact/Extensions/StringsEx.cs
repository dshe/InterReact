using InterReact.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace InterReact.Extensions
{
    public static class StringsEx
    {
        public static string JoinStrings(this IEnumerable<string> strings, string separator) => string.Join(separator, strings);

        public static IObservable<string> Stringify(this IObservable<object> source, bool includeTypeName = true) =>
            source.Select(m => Stringification.Stringifier.Stringify(m, includeTypeName));

        public static string Stringify(this object o, bool includeTypeName = true) =>
            Stringification.Stringifier.Stringify(o, includeTypeName);

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
             (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var keys = new HashSet<TKey>();
            foreach (TSource item in source)
            {
                if (keys.Add(keySelector(item)))
                {
                    yield return item;
                }
            }
        }

    }
}
