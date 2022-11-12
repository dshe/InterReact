using StringEnums;

namespace InterReact;

public sealed class FundamentalDataReportType : StringEnum<FundamentalDataReportType>
{
    public static FundamentalDataReportType CompanyOverview { get; } = Create("ReportSnapshot");
    public static FundamentalDataReportType FinancialSummary { get; } = Create("ReportsFinSummary");
    public static FundamentalDataReportType FinancialRatios { get; } = Create("ReportRatios");
    public static FundamentalDataReportType FinancialStatements { get; } = Create("ReportsFinStatements");
    public static FundamentalDataReportType AnalystEstimates { get; } = Create("RESC");
    public static FundamentalDataReportType CompanyCalendar { get; } = Create("CalendarReport");
}
