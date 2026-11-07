using System.Reactive.Threading.Tasks;
namespace InterReact;

public partial class Service
{
    /// <summary>
    /// For options or futures:
    /// If expiry is not specified, ContractDetails objects are retrieved for all expiries.
    /// If strike is not specified, ContractDetails objects are retrieved for all strikes.
    /// And so on. So beware that calling this method may result in attempting to retrieve a large number of contracts.
    /// </summary>
    public async Task<IHasRequestId[]> GetContractDetailsAsync(Contract contract, bool includeExpired = false, CancellationToken ct = default)
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

        int requestId = _request.GetNextId();

        Task<IHasRequestId[]> task = _response
            .WithRequestId(requestId)
            .TakeUntil(m => m is Alert || m is ContractDetailsEnd)
            .Where(m => m is not ContractDetailsEnd)
            .ToArray()
            .ToTask(ct);

        await _request.RequestContractDetailsAsync(requestId, contract, includeExpired).ConfigureAwait(false);

        IHasRequestId[] messages = await task.ConfigureAwait(false);

        return messages;
    }
}
