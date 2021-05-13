namespace InterReact
{
    public sealed class OrderComboLeg // input + output
    {
        public double? Price { get; }
        public OrderComboLeg(double? price) => Price = price;
    }
}