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
        internal AccountValue(ResponseReader r)
        {
            r.RequireVersion(2);
            Key = r.ReadString();
            Value = r.ReadString();
            Currency = r.ReadString();
            Account = r.ReadString();
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
        internal PortfolioValue(ResponseReader r)
        {
            r.RequireVersion(8);
            Contract = new Contract
            {
                ContractId = r.ReadInt(),
                Symbol = r.ReadString(),
                SecurityType = r.ReadStringEnum<SecurityType>(),
                LastTradeDateOrContractMonth = r.ReadString(),
                Strike = r.ReadDouble(),
                Right = r.ReadStringEnum<OptionRightType>(),
                Multiplier = r.ReadString(),
                PrimaryExchange = r.ReadString(),
                Currency = r.ReadString(),
                LocalSymbol = r.ReadString(),
                TradingClass = r.ReadString()
            };
            Position = r.ReadDouble();
            MarketPrice = r.ReadDouble();
            MarketValue = r.ReadDouble();
            AverageCost = r.ReadDouble();
            UnrealizedPnl = r.ReadDouble();
            RealizedPnl = r.ReadDouble();
            Account = r.ReadString();
        }
    }

    public sealed class AccountUpdateTime
    {
        private static readonly LocalTimePattern TimePattern = LocalTimePattern.CreateWithInvariantCulture("HH:mm");
        public LocalTime Time { get; init; }
        public AccountUpdateTime(LocalTime time) => Time = time;
        internal AccountUpdateTime(ResponseReader r)
        {
            r.IgnoreVersion();
            Time = r.ReadLocalTime(TimePattern);
        }
    }

    /// <summary>
    /// This signals the end of update values for a particular account, not the end of the observable.
    /// </summary>
    public sealed class AccountUpdateEnd
    {
        public string Account { get; init; }
        public AccountUpdateEnd(string account = "") => Account = account;
        internal AccountUpdateEnd(ResponseReader r)
        {
            r.IgnoreVersion();
            Account = r.ReadString();
        }
    }
}
