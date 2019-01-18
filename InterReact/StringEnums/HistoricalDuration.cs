using StringEnums;

namespace InterReact.StringEnums
{
    public sealed class HistoricalDuration : StringEnum<HistoricalDuration>
    {
        public static readonly HistoricalDuration OneMinute = Create("60 S");
        public static readonly HistoricalDuration TwoMinutes = Create("120 S");
        public static readonly HistoricalDuration ThreeMinutes = Create("180 S");
        public static readonly HistoricalDuration FiveMinutes = Create("300 S");
        public static readonly HistoricalDuration TenMinutes = Create("600 S");
        public static readonly HistoricalDuration FifteenMinutes = Create("900 S");
        public static readonly HistoricalDuration TwentyMinutes = Create("1200 S");
        public static readonly HistoricalDuration ThirtyMinutes = Create("1800 S");
        public static readonly HistoricalDuration OneHour = Create("3600 S");
        public static readonly HistoricalDuration TwoHours = Create("7200 S");
        public static readonly HistoricalDuration ThreeHours = Create("10800 S");
        public static readonly HistoricalDuration FourHours = Create("14400 S");
        public static readonly HistoricalDuration EightHours = Create("28800 S");
        public static readonly HistoricalDuration OneDay = Create("1 D");
        public static readonly HistoricalDuration TwoDays = Create("2 D");
        public static readonly HistoricalDuration OneWeek = Create("1 W");
        public static readonly HistoricalDuration TwoWeeks = Create("2 W");
        public static readonly HistoricalDuration OneMonth = Create("1 M");
        public static readonly HistoricalDuration TwoMonths = Create("2 M");
        public static readonly HistoricalDuration ThreeMonths = Create("3 M");
        public static readonly HistoricalDuration SixMonths = Create("6 M");
        public static readonly HistoricalDuration OneYear = Create("1 Y");
    }
}
