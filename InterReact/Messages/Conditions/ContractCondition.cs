/* Copyright (C) 2019 Interactive Brokers LLC. All rights reserved. This code is subject to the terms
 * and conditions of the IB API Non-Commercial License or the IB API Commercial License, as applicable. */

namespace InterReact;

#pragma warning disable CA1012, CA1307, CA1309, CA1031

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
        ContractCondition? other = obj as ContractCondition;

        if (other is null)
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
        ArgumentNullException.ThrowIfNull(cond);

        try
        {
            if (cond.Substring(0, cond.IndexOf(delimiter, StringComparison.Ordinal)) != Type.ToString())
                return false;

            cond = cond[(cond.IndexOf(delimiter, StringComparison.Ordinal) + delimiter.Length)..];
            int conid;

            if (!int.TryParse(cond.AsSpan(0, cond.IndexOf("(", StringComparison.Ordinal)), out conid))
                return false;

            ConId = conid;
            cond = cond[(cond.IndexOf("(", StringComparison.Ordinal) + 1)..];
            Exchange = cond[..cond.IndexOf(")", StringComparison.Ordinal)];
            cond = cond[(cond.IndexOf(")", StringComparison.Ordinal) + 1)..];

            return base.TryParse(cond);
        }
        catch
        {
            return false;
        }
    }

    internal override void Deserialize(ResponseReader r)
    {
        base.Deserialize(r);
        ConId = r.ReadInt();
        Exchange = r.ReadString();
    }

    internal override void Serialize(RequestMessage message)
    {
        base.Serialize(message);
        message.Write(ConId, Exchange);
    }


}

#pragma warning restore CA1012, CA1307, CA1309, CA1031
