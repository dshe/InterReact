using StringEnums;

namespace InterReact.StringEnums
{
    public sealed class SecurityIdType : StringEnum<SecurityIdType>
    {
        public static readonly SecurityIdType Undefined = Create("");

        /// <summary>
        /// Example: Apple: US0378331005
        /// International.
        /// Splits usually involve a new ISIN.
        /// </summary>
        public static readonly SecurityIdType Isin = Create("ISIN");

        /// <summary>
        /// Example: Apple: 037833100
        /// North America.
        /// </summary>
        public static readonly SecurityIdType Cusip = Create("CUSIP");

        /// <summary>
        /// Consists of 6-AN + check digit. Example: BAE: 0263494
        /// UK.
        /// </summary>
        public static readonly SecurityIdType Sedol = Create("SEDOL");

        /// <summary>
        /// Consists of exchange-independent RIC Root and a suffix identifying the exchange. Example: AAPL.O for Apple on NASDAQ.
        /// </summary>
        public static readonly SecurityIdType Ric = Create("RIC");
    }
}