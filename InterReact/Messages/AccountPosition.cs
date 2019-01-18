namespace InterReact.Messages
{
    public sealed class AccountPosition
    {
        public string Account { get; internal set; }
        public Contract Contract { get; } = new Contract();
        public double Position { get; internal set; }
        public double AverageCost { get; internal set; }
    }

    public sealed class AccountPositionEnd {}

}
