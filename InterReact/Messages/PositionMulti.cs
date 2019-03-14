using InterReact.Core;
using InterReact.Interfaces;
using InterReact.StringEnums;

namespace InterReact.Messages
{
    public sealed class PositionMulti : IHasRequestId
    {
        public int RequestId { get; }
        public string Account { get; }
        public Contract Contract { get; }
        public double Pos { get; }
        public double AvgCost { get; }
        public string ModelCode { get; }
        internal PositionMulti(ResponseReader c)
        {
            c.IgnoreVersion();
            RequestId = c.Read<int>();
            Account = c.ReadString();
            Contract = new Contract
            {
                ContractId = c.Read<int>(),
                Symbol = c.ReadString(),
                SecurityType = c.ReadStringEnum<SecurityType>(),
                LastTradeDateOrContractMonth = c.ReadString(),
                Strike = c.Read<double>(),
                Right = c.ReadStringEnum<RightType>(),
                Multiplier = c.ReadString(),
                Exchange = c.ReadString(),
                Currency = c.ReadString(),
                LocalSymbol = c.ReadString(),
                TradingClass = c.ReadString()
            };
            Pos = c.Read<double>();
            AvgCost = c.Read<double>();
            ModelCode = c.ReadString();
        }
    }

    public sealed class PositionMultiEnd : IHasRequestId
    {
        public int RequestId { get; }
        internal PositionMultiEnd(ResponseReader c)
        {
            c.IgnoreVersion();
            RequestId = c.Read<int>();
        }
    }

}
