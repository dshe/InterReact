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
        internal AccountPositionMulti(ResponseReader c)
        {
            c.IgnoreVersion();
            RequestId = c.ReadInt();
            Account = c.ReadString();
            Contract = new Contract
            {
                ContractId = c.ReadInt(),
                Symbol = c.ReadString(),
                SecurityType = c.ReadStringEnum<SecurityType>(),
                LastTradeDateOrContractMonth = c.ReadString(),
                Strike = c.ReadDouble(),
                Right = c.ReadStringEnum<OptionRightType>(),
                Multiplier = c.ReadString(),
                Exchange = c.ReadString(),
                Currency = c.ReadString(),
                LocalSymbol = c.ReadString(),
                TradingClass = c.ReadString()
            };
            Pos = c.ReadDouble();
            AvgCost = c.ReadDouble();
            ModelCode = c.ReadString();
        }
    }

    public sealed class AccountPositionMultiEnd : IHasRequestId
    {
        public int RequestId { get; }
        internal AccountPositionMultiEnd(ResponseReader c)
        {
            c.IgnoreVersion();
            RequestId = c.ReadInt();
        }
    }

}
