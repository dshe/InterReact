using System.Collections.Generic;
using System.Linq;

namespace InterReact
{
    public sealed class HistoricalTicks : IHasRequestId
    {
        public int RequestId { get; }
        public List<HistoricalTick> Ticks { get; } = new List<HistoricalTick>();
        public bool Done { get; }
        public HistoricalTicks(ResponseReader c)
        {
            RequestId = c.ReadInt();
            int n = c.ReadInt();
            for (int i = 0; i < n; i++)
                Ticks.Add(new HistoricalTick(c));
            Done = c.ReadBool();
        }

    }

    public sealed class HistoricalTick
    {
        public long Time { get; }
        public double Price { get; }
        public long Size { get; }
        internal HistoricalTick(ResponseReader c)
        {
            Time = c.ReadLong();
            c.ReadInt(); // ?
            Price = c.ReadDouble();
            Size = c.ReadLong();
        }
    }

    public sealed class HistoricalLastTicks : IHasRequestId
    {
        public int RequestId { get; }
        public List<HistoricalLastTick> Ticks { get; } = new List<HistoricalLastTick>();
        public bool Done { get; }
        public HistoricalLastTicks(ResponseReader c)
        {
            RequestId = c.ReadInt();
            int n = c.ReadInt();

            Ticks = Enumerable.Repeat(new HistoricalLastTick(c), n).ToList();
            Done = c.ReadBool();
        }

    }

    public sealed class HistoricalLastTick
    {
        public long Time { get; }
        public TickAttribLast TickAttribLast { get; }
        public double Price { get; }
        public long Size { get; }
        public string Exchange { get; }
        public string SpecialConditions { get; }
        internal HistoricalLastTick(ResponseReader c)
        {
            Time = c.ReadLong();
            TickAttribLast = new TickAttribLast(c.ReadInt());
            Price = c.ReadDouble();
            Size = c.ReadLong();
            Exchange = c.ReadString();
            SpecialConditions = c.ReadString();
        }
    }

    public sealed class HistoricalBidAskTicks : IHasRequestId
    {
        public int RequestId { get; }
        public List<HistoricalTick> Ticks { get; } = new List<HistoricalTick>();
        public bool Done { get; }
        public HistoricalBidAskTicks(ResponseReader c)
        {
            RequestId = c.ReadInt();
            int n = c.ReadInt();
            for (int i = 0; i < n; i++)
                Ticks.Add(new HistoricalTick(c));
            Done = c.ReadBool();
        }

    }

    public sealed class HistoricalBidAskTick
    {
        public long Time { get; }
        public TickAttribBidAsk TickAttribBidAsk { get; }
        public double PriceBid { get; }
        public double PriceAsk { get; }
        public long SizeBid { get; }
        public long SizeAsk { get; }
        internal HistoricalBidAskTick(ResponseReader c)
        {
            Time = c.ReadLong();
            TickAttribBidAsk = new TickAttribBidAsk(c.ReadInt());
            PriceBid = c.ReadDouble();
            PriceAsk = c.ReadDouble();
            SizeBid = c.ReadLong();
            SizeAsk = c.ReadLong();
        }
    }

}
