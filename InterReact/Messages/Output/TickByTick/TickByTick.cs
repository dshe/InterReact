namespace InterReact;

internal static class TickByTick
{
    internal static ITickByTick Create(ResponseReader r)
    {
        int requestId = r.ReadInt();
        TickByTickType tickType = r.ReadEnum<TickByTickType>();
        return tickType switch
        {
            TickByTickType.Last     => new TickByTickAllLast (requestId, tickType, r),
            TickByTickType.AllLast  => new TickByTickAllLast (requestId, tickType, r),
            TickByTickType.BidAsk   => new TickByTickBidAsk  (requestId, tickType, r),
            TickByTickType.MidPoint => new TickByTickMidpoint(requestId, tickType, r),
            _ => throw new ArgumentException("Invalid TickByTick type.")
        };
    }
}

public sealed class TickByTickAllLast : ITickByTick
{
    public int RequestId { get; }
    public TickByTickType TickByTickType { get; }
    public long Time { get; }
    public double Price { get; }
    public decimal Size { get; }
    public TickAttribLast TickAttribLast { get; }
    public string Exchange { get; }
    public string SpecialConditions { get; }

    internal TickByTickAllLast(int requestId, TickByTickType type, ResponseReader r)
    {
        RequestId = requestId;
        TickByTickType = type;
        Time = r.ReadLong();
        Price = r.ReadDouble();
        Size = r.ReadDecimal();
        TickAttribLast = new(r.ReadInt());
        Exchange = r.ReadString();
        SpecialConditions = r.ReadString();
    }
}

public sealed class TickByTickBidAsk : ITickByTick
{
    public int RequestId { get; }
    public TickByTickType TickByTickType { get; }
    public long Time { get;}
    public double BidPrice { get; }
    public double AskPrice { get; }
    public decimal BidSize { get; }
    public decimal AskSize { get; }
    public TickAttribBidAsk TickAttribBidAsk { get; }

    internal TickByTickBidAsk(int requestId, TickByTickType type, ResponseReader r)
    {
        RequestId = requestId;
        TickByTickType = type;
        Time = r.ReadLong();
        BidPrice = r.ReadDouble();
        AskPrice = r.ReadDouble();
        BidSize = r.ReadDecimal();
        AskSize = r.ReadDecimal();
        TickAttribBidAsk = new(r.ReadInt());
    }
}

public sealed class TickByTickMidpoint : ITickByTick
{
    public int RequestId { get; }
    public TickByTickType TickByTickType { get; }
    public long Time { get; }
    public double Midpoint { get; }

    internal TickByTickMidpoint(int requestId, TickByTickType type, ResponseReader r)
    {
        RequestId = requestId;
        TickByTickType = type;
        Time = r.ReadLong();
        Midpoint = r.ReadDouble();
    }
}
