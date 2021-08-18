using System;
using System.Reactive.Linq;

namespace InterReact
{
    public sealed class HistoricalDataUpdate : IHasRequestId
    {
        public int RequestId { get; }
        public int BarCount { get; }
        public string Date { get; }
        public double Open { get; }
        public double Close { get; }
        public double High { get; }
        public double Low { get; }
        public double WAP { get; }
        public long Volume { get; }
        internal HistoricalDataUpdate(ResponseReader r)
        {
            RequestId = r.ReadInt();
            BarCount = r.ReadInt();
            Date = r.ReadString();
            Open = r.ReadDouble();
            Close = r.ReadDouble();
            High = r.ReadDouble();
            Low = r.ReadDouble();
            WAP = r.ReadDouble();
            Volume = r.ReadLong();
        }
    }


}
