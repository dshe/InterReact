namespace InterReact
{
    public sealed class PnL : IHasRequestId
    {
        public int RequestId { get; }
        public double DailyPnL { get; }
        public double? RealizedPnL { get; }
        public PnL(ResponseReader r)
        {
            RequestId = r.ReadInt();
            DailyPnL = r.ReadDouble();
        }
    }

    public sealed class PnLSingle : IHasRequestId
    {
        public int RequestId { get; }
        public int Pos { get; }
        public double DailyPnL { get; }
        public double? RealizedPnL { get; }
        public double Value { get; }
        public PnLSingle(ResponseReader r)
        {
            RequestId = r.ReadInt();
            Pos = r.ReadInt();
            DailyPnL = r.ReadDouble();
            Value = r.ReadDouble();
        }
    }

}
