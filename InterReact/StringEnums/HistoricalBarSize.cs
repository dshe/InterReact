using StringEnums;

namespace InterReact.StringEnums
{
    public sealed class HistoricalBarSize : StringEnum<HistoricalBarSize>
    {
        public static readonly HistoricalBarSize OneSecond = Create("1 sec");
        public static readonly HistoricalBarSize FiveSeconds = Create("5 secs");
        public static readonly HistoricalBarSize TenSeconds = Create("10 secs");
        public static readonly HistoricalBarSize FifteenSeconds = Create("15 secs");
        public static readonly HistoricalBarSize ThirtySeconds = Create("30 secs");
        public static readonly HistoricalBarSize OneMinute = Create("1 min");
        public static readonly HistoricalBarSize TwoMinutes = Create("2 mins");
        public static readonly HistoricalBarSize ThreeMinutes = Create("3 mins");
        public static readonly HistoricalBarSize FiveMinutes = Create("5 mins");
        public static readonly HistoricalBarSize TenMinutes = Create("10 mins");
        public static readonly HistoricalBarSize FifteenMinutes = Create("15 mins");
        public static readonly HistoricalBarSize TwebtyMinutes = Create("20 mins");
        public static readonly HistoricalBarSize ThirtyMinutes = Create("30 mins");
        public static readonly HistoricalBarSize OneHour = Create("1 hour");
        public static readonly HistoricalBarSize TwoHours = Create("2 hours");
        public static readonly HistoricalBarSize ThreeHours = Create("3 hours");
        public static readonly HistoricalBarSize FourHours = Create("4 hours");
        public static readonly HistoricalBarSize EightHours = Create("8 hours");
        public static readonly HistoricalBarSize OneDay = Create("1 day");
        public static readonly HistoricalBarSize OneWeek = Create("1 week");
        public static readonly HistoricalBarSize OneMonth = Create("1 month");
    }
}
