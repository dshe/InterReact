using System;
using System.Reactive.Linq;

namespace InterReact
{
    public interface IHistoricalxxData : IHasRequestId { }

    public sealed class HistoricalDataUpdate : IHistoricalxxData
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
        internal HistoricalDataUpdate(ResponseReader c)
        {
            RequestId = c.ReadInt();
            BarCount = c.ReadInt();
            Date = c.ReadString();
            Open = c.ReadDouble();
            Close = c.ReadDouble();
            High = c.ReadDouble();
            Low = c.ReadDouble();
            WAP = c.ReadDouble();
            Volume = c.ReadLong();
        }
    }


}
