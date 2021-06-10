using System;

namespace InterReact
{
    public abstract class TickByTick : IHasRequestId
    {
        public int RequestId { get; }
        public TickByTickType TickType { get; } // not used when BidAsk
        public long Time { get; }

        protected TickByTick(int requestId, TickByTickType tickType, long time)
        {
            RequestId = requestId;
            TickType = tickType;
            Time = time;
        }

        internal static TickByTick Create(ResponseReader c)
        {
            int requestId = c.ReadInt();
            TickByTickType tickType = c.ReadEnum<TickByTickType>();
            long time = c.ReadLong();
            return tickType switch
            {
                TickByTickType.None => new TickByTickNone(),
                TickByTickType.Last => new TickByTickAllLast(requestId, tickType, time, c),
                TickByTickType.AllLast => new TickByTickAllLast(requestId, tickType, time, c),
                TickByTickType.BidAsk => new TickByTickBidAsk(requestId, tickType, time, c),
                TickByTickType.MidPoint => new TickByTickMidpoint(requestId, tickType, time, c),
                _ => throw new ArgumentException("Invalid TickByTick type.")
            };
        }

        public static readonly TickByTick None = new TickByTickNone();
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
        public TickByTickAllLast(int requestId, TickByTickType tickType, long time, ResponseReader c) : base(requestId, tickType, time)
        {
            Price = c.ReadDouble();
            Size = c.ReadInt();
            TickAttribLast = new TickAttribLast(c.ReadInt());
            Exchange = c.ReadString();
            SpecialConditions = c.ReadString();
        }
    }

    public sealed class TickByTickBidAsk : TickByTick
    {
        public double BidPrice { get; }
        public double AskPrice { get; }
        public int BidSize { get; }
        public int AskSize { get; }
        public TickAttribBidAsk TickAttribBidAsk { get; }
        public TickByTickBidAsk(int requestId, TickByTickType tickType, long time, ResponseReader c) : base(requestId, tickType, time)
        {
            BidPrice = c.ReadDouble();
            AskPrice = c.ReadDouble();
            BidSize = c.ReadInt();
            AskSize = c.ReadInt();
            TickAttribBidAsk = new TickAttribBidAsk(c.ReadInt());
        }
    }

    public sealed class TickByTickMidpoint : TickByTick
    {
        public double Midpoint { get; }
        public TickByTickMidpoint(int requestId, TickByTickType tickType, long time, ResponseReader c) : base(requestId, tickType, time)
        {
            Midpoint = c.ReadDouble();
        }
    }

}
