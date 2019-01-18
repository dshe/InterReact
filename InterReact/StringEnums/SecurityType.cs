using StringEnums;

namespace InterReact.StringEnums
{
    public sealed class SecurityType : StringEnum<SecurityType>
    {
        public static readonly SecurityType Undefined = Create("");

        public static readonly SecurityType Stock = Create("STK");
        public static readonly SecurityType Option = Create("OPT");
        public static readonly SecurityType Future = Create("FUT");
        public static readonly SecurityType Index = Create("IND");

        /// <summary>
        /// FOP = options on futures
        /// </summary>
        public static readonly SecurityType FutureOption = Create("FOP");

        public static readonly SecurityType Cash = Create("CASH");

        /// <summary>
        /// For Combination Orders - must use combo leg details.
        /// </summary>
        public static readonly SecurityType Bag = Create("BAG");
        public static readonly SecurityType Warrant = Create("WAR");
        public static readonly SecurityType Bond = Create("BOND");
        public static readonly SecurityType Commodity = Create("CMDTY");
        public static readonly SecurityType News = Create("NEWS");
        public static readonly SecurityType Fund = Create("FUND");

        public static readonly SecurityType Bill = Create("BILL");
        public static readonly SecurityType ContractForDifference = Create("CFD");
        public static readonly SecurityType IndexOption = Create("IOPT");
        public static readonly SecurityType Forward = Create("FWD");
        public static readonly SecurityType Fixed = Create("FIXED");
        public static readonly SecurityType Slb = Create("SLB");
        public static readonly SecurityType Bsk = Create("BSK");
        public static readonly SecurityType Icu = Create("ICU");
        public static readonly SecurityType Ics = Create("ICS");
    }
}
