using InterReact.Core;
using InterReact.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InterReact.Messages
{
    class HistoricalTicks : IHasRequestId
    {
        public int RequestId { get; }
        public IList<HistoricalTick> Ticks { get; } = new List<HistoricalTick>();
        public bool Done { get; }
        public HistoricalTicks(ResponseComposer c)
        {
            RequestId = c.ReadInt();
            var n = c.ReadInt();
            for (int i = 0; i < n; i++)
                Ticks.Add(new HistoricalTick(c));
            Done = c.ReadBool();
        }

    }

    public class HistoricalTick
    {
        public long Time { get; }
        public double Price { get; }
        public long Size { get; }
        internal HistoricalTick(ResponseComposer c)
        {
            Time = c.ReadLong();
            c.ReadInt(); // ?
            Price = c.ReadDouble();
            Size = c.ReadLong();
        }
    }

    class HistoricalLastTicks : IHasRequestId
    {
        public int RequestId { get; }
        public IList<HistoricalLastTick> Ticks { get; } = new List<HistoricalLastTick>();
        public bool Done { get; }
        public HistoricalLastTicks(ResponseComposer c)
        {
            RequestId = c.ReadInt();
            var n = c.ReadInt();

            Ticks = Enumerable.Repeat(new HistoricalLastTick(c), n).ToList().AsReadOnly();
            Done = c.ReadBool();
        }

    }

    public class HistoricalLastTick
    {
        public long Time { get; }
        public TickAttribLast TickAttribLast { get;  }
        public double Price { get; }
        public long Size { get; }
        public string Exchange { get; }
        public string SpecialConditions { get; }
        internal HistoricalLastTick(ResponseComposer c)
        {
            Time = c.ReadLong();
            TickAttribLast = new TickAttribLast(c.ReadInt());
            Price = c.ReadDouble();
            Size = c.ReadLong();
            Exchange = c.ReadString();
            SpecialConditions = c.ReadString();
        }
    }

    class HistoricalBidAskTicks : IHasRequestId
    {
        public int RequestId { get; }
        public IList<HistoricalTick> Ticks { get; } = new List<HistoricalTick>();
        public bool Done { get; }
        public HistoricalBidAskTicks(ResponseComposer c)
        {
            RequestId = c.ReadInt();
            var n = c.ReadInt();
            for (int i = 0; i < n; i++)
                Ticks.Add(new HistoricalTick(c));
            Done = c.ReadBool();
        }

    }

    public class HistoricalBidAskTick
    {
        public long Time { get; }
        public TickAttribBidAsk TickAttribBidAsk { get; }
        public double PriceBid { get; }
        public double PriceAsk { get; }
        public long SizeBid { get; }
        public long SizeAsk { get; }
        internal HistoricalBidAskTick(ResponseComposer c)
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
