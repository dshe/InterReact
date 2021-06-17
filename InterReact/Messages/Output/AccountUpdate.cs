using NodaTime;
using NodaTime.Text;

namespace InterReact
{
    public sealed class AccountValue
    {
        public string Account { get; init; }
        public string Key { get; init; }
        public string Currency { get; init; }
        public string Value { get; init; }
        public AccountValue() => Account = Key = Currency = Value = "";
        internal AccountValue(ResponseReader c)
        {
            c.RequireVersion(2);
            Key = c.ReadString();
            Value = c.ReadString();
            Currency = c.ReadString();
            Account = c.ReadString();
        }
    }

    public sealed class PortfolioValue
    {
        public string Account { get; init; }
        public Contract Contract { get; init; }
        public double Position { get; init; }
        public double MarketPrice { get; init; }
        public double MarketValue { get; init; }
        public double AverageCost { get; init; }
        public double UnrealizedPnl { get; init; }
        public double RealizedPnl { get; init; }
        public PortfolioValue() 
        {
            Account = "";
            Contract = new Contract();
        }
        internal PortfolioValue(ResponseReader c)
        {
            c.RequireVersion(8);
            Contract = new Contract
            {
                ContractId = c.ReadInt(),
                Symbol = c.ReadString(),
                SecurityType = c.ReadStringEnum<SecurityType>(),
                LastTradeDateOrContractMonth = c.ReadString(),
                Strike = c.ReadDouble(),
                Right = c.ReadStringEnum<OptionRightType>(),
                Multiplier = c.ReadString(),
                PrimaryExchange = c.ReadString(),
                Currency = c.ReadString(),
                LocalSymbol = c.ReadString(),
                TradingClass = c.ReadString()
            };
            Position = c.Config.SupportsServerVersion(ServerVersion.FractionalPositions) ? c.ReadDouble() : c.ReadInt();
            MarketPrice = c.ReadDouble();
            MarketValue = c.ReadDouble();
            AverageCost = c.ReadDouble();
            UnrealizedPnl = c.ReadDouble();
            RealizedPnl = c.ReadDouble();
            Account = c.ReadString();
        }
    }

    public sealed class AccountUpdateTime
    {
        private static readonly LocalTimePattern TimePattern = LocalTimePattern.CreateWithInvariantCulture("HH:mm");
        public LocalTime Time { get; init; }
        public AccountUpdateTime(LocalTime time) => Time = time;
        internal AccountUpdateTime(ResponseReader c)
        {
            c.IgnoreVersion();
            Time = c.ReadLocalTime(TimePattern);
        }
    }

    /// <summary>
    /// This signals the end of update values for a particular account, not the end of the observable.
    /// </summary>
    public sealed class AccountUpdateEnd
    {
        public string Account { get; init; }
        public AccountUpdateEnd(string account = "") => Account = account;
        internal AccountUpdateEnd(ResponseReader c)
        {
            c.IgnoreVersion();
            Account = c.ReadString();
        }
    }
}
