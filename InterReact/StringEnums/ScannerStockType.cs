using StringEnums;

namespace InterReact
{
    public sealed class ScannerStockType : StringEnum<ScannerStockType>
    {
        public static readonly ScannerStockType All = Create("ALL");
        public static readonly ScannerStockType Corporation = Create("CORP");
        public static readonly ScannerStockType AmericanDepositaryReceipt = Create("ADR");
        public static readonly ScannerStockType Stock = Create("STOCK");
        public static readonly ScannerStockType ExchangeTradedFund = Create("ETF");
        public static readonly ScannerStockType RealEstateInvestmentTrust = Create("REIT");
        public static readonly ScannerStockType ClosedEndFund = Create("CEF");
    }
}
