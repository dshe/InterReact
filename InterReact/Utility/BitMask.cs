namespace InterReact;

internal readonly struct BitMask
{
    private readonly Int32 Mask;
    internal BitMask(Int32 mask) => Mask = mask;
    internal bool this[Int32 index]
    {
        get
        {
            if (index is < 0 or >= 32)
                throw new ArgumentException("Invalid", nameof(index));

            return (Mask & (1 << index)) != 0;
        }
    }
}
