/* Copyright (C) 2019 Interactive Brokers LLC. All rights reserved. This code is subject to the terms
 * and conditions of the IB API Non-Commercial License or the IB API Commercial License, as applicable. */

#pragma warning disable CA1012, CA1307, CA1309, CA1031, CA1310, CA1305

namespace InterReact;

/**
* @class ExecutionCondition
* @brief This class represents a condition requiring a specific execution event to be fulfilled.
* Orders can be activated or canceled if a set of given conditions is met. An ExecutionCondition is met whenever a trade occurs on a certain product at the given exchange.
*/
public sealed class ExecutionCondition : OrderCondition
{
    /**
    * @brief Exchange where the symbol needs to be traded.
    */
    public string Exchange { get; set; } = "";

    /**
    * @brief Kind of instrument being monitored.
    */
    public string SecType { get; set; } = "";

    /**
    * @brief Instrument's symbol
    */
    public string Symbol { get; set; } = "";

    const string header = "trade occurs for ",
                 symbolSuffix = " symbol on ",
                 exchangeSuffix = " exchange for ",
                 secTypeSuffix = " security type";

    public override string ToString()
    {
        return header + Symbol + symbolSuffix + Exchange + exchangeSuffix + SecType + secTypeSuffix;
    }

    protected override bool TryParse(string cond)
    {
        if (!cond.StartsWith(header, System.StringComparison.Ordinal))
            return false;

        try
        {
            var parser = new StringSuffixParser(cond.Replace(header, "", System.StringComparison.Ordinal));

            Symbol = parser.GetNextSuffixedValue(symbolSuffix);
            Exchange = parser.GetNextSuffixedValue(exchangeSuffix);
            SecType = parser.GetNextSuffixedValue(secTypeSuffix);

            if (!string.IsNullOrWhiteSpace(parser.Rest))
                return base.TryParse(parser.Rest);
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
        SecType = r.ReadString();
        Exchange = r.ReadString();
        Symbol = r.ReadString();
    }

    internal override void Serialize(RequestMessage message)
    {
        base.Serialize(message);
        message.Write(SecType, Exchange, Symbol);
    }


    public override bool Equals(object? obj)
    {
        if (obj is not ExecutionCondition other)
            return false;

        return base.Equals(obj)
            && Exchange.Equals(other.Exchange, System.StringComparison.Ordinal)
            && SecType.Equals(other.SecType, System.StringComparison.Ordinal)
            && Symbol.Equals(other.Symbol, System.StringComparison.Ordinal);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode() + Exchange.GetHashCode() + SecType.GetHashCode() + Symbol.GetHashCode();
    }
}

#pragma warning restore CA1012, CA1307, CA1309, CA1031, CA1310, CA1305
