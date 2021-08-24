namespace InterReact
{
    public sealed class PnL : IHasRequestId
    {
        public int RequestId { get; }
        public double DailyPnL { get; }
        public double? UnrealizedPnL { get; }
        public double? RealizedPnL { get; }

        internal PnL() { }

        internal PnL(ResponseReader r)
        {
            RequestId = r.ReadInt();
            DailyPnL = r.ReadDouble();
            if (r.Config.SupportsServerVersion(ServerVersion.UNREALIZED_PNL))
                UnrealizedPnL = r.ReadDouble();
            if (r.Config.SupportsServerVersion(ServerVersion.REALIZED_PNL))
                RealizedPnL = r.ReadDouble();
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

        internal PnLSingle() { }

        internal PnLSingle(ResponseReader r)
        {
            RequestId = r.ReadInt();
            Pos = r.ReadInt();
            DailyPnL = r.ReadDouble();
            if (r.Config.SupportsServerVersion(ServerVersion.UNREALIZED_PNL))
                UnrealizedPnL = r.ReadDouble();
            if (r.Config.SupportsServerVersion(ServerVersion.REALIZED_PNL))
                RealizedPnL = r.ReadDouble();
            Value = r.ReadDouble();
        }
    }
}
