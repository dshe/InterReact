using System;
using InterReact.Core;

namespace InterReact.Messages.Conditions
{
	public abstract class ContractCondition : OperatorCondition
    {
        public int ConId { get; set; }
        public string Exchange { get; set; } = "";

        private const string Delimiter = " of ";

        public Func<int, string, string> ContractResolver { get; set; }

        public ContractCondition()
        {
            ContractResolver = (conid, exch) => conid + "(" + exch + ")";
        }

        public override string ToString()
        {
            return Type + Delimiter + ContractResolver(ConId, Exchange) + base.ToString();
        }

        public override bool Equals(object obj)
        {
            var other = obj as ContractCondition;

            if (other == null)
                return false;

            return base.Equals(obj)
                && ConId == other.ConId
                && Exchange.Equals(other.Exchange);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + ConId.GetHashCode() + Exchange.GetHashCode();
        }

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

        internal override void Deserialize(ResponseReader reader)
        {
            base.Deserialize(reader);

            ConId = reader.Read<int>();
            Exchange = reader.ReadString();
        }

        internal override void Serialize(RequestMessage message)
        {
            base.Serialize(message);
            message.Write(ConId, Exchange);
        }
    }
}
