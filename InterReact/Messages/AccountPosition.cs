using InterReact.Core;
using InterReact.StringEnums;

namespace InterReact.Messages
{
    public sealed class AccountPosition
    {
        public string Account { get; }
        public Contract Contract { get; }
        public double Position { get; }
        public double AverageCost { get; }
        internal AccountPosition(ResponseReader c)
        {
            c.RequireVersion(3);
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
            Position = c.Read<double>(); // may be an int
            AverageCost = c.Read<double>();
       }
    }

    public sealed class AccountPositionEnd
    {
        internal AccountPositionEnd(ResponseReader c) => c.IgnoreVersion();
    }
}
