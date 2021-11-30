namespace InterReact;

public sealed class ContractComboLeg // input + output
{
    /// <summary>
    /// The unique contract identifier.
    /// </summary>
    public int ContractId { get; init; }

    /// <summary>
    /// The relative number of contracts for the leg.
    /// </summary>
    public int Ratio { get; init; }

    public OrderAction TradeAction { get; init; } = OrderAction.Undefined;

    /// <summary>
    /// The exchange to which the order will be routed.
    /// </summary>
    public string Exchange { get; init; } = "";

    public ComboOpenClose OpenClose { get; init; } = ComboOpenClose.Undefined;

    /// <summary>
    /// For stock legs when doing a short sale.
    /// ShortSaleSlot of ThirdParty requires DesignatedLocation to be specified. 
    /// Non-empty DesignatedLocation Values for all Status cases will cause orders to be rejected.
    /// </summary>
    public ComboShortSaleSlot ComboShortSaleSlot { get; init; }

    /// <summary>
    /// Use only when ComboShortSaleSlot is ThirdParty.
    /// </summary>
    public string DesignatedLocation { get; init; } = "";

    /// <summary>
    /// Short Sale Exempt Code
    /// </summary>
    public int ExemptCode { get; init; } = -1;

    public ContractComboLeg() { }

    internal ContractComboLeg(ResponseReader r)
    {
        ContractId = r.ReadInt();
        Ratio = r.ReadInt();
        TradeAction = r.ReadStringEnum<OrderAction>();
        Exchange = r.ReadString();
        OpenClose = r.ReadEnum<ComboOpenClose>();
        ComboShortSaleSlot = r.ReadEnum<ComboShortSaleSlot>();
        DesignatedLocation = r.ReadString();
        ExemptCode = r.ReadInt();
    }
}
