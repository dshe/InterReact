using InterReact.Utility;
using NodaTime;
using NodaTime.Text;

namespace InterReact
{
    public sealed class AccountValue : NotifyPropertyChanged, IAccountUpdate
    {
        public string Account { get; }
        public string Key { get; }
        public string Currency { get; }
        public string Value { get; }
        internal AccountValue(ResponseReader c)
        {
            c.RequireVersion(2);
            Key = c.ReadString();
            Value = c.ReadString();
            Currency = c.ReadString();
            Account = c.ReadString();
        }
    }

    public sealed class PortfolioValue : NotifyPropertyChanged, IAccountUpdate
    {
        public string Account { get; }
        public Contract Contract { get; }
        public double Position { get; }
        public double MarketPrice { get; }
        public double MarketValue { get; }
        public double AverageCost { get; }
        public double UnrealizedPnl { get; }
        public double RealizedPnl { get; }
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

    public sealed class AccountUpdateTime : IAccountUpdate
    {
        private static readonly LocalTimePattern TimePattern = LocalTimePattern.CreateWithInvariantCulture("HH:mm");
        public LocalTime Time { get; }
        internal AccountUpdateTime(ResponseReader c)
        {
            c.IgnoreVersion();
            Time = c.ReadLocalTime(TimePattern);
        }
    }

    /// <summary>
    /// This signals the end of update values for a particular account, not the end of the observable.
    /// </summary>
    public sealed class AccountUpdateEnd : IAccountUpdate
    {
        public string Account { get; }
        internal AccountUpdateEnd(ResponseReader c)
        {
            c.IgnoreVersion();
            Account = c.ReadString();
        }
    }
}
