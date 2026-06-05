namespace InterReact;

[Message]
public sealed record OrderComboLeg // input + output
{
    public double Price { get; init;  }
    internal OrderComboLeg(double price) => Price = price;

}
