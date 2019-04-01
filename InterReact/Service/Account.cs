using System;
using InterReact.Core;
using InterReact.Interfaces;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using InterReact.Enums;
using InterReact.Messages;
using InterReact.Utility;
using InterReact.Utility.Rx;
using Stringification;

namespace InterReact.Service
{
    public sealed partial class Services
    {
        public IObservable<IList<string>> ManagedAccountsObservable =>
            Response
                .ToObservable<ManagedAccounts>(Request.RequestManagedAccounts)
                .Select(x => x.Accounts)
            .ToShareSource();

        /// <summary>
        /// An observable which emits account positions, then completes.
        /// </summary>
        public IObservable<AccountPosition> AccountPositionsObservable =>
            Response.ToObservable<AccountPosition>(
                Request.RequestAccountPositions,
                Request.CancelAccountPositions,
                obj => obj is AccountPositionEnd)
            .ToShareSource();

        /// <summary>
        /// An observable which emits account summary items, then completes.
        /// </summary>
        public IObservable<AccountSummary> AccountSummaryObservable =>
            Response.ToObservable<AccountSummary>(
                Request.NextId,
                requestId => Request.RequestAccountSummary(requestId),
                Request.CancelAccountSummary,
                m => m is AccountSummaryEnd)
            .ToShareSource();

        /// <summary>
        /// A connectable observable which continually emits IAccountUpdate objects for all accounts.
        /// Account update objects include AccountValue, AccountPortfolio, AccountTime and AccountUpdateEnd.
        /// AccountUpdateEnd is emitted after the values for each account have been emitted.
        /// The latest values are cached for replay to new subscribers.
        /// Call Connect() to start receiving updates.
        /// Call Dispose() the value returned from Connect() to disconnect from the source, release all subscriptions and clear the cache.
        /// </summary>
        public IConnectableObservable<IAccountUpdate> AccountUpdatesConnectableObservable =>
            Response.ToObservable<IAccountUpdate>(
                () => Request.RequestAccountData(start: true),
                () => Request.RequestAccountData(start: false),
                m => false)
            //.ToCacheSource(CacheKeySelector, sort: true);
            .Publish();

        // The key identifies unique items to be cached and specifies the order.
        private static string CacheKeySelector(IAccountUpdate v)
        {
            if (v == null)
                throw new ArgumentNullException(nameof(v));

            if (v is AccountUpdateTime)
                return "!"; // top

            if (v is PortfolioValue ap)
                return $"{ap.Account} (1) {(ap.Contract == null ? "" : ap.Contract.Stringify(includeTypeName: false))}";

            if (v is AccountValue av)
                return $"{av.Account} (2) {av.Key}:{av.Currency}";

            // AccountUpdateEnd is last and indicates that the initial values for the partcular account have been emitted.
            return $"{((AccountUpdateEnd)v).Account} (3)";
        }
    }
}
