using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using Stringification;

namespace InterReact
{
    public sealed partial class Services
    {
        private readonly Dictionary<string, IObservable<Union<ContractDetails, Alert>>> ContractDetailsCache = new();

        /// <summary>
        /// Returns an observable which emits a list of ContractDetails objects. and possibly alerts,
        /// using the supplied contract as selector, then completes. Results are cached. 
        /// If expiry is not specified, ContractData objects are retrieved for all expiries.
        /// If strike is not specified, ContractData objects are retrieved for all strikes.
        /// And so on. So beware that calling this method may result in attempting to retrieve a large number of contracts.
        /// This operation may take a long time and is subject to usage limiting, 
        /// so you may want to append the Timeout() operator.
        /// This observable may be chained to the contract details filters located in the Extensions namespace.
        /// </summary>
        public IObservable<Union<ContractDetails, Alert>> CreateContractDetailsObservable(Contract contract)
        {
            if (contract.ComboLegs.Any())
                throw new InvalidDataException("Contract must not include ComboLegs.");
            if (!string.IsNullOrEmpty(contract.ComboLegsDescription))
                throw new InvalidDataException("Contract must not include ComboLegsDescription.");
            if (contract.DeltaNeutralContract != null)
                throw new InvalidDataException("Contract must not include DeltaNeutralValidation.");
            if ((contract.SecurityIdType == null || contract.SecurityIdType == SecurityIdType.Undefined) ^ string.IsNullOrEmpty(contract.SecurityId))
                throw new ArgumentException("Invalid SecurityId/SecurityIdType combination.");

            string key = contract.Stringify();

            return Observable.Create<Union<ContractDetails, Alert>>(observer =>
            {
                lock (ContractDetailsCache)
                {
                    if (!ContractDetailsCache.TryGetValue(key, out IObservable<Union<ContractDetails, Alert>>? item))
                    {
                        item = ContractDetailsObservableImpl(contract).Replay().RefCount();
                        ContractDetailsCache.Add(key, item);
                    }
                    return item.Subscribe(observer);
                }
            });
        }

        private IObservable<Union<ContractDetails, Alert>> ContractDetailsObservableImpl(Contract contract) =>
            Response
                .ToObservableWithIdMultiple<ContractDetailsEnd>(
                    Request.GetNextId, id => Request.RequestContractDetails(id, contract))
                .Select(x => new Union<ContractDetails, Alert>(x));
    }
}

