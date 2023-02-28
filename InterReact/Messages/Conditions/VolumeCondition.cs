#pragma warning disable CA1012, CA1307, CA1309, CA1031, CA1310, CA1305

namespace InterReact;

public class VolumeCondition : ContractCondition
{
    protected override string Value
    {
        get
        {
            return Volume.ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
        }
        set
        {
            Volume = int.Parse(value, System.Globalization.NumberFormatInfo.InvariantInfo);
        }
    }

    public int Volume { get; set; }
}

#pragma warning restore CA1012, CA1307, CA1309, CA1031, CA1310, CA1305