using System.Globalization;
using NodaTime;
namespace InterReact;

public abstract class Tick : ITick
{
    public int RequestId { get; protected set; } = -1;
    public TickType TickType { get; protected set; } = TickType.Undefined;
    internal void Undelay()
    {
        TickType = TickType switch
        {
            TickType.DelayedBidPrice => TickType.BidPrice,
            TickType.DelayedAskPrice => TickType.AskPrice,
            TickType.DelayedLastPrice => TickType.LastPrice,
            TickType.DelayedBidSize => TickType.BidSize,
            TickType.DelayedAskSize => TickType.AskSize,
            TickType.DelayedLastSize => TickType.LastSize,
            TickType.DelayedHighPrice => TickType.HighPrice,
            TickType.DelayedLowPrice => TickType.LowPrice,
            TickType.DelayedVolume => TickType.Volume,
            TickType.DelayedClosePrice => TickType.ClosePrice,
            TickType.DelayedOpenPrice => TickType.OpenPrice,
            TickType.DelayedBidOptionComputation => TickType.BidOptionComputation,
            TickType.DelayedAskOptionComputation => TickType.AskOptionComputation,
            TickType.DelayedLastOptionComputation => TickType.LastOptionComputation,
            TickType.DelayedModelOptionComputation => TickType.ModelOptionComputation,
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
/// A trade/bid/ask at a price which is different from the previous trade/bid/ask price.
/// </summary>
public sealed class PriceTick : Tick
{
    public double Price { get; }
    public long Size { get; }
    public TickAttrib TickAttrib { get; }
    internal PriceTick() { TickAttrib = new TickAttrib(); }
    internal PriceTick(ResponseReader r)
    {
        r.RequireVersion(3);
        RequestId = r.ReadInt();
        TickType = r.ReadEnum<TickType>();
        Price = r.ReadDouble();
        Size = r.ReadLong();
        TickAttrib = new TickAttrib(r);
    }
}

/// <summary>
/// Size only. For example, a trade/bid/ask at the previous trade/bid/ask price.
/// </summary>
public sealed class SizeTick : Tick
{
    public long Size { get; }
    internal SizeTick() { }
    internal SizeTick(int requestId, TickType tickType, long size)
    {
        RequestId = requestId;
        TickType = tickType;
        Size = size;
    }
    internal SizeTick(ResponseReader r)
    {
        r.IgnoreVersion();
        RequestId = r.ReadInt();
        TickType = r.ReadEnum<TickType>();
        Size = r.ReadLong();
    }
}

public sealed class StringTick : Tick
{
    public string Value { get; } = "";
    internal StringTick() { }
    internal StringTick(int requestId, TickType tickType, string value)
    {
        RequestId = requestId;
        TickType = tickType;
        Value = value;
    }
    internal static Tick Create(ResponseReader r)
    {
        r.IgnoreVersion();
        int requestId = r.ReadInt();
        TickType tickType = r.ReadEnum<TickType>();
        string str = r.ReadString();
        if (tickType == TickType.RealtimeVolume)
            return new RealtimeVolumeTick(requestId, str);
        if (tickType == TickType.LastTimeStamp)
            return new TimeTick(requestId, str);
        return new StringTick(requestId, tickType, str);
    }
}

public sealed class TimeTick : Tick // from TickString
{
    /// <summary>
    /// Seconds precision.
    /// </summary>
    public Instant Time { get; }
    internal TimeTick() { }
    internal TimeTick(int requestId, string str)
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
public sealed class RealtimeVolumeTick : Tick // from TickString
{
    public double Price { get; }
    public long Size { get; }
    public long Volume { get; }
    public double Vwap { get; }
    /// <summary>
    /// Indicates whether the trade was filled by a single market-maker.
    /// </summary>
    public bool SingleTrade { get; }
    /// <summary>
    /// Milliseconds precision.
    /// </summary>
    public Instant Instant { get; }

    internal RealtimeVolumeTick() { }

    internal RealtimeVolumeTick(int requestId, string str)
    {
        RequestId = requestId;
        TickType = TickType.RealtimeVolume;
        string[] parts = str.Split(';');
        Price = ResponseParser.ParseDouble(parts[0]);
        Size = ResponseParser.ParseLong(parts[1]);
        Instant = Instant.FromUnixTimeMilliseconds(long.Parse(parts[2], NumberFormatInfo.InvariantInfo));
        Volume = ResponseParser.ParseLong(parts[3]);
        Vwap = ResponseParser.ParseDouble(parts[4]);
        SingleTrade = ResponseParser.ParseBool(parts[5]);
    }
};

public sealed class GenericTick : Tick
{
    public double Value { get; }
    internal GenericTick() { }
    internal GenericTick(int requestId, TickType tickType, double value)
    {
        RequestId = requestId;
        TickType = tickType;
        Value = value;
    }
    internal static Tick Create(ResponseReader r)
    {
        r.IgnoreVersion();
        int requestId = r.ReadInt();
        TickType tickType = r.ReadEnum<TickType>();
        double value = r.ReadDouble();
        if (tickType == TickType.Halted)
            return new HaltedTick(requestId, tickType, value == 0 ? HaltType.NotHalted : HaltType.GeneralHalt);
        return new GenericTick(requestId, tickType, value);
    }
};

public sealed class ExchangeForPhysicalTick : Tick
{
    public double BasisPoints { get; }
    public string FormattedBasisPoints { get; } = "";
    public double ImpliedFuturesPrice { get; }
    public int HoldDays { get; }
    public string FutureLastTradeDate { get; } = "";
    public double DividendImpact { get; }
    public double DividendsToLastTradeDate { get; }
    internal ExchangeForPhysicalTick() { }
    internal ExchangeForPhysicalTick(ResponseReader r)
    {
        r.IgnoreVersion();
        RequestId = r.ReadInt();
        TickType = r.ReadEnum<TickType>();
        BasisPoints = r.ReadDouble();
        FormattedBasisPoints = r.ReadString();
        ImpliedFuturesPrice = r.ReadDouble();
        HoldDays = r.ReadInt();
        FutureLastTradeDate = r.ReadString();
        DividendImpact = r.ReadDouble();
        DividendsToLastTradeDate = r.ReadDouble();
    }
}

/// <summary>
/// Missing Values are indicated with null.
/// IB's Java client indicates missing Values using DOUBLE_MAX.
/// </summary>
public sealed class OptionComputationTick : Tick
{
    public int? TickAttrib { get; }
    public double? ImpliedVolatility { get; }
    public double? Delta { get; }
    public double? OptPrice { get; }
    public double? PvDividend { get; }
    public double? Gamma { get; }
    public double? Vega { get; }
    public double? Theta { get; }
    public double? UndPrice { get; }
    internal OptionComputationTick() { }
    internal OptionComputationTick(ResponseReader r)
    {
        if (!r.Connector.SupportsServerVersion(ServerVersion.PRICE_BASED_VOLATILITY))
            r.RequireVersion(6);

        RequestId = r.ReadInt();
        TickType = r.ReadEnum<TickType>();
        if (r.Connector.SupportsServerVersion(ServerVersion.PRICE_BASED_VOLATILITY))
            TickAttrib = r.ReadInt();

        ImpliedVolatility = r.ReadDoubleNullable();
        if (ImpliedVolatility == -1)
            ImpliedVolatility = null;

        Delta = r.ReadDoubleNullable();
        if (Delta == -2)
            Delta = null;

        OptPrice = r.ReadDoubleNullable();
        if (OptPrice == -1)
            OptPrice = null;

        PvDividend = r.ReadDoubleNullable();
        if (PvDividend == -1)
            PvDividend = null;

        Gamma = r.ReadDoubleNullable();
        if (Gamma == -2)
            Gamma = null;

        Vega = r.ReadDoubleNullable();
        if (Vega == -2)
            Vega = null;

        Theta = r.ReadDoubleNullable();
        if (Theta == -2)
            Theta = null;

        UndPrice = r.ReadDoubleNullable();
        if (UndPrice == -1)
            UndPrice = null;
    }
}

/// <summary>
/// TickHalted is obtained from from TickGeneric.
/// </summary>
public sealed class HaltedTick : Tick
{
    public HaltType HaltType { get; } = HaltType.Undefined;
    internal HaltedTick() { }
    internal HaltedTick(int requestId, TickType tickType, HaltType haltType)
    {
        RequestId = requestId;
        TickType = tickType;
        HaltType = haltType;
    }
};

/// <summary>
/// This message indicates a change in MarketDataType. 
/// Tws sends this message to announce that market data has been switched between frozen and real-time.
/// This response could also result from calling: RequestMarketData(returns Tick),
/// </summary>
public sealed class MarketDataTypeTick : Tick
{
    public MarketDataType MarketDataType { get; } = MarketDataType.Undefined;
    internal MarketDataTypeTick() { }
    internal MarketDataTypeTick(ResponseReader r)
    {
        r.IgnoreVersion();
        RequestId = r.ReadInt();
        TickType = TickType.MarketDataType;
        MarketDataType = r.ReadEnum<MarketDataType>();
    }
}

public sealed class SnapshotEndTick : IHasRequestId
{
    public int RequestId { get; }
    internal SnapshotEndTick(ResponseReader r)
    {
        r.IgnoreVersion();
        RequestId = r.ReadInt();
    }
};
