/* Copyright (C) 2019 Interactive Brokers LLC. All rights reserved. This code is subject to the terms
 * and conditions of the IB API Non-Commercial License or the IB API Commercial License, as applicable. */

using System;

namespace InterReact
{
	public abstract class ContractCondition : OperatorCondition
    {
        public int ConId { get; set; }
        public string Exchange { get; set; } = "";

        const string delimiter = " of ";

        public Func<int, string, string> ContractResolver { get; set; }

        public ContractCondition()
        {
            ContractResolver = (conid, exch) => conid + "(" + exch + ")";
        }

        public override string ToString()
        {
            return Type + delimiter + ContractResolver(ConId, Exchange) + base.ToString();
        }

        public override bool Equals(object? obj)
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
                if (cond.Substring(0, cond.IndexOf(delimiter)) != Type.ToString())
                    return false;

                cond = cond.Substring(cond.IndexOf(delimiter) + delimiter.Length);
                int conid;

                if (!int.TryParse(cond.Substring(0, cond.IndexOf("(")), out conid))
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
