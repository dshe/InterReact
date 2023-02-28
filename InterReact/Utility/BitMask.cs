namespace InterReact;

internal readonly struct BitMask
{
    private readonly int Mask;
    internal BitMask(int i) => Mask = i;
    internal bool this[int index]
    {
        get
        {
            if (index is < 0 or >= 32)
                throw new ArgumentException("Invalid", nameof(index));
            
            return (Mask & (1 << index)) != 0;
        }
    }
}
