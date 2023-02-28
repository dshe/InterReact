#pragma warning disable CA1012, CA1307, CA1309, CA1031, CA1310, CA1305

namespace InterReact;

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

    public virtual void Serialize(RequestMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);
        message.Write(IsConjunctionConnection ? "a" : "o");
    }

    public virtual void Deserialize(ResponseReader r)
    {
        ArgumentNullException.ThrowIfNull(r);
        IsConjunctionConnection = r.ReadString() == "a";
    }

    virtual protected bool TryParse(string cond)
    {
        IsConjunctionConnection = cond == " and";

        return IsConjunctionConnection || cond == " or";
    }

    public static OrderCondition? Parse(string cond)
    {
        ArgumentNullException.ThrowIfNull(cond);
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

sealed class StringSuffixParser
{
    public StringSuffixParser(string str)
    {
        Rest = str;
    }

    string SkipSuffix(string perfix)
    {
        return Rest.Substring(Rest.IndexOf(perfix) + perfix.Length);
    }

    public string GetNextSuffixedValue(string prefix)
    {
        var rval = Rest.Substring(0, Rest.IndexOf(prefix));
        Rest = SkipSuffix(prefix);

        return rval;
    }

    public string Rest { get; private set; }
}

#pragma warning restore CA1012, CA1307, CA1309, CA1031, CA1310, CA1305