namespace InterReact
{
    public sealed class AccountPosition
    {
        public string Account { get; init; }
        public Contract Contract { get; init; }
        public double Position { get; init; }
        public double AverageCost { get; init; }
        public AccountPosition()
        {
            Account = "";
            Contract = new Contract();
        }
        internal AccountPosition(ResponseReader c)
        {
            c.RequireVersion(3);
            Account = c.ReadString();
            Contract = GetContract();
            Position = c.Config.SupportsServerVersion(ServerVersion.FractionalPositions) ? c.ReadDouble() : c.ReadInt();
            AverageCost = c.ReadDouble();

            Contract GetContract() => new()
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
        }
    }

    public sealed class AccountPositionEnd
    {
        internal AccountPositionEnd(ResponseReader c) => c.IgnoreVersion();
    }
}
