using StringEnums;

namespace InterReact;

public sealed class ScannerStockType : StringEnum<ScannerStockType>
{
    public static ScannerStockType All { get; } = Create("ALL");
    public static ScannerStockType Corporation { get; } = Create("CORP");
    public static ScannerStockType AmericanDepositaryReceipt { get; } = Create("ADR");
    public static ScannerStockType Stock { get; } = Create("STOCK");
    public static ScannerStockType ExchangeTradedFund { get; } = Create("ETF");
    public static ScannerStockType RealEstateInvestmentTrust { get; } = Create("REIT");
    public static ScannerStockType ClosedEndFund { get; } = Create("CEF");
}
