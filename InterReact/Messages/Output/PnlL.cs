namespace InterReact
{
    public sealed class PnL : IHasRequestId
    {
        public int RequestId { get; }
        public double DailyPnL { get; }
        public double? RealizedPnL { get; }
        public PnL(ResponseReader c)
        {
            RequestId = c.ReadInt();
            DailyPnL = c.ReadDouble();
        }
    }

    public sealed class PnLSingle : IHasRequestId
    {
        public int RequestId { get; }
        public int Pos { get; }
        public double DailyPnL { get; }
        public double? RealizedPnL { get; }
        public double Value { get; }
        public PnLSingle(ResponseReader c)
        {
            RequestId = c.ReadInt();
            Pos = c.ReadInt();
            DailyPnL = c.ReadDouble();
            Value = c.ReadDouble();
        }
    }

}
