using InterReact.Core;
using InterReact.Enums;
using InterReact.StringEnums;

namespace InterReact.Messages
{
    public sealed class ContractComboLeg // input + output
    {
        /// <summary>
        /// The unique contract identifier.
        /// </summary>
        public int ContractId { get; set; }

        /// <summary>
        /// The relative number of contracts for the leg.
        /// </summary>
        public int Ratio { get; set; }

        public TradeAction TradeAction { get; set; } = TradeAction.Undefined;

        /// <summary>
        /// The exchange to which the order will be routed.
        /// </summary>
        public string Exchange { get; set; } = "";

        public ComboOpenClose OpenClose { get; set; } = ComboOpenClose.Undefined;

        /// <summary>
        /// For stock legs when doing a short sale.
        /// ShortSaleSlot of ThirdParty requires DesignatedLocation to be specified. 
        /// Non-empty DesignatedLocation Values for all Status cases will cause orders to be rejected.
        /// </summary>
        public ComboShortSaleSlot ComboShortSaleSlot { get; set; }

        /// <summary>
        /// Use only when ComboShortSaleSlot is ThirdParty.
        /// </summary>
        public string DesignatedLocation { get; set; } = "";

        /// <summary>
        /// Short Sale Exempt Code
        /// </summary>
        public int ExemptCode { get; set; } = -1;

        public ContractComboLeg() { }

        internal ContractComboLeg(ResponseReader c)
        {
            ContractId = c.Read<int>();
            Ratio = c.Read<int>();
            TradeAction = c.ReadStringEnum<TradeAction>();
            Exchange = c.ReadString();
            OpenClose = c.Read<ComboOpenClose>();
            ComboShortSaleSlot = c.Read<ComboShortSaleSlot>();
            DesignatedLocation = c.ReadString();
            ExemptCode = c.Read<int>();
        }
    }
}
