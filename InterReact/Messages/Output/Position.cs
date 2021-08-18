namespace InterReact
{
    public sealed class Position
    {
        public string Account { get; init; }
        public Contract Contract { get; init; }
        public double Quantity { get; init; }
        public double AverageCost { get; init; }
        public Position()
        {
            Account = "";
            Contract = new Contract();
        }
        internal Position(ResponseReader r)
        {
            r.RequireVersion(3);
            Account = r.ReadString();
            Contract = GetContract();
            Quantity = r.ReadDouble();
            AverageCost = r.ReadDouble();

            Contract GetContract() => new()
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
        }
    }

    public sealed class PositionEnd
    {
        internal PositionEnd(ResponseReader r) => r.IgnoreVersion();
    }
}
