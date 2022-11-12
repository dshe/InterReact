using NodaTime;
using NodaTime.Text;
using System.Collections.Generic;

namespace InterReact;

public sealed class HistoricalData : IHasRequestId
{
    internal static readonly LocalDateTimePattern DateTimePattern = LocalDateTimePattern.CreateWithInvariantCulture("yyyyMMdd  HH:mm:ss");

    public int RequestId { get; }
    public LocalDateTime Start { get; }
    public LocalDateTime End { get; }
    public IList<HistoricalDataBar> Bars { get; } = new List<HistoricalDataBar>();

    internal HistoricalData() { }

    internal HistoricalData(ResponseReader r) // a one-shot deal
    {
        if (!r.Connector.SupportsServerVersion(ServerVersion.SYNT_REALTIME_BARS))
            r.RequireVersion(3);

        RequestId = r.ReadInt();
        Start = r.ReadLocalDateTime(DateTimePattern);
        End = r.ReadLocalDateTime(DateTimePattern);
        int n = r.ReadInt();
        for (int i = 0; i < n; i++)
            Bars.Add(new HistoricalDataBar(r));
    }
}

public sealed class HistoricalDataBar
{
    public LocalDateTime Date { get; }
    public double Open { get; }
    public double High { get; }
    public double Low { get; }
    public double Close { get; }
    public long Volume { get; }
    public double WeightedAveragePrice { get; }
    public int Count { get; }

    internal HistoricalDataBar() { }

    internal HistoricalDataBar(ResponseReader r)
    {
        Date = r.ReadLocalDateTime(HistoricalData.DateTimePattern);
        Open = r.ReadDouble();
        High = r.ReadDouble();
        Low = r.ReadDouble();
        Close = r.ReadDouble();
        Volume = r.ReadLong();
        WeightedAveragePrice = r.ReadDouble();
        if (!r.Connector.SupportsServerVersion(ServerVersion.SYNT_REALTIME_BARS))
            r.ReadString(); /*string hasGaps = */
        Count = r.ReadInt();
    }
}
