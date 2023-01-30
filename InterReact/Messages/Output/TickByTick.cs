namespace InterReact;

internal static class TickByTick
{
    internal static ITickByTick Create(ResponseReader r)
    {
        int requestId = r.ReadInt();
        TickByTickType tickType = r.ReadEnum<TickByTickType>();
        return tickType switch
        {
            TickByTickType.None => new TickByTickNone(),
            TickByTickType.Last => new TickByTickAllLast(requestId, tickType, r),
            TickByTickType.AllLast => new TickByTickAllLast(requestId, tickType, r),
            TickByTickType.BidAsk => new TickByTickBidAsk(requestId, tickType, r),
            TickByTickType.MidPoint => new TickByTickMidpoint(requestId, tickType, r),
            _ => throw new ArgumentException("Invalid TickByTick type.")
        };
    }
}

public sealed class TickByTickNone : ITickByTick
{
    public int RequestId { get; }
    public TickByTickType TickByTickType { get; }
    public long Time { get; }
    internal TickByTickNone() { }
}

public sealed class TickByTickAllLast : ITickByTick
{
    public int RequestId { get; }
    public TickByTickType TickByTickType { get; }
    public long Time { get; }
    public double Price { get; }
    public long Size { get; }
    public TickAttribLast TickAttribLast { get; } = new();
    public string Exchange { get; } = "";
    public string SpecialConditions { get; } = "";

    internal TickByTickAllLast(int requestId, TickByTickType type, ResponseReader r)
    {
        RequestId = requestId;
        TickByTickType = type;
        Time = r.ReadLong();
        Price = r.ReadDouble();
        Size = r.ReadLong();
        TickAttribLast.Set(r.ReadInt());
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
    public long BidSize { get; }
    public long AskSize { get; }
    public TickAttribBidAsk TickAttribBidAsk { get; } = new();

    internal TickByTickBidAsk(int requestId, TickByTickType type, ResponseReader r)
    {
        RequestId = requestId;
        TickByTickType = type;
        Time = r.ReadLong(); 
        BidPrice = r.ReadDouble();
        AskPrice = r.ReadDouble();
        BidSize = r.ReadLong();
        AskSize = r.ReadLong();
        TickAttribBidAsk.Set(r.ReadInt());
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
