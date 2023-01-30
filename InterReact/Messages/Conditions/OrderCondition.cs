/* Copyright (C) 2019 Interactive Brokers LLC. All rights reserved. This code is subject to the terms
 * and conditions of the IB API Non-Commercial License or the IB API Commercial License, as applicable. */

using System.Collections.Generic;
using System.Linq;

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

        if (rval is not null)
            rval.Type = type;

        return rval ?? throw new InvalidOperationException("Invalid OrderConditionType.");
    }

    internal virtual void Serialize(RequestMessage message) =>
        message.Write(IsConjunctionConnection ? "a" : "o");

    internal virtual void Deserialize(ResponseReader r) =>
        IsConjunctionConnection = r.ReadString() == "a";

    virtual protected bool TryParse(string cond)
    {
        IsConjunctionConnection = cond == " and";

        return IsConjunctionConnection || cond == " or";
    }

    public static OrderCondition? Parse(string cond)
    {
        List<OrderCondition> conditions = Enum.GetValues(typeof(OrderConditionType)).OfType<OrderConditionType>().Select(t => Create(t)).ToList();

        return conditions.FirstOrDefault(c => c.TryParse(cond));
    }

    public override bool Equals(object? obj)
    {
        if (obj is not OrderCondition other)
            return false;

        return IsConjunctionConnection == other.IsConjunctionConnection && Type == other.Type;
    }

    public override int GetHashCode()
    {
        return IsConjunctionConnection.GetHashCode() + Type.GetHashCode();
    }
}

public sealed class StringSuffixParser
{
    public StringSuffixParser(string str)
    {
        Rest = str;
    }

    string SkipSuffix(string perfix)
    {
        return Rest[(Rest.IndexOf(perfix) + perfix.Length)..];
    }

    public string GetNextSuffixedValue(string perfix)
    {
        ArgumentNullException.ThrowIfNull(perfix);
        var rval = Rest.Substring(0, Rest.IndexOf(perfix));
        Rest = SkipSuffix(perfix);

        return rval;
    }

    public string Rest { get; private set; }
}

#pragma warning restore CA1012, CA1307, CA1309, CA1031, CA1310, CA1305
