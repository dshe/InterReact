/* Copyright (C) 2019 Interactive Brokers LLC. All rights reserved. This code is subject to the terms
 * and conditions of the IB API Non-Commercial License or the IB API Commercial License, as applicable. */

using System;
using System.IO;
using System.Linq;

namespace InterReact
{
    [System.Runtime.InteropServices.ComVisible(true)]
    public abstract class OrderCondition
    {
        public OrderConditionType Type { get; private set; }
        public bool IsConjunctionConnection { get; set; }

        public static OrderCondition Create(OrderConditionType type)
        {
            OrderCondition? rval = null;

            switch (type)
            {
                case OrderConditionType.Execution:
                    rval = new ExecutionCondition();
                    break;

                case OrderConditionType.Margin:
                    rval = new MarginCondition();
                    break;

                case OrderConditionType.PercentChange:
                    rval = new PercentChangeCondition();
                    break;

                case OrderConditionType.Price:
                    rval = new PriceCondition();
                    break;

                case OrderConditionType.Time:
                    rval = new TimeCondition();
                    break;

                case OrderConditionType.Volume:
                    rval = new VolumeCondition();
                    break;
            }

            if (rval != null)
                rval.Type = type;

            return rval ?? throw new InvalidOperationException("Invalid OrderConditionType.");
        }

        internal virtual void Serialize(RequestMessage message) =>
            message.Write(IsConjunctionConnection ? "a" : "o");

        internal virtual void Deserialize(ResponseReader c) =>
            IsConjunctionConnection = c.ReadString() == "a";

        virtual protected bool TryParse(string cond)
        {
            IsConjunctionConnection = cond == " and";

            return IsConjunctionConnection || cond == " or";
        }

        public static OrderCondition? Parse(string cond)
        {
            var conditions = Enum.GetValues(typeof(OrderConditionType)).OfType<OrderConditionType>().Select(t => Create(t)).ToList();

            return conditions.FirstOrDefault(c => c.TryParse(cond));
        }

        public override bool Equals(object? obj)
        {
            var other = obj as OrderCondition;

            if (other == null)
                return false;

            return IsConjunctionConnection == other.IsConjunctionConnection && Type == other.Type;
        }

        public override int GetHashCode()
        {
            return IsConjunctionConnection.GetHashCode() + Type.GetHashCode();
        }
    }

    class StringSuffixParser
    {
        public StringSuffixParser(string str)
        {
            Rest = str;
        }

        string SkipSuffix(string perfix)
        {
            return Rest.Substring(Rest.IndexOf(perfix) + perfix.Length);
        }

        public string GetNextSuffixedValue(string perfix)
        {
            var rval = Rest.Substring(0, Rest.IndexOf(perfix));
            Rest = SkipSuffix(perfix);

            return rval;
        }

        public string Rest { get; private set; }
    }
}
