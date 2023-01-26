using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace InterReact;

public partial class Service
{
    /// <summary>
    /// Returns Execution, CommissionReport, and possibly Alert objects. 
    /// </summary>
    public async Task<IList<object>> GetExecutionsAsync()
    {
        int id = Request.GetNextId();

        Task<IList<object>> task = Response
            .WithRequestId(id)
            .TakeWhile(m => m is not ExecutionEnd)
            .ToList()
            .ToTask();

        Request.RequestExecutions(id);

        IList<object> list = await task.ConfigureAwait(false);
 
        return list;
    }
}

