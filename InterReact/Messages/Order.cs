using System.Collections.Generic;

namespace InterReact;

public sealed class Order : IHasOrderId  // input + output
{
    /// <summary>
    /// The OrderId property identifies the order.
    /// </summary>
    public int OrderId { get; internal set; } // output

    public bool Solicited { get; set; }

    /// <summary>
    /// The ClientId of the API client which places the order.
    /// </summary>
    public int ClientId { get; internal set; } // output

    public int PermanentId { get; internal set; } // output

    // Primary attributes

    /// <summary>
    /// Identifies the order side.
    /// </summary>
    public OrderAction OrderAction { get; set; } = OrderAction.Undefined;

    /// <summary>
    /// The number of units to trade.
    /// </summary>
    public double TotalQuantity { get; set; }

    /// <summary>
    /// The order type: Market, Limit...
    /// </summary>
    public OrderType OrderType { get; set; } = OrderType.Undefined;

    /// <summary>
    /// The limit price, used for limit, stop-limit and relative orders.
    /// </summary>
    public double? LimitPrice { get; set; }

    /// <summary>
    /// The aux price, used as STOP price for stop + stop-limit orders,
    /// and the offset amount for relative orders.
    /// </summary>
    public double? AuxPrice { get; set; }

    /// <summary>
    /// The Time-In-Force. For example: GTC (good-till-cancelled).
    /// </summary>
    public TimeInForce TimeInForce { get; set; } = TimeInForce.Day;

    /// <summary>
    /// The One-Cancels-All group identifier.
    /// </summary>
    public string OcaGroup { get; set; } = "";

    /// <summary>
    /// The One-Cancels-All type.
    /// </summary>
    public OcaType OcaType { get; set; } = OcaType.Undefined;

    /// <summary>
    /// The order reference. 
    /// Intended for institutional customers only, although all customers may use it to identify the API client
    /// that sent the order when multiple API clients are running.
    /// </summary>
    public string OrderRef { get; set; } = "";

    /// <summary>
    /// If false, order will be created but not transmited.
    /// </summary>
    public bool Transmit { get; set; } = true;

    /// <summary>
    /// Parent order Id is used to associate Auto STP or TRAIL orders with the original order. 
    /// </summary>
    public int ParentId { get; set; }

    /// <summary>
    /// Specifies that the order is an ISE Block order.
    /// </summary>
    public bool BlockOrder { get; set; }

    public bool SweepToFill { get; set; }

    /// <summary>
    /// The publicly disclosed order size, used when placing Iceberg orders.
    /// </summary>
    public int? DisplaySize { get; set; }

    public TriggerMethod TriggerMethod { get; set; } = TriggerMethod.Default;

    /// <summary>
    /// Allows orders to be filled outside of regular trading hours.
    /// </summary>
    public bool OutsideRegularTradingHours { get; set; }

    /// <summary>
    /// If set to true, the order will not be visible when viewing the market depth. Only applies to orders routed to the ISLAND exchange.
    /// </summary>
    public bool Hidden { get; set; }

    /// <summary>
    /// Specifies the date and time after which the order will be active.
    /// yyymmdd hh:mm:ss {optional Timezone}
    /// </summary>
    public string GoodAfterTime { get; set; } = "";

    /// <summary>
    /// The date and time until the order will be active.
    /// You must enter GTD as the time in force to use this string. 
    /// The trade's 'Good Till Date", format "YYYYMMDD hh:mm:ss (optional time zone)"
    /// </summary>
    public string GoodUntilDate { get; set; } = "";

    /// <summary>
    /// Precautionary constraints are defined on the TWS Presets page, and help ensure tha tyour price and size order Values are reasonable.
    /// Orders sent from the API are also validated against these safety constraints, and may be rejected if any constraint is violated.
    /// To override validation, set this parameter’s value to True.
    /// </summary>
    public bool OverridePercentageConstraints { get; set; }

    public AgentDescription Rule80A { get; set; } = AgentDescription.None;

    /// <summary>
    /// Indicates whether all the order has to be filled in a single execution.
    /// </summary>
    public bool AllOrNone { get; set; }

    public int? MinimumQuantity { get; set; }

    /// <summary>
    /// The percent offset amount for relative orders.
    /// Specify the decimal. For example: .04 not 4
    /// </summary>
    public double? PercentOffset { get; set; }

    /// <summary>
    /// Trailing stop price for trailing limit orders.
    /// </summary>
    public double? TrailingStopPrice { get; set; }

    /// <summary>
    /// Specifies the trailing amount of a trailing stop order as a percentage.
    /// Specify the percentage. For example: 3, not .03
    /// </summary>
    public double? TrailingStopPercent { get; set; }

    public string FinancialAdvisorGroup { get; set; } = "";
    public string FinancialAdvisorProfile { get; set; } = "";
    public FinancialAdvisorAllocationMethod FinancialAdvisorMethod { get; set; } = FinancialAdvisorAllocationMethod.None;
    public string FinancialAdvisorPercentage { get; set; } = "";

    public OrderOpenClose OpenClose { get; set; } = OrderOpenClose.Undefined;
    public OrderOrigin Origin { get; set; } = OrderOrigin.Customer;

    /// <summary>
    /// 1 if you hold the shares, 2 if they will be delivered from elsewhere.
    /// Only for TradeAction="SSHORT".
    /// </summary>
    public ShortSaleSlot ShortSaleSlot { get; set; } = ShortSaleSlot.Inapplicable;

    public string DesignatedLocation { get; set; } = "";
    public int ExemptCode { get; set; } = -1;

    /// <summary>
    /// The amount off the limit price allowed for discretionary orders.
    /// </summary>
    public double DiscretionaryAmount { get; set; }

    /// <summary>
    /// Use to opt out of default SmartRouting for orders routed directly to ASX.
    /// This attribute defaults to false unless explicitly set to true.
    /// When set to false, orders routed directly to ASX will NOT use SmartRouting.
    /// When set to true, orders routed directly to ASX orders WILL use SmartRouting.
    /// </summary>
    public bool OptOutSmartRouting { get; set; }

    public AuctionStrategy AuctionStrategy { get; set; } = AuctionStrategy.Undefined;

    /// <summary>
    /// The auction's starting price.
    /// </summary>
    public double? StartingPrice { get; set; }

    /// <summary>
    /// The stock's reference price.
    /// The reference price is used for VOL orders to compute the limit price sent to an exchange (whether or not Continuous Update is selected), and for price range monitoring.
    /// </summary>
    public double? StockReferencePrice { get; set; }
    public double? Delta { get; set; }

    /// <summary>
    /// The lower value for the acceptable underlying stock price range.
    /// For price improvement option orders on BOX and VOL orders with dynamic management.
    /// </summary>
    public double? StockRangeLower { get; set; }

    /// <summary>
    /// The upper value for the acceptable underlying stock price range.
    /// For price improvement option orders on BOX and VOL orders with dynamic management.
    /// </summary>
    public double? StockRangeUpper { get; set; }

    /// <summary>
    /// Enter percentage not decimal, e.g. 2 not .02
    /// </summary>
    public double? Volatility { get; set; }
    public VolatilityType VolatilityType { get; set; } = VolatilityType.Undefined;
    public int ContinuousUpdate { get; set; }

    /// <summary>
    /// 1=Bid/Ask midpoint, 2 = BidOrAsk.
    /// </summary>
    public ReferencePriceType ReferencePriceType { get; set; } = ReferencePriceType.Undefined;

    public string DeltaNeutralOrderType { get; set; } = "";

    public double? DeltaNeutralAuxPrice { get; set; }
    public int DeltaNeutralContractId { get; set; }

    /// <summary>
    /// Set when slot=2 only.
    /// </summary>
    public string DeltaNeutralSettlingFirm { get; set; } = "";
    public string DeltaNeutralClearingAccount { get; set; } = "";
    public string DeltaNeutralClearingIntent { get; set; } = "";

    public string DeltaNeutralOpenClose { get; set; } = "";
    public bool DeltaNeutralShortSale { get; set; }
    public int DeltaNeutralShortSaleSlot { get; set; }
    public string DeltaNeutralDesignatedLocation { get; set; } = "";

    /// <summary>
    /// EFP orders only.
    /// </summary>
    public double? BasisPoints { get; set; }

    /// <summary>
    /// EFP orders only.
    /// </summary>
    public int? BasisPointsType { get; set; }

    public int? ScaleInitLevelSize { get; set; }
    public int? ScaleSubsLevelSize { get; set; }
    public double? ScalePriceIncrement { get; set; }
    public double? ScalePriceAdjustValue { get; set; }
    public int? ScalePriceAdjustInterval { get; set; }
    public double? ScaleProfitOffset { get; set; }
    public bool ScaleAutoReset { get; set; }
    public int? ScaleInitPosition { get; set; }
    public int? ScaleInitFillQty { get; set; }
    public bool ScaleRandomPercent { get; set; }

    public HedgeType HedgeType { get; set; } = HedgeType.Undefined;

    /// <summary>
    /// Beta value for beta hedge (in range 0-1), ratio for pair hedge.
    /// </summary>
    public string HedgeParam { get; set; } = "";


    public string Account { get; set; } = "";
    public string SettlingFirm { get; set; } = "";

    /// <summary>
    /// True beneficiary of the order.
    /// </summary>
    public string ClearingAccount { get; set; } = "";

    public ClearingIntent ClearingIntent { get; set; } = ClearingIntent.Default;

    public string AlgoStrategy { get; set; } = "";
    public IList<Tag> AlgoParams { get; } = new List<Tag>(); // input + output

    public bool WhatIf { get; set; }

    public string AlgoId { get; set; } = "";

    public bool NotHeld { get; set; }

    public IList<Tag> SmartComboRoutingParams { get; } = new List<Tag>(); // input + output
    public IList<OrderComboLeg> ComboLegs { get; } = new List<OrderComboLeg>(); // input + output
    public IList<Tag> MiscOptions { get; } = new List<Tag>(); // input

    /// <summary>
    /// For GTC(good-till cancelled) orders.
    /// </summary>
    public string ActiveStartTime { get; set; } = "";

    /// <summary>
    /// For GTC orders.
    /// </summary>
    public string ActiveStopTime { get; set; } = "";
    public string ScaleTable { get; set; } = "";
    public string ModelCode { get; set; } = "";
    public string ExtOperator { get; set; } = "";
    public double? CashQty { get; set; }
    public string Mifid2DecisionMaker { get; set; } = "";
    public string Mifid2DecisionAlgo { get; set; } = "";
    public string Mifid2ExecutionTrader { get; set; } = "";
    public string Mifid2ExecutionAlgo { get; set; } = "";
    public bool DontUseAutoPriceForHedge { get; set; }
    public string AutoCancelDate { get; set; } = "";
    public double? FilledQuantity { get; set; }
    public int RefFuturesConId { get; set; }

    public bool AutoCancelParent { get; set; }

    public string Shareholder { get; set; } = "";

    public bool ImbalanceOnly { get; set; }

    public bool RouteMarketableToBbo { get; set; }

    public long ParentPermId { get; set; }


    public bool RandomizeSize { get; set; }
    public bool RandomizePrice { get; set; }
    public int ReferenceContractId { get; set; }
    public bool IsPeggedChangeAmountDecrease { get; set; }
    public double? PeggedChangeAmount { get; set; }
    public double? ReferenceChangeAmount { get; set; }
    public string ReferenceExchange { get; set; } = "";
    public string AdjustedOrderType { get; set; } = "";
    public double? TriggerPrice { get; set; }
    public double? LmtPriceOffset { get; set; }
    public double? AdjustedStopPrice { get; set; }
    public double? AdjustedStopLimitPrice { get; set; }
    public double? AdjustedTrailingAmount { get; set; }
    public int AdjustableTrailingUnit { get; set; }
    public IList<OrderCondition> Conditions { get; } = new List<OrderCondition>();
    public bool ConditionsIgnoreRegularTradingHours { get; set; }
    public bool ConditionsCancelOrder { get; set; }
    public SoftDollarTier SoftDollarTier { get; } = new();
    public bool IsOmsContainer { get; set; }
    public bool DiscretionaryUpToLimitPrice { get; set; }
    public bool? UsePriceMgmtAlgo { get; set; }
    public int? Duration { get; set; }
    public int? PostToAts { get; set; }
}
