namespace InterReact;

public sealed class OrderComboLeg // input + output
{
    public double Price { get; } = double.MaxValue;
    public OrderComboLeg(double price) => Price = price;
}
