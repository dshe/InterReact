using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

// pseudo discriminated unions
namespace InterReact
{
    public abstract class Union
    {
        public object Source { get; }
        public Union(object source) => Source = source;
    }

    public class Union<T1, T2> : Union
        where T1 : notnull
        where T2 : notnull
    {
        public Union(object source) : base(source)
        {
            Debug.Assert(source is T1 || source is T2);
        }
    }

    public class Union<T1, T2, T3> : Union
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull
    {
        public Union(object source) : base(source)
        {
            Debug.Assert(source is T1 || source is T2 || source is T3);
        }
    }

    public class Union<T1, T2, T3, T4> : Union
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull
        where T4 : notnull
    {
        public Union(object source) : base(source)
        {
            Debug.Assert(source is T1 || source is T2 || source is T3 || source is T4);
        }
    }

    public static partial class Extensions
    {
        public static IObservable<T> OfTypeUnionSource<T>(this IObservable<Union> source) =>
            source.Select(x => x.Source).OfType<T>();
    }

}
