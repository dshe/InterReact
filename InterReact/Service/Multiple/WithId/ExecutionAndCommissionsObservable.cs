using System;
using System.Reactive.Linq;

namespace InterReact
{
    public partial class Services
    {
        /// <summary>
        /// An observable which emits Execution and CommissionReport objects, then completes.
        /// Each subscription makes a separate request.
        /// </summary>
        public IObservable<Union<Execution, Alert>> ExecutionAndCommissionsObservable { get; }

        private IObservable<Union<Execution, Alert>> CreateExecutionAndCommissionsObservable() =>
            Response
                .ToObservableMultipleWithId<ExecutionEnd>(
                    Request.GetNextId, id => Request.RequestExecutions(id))
                .Select(x => new Union<Execution, Alert>(x));
    }
}
