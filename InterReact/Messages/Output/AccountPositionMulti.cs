namespace InterReact
{
    public sealed class AccountPositionMulti : IHasRequestId
    {
        public int RequestId { get; }
        public string Account { get; }
        public Contract Contract { get; }
        public double Pos { get; }
        public double AvgCost { get; }
        public string ModelCode { get; }
        internal AccountPositionMulti(ResponseReader r)
        {
            r.IgnoreVersion();
            RequestId = r.ReadInt();
            Account = r.ReadString();
            Contract = new Contract
            {
                ContractId = r.ReadInt(),
                Symbol = r.ReadString(),
                SecurityType = r.ReadStringEnum<SecurityType>(),
                LastTradeDateOrContractMonth = r.ReadString(),
                Strike = r.ReadDouble(),
                Right = r.ReadStringEnum<OptionRightType>(),
                Multiplier = r.ReadString(),
                Exchange = r.ReadString(),
                Currency = r.ReadString(),
                LocalSymbol = r.ReadString(),
                TradingClass = r.ReadString()
            };
            Pos = r.ReadDouble();
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
