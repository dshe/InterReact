using StringEnums;

namespace InterReact;

public sealed class HistoricalDuration : StringEnum<HistoricalDuration>
{
    public static HistoricalDuration OneMinute { get; } = Create("60 S");
    public static HistoricalDuration TwoMinutes { get; } = Create("120 S");
    public static HistoricalDuration ThreeMinutes { get; } = Create("180 S");
    public static HistoricalDuration FiveMinutes { get; } = Create("300 S");
    public static HistoricalDuration TenMinutes { get; } = Create("600 S");
    public static HistoricalDuration FifteenMinutes { get; } = Create("900 S");
    public static HistoricalDuration TwentyMinutes { get; } = Create("1200 S");
    public static HistoricalDuration ThirtyMinutes { get; } = Create("1800 S");
    public static HistoricalDuration OneHour { get; } = Create("3600 S");
    public static HistoricalDuration TwoHours { get; } = Create("7200 S");
    public static HistoricalDuration ThreeHours { get; } = Create("10800 S");
    public static HistoricalDuration FourHours { get; } = Create("14400 S");
    public static HistoricalDuration EightHours { get; } = Create("28800 S");
    public static HistoricalDuration OneDay { get; } = Create("1 D");
    public static HistoricalDuration TwoDays { get; } = Create("2 D");
    public static HistoricalDuration OneWeek { get; } = Create("1 W");
    public static HistoricalDuration TwoWeeks { get; } = Create("2 W");
    public static HistoricalDuration OneMonth { get; } = Create("1 M");
    public static HistoricalDuration TwoMonths { get; } = Create("2 M");
    public static HistoricalDuration ThreeMonths { get; } = Create("3 M");
    public static HistoricalDuration SixMonths { get; } = Create("6 M");
    public static HistoricalDuration OneYear { get; } = Create("1 Y");
}
