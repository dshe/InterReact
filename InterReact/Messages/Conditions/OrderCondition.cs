using System;
using System.Linq;
using InterReact.Core;
using InterReact.Enums;

namespace InterReact.Messages.Conditions
{

    public abstract class OrderCondition
    {
        public OrderConditionType Type { get; private set; }
        public bool IsConjunctionConnection { get; set; }

        public static OrderCondition Create(OrderConditionType type)
        {
            OrderCondition rval = null;
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
            return rval;
        }

        internal virtual void Serialize(RequestMessage message)
        {
            message.Write(IsConjunctionConnection ? "a" : "o");
        }

        internal virtual void Deserialize(ResponseComposer reader)
        {
            IsConjunctionConnection = reader.ReadString() == "a";
        }

        virtual protected bool TryParse(string cond)
        {
            IsConjunctionConnection = cond == " and";
            return IsConjunctionConnection || cond == " or";
        }

        internal static OrderCondition Parse(string cond)
        {
            var conditions = Enum.GetValues(typeof(OrderConditionType)).OfType<OrderConditionType>().Select(t => Create(t)).ToList();
            return conditions.FirstOrDefault(c => c.TryParse(cond));
        }

        public override bool Equals(object obj)
        {
            var other = obj as OrderCondition;
            if (other == null)
                return false;
            return this.IsConjunctionConnection == other.IsConjunctionConnection && this.Type == other.Type;
        }

        public override int GetHashCode()
        {
            return IsConjunctionConnection.GetHashCode() + Type.GetHashCode();
        }
    }

    class StringSuffixParser
    {
        string str;

        public StringSuffixParser(string str)
        {
            this.str = str;
        }

        string SkipSuffix(string perfix)
        {
            return str.Substring(str.IndexOf(perfix) + perfix.Length);
        }

        public string GetNextSuffixedValue(string perfix)
        {
            var rval = str.Substring(0, str.IndexOf(perfix));
            str = SkipSuffix(perfix);

            return rval;
        }

        public string Rest => str;
    }
}
