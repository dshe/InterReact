using System;
using System.Collections.Generic;

namespace InterReact.Extensions
{
    public static class DistinctByExtension
    {
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
