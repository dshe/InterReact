namespace InterReact
{
    public sealed class AccountPosition
    {
        public string Account { get; }
        public Contract Contract { get; }
        public double Position { get; }
        public double AverageCost { get; }
        internal AccountPosition(ResponseReader c)
        {
            c.RequireVersion(3);
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
            Position = c.Config.SupportsServerVersion(ServerVersion.FractionalPositions) ? c.ReadDouble() : c.ReadInt();
            AverageCost = c.ReadDouble();
        }
    }

    public sealed class AccountPositionEnd
    {
        internal AccountPositionEnd(ResponseReader c) => c.IgnoreVersion();
    }
}
