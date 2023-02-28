#pragma warning disable CA1012, CA1307, CA1309, CA1031, CA1310, CA1305

namespace InterReact;

public class MarginCondition : OperatorCondition
{
    const string header = "the margin cushion percent";

    protected override string Value
    {
        get
        {
            return Percent.ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
        }
        set
        {
            Percent = int.Parse(value, System.Globalization.NumberFormatInfo.InvariantInfo);
        }
    }

    public override string ToString()
    {
        return header + base.ToString();
    }

    /**
    * @brief Margin percent to trigger condition.
    */
    public int Percent { get; set; }

    protected override bool TryParse(string cond)
    {
        ArgumentNullException.ThrowIfNull(cond);
        if (!cond.StartsWith(header))
            return false;

        cond = cond.Replace(header, "");

        return base.TryParse(cond);
    }
}

#pragma warning restore CA1012, CA1307, CA1309, CA1031, CA1310, CA1305