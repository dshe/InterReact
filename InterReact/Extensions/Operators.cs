using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace InterReact;

public static partial class Extension
{
    internal static IObservable<object> WithRequestId(this IObservable<object> source, int id)
    {
        return source
            .OfType<IHasRequestId>()
            .Where(x => x.RequestId == id)
            .Cast<object>();
    }
}
