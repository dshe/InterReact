namespace InterReact
{
    public sealed class ScannerParameters
    {
        public string Parameters { get; }
        internal ScannerParameters(ResponseReader c)
        {
            c.IgnoreVersion();
            Parameters = c.ReadString();
        }
    }

    public sealed class ScannerSubscription // input
    {
        /// <summary>
        /// Defines the number of rows of data to return for a query.
        /// </summary>
        public int NumberOfRows { get; set; } = -1;

        /// <summary>
        /// Defines the instrument type for the scan.
        /// </summary>
        public string Instrument { get; set; } = "";

        /// <summary>
        /// The location, currently the only valid location is US stocks.
        /// </summary>
        public string LocationCode { get; set; } = "";

        public string ScanCode { get; set; } = "";

        /// <summary>
        /// Filter out contracts with a price lower than this value.
        /// </summary>
        public double AbovePrice { get; set; }

        /// <summary>
        /// Filter out contracts with a price higher than this value.
        /// </summary>
        public double BelowPrice { get; set; }

        /// <summary>
        /// Filter out contracts with a volume lower than this value.
        /// </summary>
        public int AboveVolume { get; set; }

        public int AverageOptionVolumeAbove { get; set; }

        /// <summary>
        /// Filter out contracts with a market cap lower than this value.
        /// </summary>
        public double MarketCapAbove { get; set; }

        /// <summary>
        /// Filter out contracts with a market cap above this value.
        /// </summary>
        public double MarketCapBelow { get; set; }

        /// <summary>
        /// Filter out contracts with a Moody rating below this value.
        /// </summary>
        public string MoodyRatingAbove { get; set; } = "";

        /// <summary>
        /// Filter out contracts with a Moody rating above this value.
        /// </summary>
        public string MoodyRatingBelow { get; set; } = "";

        /// <summary>
        /// Filter out contracts with an SP rating below this value.
        /// </summary>
        public string SpRatingAbove { get; set; } = "";

        /// <summary>
        /// Filter out contracts with an SP rating above this value.
        /// </summary>
        public string SpRatingBelow { get; set; } = "";

        /// <summary>
        /// Filter out contracts with a maturity date earlier than this value.
        /// </summary>
        public string MaturityDateAbove { get; set; } = "";

        /// <summary>
        /// Filter out contracts with a maturity date later than this value.
        /// </summary>
        public string MaturityDateBelow { get; set; } = "";

        /// <summary>
        /// Filter out contracts with a coupon rate lower than this value.
        /// </summary>
        public double CouponRateAbove { get; set; }

        /// <summary>
        /// Filter out contracts with a coupon rate higher than this value.
        /// </summary>
        public double CouponRateBelow { get; set; }

        /// <summary>
        /// Filter out convertible bonds.
        /// </summary>
        public string ExcludeConvertible { get; set; } = "";

        /// <summary>
        /// Can leave empty. For example, a pairing "Annual, true" used on the
        /// "top Option Implied Vol % Gainers" scan would return annualized volatilities.
        /// </summary>
        public string ScannerSettingPairs { get; set; } = "";

        public ScannerStockType StockType { get; set; } = ScannerStockType.All;
    }
}
