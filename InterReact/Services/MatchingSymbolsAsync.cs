using Stringification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace InterReact;

public partial class Service
{
    public async Task<SymbolSamples> GetMatchingSymbolsAsync(string pattern)
    {
        int requestId = Request.GetNextId();

        Task<IHasRequestId> task = Response
            .OfType<IHasRequestId>()
            .Where(m => m.RequestId == requestId)
            .FirstAsync()
            .ToTask();

        Request.RequestMatchingSymbols(requestId, pattern);

        var message = await task.ConfigureAwait(false);

        if (message is Alert alert)
            throw new Alert().ToException();

        return (SymbolSamples)message;
    }
}
