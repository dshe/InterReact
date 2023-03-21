using System.Reactive.Threading.Tasks;

namespace InterReact;

public partial class Service
{
    /// <summary>
    /// Returns Execution, CommissionReport, and possibly Alert objects. 
    /// </summary>
    public async Task<IList<IHasRequestId>> GetExecutionsAsync(CancellationToken ct = default)
    {
        int id = Request.GetNextId();

        Task<IList<IHasRequestId>> task = Response
            .WithRequestId(id)
            .TakeWhile(m => m is not ExecutionEnd)
            .ToList()
            .ToTask(ct);

        var filter = new ExecutionFilter();
        
        Request.RequestExecutions(id , new ExecutionFilter());

        IList<IHasRequestId> list = await task.ConfigureAwait(false);
 
        return list;
    }
}
