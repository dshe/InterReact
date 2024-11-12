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
