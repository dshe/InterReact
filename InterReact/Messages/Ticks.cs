using System;
using InterReact.Enums;
using InterReact.Interfaces;
using InterReact.StringEnums;
using NodaTime;

namespace InterReact.Messages
{
    // This abstract class just avoids repeating the two properties in derived classes.
    public abstract class Tick : ITick
    {
        public int RequestId { get; internal set; }
        public TickType TickType { get; internal set; } = TickType.Undefined;
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
        public int Size { get; internal set; }
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
        public double Price { get; internal set; }
        public bool CanAutoExecute { get; internal set; }
        public bool PastLimit { get; internal set; }
    }

    public sealed class TickString : Tick
    {
        public string Value { get; internal set; }
    }

    public sealed class TickTime : Tick // from TickString
    {
        /// <summary>
        /// Seconds precision.
        /// </summary>
        public Instant Time { get; internal set; }
    }

    /// <summary>
    /// TickRealtimeVolume data provides a useful alternative to data provided by LastPrice, LastSize, Volume and Time ticks.
    /// TickRealtimeVolume data is obtained by requesting market data with GenericTickType.RealtimeVolume.
    /// TickRealtimeVolume ticks are obtained by parsing TickString.
    /// When TickRealtimeVolume is used, redundant Tick messages (above) can be removed using the TickRedundantRealtimeVolumeFilter.
    /// </summary>
    public sealed class TickRealtimeVolume : Tick // from TickString
    {
        public double Price { get; internal set; }
        public int Size { get; internal set; }
        public int Volume { get; internal set; }
        public double Vwap { get; internal set; }
        /// <summary>
        /// Indicates whether the trade was filled by a single market-maker.
        /// </summary>
        public bool SingleTrade { get; internal set; }
        /// <summary>
        /// Milliseconds precision.
        /// </summary>
        public Instant Instant { get; internal set; }
    }

    public sealed class TickGeneric : Tick
    {
        public double Value { get; internal set; }
    }

    public sealed class TickExchangeForPhysical : Tick
    {
        public double BasisPoints { get; internal set; }
        public string FormattedBasisPoints { get; internal set; }
        public double ImpliedFuturesPrice { get; internal set; }
        public int HoldDays { get; internal set; }
        public string FutureExpiry { get; internal set; }
        public double DividendImpact { get; internal set; }
        public double DividendsToExpiry { get; internal set; }
    }

    /// <summary>
    /// Missing Values are indicated with null.
    /// IB's Java client indicates missing Values using DOUBLE_MAX.
    /// </summary>
    public sealed class TickOptionComputation : Tick
    {
        public double? ImpliedVolatility { get; internal set; }
        public double? Delta { get; internal set; }
        public double? OptPrice { get; internal set; }
        public double? PvDividend { get; internal set; }
        public double? Gamma { get; internal set; }
        public double? Vega { get; internal set; }
        public double? Theta { get; internal set; }
        public double? UndPrice { get; internal set; }
    }

    /// <summary>
    /// TickHalted is obtained from from TickGeneric.
    /// </summary>
    public sealed class TickHalted : Tick
    {
        public HaltType HaltType { get; internal set; } = HaltType.Undefined;
    }

    /// <summary>
    /// Tws sends this message to announce that market data has been switched between frozen and real-time.
    /// This message indicates a change in MarketDataType. 
    /// This response could also result from calling: RequestMarketData(returns Tick),
    /// </summary>
    public sealed class TickMarketDataType : Tick
    {
        public MarketDataType MarketDataType { get; internal set; }
    }

    public sealed class TickSnapshotEnd : IHasRequestId
    {
        public int RequestId { get; internal set; }
    }

}
