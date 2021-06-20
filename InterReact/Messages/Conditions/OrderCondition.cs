using System;
using System.Linq;

namespace InterReact
{
    public abstract class OrderCondition
    {
        public OrderConditionType Type { get; private set; }
        public bool IsConjunctionConnection { get; set; }

        public static OrderCondition Create(OrderConditionType type)
        {
            OrderCondition result = type switch
            {
                OrderConditionType.Execution => new ExecutionCondition() as OrderCondition,
                OrderConditionType.Margin => new MarginCondition(),
                OrderConditionType.PercentChange => new PercentChangeCondition(),
                OrderConditionType.Price => new PriceCondition(),
                OrderConditionType.Time => new TimeCondition(),
                OrderConditionType.Volume => new VolumeCondition(),
                _ => throw new Exception("Invalid order condition")
            };
            result.Type = type;
            return result;
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

        internal static OrderCondition? Parse(string cond)
        {
            var conditions = Enum.GetValues(typeof(OrderConditionType)).OfType<OrderConditionType>().Select(t => Create(t)).ToList();
            return conditions.FirstOrDefault(c => c.TryParse(cond));
        }

        public override bool Equals(object? obj)
        {
            OrderCondition? other = obj as OrderCondition;
            if (other == null)
                return false;
            return IsConjunctionConnection == other.IsConjunctionConnection && this.Type == other.Type;
        }

        public override int GetHashCode() => IsConjunctionConnection.GetHashCode() + Type.GetHashCode();

    }

    class StringSuffixParser
    {
        private string Str;
        public StringSuffixParser(string str) => Str = str;

        string SkipSuffix(string prefix) =>
            Str.Substring(Str.IndexOf(prefix) + prefix.Length);

        public string GetNextSuffixedValue(string prefix)
        {
            var rval = Str.Substring(0, Str.IndexOf(prefix));
            Str = SkipSuffix(prefix);
            return rval;
        }

        public string Rest => Str;
    }
}
