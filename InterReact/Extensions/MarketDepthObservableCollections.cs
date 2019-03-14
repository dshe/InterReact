using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using InterReact.Enums;
using InterReact.Messages;
using InterReact.Utility;

namespace InterReact.Extensions
{
    /// <summary>
    /// Create observable collections from bid or ask updates.
    /// var collections = client.Services
    ///    .MarketDepthObservable(contract)
    ///    .ObserveOn(synchronizationContext); // SynchronizationContext.Current or ObserveOnDispatcher()
    ///    .ToMarketDepthObservableCollections();
    /// </summary>
    public static class MarketDepthObservableCollectionsEx
    {
        public static (ReadOnlyObservableList<MarketDepthItem> bids, ReadOnlyObservableList<MarketDepthItem> asks)
            ToMarketDepthObservableCollections(this IObservable<MarketDepth> source, CancellationToken ct = default)
        {
            var bids = source.Where(d => d.Side == MarketDepthSide.Bid).ToMarketDepthObservableCollection(ct);
            var asks = source.Where(d => d.Side == MarketDepthSide.Ask).ToMarketDepthObservableCollection(ct);

            return (bids, asks);
        }

        private static ReadOnlyObservableList<MarketDepthItem>
            ToMarketDepthObservableCollection(this IObservable<MarketDepth> source, CancellationToken ct)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var list = new ObservableCollection<MarketDepthItem>();

            source.Subscribe(
                onNext: depth =>
                {
                    switch (depth.Operation)
                    {
                        case MarketDepthOperation.Insert:
                            list.Insert(depth.Position, new MarketDepthItem(depth.Price, depth.Size, depth.MarketMaker));
                            break;
                        case MarketDepthOperation.Update:
                            var item = list[depth.Position];
                            if (item.Price != depth.Price)
                            {
                                item.Price = depth.Price;
                                item.PropertyChange(nameof(item.Price));
                            }
                            if (item.Size != depth.Size)
                            {
                                item.Size = depth.Size;
                                item.PropertyChange(nameof(item.Size));
                            }
                            if (item.MarketMaker != depth.MarketMaker)
                            {
                                item.MarketMaker = depth.MarketMaker;
                                item.PropertyChange(nameof(item.MarketMaker));
                            }
                            break;
                        case MarketDepthOperation.Delete:
                            if (depth.Position < list.Count)
                                list.RemoveAt(depth.Position);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(depth.Operation), depth.Operation, null);
                    }
                },
                //onError: e => { },     // ???
                onCompleted: list.Clear, // Note that the list is cleared when the observable completes.
                ct);                     // It is possible to unsubscribe by cancelling the token source.
            return new ReadOnlyObservableList<MarketDepthItem>(list);
        }

        // this needs to be tested
        public static IObservable<NotifyCollectionChangedEventArgs> ToNotifyCollectionChanged(this IObservable<MarketDepth> source)
        {
            return Observable.Create<NotifyCollectionChangedEventArgs>(observer =>
            {
                return source.Subscribe(
                    onNext: depth =>
                    {

                        NotifyCollectionChangedEventArgs? args = depth.Operation switch
                        {
                            MarketDepthOperation.Insert => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, depth, depth.Position),
                            MarketDepthOperation.Update => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, depth, depth.Position),
                            MarketDepthOperation.Delete => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, depth.Position),
                            _ => throw new ArgumentOutOfRangeException(nameof(depth.Operation), depth.Operation, null)
                        };
                        observer.OnNext(args);
                    },
                    //onError: e => { },   // ???
                    onCompleted: () => { } // Note that the list is cleared when the observable completes.
                );
            });
        }

        private static ReadOnlyObservableList<MarketDepthItem>
            ToMarketDepthObservableCollectionx(this IObservable<NotifyCollectionChangedEventArgs> source, CancellationToken ct = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var list = new ObservableCollection<MarketDepthItem>();

            source.Subscribe(
                onNext: depth =>
                {
                    /*
                    switch (depth.Operation)
                    {
                        case MarketDepthOperation.Insert:
                            list.Insert(depth.Position, new MarketDepthItem
                            {
                                Price = depth.Price,
                                Size = depth.Size,
                                MarketMaker = depth.MarketMaker
                            });
                            break;
                        case MarketDepthOperation.Update:
                            var item = list[depth.Position];
                            if (item.Price != depth.Price)
                            {
                                item.Price = depth.Price;
                                item.PropertyChange(nameof(item.Price));
                            }
                            if (item.Size != depth.Size)
                            {
                                item.Size = depth.Size;
                                item.PropertyChange(nameof(item.Size));
                            }
                            if (item.MarketMaker != depth.MarketMaker)
                            {
                                item.MarketMaker = depth.MarketMaker;
                                item.PropertyChange(nameof(item.MarketMaker));
                            }
                            break;
                        case MarketDepthOperation.Delete:
                            if (depth.Position < list.Count)
                                list.RemoveAt(depth.Position);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(depth.Operation), depth.Operation, null);
                    }
                    */
                },
                //onError: e => { },     // ???
                onCompleted: list.Clear, // Note that the list is cleared when the observable completes.
                ct);                     // It is possible to unsubscribe by cancelling the token source.
            return new ReadOnlyObservableList<MarketDepthItem>(list);
        }


        public static void Xx()
        {
            var o = Observable.Never<MarketDepth>();

            //var oo = o.ToNotifyCollectionChanged();

            //var xxx = Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(o);
        }

    }

    public sealed class MarketDepthItem : NotifyPropertyChanged
    {
        public double Price       { get; internal set; }
        public int    Size        { get; internal set; }
        public string MarketMaker { get; internal set; }
        public MarketDepthItem(double price, int size, string marketMaker) =>
            (Price, Size, MarketMaker) = (price, size, marketMaker);
    }
}
