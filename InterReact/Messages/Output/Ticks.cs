using System.Globalization;

namespace InterReact;

// There are many tick types, as identified by the enum TickType. For example: TickType.BidSize.
// Each tick type maps to one of the classes below. For example, TickType.BidSize is represented by objects of class type TickSize.
// https://www.interactivebrokers.com/en/software/api/apiguide/tables/tick_types.htm

/// <summary>
/// A trade/bid/ask at a price which is different from the previous trade/bid/ask price.
/// </summary>
public sealed class PriceTick : ITick
{
    public int RequestId { get; }
    public TickType TickType { get; }
    public double Price { get; }
    public long Size { get; }
    public TickAttrib TickAttrib { get; }
    internal PriceTick() 
    {
        TickType = TickType.Undefined;
        TickAttrib = new TickAttrib();
    }
    internal PriceTick(ResponseReader r)
    {
        r.RequireMessageVersion(3);
        RequestId = r.ReadInt();
        TickType = r.ReadTickTypeEnum();
        Price = r.ReadDouble();
        Size = r.ReadLong();
        TickAttrib = new TickAttrib(r);
    }
}

/// <summary>
/// Size only. For example, a trade/bid/ask at the previous trade/bid/ask price.
/// </summary>
public sealed class SizeTick : ITick
{
    public int RequestId { get; init; }
    public TickType TickType { get; init; }
    public long Size { get; init; }
    internal SizeTick() 
    { 
        TickType = TickType.Undefined;
    }
    internal SizeTick(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        RequestId = r.ReadInt();
        TickType = r.ReadTickTypeEnum();
        Size = r.ReadLong();
    }
}

public sealed class StringTick : ITick
{
    public int RequestId { get; init; }
    public TickType TickType { get; init; }
    public string Value { get; init; } = "";
    internal StringTick() 
    {
        TickType = TickType.Undefined;
    }
    internal static ITick Create(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        int requestId = r.ReadInt();
        TickType tickType = r.ReadTickTypeEnum();
        string str = r.ReadString();
        if (tickType == TickType.LastTimeStamp || tickType == TickType.DelayedLastTimeStamp)
        {
            return new TimeTick()
            {
                RequestId = requestId,
                TickType = tickType,
                Time = Instant.FromUnixTimeSeconds(long.Parse(str, NumberFormatInfo.InvariantInfo))
            };
        }
        if (tickType == TickType.RealtimeVolume) // there is no delayed RealTimeVolue
        {
            ResponseParser parser = r.Parser;
            string[] parts = str.Split(';');
            return new RealtimeVolumeTick
            {
                RequestId = requestId,
                TickType = tickType,
                Price = parser.ParseDouble(parts[0]),
                Size = parser.ParseLong(parts[1]),
                Instant = Instant.FromUnixTimeMilliseconds(long.Parse(parts[2], NumberFormatInfo.InvariantInfo)),
                Volume = parser.ParseLong(parts[3]),
                Vwap = parser.ParseDouble(parts[4]),
                SingleTrade = parser.ParseBool(parts[5])
            };
        }
        return new StringTick()
        {
            RequestId = requestId,
            TickType = tickType,
            Value = str
        };
    }
}

public sealed class TimeTick : ITick // from TickString
{
    public int RequestId { get; init; }
    public TickType TickType { get; init; } = TickType.Undefined;
    /// <summary>
    /// Seconds precision.
    /// </summary>
    public Instant Time { get; init; }
    internal TimeTick() { }
}

/// <summary>
/// TickRealtimeVolume data provides a useful alternative to data provided by LastPrice, LastSize, Volume and Time ticks.
/// TickRealtimeVolume data is obtained by requesting market data with GenericTickType.RealtimeVolume.
/// TickRealtimeVolume ticks are obtained by parsing TickString.
/// When TickRealtimeVolume is used, redundant Tick messages (above) can be removed using the TickRedundantRealtimeVolumeFilter.
/// </summary>
public sealed class RealtimeVolumeTick : ITick // from TickString
{
    public int RequestId { get; init; }
    public TickType TickType { get; init; } = TickType.Undefined;
    public double Price { get; init; }
    public long Size { get; init; }
    public long Volume { get; init; }
    public double Vwap { get; init; }
    /// <summary>
    /// Indicates whether the trade was filled by a single market-maker.
    /// </summary>
    public bool SingleTrade { get; init; }
    /// <summary>
    /// Milliseconds precision.
    /// </summary>
    public Instant Instant { get; init; }
    internal RealtimeVolumeTick() { }
 };

public sealed class GenericTick : ITick
{
    public int RequestId { get; init; }
    public TickType TickType { get; init; }
    public double Value { get; init; }
    internal GenericTick() 
    {
        TickType = TickType.Undefined;
    }
    internal static ITick Create(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        int requestId = r.ReadInt();
        TickType tickType = r.ReadTickTypeEnum();
        double value = r.ReadDouble();
        if (tickType == TickType.Halted || tickType == TickType.DelayedHalted)
        {
            return new HaltedTick()
            {
                RequestId = requestId,
                TickType = tickType,
                HaltType = value == 0 ? HaltType.NotHalted : HaltType.GeneralHalt
            };
        }
        return new GenericTick()
        {
            RequestId = requestId,
            TickType = tickType,
            Value = value
        };
    }
};

/// <summary>
/// TickHalted is obtained from from TickGeneric.
/// </summary>
public sealed class HaltedTick : ITick
{
    public int RequestId { get; init; }
    public TickType TickType { get; init; } = TickType.Undefined;
    public HaltType HaltType { get; init; } = HaltType.Undefined;
    internal HaltedTick() { }
};

public sealed class ExchangeForPhysicalTick : ITick
{
    public int RequestId { get; }
    public TickType TickType { get; }
    public double BasisPoints { get; }
    public string FormattedBasisPoints { get; } = "";
    public double ImpliedFuturesPrice { get; }
    public int HoldDays { get; }
    public string FutureLastTradeDate { get; } = "";
    public double DividendImpact { get; }
    public double DividendsToLastTradeDate { get; }
    internal ExchangeForPhysicalTick() { TickType = TickType.Undefined; }
    internal ExchangeForPhysicalTick(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        RequestId = r.ReadInt();
        TickType = r.ReadTickTypeEnum();
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
public sealed class OptionComputationTick : ITick
{
    public int RequestId { get; }
    public TickType TickType { get; }
    public int? TickAttrib { get; }
    public double? ImpliedVolatility { get; }
    public double? Delta { get; }
    public double? OptPrice { get; }
    public double? PvDividend { get; }
    public double? Gamma { get; }
    public double? Vega { get; }
    public double? Theta { get; }
    public double? UndPrice { get; }
    internal OptionComputationTick() { TickType = TickType.Undefined; }
    internal OptionComputationTick(ResponseReader r)
    {
        if (!r.Connector.SupportsServerVersion(ServerVersion.PRICE_BASED_VOLATILITY))
            r.RequireMessageVersion(6);

        RequestId = r.ReadInt();
        TickType = r.ReadTickTypeEnum();
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
