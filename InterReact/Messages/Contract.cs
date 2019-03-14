using System.Collections.Generic;
using InterReact.Enums;
using System.IO;
using System.Linq;
using System;
using InterReact.StringEnums;

namespace InterReact.Messages
{
    public sealed class Contract // input + output
    {
        /// <summary>
        /// The unique contract identifier.
        /// </summary>
        public int ContractId { get; set; }

        public string Symbol { get; set; } = "";

        public SecurityType SecurityType { get; set; } = SecurityType.Undefined;

        public string LastTradeDateOrContractMonth { get; set; } = "";  // Expiry: YYYYMM or YYYYMMDD

        public double Strike { get; set; }
        public RightType Right { get; set; } = RightType.Undefined;
        public string Multiplier { get; set; } = "";

        /// <summary>
        /// The exchange where the contract is traded or the the order destination. For example: ARCA, SMART.
        /// </summary>
        public string Exchange { get; set; } = "";

        public string Currency { get; set; } = "";

        /// <summary>
        /// The contract's symbol on the primary exchange.
        /// </summary>
        public string LocalSymbol { get; set; } = "";

        /// <summary>
        /// The non-aggregate (not SMART) primary exchange.
        /// </summary>
        public string PrimaryExchange { get; set; } = "";

        /// <summary>
        /// The trading class name for this contract.
        /// </summary>
        public string TradingClass { get; set; } = "";

        /// <summary>
        /// If set to true, contract details requests and historical data queries can be performed pertaining to expired contracts.
        /// Historical data queries on expired contracts are limited to the last year of the contracts life,
        /// and are initially only supported for expired futures contracts.
        /// Must not be set to true for orders.
        /// </summary>
        public bool IncludeExpired { get; set; } // input only
        public SecurityIdType SecurityIdType { get; set; } = SecurityIdType.Undefined; // input only
        public string SecurityId { get; set; } = ""; // input only

        public string ComboLegsDescription { get; internal set; } = "";

        /// <summary>
        /// The legs of a combined contract definition.
        /// </summary>
        public IList<ContractComboLeg> ComboLegs { get; } = new List<ContractComboLeg>(); // input + output

        /// <summary>
        /// Delta and underlying price for Delta-Neutral combo orders.
        /// </summary>
        public UnderComp? Undercomp { get; set; }
    }
}
