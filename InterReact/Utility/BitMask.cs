namespace InterReact;

internal readonly struct BitMask
{
    private readonly int Mask;
    internal BitMask(int i) => Mask = i;
    internal bool this[int index]
    {
        get
        {
            if (index < 0 || index >= 32)
                throw new ArgumentException("Invalid", nameof(index));
            
            return (Mask & (1 << index)) != 0;
        }
    }
}
