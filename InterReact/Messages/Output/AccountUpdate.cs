using NodaTime;
using NodaTime.Text;

namespace InterReact
{
    public sealed class AccountValue
    {
        public string Key { get; init; } = "";
        public string Value { get; init; } = "";
        public string Currency { get; init; } = "";
        public string Account { get; init; } = "";

        internal AccountValue() { }
        internal AccountValue(ResponseReader r)
        {
            int version = r.GetVersion();
            Key = r.ReadString();
            Value = r.ReadString();
            Currency = r.ReadString();
            Account = version >= 2 ? r.ReadString() : "";
        }
    }

    public sealed class PortfolioValue
    {
        public string AccountName { get; } = "";
        public Contract Contract { get; } = new();
        public double Position { get; }
        public double MarketPrice { get; }
        public double MarketValue { get; }
        public double AverageCost { get; }
        public double UnrealizedPnl { get; }
        public double RealizedPnl { get; }
        internal PortfolioValue() {}
        internal PortfolioValue(ResponseReader r)
        {
            r.RequireVersion(8);
            Contract.ContractId = r.ReadInt();
            Contract.Symbol = r.ReadString();
            Contract.SecurityType = r.ReadStringEnum<SecurityType>();
            Contract.LastTradeDateOrContractMonth = r.ReadString();
            Contract.Strike = r.ReadDouble();
            Contract.Right = r.ReadStringEnum<OptionRightType>();
            Contract.Multiplier = r.ReadString();
            Contract.PrimaryExchange = r.ReadString();
            Contract.Currency = r.ReadString();
            Contract.LocalSymbol = r.ReadString();
            Contract.TradingClass = r.ReadString();
            Position = r.ReadDouble();
            MarketPrice = r.ReadDouble();
            MarketValue = r.ReadDouble();
            AverageCost = r.ReadDouble();
            UnrealizedPnl = r.ReadDouble();
            RealizedPnl = r.ReadDouble();
            AccountName = r.ReadString();
        }
    }

    public sealed class AccountUpdateTime
    {
        private static readonly LocalTimePattern TimePattern = LocalTimePattern.CreateWithInvariantCulture("HH:mm");
        public LocalTime Time { get; }
        internal AccountUpdateTime() { }
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
        public string Account { get; }
        public AccountUpdateEnd(string account = "") => Account = account;
        internal AccountUpdateEnd(ResponseReader r)
        {
            r.IgnoreVersion();
            Account = r.ReadString();
        }
    }
}
