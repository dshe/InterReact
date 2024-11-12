namespace InterReact;

public partial class Service
{
    /// <summary>
    /// Creates an observable tha emits one or more contract details messages using the supplied contract as selector.
    /// For options or futures:
    /// If expiry is not specified, ContractDetails objects are retrieved for all expiries.
    /// If strike is not specified, ContractDetails objects are retrieved for all strikes.
    /// And so on. So beware that calling this method may result in attempting to retrieve a large number of contracts.
    /// </summary>
    public IObservable<ContractDetails> CreateContractDetailsObservable(Contract contract, bool includeExpired = false)
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

        return Response.ToObservableWithId<ContractDetails>(
            Request.GetNextId, 
            id => Request.RequestContractDetails(id, contract, includeExpired),
            null,
            m => m is ContractDetailsEnd);
    }

    public async Task<ContractDetails[]> GetContractDetailsAsync(Contract contract, bool includeExpired = false, TimeSpan? timeSpan = null)
    {
        return await CreateContractDetailsObservable(contract, includeExpired)
            .ToArray()
            .Timeout(timeSpan ?? TimeSpan.MaxValue);
    }

}
