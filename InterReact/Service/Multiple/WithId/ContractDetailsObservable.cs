using System;
using System.Linq;
using System.Reactive.Linq;

namespace InterReact
{
    public sealed partial class Services
    {
        /// <summary>
        /// Returns an observable which emits a list of ContractDetails objects and possibly alerts,
        /// using the supplied contract as selector, then completes. Results are cached. 
        /// If expiry is not specified, ContractData objects are retrieved for all expiries.
        /// If strike is not specified, ContractData objects are retrieved for all strikes.
        /// And so on. So beware that calling this method may result in attempting to retrieve a large number of contracts.
        /// This operation may take a long time and is subject to usage limiting, 
        /// so the Timeout() operator may be useful.
        /// Each subscription makes a separate request.
        /// </summary>
        public IObservable<Union<ContractDetails, Alert>> CreateContractDetailsObservable(Contract contract)
        {
            if (contract.ComboLegs.Any())
                throw new ArgumentException("Contract must not include ComboLegs.");
            if (!string.IsNullOrEmpty(contract.ComboLegsDescription))
                throw new ArgumentException("Contract must not include ComboLegsDescription.");
            if (contract.DeltaNeutralContract != null)
                throw new ArgumentException("Contract must not include DeltaNeutralValidation.");
            if ((contract.SecurityIdType == null || contract.SecurityIdType == SecurityIdType.Undefined) ^ string.IsNullOrEmpty(contract.SecurityId))
                throw new ArgumentException("Invalid SecurityId/SecurityIdType combination.");

            return Response
                .ToObservableMultipleWithId<ContractDetailsEnd>(
                    Request.GetNextId, id => Request.RequestContractDetails(id, contract))
                .Select(x => new Union<ContractDetails, Alert>(x));
        }
    }
}

