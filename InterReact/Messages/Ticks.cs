using System;
using System.Collections.Generic;
using System.Globalization;
using InterReact.Core;
using InterReact.Enums;
using InterReact.Interfaces;
using InterReact.Utility;
using NodaTime;

#nullable enable

namespace InterReact.Messages
{
    public abstract class Tick : IHasRequestId
    {
        public int RequestId { get; protected set; }
        public TickType TickType { get; protected set; }
        internal void Undelay()
        {
            TickType = TickType switch
            {
                TickType.DelayedBidPrice => TickType.BidSize,
                TickType.DelayedAskPrice => TickType.AskPrice,
                TickType.DelayedLastPrice => TickType.LastPrice,
                TickType.DelayedBidSize => TickType.BidSize,
                TickType.DelayedAskSize => TickType.AskSize,
                TickType.DelayedLastSize => TickType.LastSize,
                TickType.DelayedHigh => TickType.HighPrice,
                TickType.DelayedLow => TickType.LowPrice,
                TickType.DelayedVolume => TickType.Volume,
                TickType.DelayedClose => TickType.ClosePrice,
                TickType.DelayedOpen => TickType.OpenPrice,
                TickType.DelayedBidOption => TickType.BidOptionComputation,
                TickType.DelayedAskOption => TickType.AskOptionComputation,
                TickType.DelayedLastOption => TickType.LastOptionComputation,
                TickType.DelayedModelOption => TickType.ModelOptionComputation,
                TickType.DelayedLastTimeStamp => TickType.LastTimeStamp,
                _ => TickType
            };
        }
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
        internal TickSize(ResponseComposer c)
        {
            c.IgnoreVersion();
            RequestId = c.ReadInt();
            TickType = c.ReadEnum<TickType>();
            Size = c.ReadInt();
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
        public TickAttrib TickAttrib { get; }
        private TickPrice(int requestId, TickType tickType, double price, TickAttrib attrib)
        {
            RequestId = requestId;
            TickType = tickType;
            Price = price;
            TickAttrib = attrib;
        }

        // This is the only method that can send more than 1 message. Output(message) used instead of return(message).
        internal static void Send(ResponseComposer p)
        {
            var version = p.GetVersion();
            var requestId = p.ReadInt();
            if (requestId == int.MaxValue)
                p.ReadDouble(); // trigger parse exception for testing
            var priceTickType = p.ReadEnum<TickType>();
            var price = p.ReadDouble();
            var size = version >= 2 ? p.ReadInt() : 0;

            var priceTick = new TickPrice(requestId, priceTickType, price, new TickAttrib(version >= 3? p: null));
            p.Output(priceTick);

            if (version >= 2)
            {
                TickType tickTypeSize = GetTickTypeSize(priceTickType);
                if (tickTypeSize != TickType.Undefined)
                    p.Output(new TickSize(requestId, tickTypeSize, size));
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

        internal static Tick Create(ResponseComposer c)
        {
            c.IgnoreVersion();
            var requestId = c.ReadInt();
            var tickType = c.ReadEnum<TickType>();
            var str = c.ReadString();
            if (tickType == TickType.RealtimeVolume)
                return new TickRealtimeVolume(requestId, str, c.Parser);
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
        internal TickRealtimeVolume(int requestId, string str, ResponseParser c)
        {
            RequestId = requestId;
            TickType = TickType.RealtimeVolume;
            var parts = str.Split(';');
            Price = c.ParseDouble(parts[0]);
            Size = c.ParseInt(parts[1]);
            Instant = Instant.FromUnixTimeMilliseconds(long.Parse(parts[2], NumberFormatInfo.InvariantInfo));
            Volume = c.ParseInt(parts[3]);
            Vwap = c.ParseDouble(parts[4]);
            SingleTrade = c.ParseBool(parts[5]);
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
        internal static Tick Create(ResponseComposer c)
        {
            c.IgnoreVersion();
            var requestId = c.ReadInt();
            var tickType = c.ReadEnum<TickType>();
            var value = c.ReadDouble();
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
        public double DividendsToLastTradeDate { get; }
        internal TickExchangeForPhysical(ResponseComposer c)
        {
            c.IgnoreVersion();
            RequestId = c.ReadInt();
            TickType = c.ReadEnum<TickType>();
            BasisPoints = c.ReadDouble();
            FormattedBasisPoints = c.ReadString();
            ImpliedFuturesPrice = c.ReadDouble();
            HoldDays = c.ReadInt();
            FutureExpiry = c.ReadString();
            DividendImpact = c.ReadDouble();
            DividendsToLastTradeDate = c.ReadDouble();
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
        internal TickOptionComputation(ResponseComposer c)
        {
            var version = c.GetVersion();
            RequestId = c.ReadInt();
            TickType = c.ReadEnum<TickType>();
            ImpliedVolatility = c.ReadDoubleNullable();
            if (ImpliedVolatility == -1)
                ImpliedVolatility = null;

            Delta = c.ReadDoubleNullable();
            if (Delta == -2)
                Delta = null;

            if (version >= 6 || TickType == TickType.ModelOptionComputation || TickType == TickType.DelayedModelOption)
            {
                OptPrice = c.ReadDoubleNullable();
                if (OptPrice == -1)
                    OptPrice = null;
                PvDividend = c.ReadDoubleNullable();
                if (PvDividend == -1)
                    PvDividend = null;
            }
            if (version >= 6)
            {
                Gamma = c.ReadDoubleNullable();
                if (Gamma == -2)
                    Gamma = null;
                Vega = c.ReadDoubleNullable();
                if (Vega == -2)
                    Vega = null;
                Theta = c.ReadDoubleNullable();
                if (Theta == -2)
                    Theta = null;
                UndPrice = c.ReadDoubleNullable();
                if (UndPrice == -1)
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
        internal TickMarketDataType(ResponseComposer c)
        {
            c.IgnoreVersion();
            RequestId = c.ReadInt();
            TickType = TickType.MarketDataType; //
            MarketDataType = c.ReadEnum<MarketDataType>();
        }
    }

    public sealed class TickSnapshotEnd : IHasRequestId
    {
        public int RequestId { get; }
        public TickSnapshotEnd(ResponseComposer c)
        {
            c.IgnoreVersion();
            RequestId = c.ReadInt();
        }
    };

}
