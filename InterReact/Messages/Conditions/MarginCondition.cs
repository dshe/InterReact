using System.Diagnostics.CodeAnalysis;
namespace InterReact;

[SuppressMessage("Usage", "CA1307", Scope = "member")]
[SuppressMessage("Usage", "CA1310", Scope = "member")]
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
