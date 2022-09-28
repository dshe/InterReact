/* Copyright (C) 2019 Interactive Brokers LLC. All rights reserved. This code is subject to the terms
 * and conditions of the IB API Non-Commercial License or the IB API Commercial License, as applicable. */

#pragma warning disable CA1012, CA1307, CA1309, CA1031, CA1310, CA1305, CA1062

namespace InterReact;

public abstract class OperatorCondition : OrderCondition
{
    protected abstract string Value { get; set; }
    public bool IsMore { get; set; }

    const string header = " is ";

    public override string ToString()
    {
        return header + (IsMore ? ">= " : "<= ") + Value;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not OperatorCondition other)
            return false;

        return base.Equals(obj)
            && Value.Equals(other.Value)
            && IsMore == other.IsMore;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode() + Value.GetHashCode() + IsMore.GetHashCode();
    }

    protected override bool TryParse(string cond)
    {
        if (!cond.StartsWith(header))
            return false;

        try
        {
            cond = cond.Replace(header, "");

            if (!cond.StartsWith(">=") && !cond.StartsWith("<="))
                return false;

            IsMore = cond.StartsWith(">=");

            if (base.TryParse(cond.Substring(cond.LastIndexOf(" "))))
                cond = cond.Substring(0, cond.LastIndexOf(" "));

            Value = cond.Substring(3);
        }
        catch
        {
            return false;
        }

        return true;
    }

    internal override void Deserialize(ResponseReader r)
    {
        base.Deserialize(r);
        IsMore = r.ReadBool();
        Value = r.ReadString();
    }

    internal override void Serialize(RequestMessage message)
    {
        base.Serialize(message);
        message.Write(IsMore, Value);
    }

}

#pragma warning restore CA1012, CA1307, CA1309, CA1031, CA1310, CA1305, CA1062
