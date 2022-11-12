using StringEnums;

namespace InterReact;

public sealed class HistoricalBarSize : StringEnum<HistoricalBarSize>
{
    public static HistoricalBarSize OneSecond { get; } = Create("1 sec");
    public static HistoricalBarSize FiveSeconds { get; } = Create("5 secs");
    public static HistoricalBarSize TenSeconds { get; } = Create("10 secs");
    public static HistoricalBarSize FifteenSeconds { get; } = Create("15 secs");
    public static HistoricalBarSize ThirtySeconds { get; } = Create("30 secs");
    public static HistoricalBarSize OneMinute { get; } = Create("1 min");
    public static HistoricalBarSize TwoMinutes { get; } = Create("2 mins");
    public static HistoricalBarSize ThreeMinutes { get; } = Create("3 mins");
    public static HistoricalBarSize FiveMinutes { get; } = Create("5 mins");
    public static HistoricalBarSize TenMinutes { get; } = Create("10 mins");
    public static HistoricalBarSize FifteenMinutes { get; } = Create("15 mins");
    public static HistoricalBarSize TwebtyMinutes { get; } = Create("20 mins");
    public static HistoricalBarSize ThirtyMinutes { get; } = Create("30 mins");
    public static HistoricalBarSize OneHour { get; } = Create("1 hour");
    public static HistoricalBarSize TwoHours { get; } = Create("2 hours");
    public static HistoricalBarSize ThreeHours { get; } = Create("3 hours");
    public static HistoricalBarSize FourHours { get; } = Create("4 hours");
    public static HistoricalBarSize EightHours { get; } = Create("8 hours");
    public static HistoricalBarSize OneDay { get; } = Create("1 day");
    public static HistoricalBarSize OneWeek { get; } = Create("1 week");
    public static HistoricalBarSize OneMonth { get; } = Create("1 month");
}
