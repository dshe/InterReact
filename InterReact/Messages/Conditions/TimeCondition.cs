#pragma warning disable CA1012, CA1307, CA1309, CA1031, CA1310, CA1305

namespace InterReact;

public class TimeCondition : OperatorCondition
{
    const string header = "time";

    protected override string Value
    {
        get
        {
            return Time;
        }
        set
        {
            Time = value;
        }
    }

    public override string ToString()
    {
        return header + base.ToString();
    }

    /**
    * @brief Time field used in conditional order logic. Valid format: YYYYMMDD HH:MM:SS
    */

    public string Time { get; set; } = "";

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