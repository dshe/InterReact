using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace InterReact;

public partial class Service
{    
    /// <summary>
    /// Returns SymbolSamples and/or Alert objects.
    /// </summary>
    public async Task<IList<object>> GetMatchingSymbolsAsync(string pattern)
    {
        int id = Request.GetNextId();

        Task<IList<object>> task = Response
            .WithRequestId(id)
            .TakeUntil(x => x is SymbolSamples)
            .ToList()
            .ToTask();

        Request.RequestMatchingSymbols(id, pattern);

        IList<object> list = await task.ConfigureAwait(false);

        return list;
    }
}
