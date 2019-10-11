using InterReact.Core;
using InterReact.Enums;
using InterReact.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace InterReact.Messages
{
    public sealed class PnL : IHasRequestId
    {
        public int RequestId { get; }
        public double DailyPnL { get; }
        public double? UnrealizedPnL { get; }
        public double? RealizedPnL { get; }
        public PnL(ResponseComposer c)
        {
            RequestId = c.ReadInt();
            DailyPnL = c.ReadDouble();
            if (c.Config.SupportsServerVersion(ServerVersion.UnrealizedPnl))
                UnrealizedPnL = c.ReadDouble();
            if (c.Config.SupportsServerVersion(ServerVersion.RealizedPnl))
                RealizedPnL = c.ReadDouble();
        }
    }

    public sealed class PnLSingle : IHasRequestId
    {
        public int RequestId { get; }
        public int Pos { get; }
        public double DailyPnL { get; }
        public double? UnrealizedPnL { get; }
        public double? RealizedPnL { get; }
        public double Value { get; }
        public PnLSingle(ResponseComposer c)
        {
            RequestId = c.ReadInt();
            Pos = c.ReadInt();
            DailyPnL = c.ReadDouble();
            if (c.Config.SupportsServerVersion(ServerVersion.UnrealizedPnl))
                UnrealizedPnL = c.ReadDouble();
            if (c.Config.SupportsServerVersion(ServerVersion.RealizedPnl))
                RealizedPnL = c.ReadDouble();
            Value = c.ReadDouble();
        }
    }

}
