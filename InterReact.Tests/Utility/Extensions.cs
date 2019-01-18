using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace InterReact.Tests.Utility
{
    public static class Extensions
    {
        public static Exception WriteMessageTo(this Exception ex, Action<string> write)
        {
            write(ex.Message);
            if (ex.InnerException != null)
                write(ex.InnerException.Message);
            write("");
            return ex;
        }

        public static IObservable<T> ToTestObservable<T>(this IEnumerable<T> source, bool complete = true)
        {
            return Observable.Create<T>(observer =>
            {
                foreach (var item in source)
                    observer.OnNext(item);
                if (complete)
                    observer.OnCompleted();
                return Disposable.Empty;
            });
        }


    }
}
