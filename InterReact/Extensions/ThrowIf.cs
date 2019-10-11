using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace InterReact.Extensions
{
    public static class ThrowIf
    {
        public static T ThrowIfNull<T>(T source) where T : class =>
            source ?? throw new ArgumentNullException(nameof(source));

        public static IObservable<T> ThrowIfEmpty<T>(IObservable<T> source) where T : IEnumerable =>
            (source ?? throw new ArgumentNullException(nameof(source)))
            .Do(m =>
            {
                if (!(m as IEnumerable).GetEnumerator().MoveNext())
                    throw new InvalidDataException("Empty collection.");
            });
    }
}
