using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace InterReact;

public partial class Service
{
    public async Task<IList<IHasRequestId>> GetExecutionsAsync()
    {
        var requestId = Request.GetNextId();

        var task = Response
            .OfType<IHasRequestId>()
            .Where(x => x.RequestId == requestId)
            .TakeUntil(m => m is ExecutionEnd || m is Alert)
            .Where(m => m is not ExecutionEnd)
            .ToList()
            .ToTask();

        // may return Execution, CommissionReport, ExecutionEnd and/or Alert objects.
        Request.RequestExecutions(requestId);

        IList<IHasRequestId> list = await task.ConfigureAwait(false);

        Alert? alert = list.OfType<Alert>().FirstOrDefault();
        if (alert != null)
            throw new Alert().ToException();

        return list;
    }


}

