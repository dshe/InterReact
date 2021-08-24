using System;

namespace InterReact
{
    public abstract class TickByTick : IHasRequestId
    {
        public static readonly TickByTick None = new TickByTickNone();
        public int RequestId { get; } = -1;
        public TickByTickType TickType { get; } = TickByTickType.None; // not used when BidAsk
        public long Time { get; }

        protected TickByTick() { }

        protected TickByTick(int requestId, TickByTickType tickType, long time)
        {
            RequestId = requestId;
            TickType = tickType;
            Time = time;
        }

        internal static TickByTick Create(ResponseReader r)
        {
            int requestId = r.ReadInt();
            TickByTickType tickType = r.ReadEnum<TickByTickType>();
            long time = r.ReadLong();
            return tickType switch
            {
                TickByTickType.None => None,
                TickByTickType.Last => new TickByTickAllLast(requestId, tickType, time, r),
                TickByTickType.AllLast => new TickByTickAllLast(requestId, tickType, time, r),
                TickByTickType.BidAsk => new TickByTickBidAsk(requestId, tickType, time, r),
                TickByTickType.MidPoint => new TickByTickMidpoint(requestId, tickType, time, r),
                _ => throw new ArgumentException("Invalid TickByTick type.")
            };
        }
    }

    public sealed class TickByTickNone : TickByTick
    {
        internal TickByTickNone() { }
    }

    public sealed class TickByTickAllLast : TickByTick
    {
        public double Price { get; }
        public long Size { get; }
        public TickAttribLast TickAttribLast { get; } = new TickAttribLast();
        public string Exchange { get; } = "";
        public string SpecialConditions { get; } = "";

        internal TickByTickAllLast() { }

        internal TickByTickAllLast(int requestId, TickByTickType tickType, long time, ResponseReader r)
            : base(requestId, tickType, time)
        {
            Price = r.ReadDouble();
            Size = r.ReadLong();
            TickAttribLast.Set(r.ReadInt());
            Exchange = r.ReadString();
            SpecialConditions = r.ReadString();
        }
    }

    public sealed class TickByTickBidAsk : TickByTick
    {
        public double BidPrice { get; }
        public double AskPrice { get; }
        public long BidSize { get; }
        public long AskSize { get; }
        public TickAttribBidAsk TickAttribBidAsk { get; } = new TickAttribBidAsk();

        internal TickByTickBidAsk() { }

        internal TickByTickBidAsk(int requestId, TickByTickType tickType, long time, ResponseReader r)
            : base(requestId, tickType, time)
        {
            BidPrice = r.ReadDouble();
            AskPrice = r.ReadDouble();
            BidSize = r.ReadLong();
            AskSize = r.ReadLong();
            TickAttribBidAsk.Set(r.ReadInt());
        }
    }

    public sealed class TickByTickMidpoint : TickByTick
    {
        public double Midpoint { get; }

        internal TickByTickMidpoint() { }

        internal TickByTickMidpoint(int requestId, TickByTickType tickType, long time, ResponseReader r)
            : base(requestId, tickType, time)
        {
            Midpoint = r.ReadDouble();
        }
    }
}
