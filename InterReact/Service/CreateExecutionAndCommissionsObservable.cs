using System;

namespace InterReact
{
    public sealed partial class Services
    {
        /// <summary>
        /// An observable which emits Execution and CommissionReport objects, then completes.
        /// </summary>
        internal IObservable<IHasRequestId> CreateExecutionAndCommissionsObservable() =>
            Response
                .ToObservableWithIdMultiple<IHasRequestId,ExecutionEnd>(
                    Request.GetNextId, id => Request.RequestExecutions(id))
                .ToShareSource();
    }
}
