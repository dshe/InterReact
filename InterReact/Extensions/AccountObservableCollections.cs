using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using InterReact.Enums;
using InterReact.Messages;
using InterReact.Utility;
using System.Threading;
using InterReact.Interfaces;

//https://github.com/dotnet/corefx/issues/970

namespace InterReact.Extensions
{
    public static class ToObservableCollectionEx
    {
        /// <summary>
        /// Create observable collections from AccountValue updates.
        /// var collections = client.Services
        ///    .AccountUpdatesObservable(contract)
        ///    .ObserveOn(SynchronizationContext.Current); // or ObserveOnDispatcher()
        ///    .ToObservableCollections();
        /// Don't forget to Connect() the ConnectableObservable.
        /// </summary>
        public static (ReadOnlyObservableList<AccountValue> accountValue, ReadOnlyObservableList<PortfolioValue> portfolioValue)
            ToObservableCollections(this IObservable<IAccountUpdate> source, CancellationToken ct = default)
        {
            var av = source.OfType<AccountValue>  ().ToObservableCollection(v => $"{v.Account}+{v.Key}+{v.Currency}", ct);
            var pv = source.OfType<PortfolioValue>().ToObservableCollection(v => $"{v.Account}+{v.Contract?.Stringify()}", ct);

            return (av, pv);
        }

        internal static ReadOnlyObservableList<TValue> ToObservableCollection<TKey, TValue>
            (this IObservable<TValue> source, Func<TValue, TKey> getKey, CancellationToken ct = default)
                where TValue: NotifyPropertyChanged
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (getKey == null)
                throw new ArgumentNullException(nameof(getKey));

            // compare keys using the default comparer for the key type
            var comparer = Comparer<TValue>.Create((a, b) => Comparer<TKey>.Default.Compare(getKey(a), getKey(b)));
            var collection = new ObservableCollection<TValue>();

            source.Subscribe(
                onNext: item =>
                {
                    var index = collection.BinarySearch(item, comparer);
                    if (index < 0)
                    {
                        index = ~index;
                        collection.Insert(index, item);
                    }
                    else
                    {
                        collection[index] = item;
                        item.PropertyChange();
                    }
                },
                //onError: e => { },           // ???
                onCompleted: collection.Clear, // Note that the list is cleared when the observable completes.
                ct);                           // It is possible to unsubscribe by cancelling the token source.

            return new ReadOnlyObservableList<TValue>(collection);
        }
    }
}
