using Microsoft.Extensions.Logging.Abstractions;
using Stringification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace InterReact
{
    public partial class Services
    {
        private readonly Dictionary<string, ContractDetails[]> Contracts = new();

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

            var objs = new object?[] {
                    contract.ContractId,
                    contract.Symbol, contract.SecurityType, contract.LastTradeDateOrContractMonth,
                    contract.Strike, contract.Right, contract.Multiplier,
                    contract.Exchange, contract.PrimaryExchange,
                    contract.Currency, contract.LocalSymbol, contract.TradingClass,
                    contract.IncludeExpired, contract.SecurityIdType, contract.SecurityId };


            IEnumerable<string> strings = RequestMessage.GetStrings(objs);
            string key = string.Join(',', strings);

            /*
            Observable.Create<Union<ContractDetails, Alert>>(observer =>
            {
                return Disposable.Empty;
            });
            */

            return Response
                .ToObservableMultipleWithId<ContractDetailsEnd>(
                    Request.GetNextId, id => Request.RequestContractDetails(id, contract))
                .Select(x => new Union<ContractDetails, Alert>(x));
        }

        internal IObservable<IHasRequestId> ToObservableMultipleWithIdX(Contract contract)
        {
            return Observable.Create<IHasRequestId>(observer =>
            {
                int id = Request.GetNextId();

                IDisposable subscription = Response
                    .OfType<IHasRequestId>() // IMPORTANT!
                    .Where(m => m.RequestId == id)
                    .SubscribeSafe(Observer.Create<IHasRequestId>(
                        onNext: m =>
                        {
                            if (m is not ContractDetailsEnd)
                                observer.OnNext(m);
                            if (m is ContractDetailsEnd or Alert)
                                observer.OnCompleted();
                        },
                        onError: observer.OnError,
                        onCompleted: observer.OnCompleted));

                Request.RequestContractDetails(id, contract);

                return subscription;
            });
        }

    }
}

