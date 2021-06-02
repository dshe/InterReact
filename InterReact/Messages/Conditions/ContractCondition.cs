using System;

namespace InterReact
{
    public abstract class ContractCondition : OperatorCondition
    {
        private const string Delimiter = " of ";
        public int ConId { get; set; }
        public string Exchange { get; set; } = "";

        public Func<int, string, string> ContractResolver { get; set; }

        public ContractCondition() =>
            ContractResolver = (conid, exch) => conid + "(" + exch + ")";

        public override string ToString()
            => Type + Delimiter + ContractResolver(ConId, Exchange) + base.ToString();

        public override bool Equals(object? obj)
        {
            return (obj is ContractCondition other)
                && base.Equals(other)
                && ConId == other.ConId
                && Exchange.Equals(other.Exchange);
        }

        public override int GetHashCode()
            => base.GetHashCode() + ConId.GetHashCode() + Exchange.GetHashCode();

        protected override bool TryParse(string cond)
        {
            try
            {
                if (cond.Substring(0, cond.IndexOf(Delimiter)) != Type.ToString())
                    return false;

                cond = cond.Substring(cond.IndexOf(Delimiter) + Delimiter.Length);

                if (!int.TryParse(cond.Substring(0, cond.IndexOf("(")), out int conid))
                    return false;

                ConId = conid;
                cond = cond.Substring(cond.IndexOf("(") + 1);
                Exchange = cond.Substring(0, cond.IndexOf(")"));
                cond = cond.Substring(cond.IndexOf(")") + 1);
                return base.TryParse(cond);
            }
            catch
            {
                return false;
            }
        }

        internal override void Deserialize(ResponseReader c)
        {
            base.Deserialize(c);
            ConId = c.ReadInt();
            Exchange = c.ReadString();
        }

        internal override void Serialize(RequestMessage message)
        {
            base.Serialize(message);
            message.Write(ConId, Exchange);
        }
    }
}
