using System;
using InterReact.Extensions;


namespace InterReact
{
    public sealed partial class Services
    {
        /// <summary>
        /// An observable which emits Execution and CommissionReport objects, then completes.
        /// </summary>
        internal IObservable<IHasRequestId> CreateExecutionAndCommissionsObservable() =>
            Response
            .ToObservableWithId<IHasRequestId,ExecutionEnd>(
                Request.GetNextId, requestId => Request.RequestExecutions(requestId))
            .ToShareSource();
    }
}
