using StringEnums;

namespace InterReact.StringEnums
{
    public sealed class FundamentalDataReportType : StringEnum<FundamentalDataReportType>
    {
        public static readonly FundamentalDataReportType CompanyOverview = Create("ReportSnapshot");
        public static readonly FundamentalDataReportType FinancialSummary = Create("ReportsFinSummary");
        public static readonly FundamentalDataReportType FinancialRatios = Create("ReportRatios");
        public static readonly FundamentalDataReportType FinancialStatements = Create("ReportsFinStatements");
        public static readonly FundamentalDataReportType AnalystEstimates = Create("RESC");
        public static readonly FundamentalDataReportType CompanyCalendar = Create("CalendarReport");
    }
}
