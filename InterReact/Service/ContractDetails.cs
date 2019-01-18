using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using InterReact.Core;
using InterReact.Enums;
using InterReact.Interfaces;
using InterReact.Messages;
using InterReact.StringEnums;
using InterReact.Utility.Rx;
using Stringification;
using NodaTime;

namespace InterReact.Service
{
    public sealed partial class Services
    {
        private readonly Dictionary<string, IObservable<IReadOnlyList<ContractDetails>>> cache
            = new Dictionary<string, IObservable<IReadOnlyList<ContractDetails>>>();

        public Duration ContractDetailsExpiry { get; set; } = Duration.FromHours(12);

        /// <summary>
        /// Returns an observable which emits a list of ContractDetails objects using the supplied contract as selector, then completes.
        /// Results are cached. The default cache expiry is 12 hours.
        /// If expiry is not specified, ContractDetails objects are retrieved for all expiries.
        /// If strike is not specified, ContractDetails objects are retrieved for all strikes.
        /// And so on. So beware that calling this method may result in attempting to retrieve a large number of contracts.
        /// This operation may take a long time and is subject to usage limiting, so you may want to append the Timeout() operator.
        /// Unfortunately, this operation may not be cancelled.
        /// This observable may be chained to the contract details filters located in the Extensions namespace.
        /// </summary>
        public IObservable<IReadOnlyList<ContractDetails>> ContractDetailsObservable(Contract contract)
        {
            if (contract == null)
                throw new ArgumentNullException(nameof(contract));
            if (contract.ComboLegs.Any())
                throw new InvalidDataException("Contract must not include ComboLegs.");
            if (contract.ComboLegsDescription != null)
                throw new InvalidDataException("Contract must not include ComboLegsDescription.");
            if (contract.Undercomp != null)
                throw new InvalidDataException("Contract must not include DeltaNeutralValidation.");
            if ((contract.SecurityIdType == null || contract.SecurityIdType == SecurityIdType.Undefined) ^ string.IsNullOrEmpty(contract.SecurityId))
                throw new ArgumentException("Invalid SecurityId/SecurityIdType combination.");

            return Observable.Create<IReadOnlyList<ContractDetails>>(observer =>
            {
                // This key identifies the request, not necessarily the contract details(s) returned.
                var key = contract.Stringify();
                lock (cache)
                {
                    if (!cache.TryGetValue(key, out IObservable<IReadOnlyList<ContractDetails>> item))
                    {
                        item = ContractDetailsObservableImpl(contract).ToAsyncSource(ContractDetailsExpiry, Config.Clock);
                        cache.Add(key, item);
                    }
                    return item.Subscribe(observer);
                }
            });
        }

        private IObservable<IReadOnlyList<ContractDetails>> ContractDetailsObservableImpl(Contract contract)
            => Response.ToObservable<ContractDetails>(Request.NextId, requestId
                    => Request.RequestContractDetails(requestId, contract), null, m => m is ContractDetailsEnd)
                .ToArray();
    }

}
