#pragma warning disable CA1012, CA1307, CA1309, CA1031, CA1310, CA1305

namespace InterReact;

public class ExecutionCondition : OrderCondition
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
        ArgumentNullException.ThrowIfNull(cond);
        if (!cond.StartsWith(header))
            return false;
        try
        {
            var parser = new StringSuffixParser(cond.Replace(header, ""));

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

    public override void Deserialize(ResponseReader r)
    {
        ArgumentNullException.ThrowIfNull(r);
        base.Deserialize(r);

        SecType = r.ReadString();
        Exchange = r.ReadString();
        Symbol = r.ReadString();
    }

    public override void Serialize(RequestMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);
        base.Serialize(message);
        message.Write(SecType);
        message.Write(Exchange);
        message.Write(Symbol);
    }

    public override bool Equals(object? obj)
    {
        var other = obj as ExecutionCondition;

        if (other == null)
            return false;

        return base.Equals(obj)
            && Exchange.Equals(other.Exchange)
            && SecType.Equals(other.SecType)
            && Symbol.Equals(other.Symbol);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode() + Exchange.GetHashCode() + SecType.GetHashCode() + Symbol.GetHashCode();
    }
}

#pragma warning restore CA1012, CA1307, CA1309, CA1031, CA1310, CA1305