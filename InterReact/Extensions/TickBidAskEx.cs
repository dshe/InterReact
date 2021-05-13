using System;
using System.Reactive;
using System.Reactive.Linq;

namespace InterReact.Extensions
{
    /// <summary>
    /// The spread, (ask - bid), is normally positive (bid &lt; ask).
    /// If the market is locked (bid == ask), the spread equals 0.
    /// If the market is crossed (bid &gt; ask), the spread is negative.
    /// If either the bid or the ask is not greater than zero, the value are equal to null.
    /// </summary>
    public sealed class TickBidAskPrice
    {
        public double BidPrice { get; internal set; }
        public double AskPrice { get; internal set; }

        private bool IsValid => BidPrice > 0 && AskPrice > 0;

        public double? Spread()
        {
            if (!IsValid)
                return null;
            return AskPrice - BidPrice;
        }

        public double? SpreadRatio()
        {
            if (!IsValid)
                return null;
            return Math.Log(AskPrice / BidPrice) * 10000;
        }

        public double? Mid()
        {
            if (!IsValid)
                return null;
            return Math.Sqrt(AskPrice * BidPrice);
        }

        public double? Min()
        {
            if (!IsValid)
                return null;
            return Math.Min(AskPrice, BidPrice);
        }

        public double? Max()
        {
            if (!IsValid)
                return null;
            return Math.Max(AskPrice, BidPrice);
        }
    }

    public static class TickBidAskPriceExtensions
    {
        public static IObservable<TickBidAskPrice> ToBidAskTicks(this IObservable<Tick> source)
        {
            var bal = new TickBidAskPrice();

            return Observable.Create<TickBidAskPrice>(observer =>
            {
                return source.OfType<TickPrice>()
                    .Subscribe(Observer.Create<TickPrice>(
                        onNext: tickPrice =>
                        {
                            var price = tickPrice.Price;
                            if (tickPrice.TickType == TickType.BidPrice && bal.BidPrice != price)
                                bal.BidPrice = price;
                            else if (tickPrice.TickType == TickType.AskPrice && bal.AskPrice != price)
                                bal.AskPrice = price;
                            else
                                return;
                            observer.OnNext(bal);
                        },
                        onError: observer.OnError,
                        onCompleted: observer.OnCompleted));

            }).Publish().RefCount();
        }

    }
}
