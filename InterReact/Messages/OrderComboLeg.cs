namespace InterReact;

public sealed class OrderComboLeg // input + output
{
    public double? Price { get; }
    internal OrderComboLeg() { }
    public OrderComboLeg(double? price) => Price = price;
}
