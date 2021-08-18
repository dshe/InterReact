using System;

namespace InterReact
{
    public abstract class TickByTick : IHasRequestId
    {
        public static readonly TickByTick None = new TickByTickNone();
        public int RequestId { get; }
        public TickByTickType TickType { get; } // not used when BidAsk
        public long Time { get; }

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
                //TickByTickType.None => new TickByTickNone(),
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
        public TickByTickNone() : base(0, TickByTickType.None, 0) { }
    }

    public sealed class TickByTickAllLast : TickByTick
    {
        public double Price { get; }
        public int Size { get; }
        public TickAttribLast TickAttribLast { get; }
        public string Exchange { get; }
        public string SpecialConditions { get; }
        public TickByTickAllLast(int requestId, TickByTickType tickType, long time, ResponseReader r)
            : base(requestId, tickType, time)
        {
            Price = r.ReadDouble();
            Size = r.ReadInt();
            TickAttribLast = new TickAttribLast(r.ReadInt());
            Exchange = r.ReadString();
            SpecialConditions = r.ReadString();
        }
    }

    public sealed class TickByTickBidAsk : TickByTick
    {
        public double BidPrice { get; }
        public double AskPrice { get; }
        public int BidSize { get; }
        public int AskSize { get; }
        public TickAttribBidAsk TickAttribBidAsk { get; }
        public TickByTickBidAsk(int requestId, TickByTickType tickType, long time, ResponseReader r)
            : base(requestId, tickType, time)
        {
            BidPrice = r.ReadDouble();
            AskPrice = r.ReadDouble();
            BidSize = r.ReadInt();
            AskSize = r.ReadInt();
            TickAttribBidAsk = new TickAttribBidAsk(r.ReadInt());
        }
    }

    public sealed class TickByTickMidpoint : TickByTick
    {
        public double Midpoint { get; }
        public TickByTickMidpoint(int requestId, TickByTickType tickType, long time, ResponseReader r)
            : base(requestId, tickType, time)
        {
            Midpoint = r.ReadDouble();
        }
    }

}
