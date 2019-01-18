using System;
using InterReact.Interfaces;
using InterReact.Utility;
using NodaTime;

namespace InterReact.Messages
{
    public sealed class AccountValue : NotifyPropertyChanged, IAccountUpdate
    {
        public string Account { get; internal set; }
        public string Key { get; internal set; }
        public string Currency { get; internal set; }
        public string Value { get; internal set; }
    }

    public sealed class PortfolioValue : NotifyPropertyChanged, IAccountUpdate
    {
        public string Account { get; internal set; }
        public Contract Contract { get; } = new Contract();
        public double Position { get; internal set; }
        public double MarketPrice { get; internal set; }
        public double MarketValue { get; internal set; }
        public double AverageCost { get; internal set; }
        public double UnrealizedPnl { get; internal set; }
        public double RealizedPnl { get; internal set; }
    }

    public sealed class AccountUpdateTime : IAccountUpdate
    {
        public LocalTime Time { get; internal set; }
    }

    /// <summary>
    /// This signals the end of update values for a particular account, not the end of the observable.
    /// </summary>
    public sealed class AccountUpdateEnd : IAccountUpdate
    {
        public string Account { get; internal set; }
    }

}
