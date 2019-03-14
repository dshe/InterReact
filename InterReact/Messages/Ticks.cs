using System;
using System.Collections.Generic;
using System.Globalization;
using InterReact.Core;
using InterReact.Enums;
using InterReact.Interfaces;
using InterReact.Utility;
using NodaTime;

namespace InterReact.Messages
{
    // This abstract class just avoids repeating the two properties in derived classes.
    public abstract class Tick : ITick
    {
        public int RequestId { get; protected set; }
        public TickType TickType { get; protected set; }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // There are many tick types, as identified by the enum TickType. For example: TickType.BidSize.
    // Each tick type maps to one of the classes below. For example, TickType.BidSize is represented by objects of class type TickSize.
    // https://www.interactivebrokers.com/en/software/api/apiguide/tables/tick_types.htm

    /// <summary>
    /// Size only. For example, a trade/bid/ask at the previous trade/bid/ask price.
    /// </summary>
    public sealed class TickSize : Tick
    {
        public int Size { get; }
        internal TickSize(int requestId, TickType tickType, int size)
        {
            RequestId = requestId;
            TickType = tickType;
            Size = size;
        }
        internal TickSize(ResponseReader c)
        {
            c.IgnoreVersion();
            RequestId = c.Read<int>();
            TickType = c.Read<TickType>();
            Size = c.Read<int>();
        }
    }

    /// <summary>
    /// Combined Price AND Size tick. IB's own API client sends separate messages.
    /// A trade/bid/ask at a price which is different from the previous trade/bid/ask price.
    /// An TickPrice is followed by a corresponding TickSize tick. 
    /// You can either choose to ignore the size property in TickPrice,
    /// or else filter out the redundant TickSize messages using TickRedundentSizeFilter.
    /// </summary>
    public sealed class TickPrice : Tick
    {
        public double Price { get; }
        public bool CanAutoExecute { get; set; }
        public bool PastLimit { get; set; }
        public TickPrice(int requestId, TickType tickType, double price)
        {
            RequestId = requestId;
            TickType = tickType;
            Price = price;
        }

        // This is the only method that can compose more than message. Output(message) used instead of return(message).
        internal static void Send(ResponseReader p)
        {
            var version = p.GetVersion();
            var requestId = p.Read<int>();
            if (requestId == int.MaxValue)
                p.Read<double>(); // trigger parse exception for testing
            var priceTickType = p.Read<TickType>();
            var price = p.Read<double>();
            var size = version >= 2 ? p.Read<int>() : 0;

            var tickPrice = new TickPrice(requestId, priceTickType, price);

            if (version >= 3)
            {
                var i = p.Read<int>();
                tickPrice.CanAutoExecute = i == 1;
                if (p.Config.SupportsServerVersion(ServerVersion.PastLimit))
                {
                    var mask = new BitMask(i);
                    tickPrice.CanAutoExecute = mask[0];
                    tickPrice.PastLimit = mask[1];
                }
            }

            p.Output(tickPrice);

            if (version >= 2)
            {
                TickType tickTypeSize = GetTickTypeSize(priceTickType);
                if (tickTypeSize != TickType.Undefined)
                {
                    p.Output(tickPrice);  // ??
                    p.Output(new TickSize(requestId, tickTypeSize, size));
                }
            }
        }

        private static TickType GetTickTypeSize(TickType tickType) => tickType switch
        {
            TickType.BidPrice => TickType.BidSize,
            TickType.AskPrice => TickType.AskSize,
            TickType.LastPrice => TickType.LastSize,
            TickType.DelayedBidPrice => TickType.DelayedBidSize,
            TickType.DelayedAskPrice => TickType.DelayedAskSize,
            TickType.DelayedLastPrice => TickType.DelayedLastSize,
            _ => TickType.Undefined
        };


    }

    public sealed class TickString : Tick
    {
        public string Value { get; }
        public TickString(int requestId, TickType tickType, string value)
        {
            RequestId = requestId;
            TickType = tickType;
            Value = value;
        }

        internal static Tick Create(ResponseReader c)
        {
            c.IgnoreVersion();
            var requestId = c.Read<int>();
            var tickType = c.Read<TickType>();
            var str = c.ReadString();
            if (tickType == TickType.RealtimeVolume)
                return new TickRealtimeVolume(requestId, str, c);
            if (tickType == TickType.LastTimeStamp)
                return new TickTime(requestId, str);
            return new TickString(requestId, tickType, str);
        }
    }

    public sealed class TickTime : Tick // from TickString
    {
        /// <summary>
        /// Seconds precision.
        /// </summary>
        public Instant Time { get; }
        internal TickTime(int requestId, string str)
        {
            RequestId = requestId;
            TickType = TickType.LastTimeStamp;
            Time = Instant.FromUnixTimeSeconds(long.Parse(str, NumberFormatInfo.InvariantInfo));
        }
    }

    /// <summary>
    /// TickRealtimeVolume data provides a useful alternative to data provided by LastPrice, LastSize, Volume and Time ticks.
    /// TickRealtimeVolume data is obtained by requesting market data with GenericTickType.RealtimeVolume.
    /// TickRealtimeVolume ticks are obtained by parsing TickString.
    /// When TickRealtimeVolume is used, redundant Tick messages (above) can be removed using the TickRedundantRealtimeVolumeFilter.
    /// </summary>
    public sealed class TickRealtimeVolume : Tick // from TickString
    {
        public double Price { get; }
        public int Size { get; }
        public int Volume { get; }
        public double Vwap { get; }
        /// <summary>
        /// Indicates whether the trade was filled by a single market-maker.
        /// </summary>
        public bool SingleTrade { get; }
        /// <summary>
        /// Milliseconds precision.
        /// </summary>
        public Instant Instant { get; }
        internal TickRealtimeVolume(int requestId, string str, ResponseReader c)
        {
            RequestId = requestId;
            TickType = TickType.RealtimeVolume;
            var parts = str.Split(';');
            Price = c.Parse<double>(parts[0]);
            Size = c.Parse<int>(parts[1]);
            Instant = Instant.FromUnixTimeMilliseconds(long.Parse(parts[2], NumberFormatInfo.InvariantInfo));
            Volume = c.Parse<int>(parts[3]);
            Vwap = c.Parse<double>(parts[4]);
            SingleTrade = c.Parse<bool>(parts[5]);
        }
    };

    public sealed class TickGeneric : Tick
    {
        public double Value { get; }
        internal TickGeneric(int requestId, TickType tickType, double value)
        {
            RequestId = requestId;
            TickType = tickType;
            Value = value;
        }
        internal static Tick Create(ResponseReader c)
        {
            c.IgnoreVersion();
            var requestId = c.Read<int>();
            var tickType = c.Read<TickType>();
            var value = c.Read<double>();
            if (tickType == TickType.Halted)
                return new TickHalted(requestId, tickType, value == 0 ? HaltType.NotHalted : HaltType.GeneralHalt);
            return new TickGeneric(requestId, tickType, value);
        }
    };

    public sealed class TickExchangeForPhysical : Tick
    {
        public double BasisPoints { get; }
        public string FormattedBasisPoints { get; }
        public double ImpliedFuturesPrice { get; }
        public int HoldDays { get; }
        public string FutureExpiry { get; }
        public double DividendImpact { get; }
        public double DividendsToExpiry { get; }
        internal TickExchangeForPhysical(ResponseReader c)
        {
            c.IgnoreVersion();
            RequestId = c.Read<int>();
            TickType = c.Read<TickType>();
            BasisPoints = c.Read<double>();
            FormattedBasisPoints = c.ReadString();
            ImpliedFuturesPrice = c.Read<double>();
            HoldDays = c.Read<int>();
            FutureExpiry = c.ReadString();
            DividendImpact = c.Read<double>();
            DividendsToExpiry = c.Read<double>();
        }
    }

    /// <summary>
    /// Missing Values are indicated with null.
    /// IB's Java client indicates missing Values using DOUBLE_MAX.
    /// </summary>
    public sealed class TickOptionComputation : Tick
    {
        public double? ImpliedVolatility { get; }
        public double? Delta { get; }
        public double? OptPrice { get; }
        public double? PvDividend { get;}
        public double? Gamma { get;}
        public double? Vega { get;}
        public double? Theta { get;}
        public double? UndPrice { get;}
        internal TickOptionComputation(ResponseReader c)
        {
            var version = c.GetVersion();
            RequestId = c.Read<int>();
            TickType = c.Read<TickType>();
            ImpliedVolatility = c.Read<double?>();

            if (ImpliedVolatility < 0)
                ImpliedVolatility = null;

            Delta = c.Read<double?>();
            if (Delta != null && Math.Abs(Delta.Value) > 1)
                Delta = null;

            if (version >= 6 || TickType == TickType.ModelOptionComputation)
            {
                OptPrice = c.Read<double?>();
                if (OptPrice < 0)
                    OptPrice = null;
                PvDividend = c.Read<double?>();
                if (PvDividend < 0)
                    PvDividend = null;
            }
            if (version >= 6)
            {
                Gamma = c.Read<double?>();
                if (Gamma != null && Math.Abs(Gamma.Value) > 1)
                    Gamma = null;
                Vega = c.Read<double?>();
                if (Vega != null && Math.Abs(Vega.Value) > 1)
                    Vega = null;
                Theta = c.Read<double?>();
                if (Theta != null && Math.Abs(Theta.Value) > 1)
                    Theta = null;
                UndPrice = c.Read<double?>();
                if (UndPrice < 0)
                    UndPrice = null;
            }
        }
    }

    /// <summary>
    /// TickHalted is obtained from from TickGeneric.
    /// </summary>
    public sealed class TickHalted : Tick
    {
        public HaltType HaltType { get; }
        internal TickHalted(int requestId, TickType tickType, HaltType haltType)
        {
            RequestId = requestId;
            TickType = tickType;
            HaltType = haltType;
        }
    };

    /// <summary>
    /// Tws sends this message to announce that market data has been switched between frozen and real-time.
    /// This message indicates a change in MarketDataType. 
    /// This response could also result from calling: RequestMarketData(returns Tick),
    /// </summary>
    public sealed class TickMarketDataType : Tick
    {
        public MarketDataType MarketDataType { get; }
        internal TickMarketDataType(ResponseReader c)
        {
            c.IgnoreVersion();
            RequestId = c.Read<int>();
            TickType = TickType.MarketDataType;
            MarketDataType = c.Read<MarketDataType>();
        }
    }

    public sealed class TickSnapshotEnd : IHasRequestId
    {
        public int RequestId { get; }
        public TickSnapshotEnd(ResponseReader c)
        {
            c.IgnoreVersion();
            RequestId = c.Read<int>();
        }
    };




}
