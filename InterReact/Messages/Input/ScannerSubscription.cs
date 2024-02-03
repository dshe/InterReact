namespace InterReact;

public sealed class ScannerSubscription
{
    /// <summary>
    /// Defines the number of rows of data to return for a query.
    /// </summary>
    public int NumberOfRows { get; init; } = -1;

    /// <summary>
    /// Defines the instrument type for the scan.
    /// </summary>
    public string Instrument { get; init; } = "";

    /// <summary>
    /// The location, currently the only valid location is US stocks.
    /// </summary>
    public string LocationCode { get; init; } = "";

    public string ScanCode { get; init; } = "";

    /// <summary>
    /// Filter out contracts with a price lower than this value.
    /// </summary>
    public double AbovePrice { get; init; } = double.MaxValue;

    /// <summary>
    /// Filter out contracts with a price higher than this value.
    /// </summary>
    public double BelowPrice { get; init; } = double.MaxValue;

    /// <summary>
    /// Filter out contracts with a volume lower than this value.
    /// </summary>
    public int AboveVolume { get; init; } = int.MaxValue;

    public int AverageOptionVolumeAbove { get; init; } = int.MaxValue;

    /// <summary>
    /// Filter out contracts with a market cap lower than this value.
    /// </summary>
    public double MarketCapAbove { get; init; } = double.MaxValue;

    /// <summary>
    /// Filter out contracts with a market cap above this value.
    /// </summary>
    public double MarketCapBelow { get; init; } = double.MaxValue;

    /// <summary>
    /// Filter out contracts with a Moody rating below this value.
    /// </summary>
    public string MoodyRatingAbove { get; init; } = "";

    /// <summary>
    /// Filter out contracts with a Moody rating above this value.
    /// </summary>
    public string MoodyRatingBelow { get; init; } = "";

    /// <summary>
    /// Filter out contracts with an SP rating below this value.
    /// </summary>
    public string SpRatingAbove { get; init; } = "";

    /// <summary>
    /// Filter out contracts with an SP rating above this value.
    /// </summary>
    public string SpRatingBelow { get; init; } = "";

    /// <summary>
    /// Filter out contracts with a maturity date earlier than this value.
    /// </summary>
    public string MaturityDateAbove { get; init; } = "";

    /// <summary>
    /// Filter out contracts with a maturity date later than this value.
    /// </summary>
    public string MaturityDateBelow { get; init; } = "";

    /// <summary>
    /// Filter out contracts with a coupon rate lower than this value.
    /// </summary>
    public double CouponRateAbove { get; init; } = double.MaxValue;

    /// <summary>
    /// Filter out contracts with a coupon rate higher than this value.
    /// </summary>
    public double CouponRateBelow { get; init; } = double.MaxValue;

    /// <summary>
    /// Filter out convertible bonds.
    /// </summary>
    public string ExcludeConvertible { get; init; } = "";

    /// <summary>
    /// Can leave empty. For example, a pairing "Annual, true" used on the
    /// "top Option Implied Vol % Gainers" scan would return annualized volatilities.
    /// </summary>
    public string ScannerSettingPairs { get; init; } = "";

    public string StockType { get; init; } = ScannerStockType.All;
}
