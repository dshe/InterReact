using Stringification;
using System.Reactive.Threading.Tasks;

namespace InterReact;

public partial class Service
{
    private readonly Dictionary<string, Task<IList<ContractDetails>>> ContractDetailsCache = new();

    /// <summary>
    /// Returns a list of one or more contract details objects using the supplied 
    /// contract as selector. Results are cached. 
    /// For options or futures:
    /// If expiry is not specified, ContractDetails objects are retrieved for all expiries.
    /// If strike is not specified, ContractDetails objects are retrieved for all strikes.
    /// And so on. So beware that calling this method may result in attempting to retrieve a large number of contracts.
    /// </summary>
    public async Task<IList<ContractDetails>> GetContractDetailsAsync(Contract contract, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(contract);

        if (contract.ComboLegs.Any())
            throw new ArgumentException("Contract must not include ComboLegs.");
        if (!string.IsNullOrEmpty(contract.ComboLegsDescription))
            throw new ArgumentException("Contract must not include ComboLegsDescription.");
        if (contract.DeltaNeutralContract is not null)
            throw new ArgumentException("Contract must not include DeltaNeutralValidation.");
        if (contract.SecurityIdType.Length == 0 ^ string.IsNullOrEmpty(contract.SecurityId))
            throw new ArgumentException("Invalid SecurityId/SecurityIdType combination.");

        string key = contract.Stringify();

        Task<IList<ContractDetails>>? task;
        lock (ContractDetailsCache)
        {
            if (!ContractDetailsCache.TryGetValue(key, out task))
            {
                task = GetContractDetailsTask(contract, ct); // start task
                ContractDetailsCache[key] = task;
            }
        }

        try
        {
            // await task outside lock
            return await task.WaitAsync(ct).ConfigureAwait(false);
        }
        catch
        {
            lock (ContractDetailsCache)
            {
                ContractDetailsCache.Remove(key); // try
            }
            throw;
        }
    }

    private Task<IList<ContractDetails>> GetContractDetailsTask(Contract contract, CancellationToken ct)
    {
        int id = Request.GetNextId();

        Task<IList<ContractDetails>> task = Response
            .WithRequestId(id)
            .AlertMessageToError()
            .TakeWhile(m => m is not ContractDetailsEnd)
            .Cast<ContractDetails>()
            .ToList()
            .ToTask(ct);

        Request.RequestContractDetails(id, contract);

        return task;
    }
}
