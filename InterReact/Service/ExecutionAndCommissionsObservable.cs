using System;
using System.Reactive.Linq;

namespace InterReact
{
    public sealed partial class Services
    {
        /// <summary>
        /// An observable which emits Execution and CommissionReport objects, then completes.
        /// </summary>
        internal IObservable<IExecution> CreateExecutionAndCommissionsObservable() =>
            Response
                .ToObservableWithIdMultiple<IExecution, ExecutionEnd>(
                    Request.GetNextId, id => Request.RequestExecutions(id))
                .ToShareSource();
    }

    public static partial class Extensions
    {
        public static IObservable<Execution> OfTypeExecution(this IObservable<IExecution> source) =>
            source.OfType<Execution>();
    }

}
