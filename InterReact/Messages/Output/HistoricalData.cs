using NodaTime.Text;

namespace InterReact;

public sealed class HistoricalData : IHasRequestId
{
    internal static ZonedDateTimePattern DateTimePattern = ZonedDateTimePattern
        .CreateWithInvariantCulture("yyyyMMdd HH:mm:ss z", DateTimeZoneProviders.Tzdb);

    public int RequestId { get; }
    public Instant Start { get; }
    public Instant End { get; }
    public IList<HistoricalDataBar> Bars { get; } = new List<HistoricalDataBar>();

    internal HistoricalData(ResponseReader r) // a one-shot deal
    {
        RequestId = r.ReadInt();
        Start = r.ReadZonedDateTime(DateTimePattern).ToInstant();
        End = r.ReadZonedDateTime(DateTimePattern).ToInstant();
        int n = r.ReadInt();
        for (int i = 0; i < n; i++)
            Bars.Add(new HistoricalDataBar(r));
    }
}

public sealed class HistoricalDataBar
{
    public Instant Date { get; }
    public double Open { get; }
    public double High { get; }
    public double Low { get; }
    public double Close { get; }
    public decimal Volume { get; }
    public decimal WeightedAveragePrice { get; }
    public int Count { get; }

    internal HistoricalDataBar(ResponseReader r)
    {
        Date = r.ReadZonedDateTime(HistoricalData.DateTimePattern).ToInstant();
        Open = r.ReadDouble();
        High = r.ReadDouble();
        Low = r.ReadDouble();
        Close = r.ReadDouble();
        Volume = r.ReadDecimal();
        WeightedAveragePrice = r.ReadDecimal();
        Count = r.ReadInt();
    }
}
