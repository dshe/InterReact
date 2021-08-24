namespace InterReact
{
    public sealed class AccountPositionMulti : IHasRequestId
    {
        public int RequestId { get; }
        public string Account { get; } = "";
        public Contract Contract { get; } = new();
        public double Position { get; }
        public double AvgCost { get; }
        public string ModelCode { get; } = "";
        internal AccountPositionMulti() { }
        internal AccountPositionMulti(ResponseReader r)
        {
            r.IgnoreVersion();
            RequestId = r.ReadInt();
            Account = r.ReadString();
            Contract.ContractId = r.ReadInt();
            Contract.Symbol = r.ReadString();
            Contract.SecurityType = r.ReadStringEnum<SecurityType>();
            Contract.LastTradeDateOrContractMonth = r.ReadString();
            Contract.Strike = r.ReadDouble();
            Contract.Right = r.ReadStringEnum<OptionRightType>();
            Contract.Multiplier = r.ReadString();
            Contract.Exchange = r.ReadString();
            Contract.Currency = r.ReadString();
            Contract.LocalSymbol = r.ReadString();
            Contract.TradingClass = r.ReadString();
            Position = r.ReadDouble();
            AvgCost = r.ReadDouble();
            ModelCode = r.ReadString();
        }
    }

    public sealed class AccountPositionMultiEnd : IHasRequestId
    {
        public int RequestId { get; }
        internal AccountPositionMultiEnd(ResponseReader r)
        {
            r.IgnoreVersion();
            RequestId = r.ReadInt();
        }
    }

}
