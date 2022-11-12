using System.Collections.Generic;
using System.Linq;

namespace InterReact;

public sealed class HistoricalTicks : IHasRequestId
{
    public int RequestId { get; }
    public IList<HistoricalTick> Ticks { get; } = new List<HistoricalTick>();
    public bool Done { get; }

    internal HistoricalTicks() { }

    internal HistoricalTicks(ResponseReader r)
    {
        RequestId = r.ReadInt();
        int n = r.ReadInt();
        for (int i = 0; i < n; i++)
            Ticks.Add(new HistoricalTick(r));
        Done = r.ReadBool();
    }

}

public sealed class HistoricalTick
{
    public long Time { get; }
    public double Price { get; }
    public long Size { get; }

    internal HistoricalTick() { }

    internal HistoricalTick(ResponseReader r)
    {
        Time = r.ReadLong();
        r.ReadInt(); // ?
        Price = r.ReadDouble();
        Size = r.ReadLong();
    }
}

public sealed class HistoricalLastTicks : IHasRequestId
{
    public int RequestId { get; }
    public IList<HistoricalLastTick> Ticks { get; } = new List<HistoricalLastTick>();
    public bool Done { get; }

    internal HistoricalLastTicks() { }
    internal HistoricalLastTicks(ResponseReader r)
    {
        RequestId = r.ReadInt();
        int n = r.ReadInt();

        Ticks = Enumerable.Repeat(new HistoricalLastTick(r), n).ToList();
        Done = r.ReadBool();
    }

}

public sealed class HistoricalLastTick
{
    public long Time { get; }
    public TickAttribLast TickAttribLast { get; } = new();
    public double Price { get; }
    public long Size { get; }
    public string Exchange { get; } = "";
    public string SpecialConditions { get; } = "";

    internal HistoricalLastTick() { }

    internal HistoricalLastTick(ResponseReader r)
    {
        Time = r.ReadLong();
        TickAttribLast.Set(r.ReadInt());
        Price = r.ReadDouble();
        Size = r.ReadLong();
        Exchange = r.ReadString();
        SpecialConditions = r.ReadString();
    }
}

public sealed class HistoricalBidAskTicks : IHasRequestId
{
    public int RequestId { get; }
    public IList<HistoricalBidAskTick> Ticks { get; } = new List<HistoricalBidAskTick>();
    public bool Done { get; }

    internal HistoricalBidAskTicks() { }

    internal HistoricalBidAskTicks(ResponseReader r)
    {
        RequestId = r.ReadInt();
        int n = r.ReadInt();
        for (int i = 0; i < n; i++)
            Ticks.Add(new HistoricalBidAskTick(r));
        Done = r.ReadBool();
    }

}

public sealed class HistoricalBidAskTick
{
    public long Time { get; }
    public TickAttribBidAsk TickAttribBidAsk { get; } = new TickAttribBidAsk();
    public double PriceBid { get; }
    public double PriceAsk { get; }
    public long SizeBid { get; }
    public long SizeAsk { get; }

    internal HistoricalBidAskTick() { }

    internal HistoricalBidAskTick(ResponseReader r)
    {
        Time = r.ReadLong();
        TickAttribBidAsk.Set(r.ReadInt());
        PriceBid = r.ReadDouble();
        PriceAsk = r.ReadDouble();
        SizeBid = r.ReadLong();
        SizeAsk = r.ReadLong();
    }
}
