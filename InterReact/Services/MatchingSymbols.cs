﻿using System.Reactive.Threading.Tasks;

namespace InterReact;

public partial class Service
{
    /// <summary>
    /// Returns a list of ContractDescriptions for the supplied pattern.
    /// TWS does not support more than one concurrent request.
    /// TWS error messages (AlertMessage) are directed to OnError(AlertException).
    /// </summary>
    public async Task<IList<ContractDescription>> GetMatchingSymbolsAsync(string pattern, CancellationToken ct = default)
    {
        int id = Request.GetNextId();

        Task<IList<ContractDescription>> task = Response
            .WithRequestId(id)
            .CastTo<SymbolSamples>() // Throws for other types.
            .Select(x => x.Descriptions)
            .FirstAsync()
            .ToTask(ct);

        Request.RequestMatchingSymbols(id, pattern);

        return await task.ConfigureAwait(false);
    }
}
