using System;
using System.Reactive.Linq;
using InterReact.Enums;
using InterReact.Interfaces;
using InterReact.Messages;
using InterReact.StringEnums;
using InterReact.Utility;

namespace InterReact.Extensions
{
    public sealed class TickOhlc
    {
        public double? Open { get; private set; }
        public double? High { get; private set; }
        public double? Low { get; private set; }
        public double? Close { get; private set; }
        internal static TickOhlc Calc(TickOhlc instance, double value)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            if (!instance.Open.HasValue)
                instance.Open = instance.High = instance.Low = value;
            else
            {
                if (!instance.High.HasValue || !instance.Low.HasValue)
                    throw new ArgumentNullException(nameof(instance));
                instance.High = Math.Max(instance.High.Value, value);
                instance.Low = Math.Min(instance.Low.Value, value);
            }
            instance.Close = value;
            return instance;
        }
    }

    public static class TickOpenHighLowCloseEx
    {
        public static IObservable<TickOhlc> TickOpenHighLowClose(this IObservable<Tick> source, TimeSpan barSize)
            => ThrowIf.ThrowIfNull(source)
                .OfType<TickPrice>()
                .Where(x => x.TickType == TickType.LastPrice)
                .Select(x => x.Price)
                .Window(barSize)
                .Select(w => w.Aggregate(new TickOhlc(), TickOhlc.Calc))
                .SelectMany(x => x);
    }
}
