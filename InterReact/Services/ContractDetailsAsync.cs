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
    private readonly Dictionary<string, Task<IList<IHasRequestId>>> ContractDetailsCache = new();

    /// <summary>
    /// Returns a list of one or more contract details objects using the supplied 
    /// contract as selector. Results are cached. 
    /// For options or futures:
    /// If expiry is not specified, ContractDetails objects are retrieved for all expiries.
    /// If strike is not specified, ContractDetails objects are retrieved for all strikes.
    /// And so on. So beware that calling this method may result in attempting to retrieve a large number of contracts.
    /// This operation may take a long time and is subject to usage limiting, 
    /// so the Timeout() operator may be useful.
    /// </summary>
    public async Task<List<ContractDetails>> GetContractDetailsAsync(Contract contract)
    {
        ArgumentNullException.ThrowIfNull(contract);

        if (contract.ComboLegs.Any())
            throw new ArgumentException("Contract must not include ComboLegs.");
        if (!string.IsNullOrEmpty(contract.ComboLegsDescription))
            throw new ArgumentException("Contract must not include ComboLegsDescription.");
        if (contract.DeltaNeutralContract is not null)
            throw new ArgumentException("Contract must not include DeltaNeutralValidation.");
        if (contract.SecurityIdType == SecurityIdType.Undefined ^ string.IsNullOrEmpty(contract.SecurityId))
            throw new ArgumentException("Invalid SecurityId/SecurityIdType combination.");

        string key = contract.Stringify();
        
        Task<IList<IHasRequestId>>? task;
        lock (ContractDetailsCache)
        {
            if (!ContractDetailsCache.TryGetValue(key, out task))
            {
                task = GetContractDetailsTask(contract); // start task
                ContractDetailsCache[key] = task;
            }
        }

        // await task outside lock
        var result = await task.ConfigureAwait(false);

        lock (ContractDetailsCache)
        {
            Alert? alert = result.OfType<Alert>().FirstOrDefault();
            if (alert == null)
                return result.Cast<ContractDetails>().ToList();
            ContractDetailsCache.Remove(key); // try

            throw new Alert().ToException();       
        }
    }

    private Task<IList<IHasRequestId>> GetContractDetailsTask(Contract contract)
    {
        int requestId = Request.GetNextId();

        Task<IList<IHasRequestId>> task = Response
            .OfType<IHasRequestId>()
            .Where(m => m.RequestId == requestId)
            .TakeUntil(m => m is ContractDetailsEnd || m is Alert)
            .Where(m => m is ContractDetails || m is Alert)
            .ToList()
            .ToTask();

        Request.RequestContractDetails(requestId, contract);

        return task;
    }

}
