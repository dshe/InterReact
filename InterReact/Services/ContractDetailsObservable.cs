﻿using Stringification;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace InterReact;

public partial class Service
{
    private readonly Dictionary<string, ContractDetails[]> ContractsCache = new();

    /// <summary>
    /// Returns an observable which emits a list of objects using the supplied 
    /// contract as selector, then completes. Results are cached. 
    /// The list contains one or more ContractDetail objects or an Alert message.
    /// If expiry is not specified, ContractDetails objects are retrieved for all expiries.
    /// If strike is not specified, ContractDetails objects are retrieved for all strikes.
    /// And so on. So beware that calling this method may result in attempting to retrieve a large number of contracts.
    /// This operation may take a long time and is subject to usage limiting, 
    /// so the Timeout() operator may be useful.
    /// </summary>
    public IObservable<IHasRequestId> CreateContractDetailsObservable(Contract contract)
    {
        ArgumentNullException.ThrowIfNull(contract);

        if (contract.ComboLegs.Any())
            throw new ArgumentException("Contract must not include ComboLegs.");
        if (!string.IsNullOrEmpty(contract.ComboLegsDescription))
            throw new ArgumentException("Contract must not include ComboLegsDescription.");
        if (contract.DeltaNeutralContract is not null)
            throw new ArgumentException("Contract must not include DeltaNeutralValidation.");
        if ((contract.SecurityIdType is null || contract.SecurityIdType == SecurityIdType.Undefined) ^ string.IsNullOrEmpty(contract.SecurityId))
            throw new ArgumentException("Invalid SecurityId/SecurityIdType combination.");

        string key = contract.Stringify();

        return Observable.Create<IHasRequestId>(observer =>
        {
            if (ContractsCache.TryGetValue(key, out ContractDetails[]? cds))
            {
                foreach (ContractDetails cd in cds)
                    observer.OnNext(cd);
                observer.OnCompleted();
                return Disposable.Empty;
            }

            int requestId = Request.GetNextId();
            List<ContractDetails> cdList = new();

            IDisposable subscription = Response
                .OfType<IHasRequestId>() // IMPORTANT!
                .Where(m => m.RequestId == requestId)
                .SubscribeSafe(Observer.Create<IHasRequestId>(
                    onNext: m =>
                    {
                        if (m is Alert alert)
                        {
                            observer.OnNext(alert);
                            observer.OnCompleted(); // take first alert and abandon request 
                        }
                        else if (m is ContractDetails cd)
                        {
                            cdList.Add(cd);
                            observer.OnNext(cd);
                        }
                        else if (m is ContractDetailsEnd)
                        {
                            ContractsCache.Add(key, cdList.ToArray());
                            observer.OnCompleted();
                        }
                        else
                            observer.OnError(new InvalidCastException($"ContractDetailsObservable: Invalid Type = {m.GetType().Name}."));
                    },
                    onError: observer.OnError,
                    onCompleted: observer.OnCompleted));

            Request.RequestContractDetails(requestId, contract);

            return subscription;

        }).ShareSource();
    }


    /// <summary>
    /// Creates an observable which emits matching symbols for the pattern, then completes.
    /// Each subscription makes a separate request.
    /// </summary>
    public IObservable<SymbolSamples> CreateMatchingSymbolsObservable(string pattern) =>
        Response
            .ToObservableSingleWithRequestId(
                Request.GetNextId, requestId => Request.RequestMatchingSymbols(requestId, pattern))
        .Cast<SymbolSamples>();
}
