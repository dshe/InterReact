using System.Reactive.Threading.Tasks;

namespace InterReact;

public partial class Service
{
    /// <summary>
    /// Returns SymbolSamples and/or Alert objects.
    /// TWS does not support more than one concurrent request.
    /// </summary>
    public async Task<IList<object>> GetMatchingSymbolsAsync(string pattern, CancellationToken ct = default)
    {
        await MatchingSymbolsSemaphore.WaitAsync(ct).ConfigureAwait(false);

        try
        {
            int id = Request.GetNextId();

            Task<IList<object>> task = Response
                .WithRequestId(id)
                .TakeUntil(x => x is SymbolSamples)
                .ToList()
                .ToTask(ct);

            Request.RequestMatchingSymbols(id, pattern);

            IList<object> list = await task.WaitAsync(ct).ConfigureAwait(false);

            return list;
        }
        finally 
        { 
            MatchingSymbolsSemaphore.Release(); 
        }
    }
}
