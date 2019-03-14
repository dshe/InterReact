using System;
using InterReact.Core;
using InterReact.Interfaces;
using InterReact.StringEnums;
using InterReact.Utility;
using NodaTime;

namespace InterReact.Messages
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
                ContractId = c.Read<int>(),
                Symbol = c.ReadString(),
                SecurityType = c.ReadStringEnum<SecurityType>(),
                LastTradeDateOrContractMonth = c.ReadString(),
                Strike = c.Read<double>(),
                Right = c.ReadStringEnum<RightType>(),
                Multiplier = c.ReadString(),
                PrimaryExchange = c.ReadString(),
                Currency = c.ReadString(),
                LocalSymbol = c.ReadString(),
                TradingClass = c.ReadString()
            };
            Position = c.Read<double>();
            MarketPrice = c.Read<double>();
            MarketValue = c.Read<double>();
            AverageCost = c.Read<double>();
            UnrealizedPnl = c.Read<double>();
            RealizedPnl = c.Read<double>();
            Account = c.ReadString();
        }
    }

    public sealed class AccountUpdateTime : IAccountUpdate
    {
        public LocalTime Time { get; }
        internal AccountUpdateTime(ResponseReader c)
        {
            c.IgnoreVersion();
            Time = c.Read<LocalTime>();
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
