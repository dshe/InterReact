using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace InterReact.Tests.Utility
{
    public static class Extensions
    {
        public static Exception WriteMessageTo(this Exception ex, ILogger logger)
        {
            logger.LogDebug(ex.Message);
            if (ex.InnerException != null)
                logger.LogDebug(ex.InnerException.Message);
            logger.LogDebug("");
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
